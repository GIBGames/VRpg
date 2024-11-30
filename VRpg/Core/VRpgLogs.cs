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
        [SerializeField] private InputField ICInputBox;
        [Space]
        [SerializeField] private CanvasGroup OOCLogGroup;
        [SerializeField] private TextMeshProUGUI OOCOutputBox;
        [SerializeField] private InputField OOCInputBox;
        [Space]
        [SerializeField] private CanvasGroup GMLogGroup;
        [SerializeField] private TextMeshProUGUI GMOutputBox;
        [SerializeField] private InputField GMInputBox;

        [Header("Synced Variables")]
        [UdonSynced] public string NewDebugText;
        [UdonSynced] public string NewLogText;
        [UdonSynced] public int SyncedLogType;


        private void Start()
        {
            ShowOOCLog();
        }
        public void DebugLog(string message, GameObject go)
        {
            Debug.Log(Utils.MakeColor($"[{VRPG.GameName}]", VRPG.LabelColor) + ": " + message, go);
        }

        public void NetworkDebugLog(string message)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            NewDebugText = message;
            RequestSerialization();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "DoNetworkDebug");
        }

        public void DoNetworkDebug()
        {
            Debug.Log(Utils.MakeColor($"[{VRPG.GameName}]//SYNC", VRPG.LabelColor) + ": " + NewDebugText);
        }

        public void Log(string message)
        {
            SendLog(message, VRPGLogType.OOC);
        }

        public void SendLog(string message, VRPGLogType logType)
        {
            if(logType == VRPGLogType.Debug)
            {
                NetworkDebugLog(message);
            }

            string newLogText = message;
            SyncedLogType = (int)logType;

            if (SyncedLogType == (int)VRPGLogType.IC)
            {
                NewLogText = $"\n{VRPG.Character.CharacterName}: {newLogText}";
            }
            else
            {
                NewLogText = $"\n{Networking.LocalPlayer.displayName}: {newLogText}";
            }

            RequestSerialization();

            switch (logType)
            {
                case VRPGLogType.IC:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogIC");
                    break;
                case VRPGLogType.OOC:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogOOC");
                    break;
                case VRPGLogType.GM:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogGM");
                    break;
                case VRPGLogType.Debug:
                    NewDebugText = NewLogText;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogGM");
                    break;
                default:
                    break;
            }

        }

        public void SendLogRaw(string message, VRPGLogType logType)
        {
            SyncedLogType = (int)logType;
            NewLogText = $"\n{message}";

            RequestSerialization();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLog");
        }

        public void SendLogLocal(string message, VRPGLogType logType)
        {
            switch (logType)
            {
                case VRPGLogType.IC:
                    ICOutputBox.text += message;
                    break;
                case VRPGLogType.OOC:
                    OOCOutputBox.text += message;
                    break;
                case VRPGLogType.GM:
                    GMOutputBox.text += message;
                    break;
                case VRPGLogType.Debug:
                    Debug.Log(Utils.MakeColor($"[{VRPG.GameName}]", VRPG.LabelColor) + ": " + NewLogText);
                    break;
            }
        }

        public void SendLogIC()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendLog(ICInputBox.text, VRPGLogType.IC);
        }
        public void SendLogOOC()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendLog(OOCInputBox.text, VRPGLogType.OOC);
        }
        public void SendLogGM()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendLog(GMInputBox.text, VRPGLogType.GM);
        }

        public void ShowICLog()
        {
            ICLogGroup.alpha = 1;
            ICLogGroup.blocksRaycasts = true;
            OOCLogGroup.alpha = 0;
            OOCLogGroup.blocksRaycasts = false;
            GMLogGroup.alpha = 0;
            GMLogGroup.blocksRaycasts = false;
        }

        public void ShowOOCLog()
        {
            ICLogGroup.alpha = 0;
            ICLogGroup.blocksRaycasts = false;
            OOCLogGroup.alpha = 1;
            OOCLogGroup.blocksRaycasts = true;
            GMLogGroup.alpha = 0;
            GMLogGroup.blocksRaycasts = false;
        }

        public void ShowGMLog()
        {
            ICLogGroup.alpha = 0;
            ICLogGroup.blocksRaycasts = false;
            OOCLogGroup.alpha = 0;
            OOCLogGroup.blocksRaycasts = false;
            GMLogGroup.alpha = 1;
            GMLogGroup.blocksRaycasts = true;
        }


        public void Sync_SendLogIC()
        {
            ICOutputBox.text += NewLogText;

        }

        public void Sync_SendLogOOC()
        {
            OOCOutputBox.text += NewLogText;

        }

        public void Sync_SendLogGM()
        {

            GMOutputBox.text += NewLogText;

        }
    }
    public enum VRPGLogType
    {
        IC,
        OOC,
        GM,
        Debug
    }
}