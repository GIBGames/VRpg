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
		//[SerializeField] private TMP_InputField ICOutputBox2;
		[SerializeField] private InputField ICInputBox;
		[SerializeField] private CanvasGroup OOCLogGroup;
		[SerializeField] private TextMeshProUGUI OOCOutputBox;
		[SerializeField] private InputField OOCInputBox;
		[SerializeField] private CanvasGroup GMLogGroup;
		[SerializeField] private TextMeshProUGUI GMOutputBox;
		[SerializeField] private InputField GMInputBox;

		[UdonSynced] public string NewDebugText;
		[UdonSynced] public string NewLogText;
		[UdonSynced] public LogType syncedLogType;

		public void DebugLog(string message, GameObject go)
        {
			//Debug.Log(Utils.MakeColor($"[{VRpg.GameName}]", VRpg.LabelColor) + ": " + message, go);
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
			//Debug.Log(Utils.MakeColor($"[{VRpg.GameName}]//SYNC", VRpg.LabelColor) + ": " + NewDebugText);
        }

		public void SendLog(string message, LogType logType)
        {
			Networking.SetOwner(Networking.LocalPlayer, gameObject);
			string newLogText = message;
			syncedLogType = logType;

			if (syncedLogType == LogType.IC)
			{
				NewLogText = $"\n{VRpg.Character.CharacterName}: {newLogText}";
			}
			else
			{
				NewLogText = $"\n{Networking.LocalPlayer.displayName}: {newLogText}";
			}

			RequestSerialization();
			SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync_SendLog");
		}

		public void SendLogIC() => SendLog(ICInputBox.text, LogType.IC);
		public void SendLogOOC() => SendLog(OOCInputBox.text, LogType.OOC);
		public void SendLogGM() => SendLog(GMInputBox.text, LogType.GM);

		public void ShowICLog()
        {
			ICLogGroup.alpha = 1;
			OOCLogGroup.alpha = 0;
			GMLogGroup.alpha = 0;
		}

		public void ShowOOCLog()
		{
			ICLogGroup.alpha = 0;
			OOCLogGroup.alpha = 1;
			GMLogGroup.alpha = 0;
		}

		public void ShowGMLog()
		{
			ICLogGroup.alpha = 0;
			OOCLogGroup.alpha = 0;
			GMLogGroup.alpha = 1;
		}


		public void Sync_SendLog()
        {
            switch (syncedLogType)
            {
                case LogType.IC:
					ICInputBox.text += NewLogText;
					break;
                case LogType.OOC:
					OOCInputBox.text += NewLogText;
					break;
                case LogType.GM:
					GMInputBox.text += NewLogText;
					break;
                case LogType.Debug:
					//Debug.Log(Utils.MakeColor($"[{VRpg.GameName}]", VRpg.LabelColor) + ": " + NewLogText);
					break;
            }
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