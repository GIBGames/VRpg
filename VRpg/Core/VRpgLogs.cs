/**
 * VRpgLogs.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
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

namespace GIB.VRpg
{
    /// <summary>
    /// Handles different types of logging interactions.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [CustomName("VRPG Log Handler")]
    public class VRpgLogs : VRpgComponent
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
            Debug.Log(Utils.MakeColor($"[{VRpg.GameName}]", VRpg.LabelColor) + ": " + message, go);
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
            Debug.Log(Utils.MakeColor($"[{VRpg.GameName}]//SYNC", VRpg.LabelColor) + ": " + NewDebugText);
        }

        public void SendLog(string message, LogType logType)
        {
            if(logType == LogType.Debug)
            {
                NetworkDebugLog(message);
            }

            string newLogText = message;
            SyncedLogType = (int)logType;

            if (SyncedLogType == (int)LogType.IC)
            {
                NewLogText = $"\n{VRpg.Character.CharacterName}: {newLogText}";
            }
            else
            {
                NewLogText = $"\n{Networking.LocalPlayer.displayName}: {newLogText}";
            }

            RequestSerialization();

            switch (logType)
            {
                case LogType.IC:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogIC");
                    break;
                case LogType.OOC:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogOOC");
                    break;
                case LogType.GM:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogGM");
                    break;
                case LogType.Debug:
                    NewDebugText = NewLogText;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLogGM");
                    break;
                default:
                    break;
            }

        }

        public void SendLogRaw(string message, LogType logType)
        {
            SyncedLogType = (int)logType;
            NewLogText = $"\n{message}";

            RequestSerialization();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLog");
        }

        public void SendLogLocal(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.IC:
                    ICOutputBox.text += message;
                    break;
                case LogType.OOC:
                    OOCOutputBox.text += message;
                    break;
                case LogType.GM:
                    GMOutputBox.text += message;
                    break;
                case LogType.Debug:
                    Debug.Log(Utils.MakeColor($"[{VRpg.GameName}]", VRpg.LabelColor) + ": " + NewLogText);
                    break;
            }
        }

        public void SendLogIC()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendLog(ICInputBox.text, LogType.IC);
        }
        public void SendLogOOC()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendLog(OOCInputBox.text, LogType.OOC);
        }
        public void SendLogGM()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendLog(GMInputBox.text, LogType.GM);
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
    public enum LogType
    {
        IC,
        OOC,
        GM,
        Debug
    }
}