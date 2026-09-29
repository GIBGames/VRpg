using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;
using VRC.SDK3.Data;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using TMPro;

namespace GIB.VRPG2
{
    public class VRPGCharacter : VRPGComponent
    {
        public bool EnableModule = false;

        public string CharacterName;

        public VRCUrl targetUrl;

        public DataDictionary CurrentCharacter;

        [TextArea] public string CharJsonString;

        public SheetModuleBase charSheet;

        //[SerializeField] private string[] characters;
        private DataDictionary characterDictionary;

        [SerializeField] private TMP_InputField idBox;
        [SerializeField] private Button FetchButton;
        [SerializeField] private GameObject pleaseWaitText;
        [SerializeField] private TextMeshProUGUI resultText;

        public DataDictionary PlayerCharOptions;

        void Start()
        {
            characterDictionary = new DataDictionary();
            if(EnableModule)
                FetchCharacters();
        }

        #region string loading
        public void FetchCharacters()
        {
            if (!EnableModule) return;


            if (FetchButton != null)
                FetchButton.interactable = false;

            PlayerCharOptions = new DataDictionary();
            GetCharacters();
            if(charSheet != null)
                charSheet.ClearSheet();
        }

        public void GetCharacters()
        {
            if (!EnableModule) return;
            VRPG.Logger.DebugLog("Trying to fetch characters...",gameObject);
            VRCStringDownloader.LoadUrl(targetUrl, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            if (!EnableModule) return;
            string fullCharString = result.Result;

            GenerateNewCharacterDictionary(fullCharString);
        }

        public void GenerateNewCharacterDictionary(string fullCharString)
        {
            if (!EnableModule) return;
            if (VRCJson.TryDeserializeFromJson(fullCharString, out DataToken charList))
            {
                // Sanity Check
                if (charList.TokenType != TokenType.DataList)
                {
                    VRPG.HandlerLog("Deserializing Character list failed; type was not DataList.");
                    return;
                }


            }

            DataToken[] charListArray = charList.DataList.ToArray();

            foreach (DataToken token in charListArray)
            {
                // string thisCharacter = token.String;
                DataDictionary thisCharacterDict = token.DataDictionary;

                if (thisCharacterDict.TryGetValue("Id", out DataToken thisId) && thisCharacterDict.TryGetValue("Char_Name", out DataToken thisName))
                {
                    // Why doesn't VRC Datadictionary santiize against duplicate keys? WRGH
                    if (characterDictionary.ContainsKey(thisId)) continue;

                    characterDictionary.Add(thisId, thisCharacterDict);
                    VRPG.HandlerLog($"Successfully read character data for {thisName.String}");
                }
            }
            pleaseWaitText.SetActive(false);
            charSheet.gameObject.SetActive(true);
            VRPG.HandlerLog("Successfully fetched characters!");
            FetchButton.interactable = true;
        }

        public void GenerateCharacterDictionaryManual()
        {
            GenerateNewCharacterDictionary(CharJsonString);
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            VRPG.HandlerLog("CharacterData> " + result.Error);
            //FetchButton.interactable = true;
        }
        #endregion

        public void TryLoadCharacter()
        {
            string localPlayerName = Networking.LocalPlayer.displayName.ToLower();
            string input = idBox.text;
            if (input == string.Empty) return;

            if (characterDictionary.TryGetValue(input, out DataToken value))
            {
                resultText.text = "Loading Character...";
                DataDictionary thisCharDictionary = value.DataDictionary;

                if (thisCharDictionary.TryGetValue("vrcName", out DataToken thisVrcName))
                {
                    string thisName = thisVrcName.String.ToLower();

                    if (VRPG.GMData.IsOnAutomodList(Networking.LocalPlayer.displayName) || thisName == localPlayerName || thisName == "any")
                    {
                        GenerateCharacterSheet(value);
                        resultText.text = "Loaded!";
                        return;
                    }
                    else
                    {
                        VRPG.HandlerLog($"charVals was {thisName}, local player name was {localPlayerName}.");
                        resultText.text = "No Permission!";
                        return;
                    }
                }
            }
            resultText.text = "Character not found!";
        }

        public void TrySetName()
        {
            string newName = "";
            string newTitle = "";
            string newDesc = "";
            if(CurrentCharacter.TryGetValue("Char_Name", out DataToken nvalue))
            {
                newName = nvalue.String;
            }
            if (CurrentCharacter.TryGetValue("Title", out DataToken tvalue))
            {
                newTitle = tvalue.String;
            }
            if (CurrentCharacter.TryGetValue("Description", out DataToken dvalue))
            {
                newDesc = dvalue.String;
            }

            VRPG.SetNameAndTitle(newName, newTitle);
            VRPG.SetDescription(newDesc);

        }

        [Button("CharSheet")]
        public void GenerateCharacterSheet(DataToken value)
        {
            CurrentCharacter = value.DataDictionary;
            resultText.text = "Loaded!";
            charSheet.ConvertCharacterSheet(CurrentCharacter);
        }


        [Button("TestCharDict")]
        public void TestCharDict()
        {
            CurrentCharacter = JsonToDictionary(CharJsonString);

            if (VRCJson.TrySerializeToJson(CurrentCharacter, JsonExportType.Beautify, out DataToken json))
            {
                // Successfully serialized! We can immediately get the string out of the token and do something with it.
                Debug.Log($"Successfully serialized to json: {json.String}");
            }
            else
            {
                // Failed to serialize for some reason, running ToString on the result should tell us why.
                Debug.Log(json.ToString());
            }
        }

        public DataDictionary JsonToDictionary(string input)
        {
            if (!EnableModule) new DataDictionary();
            if (VRCJson.TryDeserializeFromJson(input, out DataToken json))
            {
                VRPG.HandlerLog("Successfully deserialized Character Sheet from json!");

                DataDictionary newDict = json.DataDictionary;

                //charSheet.ConvertCharacterSheet(newDict);

                return newDict;
            }
            else
            {
                VRPG.HandlerLog("Error Deserializing Character Sheet from json!");
                return new DataDictionary();
            }
        }

        public string CreateSubHash(string[] input)
        {
            string subHash = "";

            foreach (string s in input)
            {
                subHash += s + "$";
            }

            subHash = subHash.Remove(subHash.Length - 1, 1);

            return subHash;
        }

        public string DecodeSubHash(string input)
        {
            return input.Replace('$', '\n');
        }
    }
}
