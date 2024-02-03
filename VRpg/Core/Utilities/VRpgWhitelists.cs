using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using VRC.SDKBase;

namespace GIB.VRpg
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
	public class VRpgWhitelists : VRpgComponent
	{
		private DataDictionary whitelists;

        public VRCUrl targetUrl;

        [UdonSynced]
        [TextArea]
        [SerializeField] private string whitelistJson;

        [SerializeField] private UdonSharpBehaviour[] subscribers;

        private void Start()
        {
            whitelists = new DataDictionary();

            if(Networking.LocalPlayer.isMaster)
                GetWhitelists();
            else
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, "DoUpdateWhitelists");
            }
        }

        public void DoUpdateWhitelists()
        {
            VRpg.Logger.NetworkDebugLog("Whitelist Owner or Master is updating whitelists...");
            SendCustomNetworkEvent(NetworkEventTarget.All, "OnUpdateWhitelists");
        }

        public void OnUpdateWhitelists()
        {
            VRpg.Logger.DebugLog("Whitelists updated.", gameObject);
            whitelists = Utils.JsonToDictionary(whitelistJson);
            InformSubscribers();
        }

        private void GetWhitelists()
        {
            VRCStringDownloader.LoadUrl(targetUrl, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            whitelistJson = result.Result;
            whitelists = Utils.JsonToDictionary(whitelistJson);
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            VRpg.Logger.DebugLog("[VRpg Whitelists] " + result.Error, gameObject);
        }

        public bool IsOnWhitelist(string listName,string userName)
        {
            if(whitelists.TryGetValue(listName,TokenType.DataList,out DataToken value))
            {
                DataList targetList = value.DataList;
                if (targetList.Contains(userName))
                    return true;
            }
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