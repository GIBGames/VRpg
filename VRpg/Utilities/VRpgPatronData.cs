/**
 * VRpgPatronData.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
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

namespace GIB.VRpg
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [CustomName("VRPG Patron Handler")]
    [HelpMessage("Patron Data must be formatted with each name separated by line breaks (\\n), and each tier separated by an @ symbol. for an example click the '?' button.")]
    public class VRpgPatronData : VRpgComponent
    {
        [SerializeField] private bool remotePatronData;
        [HideIf(nameof(DataIsLocal))]
        public VRCUrl targetUrl;
        [TextArea]
        [HideIf(nameof(DataIsRemote))]
        public string PatronHash;
        [SerializeField] private string[] CreditTiers;
        public string[] PatronArray;
        public Color[] TierColors;

        [Tooltip("Text Item to output the Patron Data to.")]
        public VRpgTextElement PatronCredits;

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
            if (PatronArray.Length == 0)
            {
                CreatePatronList();
            }
            else
            {
                GenerateCredits();
            }
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            VRpg.Logger.DebugLog("[VRpg PatronData] " + result.Error,gameObject);
        }

        [Button("Populate")]
        public void CreatePatronList()
        {
            char separatorChar = '\n';

            // Get list used for IsPatron checks
            string patronClean = PatronHash.Replace("@", "");
            PatronArray = patronClean.Split(separatorChar);

            //Create UI lists
            string tempCreditTiers = PatronHash.Replace("\n", " • ");
            CreditTiers = tempCreditTiers.Split('@');
            GenerateCredits();
        }
        public bool IsPatron(string target)
        {
            foreach (string s in PatronArray)
            {
                if (s.ToLower() == target.ToLower())
                    return true;
            }
            return false;
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
