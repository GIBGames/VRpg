/**
 * VRPGLogs.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Udon;
using TMPro;
using UdonToolkit;
using VRC.SDK3.UdonNetworkCalling;

namespace GIB.VRPG2
{
    /// <summary>
    /// Handles different types of logging interactions.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [CustomName("VRPG Log Handler")]
    public class VRPGLogs : VRPGComponent
    {
        // Log boxes here, later
        [Header("Logs")]
        [SerializeField] private CanvasGroup ICLogGroup;
        [SerializeField] private TextMeshProUGUI ICOutputBox;
        [SerializeField] private TMP_InputField ICInputBox;
        [Space]
        [SerializeField] private CanvasGroup OOCLogGroup;
        [SerializeField] private TextMeshProUGUI OOCOutputBox;
        [SerializeField] private TMP_InputField OOCInputBox;
        [Space]
        [SerializeField] private CanvasGroup GMLogGroup;
        [SerializeField] private TextMeshProUGUI GMOutputBox;
        [SerializeField] private TMP_InputField GMInputBox;

        [Header("Alert")]
        [SerializeField] private AudioSource ImSound;

        //[UdonSynced] public string NewDebugText;
        //[UdonSynced] public string NewLogText;
        //[UdonSynced] public int IntLogType;


        private void Start()
        {
            ShowOOCLog();
        }
        public void DebugLog(string message, GameObject go)
        {
            Debug.Log(Utils.MakeColor($"[{VRPG.GameName}]", VRPG.LabelColor) + ": " + message, go);
        }

        public void DebugLogWarning(string message, GameObject go)
        {
            Debug.LogWarning(Utils.MakeColor($"[{VRPG.GameName}]", VRPG.LabelColor) + ": " + message, go);
        }

        public void DebugLogError(string message, GameObject go)
        {
            Debug.LogError(Utils.MakeColor($"[{VRPG.GameName}]", VRPG.LabelColor) + ": " + message, go);
        }

        public void NetworkDebugLog(string message)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,nameof(DoNetworkDebug),message);
        }

        [NetworkCallable]
        public void DoNetworkDebug(string message)
        {
            string InDebugText = Utils.MakeColor($"[{VRPG.GameName}]//SYNC", VRPG.LabelColor) + ": " + message;

            Debug.Log(InDebugText);
        }

        public void Log(string message)
        {
            SendLog(message, VRPGLogType.OOC);
        }

        public void LogRaw(string message)
        {
            SendLog(message, VRPGLogType.OOC, true);
        }

        public void LogGM(string message)
        {
            SendLog(message, VRPGLogType.GM);
        }

        public void LogGMRaw(string message)
        {
            SendLog(message, VRPGLogType.GM, true);
        }

        public void SendLog(string message, VRPGLogType logType, bool isRaw = false)
        {
            if (message == "") return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (logType == VRPGLogType.Debug)
            {
                NetworkDebugLog(message);
                return;
            }

            string newLogText = message;
            int intLogType = (int)logType;

            
            if (isRaw)
            {
                newLogText = "\n" + newLogText;
            }
            else
            {
                if (intLogType == (int)VRPGLogType.IC)
                {
                    string logCharName = VRPG.GetLocalPlayerObject().GetCharacterName() + "";
                    newLogText = $"\n{logCharName}: {newLogText}";
                }
                else
                {
                    newLogText = $"\n{Networking.LocalPlayer.displayName}: {newLogText}";
                }
            }

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ReceiveLogNew), newLogText, intLogType);
        }

        [NetworkCallable]
        public void ReceiveLogNew(string message, int logTypeInt)
        {
            VRPGLogType thisLogType = (VRPGLogType)logTypeInt;

            switch (thisLogType)
            {
                case VRPGLogType.IC:
                    ICOutputBox.text += message;
                    if (ImSound) ImSound.Play();
                    break;
                case VRPGLogType.OOC:
                    OOCOutputBox.text += message;
                    break;
                case VRPGLogType.GM:
                    GMOutputBox.text += message;
                    break;
                case VRPGLogType.Debug:
                    Debug.Log(message);
                    break;
                default:
                    break;
            }
        }

        public void SendLogLocal(string message, VRPGLogType logType)
        {
            switch (logType)
            {
                case VRPGLogType.IC:
                    ICOutputBox.text += message;
                    if (ImSound) ImSound.Play();
                    break;
                case VRPGLogType.OOC:
                    OOCOutputBox.text += message;
                    break;
                case VRPGLogType.GM:
                    GMOutputBox.text += message;
                    break;
                case VRPGLogType.Debug:
                    Debug.Log(Utils.MakeColor($"[{VRPG.GameName}]", VRPG.LabelColor) + ": " + message);
                    break;
            }
        }

        public void SendLogIC()
        {
            SendLog(ICInputBox.text, VRPGLogType.IC);

            ICInputBox.text = "";
        }

        public void SendLogOOC()
        {
            SendLog(OOCInputBox.text, VRPGLogType.OOC);
            OOCInputBox.text = "";
        }
        public void SendLogGM()
        {
            SendLog(GMInputBox.text, VRPGLogType.GM);
            GMInputBox.text = "";
        }

        public void ShowICLog()
        {
            ShowLogType(VRPGLogType.IC);
        }

        public void ShowOOCLog()
        {
            ShowLogType(VRPGLogType.OOC);
        }

        public void ShowGMLog()
        {
            ShowLogType(VRPGLogType.GM);
        }

        private void ShowLogType(VRPGLogType thisLogType)
        {
            ICLogGroup.alpha = thisLogType == VRPGLogType.IC ? 1 : 0;
            ICLogGroup.blocksRaycasts = thisLogType == VRPGLogType.IC ? true : false;
            ICLogGroup.interactable = thisLogType == VRPGLogType.IC ? true : false;
            OOCLogGroup.alpha = thisLogType == VRPGLogType.OOC ? 1 : 0;
            OOCLogGroup.blocksRaycasts = thisLogType == VRPGLogType.OOC ? true : false;
            OOCLogGroup.interactable = thisLogType == VRPGLogType.OOC ? true : false;
            GMLogGroup.alpha = thisLogType == VRPGLogType.GM ? 1 : 0;
            GMLogGroup.blocksRaycasts = thisLogType == VRPGLogType.GM ? true : false;
            GMLogGroup.interactable = thisLogType == VRPGLogType.GM ? true : false;
        }




        #region old
        //public void Sync_SendLogIC()
        //{
        //    ICOutputBox.text += NewLogText;
        //    if (ImSound) ImSound.Play();
        //}

        //public void Sync_SendLogOOC()
        //{
        //    OOCOutputBox.text += NewLogText;
        //}

        //public void Sync_SendLogGM()
        //{

        //    GMOutputBox.text += NewLogText;

        //}

        //public void SendLogRaw(string message, VRPGLogType logType)
        //{
        //    Networking.SetOwner(Networking.LocalPlayer, gameObject);
        //    IntLogType = (int)logType;
        //    NewLogText = $"\n{message}";

        //    RequestSerialization();
        //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLog");
        //}
        //public void SendLogOld(string message, VRPGLogType logType, bool isRaw = false)
        //{
        //    if (message == "") return;

        //    Networking.SetOwner(Networking.LocalPlayer, gameObject);
        //    if (logType == VRPGLogType.Debug)
        //    {
        //        NetworkDebugLog(message);
        //    }

        //    string newLogText = message;
        //    IntLogType = (int)logType;

        //    if (isRaw)
        //    {
        //        NewLogText = "\n" + newLogText;
        //    }
        //    else
        //    {
        //        if (IntLogType == (int)VRPGLogType.IC)
        //        {
        //            string logCharName = VRPG.GetLocalPlayerObject().GetCharacterName() + "";
        //            NewLogText = $"\n{logCharName}: {newLogText}";
        //        }
        //        else
        //        {
        //            NewLogText = $"\n{Networking.LocalPlayer.displayName}: {newLogText}";
        //        }
        //    }

        //    RequestSerialization();

        //    SendCustomEventDelayedSeconds("CompleteSendLog", .5f);
        //}

        #endregion
    }
    public enum VRPGLogType
    {
        IC,
        OOC,
        GM,
        Debug
    }
}