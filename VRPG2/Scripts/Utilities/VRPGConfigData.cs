
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace GIB.VRPG2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VRPGConfigData : VRPGComponent
    {
        // This section is intentionally overcomplicated to deter cheating and exploitation.
        // Altering these values may make VRPG cease to function.
        // modify at your own risk.

        public bool UseRemoteData;

        public DataDictionary ConfigData;

        public VRCUrl configDataUrl;

        public bool Downloaded;

        [SerializeField] private bool overrideVerbose;
        private bool verbose;
        public bool Verbose
        {
            get
            {
                if (overrideVerbose)
                    return true;
                else
                    return verbose;
            }
            set
            {
                verbose = value;
            }
        }

        public bool EnableCheaterProtection;

        public string LocalCheaterKey = "milk";
        public string windowScaleLogKey = "eggs";

        [UdonSynced]
        [TextArea]
        [SerializeField] private string configJson;
        private string prevConfigJson;

        [SerializeField] private UdonSharpBehaviour[] subscribers;

        private void Start()
        {
            ConfigData = new DataDictionary();

            if (Networking.LocalPlayer.isMaster)
            {
                GetConfig();
            }
            else
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DoUpdateConfig));
            }

        }

        public override void OnDeserialization()
        {
            if (configJson != prevConfigJson)
            {
                prevConfigJson = configJson;
                VRPG.Logger.DebugLog("<color=white>[VRPG Config]</color> Config updated.", gameObject);
                ConfigData = Utils.JsonToDictionary(configJson);
                InformSubscribers();
            }
        }

        public string GetValue(string key)
        {
            DataToken token = new DataToken();

            if (ConfigData.TryGetValue(key, out token))
            {
                if (Verbose) Debug.Log($"<color=white>[VRPG Config]</color> Value '{key}' as {token.String}");
                return token.String;
            }
            else
            {
                if (Verbose) Debug.Log($"<color=white>[VRPG Config]</color> Value '{key}' Did not exist");
                return "";
            }
        }

        public bool CompareValue(string key,string target)
        {
            return GetValue(key) == target;
        }

        public bool GetBool(string key)
        {
            return GetValue(key).ToLower() == "true";
        }

        [NetworkCallable]
        public void DoUpdateConfig()
        {
            VRPG.Logger.NetworkDebugLog("<color=white>[VRPG Config]</color> Config Owner or Master is updating whitelists...");
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnUpdateConfig));
        }

        [NetworkCallable]
        public void OnUpdateConfig()
        {
            ConfigData = Utils.JsonToDictionary(configJson);

            UpdateSpecialConfigValues();

            OutputAllConfigKeys();
            InformSubscribers();
        }

        private void UpdateSpecialConfigValues()
        {
            //Special updates
            Verbose = GetBool("verboseDebug");
            EnableCheaterProtection = GetBool("enableCheaterDetection");
            LocalCheaterKey = GetValue("localCheaterKey");
            windowScaleLogKey = GetValue("windowScaleLogKey");
        }

        public void OutputAllConfigKeys()
        {
            Debug.Log($"Downloaded: {Downloaded}; Verbose:{Verbose}; ECP: {EnableCheaterProtection}; LCK: {LocalCheaterKey}; disGMB: {GetBool("disableGMBring")}; enableKeyGen:{GetBool("enableKeyGeneration")}");
        }

        private void GetConfig()
        {
            if (UseRemoteData)
                VRCStringDownloader.LoadUrl(configDataUrl, (IUdonEventReceiver)this);
            else
                ParseLoadedConfigData();
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            Downloaded = true;
            VRPG.Logger.DebugLog("<color=white>[VRPG Config]</color> Downlolad of Config Data success.", gameObject);
            configJson = result.Result;
            ParseLoadedConfigData();
        }

        private void ParseLoadedConfigData()
        {
            ConfigData = Utils.JsonToDictionary(configJson);
            RequestSerialization();

            prevConfigJson = configJson;
            VRPG.Logger.DebugLog("<color=white>[VRPG Config]</color> Whitelists parsed.", gameObject);
            ConfigData = Utils.JsonToDictionary(configJson);

            UpdateSpecialConfigValues();

            OutputAllConfigKeys();

            InformSubscribers();
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            if (Verbose) VRPG.Logger.DebugLog("<color=white>[VRPG Config]</color> " + result.Error, gameObject);
            InformSubscribers();
        }

        public bool IsOnConfigList(string listName, string userName)
        {
            if (ConfigData.TryGetValue(listName, TokenType.DataList, out DataToken value))
            {
                DataToken thisUserName = userName.ToLower();
                DataList targetList = value.DataList;
                if (targetList.Contains(thisUserName))
                {
                    if(Verbose) Debug.Log($"name {userName} exists on whitelist {listName}.");
                    return true;
                }
            }
            if (Verbose) Debug.Log($"name {userName} does not exist on whitelist {listName}.");
            return false;
        }

        public void AddToConfigList(string listName, string userName)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            DataList targetList = new DataList();
            if (ConfigData.TryGetValue(listName, TokenType.DataList, out DataToken value))
            {
                targetList = value.DataList;
                ConfigData.Remove(value);
            }

            if (!targetList.Contains(userName))
                targetList.Add(userName);

            ConfigData.Add(listName, targetList);

            configJson = Utils.DictionaryToJson(ConfigData);

            DoUpdateConfig();
        }

        public void RemoveFromConfigList(string listName, string userName)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            DataList targetList = new DataList();
            if (ConfigData.TryGetValue(listName, TokenType.DataList, out DataToken value))
            {
                targetList = value.DataList;
                ConfigData.Remove(value);
            }

            if (targetList.Contains(userName))
                targetList.Remove(userName);

            ConfigData.Add(listName, targetList);

            configJson = Utils.DictionaryToJson(ConfigData);

            DoUpdateConfig();
        }

        private void InformSubscribers()
        {
            foreach (UdonSharpBehaviour usb in subscribers)
            {
                usb.SendCustomEvent("OnConfigDataDownloaded");
            }
        }
    }
}