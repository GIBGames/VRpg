/**
 * VRPGPatronData.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;

namespace GIB.VRPG2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [CustomName("VRPG Patron Handler")]
    [HelpMessage("Patron Data must be formatted with each name separated by line breaks (\\n), and each tier separated by an @ symbol. for an example click the '?' button.")]
    public class VRPGPatronData : VRPGComponent
    {
        [SerializeField] private bool remotePatronData;
        [HideIf(nameof(DataIsLocal))]
        public VRCUrl targetUrl;
        [TextArea]
        public string PatronHash;
        [SerializeField] private string[] CreditTiers;
        public DataList PatronDataList;
        public Color[] TierColors;

        public GameObject[] PatronItems;

        [Tooltip("Text Item to output the Patron Data to.")]
        public VRPGTextElement PatronCredits;

        private void Start()
        {
            if (PatronCredits != null)
                GetPatronList();
        }

        [Button("Get List")]
        public void GetPatronList()
        {
            VRCStringDownloader.LoadUrl(targetUrl, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            PatronHash = result.Result;

            CreatePatronList();
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            VRPG.Logger.DebugLog("[VRPG PatronData] " + result.Error,gameObject);
        }

        [Button("Populate")]
        public void CreatePatronList()
        {
            PatronDataList = new DataList();
            DataDictionary patronListDict = new DataDictionary();
            DataToken patronToken = new DataToken();
            string patronDisplay = "";

            if (VRCJson.TryDeserializeFromJson(PatronHash,out patronToken))
            {
                patronListDict = patronToken.DataDictionary;
            }
            else
            {
                VRPG.Logger.DebugLog("Failed to deserialize Patron data.",gameObject);
                return;
            }

            DataList fullList = patronListDict["PatronList"].DataList;

            for (int i = 0; i < fullList.Count; i++)
            {
                DataToken fullListToken = fullList[i];
                DataList thisList = fullListToken.DataList;

                string addToDisplay = "";

                for (int j = 0; j < thisList.Count; j++)
                {
                    string currentListItem = thisList[j].String;



                    if(currentListItem.Contains('$'))
                    {
                        string[] splitPatronitem = currentListItem.Split('$');
                        PatronDataList.Add(splitPatronitem[1]);
                        addToDisplay += splitPatronitem[0] + " ~ ";
                    }
                    else
                    {
                        PatronDataList.Add(currentListItem);

                        // Don't add alias names to visible list
                        if (i==0) continue;
                        addToDisplay += currentListItem + " ~ ";
                    }
                }

                patronDisplay += Utils.MakeColor(addToDisplay, TierColors[i]) + "\n";
            }

            PatronCredits.Text = patronDisplay;

            //PatronArray = patronClean.Split(separatorChar);

            //Create UI lists
            //string tempCreditTiers = PatronHash.Replace("\n", " ~ ");
            //CreditTiers = tempCreditTiers.Split('@');
            //GenerateCredits();

            foreach (GameObject g in PatronItems)
            {
                g.SetActive(IsPatron(Networking.LocalPlayer.displayName));
            }
        }

        public void ForceShowPatronItems()
        {
            foreach (GameObject g in PatronItems)
            {
                g.SetActive(true);
            }
        }

        public bool IsPatron(string target)
        {
            if (target.ToLower() == VRPG.GMData.GameMasterName.ToLower()) return true;

            DataToken targetToken = new DataToken(target);

            return PatronDataList.Contains(targetToken);

            //for (int i = 0; i < PatronDataList.Count; i++)
            //{ 
            //    string s = PatronDataList[i].String;
            //    if (s.ToLower() == target.ToLower())
            //        return true;
            //}
            //return false;
        }

        [Button("Do Credits")]
        public void GenerateCredits()
        {
            int targetLength = TierColors.Length;

            string tempPatronString = "";

            for (int i = 0; i < targetLength; i++)
            {
                if (i > 0)
                {
                    tempPatronString += Utils.MakeColor(GetCleanString(CreditTiers[i]), TierColors[i]) + "\n";
                }
            }
            PatronCredits.Text = tempPatronString;
        }

        private string GetCleanString(string target)
        {
            return target.Replace("\n", " • ");
        }

        #region UT

        public bool DataIsRemote()
        {
            return remotePatronData;
        }

        public bool DataIsLocal()
        {
            return !remotePatronData;
        }

        #endregion
    }
}
