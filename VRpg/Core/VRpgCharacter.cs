using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;
using VRC.SDK3.Data;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;

namespace GIB.VRpg
{
    public class VRpgCharacter : VRpgComponent
    {
        public string CharacterName;

        public VRCUrl targetUrl;

        public DataDictionary CurrentCharacter;

        [TextArea] public string CharJsonString;

        public SheetModuleBase charSheet;

        //[SerializeField] private string[] characters;
        private DataDictionary characterDictionary;

        [SerializeField] private InputField idBox;
        [SerializeField] private Button FetchButton;
        [SerializeField] private GameObject pleaseWaitText;
        [SerializeField] private Text resultText;

        public DataDictionary PlayerCharOptions;

        void Start()
        {
            characterDictionary = new DataDictionary();
            FetchCharacters();
        }

        #region string loading
        public void FetchCharacters()
        {
            if (FetchButton != null)
                FetchButton.interactable = false;
            PlayerCharOptions = new DataDictionary();
            GetCharacters();
            charSheet.ClearSheet();
        }

        public void GetCharacters()
        {
            VRpg.Logger.DebugLog("Trying to fetch characters...",gameObject);
            VRCStringDownloader.LoadUrl(targetUrl, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            string fullCharString = result.Result;

            if (VRCJson.TryDeserializeFromJson(fullCharString, out DataToken charList))
            {
                // Sanity Check
                if (charList.TokenType != TokenType.DataList)
                {
                    VRpg.HandlerLog("Deserializing Character list failed; type was not DataList.");
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
                    VRpg.HandlerLog($"Successfully read character data for {thisName.String}");
                }
            }

            // testing

            if (VRCJson.TrySerializeToJson(characterDictionary, JsonExportType.Beautify, out DataToken charJsonToken))
            {
                Debug.Log("Debug: serialized back to string.");
                CharJsonString = charJsonToken.String;
            }

            pleaseWaitText.SetActive(false);
            charSheet.gameObject.SetActive(true);
            VRpg.HandlerLog("Successfully fetched characters!");
            FetchButton.interactable = true;
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            VRpg.HandlerLog("CharacterData> " + result.Error);
            FetchButton.interactable = true;
        }
        #endregion

        public void TryLoadCharacter()
        {
            string localPlayerName = Networking.LocalPlayer.displayName.ToLower();
            string input = idBox.text;
            if (input == string.Empty) return;

            if (characterDictionary.TryGetValue(input, out DataToken value))
            {
                DataDictionary thisCharDictionary = value.DataDictionary;

                if (thisCharDictionary.TryGetValue("vrcName", out DataToken thisVrcName))
                {
                    string thisName = thisVrcName.String.ToLower();

                    if (thisName == localPlayerName || thisName == "any")
                    {
                        GenerateCharacterSheet(value);
                        resultText.text = "Loaded!";
                        return;
                    }
                    else
                        VRpg.HandlerLog($"charVals was {thisName}, local player name was {localPlayerName}.");
                    resultText.text = "No Permission!";
                    return;
                }


            }

            resultText.text = "Character not found!";
        }

        [Button("CharSheet")]
        public void GenerateCharacterSheet(DataToken value)
        {
            CurrentCharacter = value.DataDictionary;
            resultText.text = "Loaded!";
            //charSheet.ConvertCharacterSheet(CurrentCharacter);
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
            if (VRCJson.TryDeserializeFromJson(input, out DataToken json))
            {
                VRpg.HandlerLog("Successfully deserialized Character Sheet from json!");

                DataDictionary newDict = json.DataDictionary;

                //charSheet.ConvertCharacterSheet(newDict);

                return newDict;
            }
            else
            {
                VRpg.HandlerLog("Error Deserializing Character Sheet from json!");
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
