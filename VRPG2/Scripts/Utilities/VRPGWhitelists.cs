using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using VRC.SDKBase;

namespace GIB.VRPG2
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
	public class VRPGWhitelists : VRPGComponent
	{
        private DataDictionary whitelists;
        private DataDictionary sessionConfig;

        public VRCUrl whitelistUrl;

        [UdonSynced]
        [TextArea]
        [SerializeField] private string whitelistJson;
        private string prevWhitelistJson;

        [SerializeField] private UdonSharpBehaviour[] subscribers;

        [SerializeField,TextArea] private string testDictOutput;

        private void Start()
        {
            whitelists = new DataDictionary();

            if (Networking.LocalPlayer.isMaster)
            {
                GetWhitelists();
            }
            else
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, "DoUpdateWhitelists");
            }

        }

        public override void OnDeserialization()
        {
            if(whitelistJson != prevWhitelistJson)
            {
                prevWhitelistJson = whitelistJson;
                VRPG.Logger.DebugLog("Whitelists updated.", gameObject);
                whitelists = Utils.JsonToDictionary(whitelistJson);
                InformSubscribers();
            }
        }

        public void DoUpdateWhitelists()
        {
            VRPG.Logger.NetworkDebugLog("Whitelist Owner or Master is updating whitelists...");
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnUpdateWhitelists");
        }

        public void OnUpdateWhitelists()
        {
            VRPG.Logger.DebugLog("Whitelists updated.", gameObject);
            whitelists = Utils.JsonToDictionary(whitelistJson);
            InformSubscribers();
        }

        private void GetWhitelists()
        {
            VRCStringDownloader.LoadUrl(whitelistUrl, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            whitelistJson = result.Result;
            whitelists = Utils.JsonToDictionary(whitelistJson);
            RequestSerialization();

            prevWhitelistJson = whitelistJson;
            VRPG.Logger.DebugLog("Whitelists updated.", gameObject);
            whitelists = Utils.JsonToDictionary(whitelistJson);
            InformSubscribers();
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            VRPG.Logger.DebugLog("[VRPG Whitelists] " + result.Error, gameObject);
            InformSubscribers();
        }

        public bool IsOnWhitelist(string listName,string userName)
        {
            if(whitelists.TryGetValue(listName,TokenType.DataList,out DataToken value))
            {
                DataToken thisUserName = userName.ToLower();
                DataList targetList = value.DataList;
                if (targetList.Contains(thisUserName))
                {
                    Debug.Log($"name {userName} exists on whitelist {listName}.");
                    return true;
                } 
            }
            Debug.Log($"name {userName} does not exist on whitelist {listName}.");
            return false;
        }

        public void AddToWhitelist(string listName, string userName)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            DataList targetList = new DataList();
            if (whitelists.TryGetValue(listName, TokenType.DataList, out DataToken value))
            {
                targetList = value.DataList;
                whitelists.Remove(value);
            }

            if (!targetList.Contains(userName))
                targetList.Add(userName);

            whitelists.Add(listName, targetList);

            whitelistJson = Utils.DictionaryToJson(whitelists);

            DoUpdateWhitelists();
        }

        public void RemoveFromWhitelist(string listName, string userName)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            DataList targetList = new DataList();
            if (whitelists.TryGetValue(listName, TokenType.DataList, out DataToken value))
            {
                targetList = value.DataList;
                whitelists.Remove(value);
            }

            if (targetList.Contains(userName))
                targetList.Remove(userName);

            whitelists.Add(listName, targetList);

            whitelistJson = Utils.DictionaryToJson(whitelists);

            DoUpdateWhitelists();
        }

        private void InformSubscribers()
        {
            foreach(UdonSharpBehaviour usb in subscribers)
            {
                usb.SendCustomEvent("OnWhitelistDownloaded");
            }
        }
    }
}