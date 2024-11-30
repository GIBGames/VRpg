/**
 * VRPGManager.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;

namespace GIB.VRPG2
{
	/// <summary>
	/// The main listener and controller of the various parts of the VRPG System.
	/// </summary>
	[CustomName("VRPG Manager")]
	public class VRPGManager : UdonSharpBehaviour
	{
		[Header("Game Info")]
		public string GameName;

		[Header("VRPG Components")]
		[Tooltip("Game Master data component.")]
		public VRPGGameMasterData GMData;
		[Tooltip("Menu Component.")]
		public VRPGMenu Menu;
		[Tooltip("Log Component.")]
		public VRPGLogs Logger;
		[Tooltip("Voice Zone Controller Component.")]
		public VRPGVoiceController VoiceController;
		[Tooltip("VIP/Patron component")]
		public VRPGPatronData PatronData;
		[Tooltip("Character component")]
		public VRPGCharacter Character;
		[Tooltip("Social component")]
		public VRPGSocial Social;
		[Tooltip("Whitelist component")]
		public VRPGWhitelists Whitelists;
		[Tooltip("Region component")]
		public VRPGRegions Regions;
		[Tooltip("World Options")]
		public VRPGOptions Options;

		[Header("ObjectPool")]
        public VRPGPlayerObject LocalPlayerObject;

        [Header("Options")]
		public Color LabelColor = Color.yellow;
		public Color OocLabelColor = Color.cyan;

		#region Constants

		#endregion

		#region Static Methods
		public static DataDictionary JsonToDictionary(string input)
		{
			if (VRCJson.TryDeserializeFromJson(input, out DataToken json))
			{
				DataDictionary newDict = json.DataDictionary;
				return newDict;
			}
			else
			{
				//Debug.Log("Error Deserializing Character Sheet from json!", gameObject);
				return new DataDictionary();
			}
		}

		public static string DictionaryToJson(DataDictionary dictionary)
		{
			if (VRCJson.TrySerializeToJson(dictionary, JsonExportType.Beautify, out DataToken json))
			{
				// Successfully serialized! We can immediately get the string out of the token and do something with it.
				return json.String;
			}
			else
			{
				// Failed to serialize for some reason, running ToString on the result should tell us why.
				// VRPG.Logger.DebugLog("Failed to Serialize to Json. Result was:" + json.ToString(), gameObject);
				return string.Empty;
			}
		}
		#endregion

		#region Unity Methods
		
		#endregion
		
		#region Public Methods
		
		public void HandlerLog(string message)
        {
			Logger.DebugLog(message, gameObject);
        }

		public void SetVoiceZone(int zone)
		{
			PlayerData.SetByte("vrpg-voiceZone", (byte)zone);

			VoiceController.UpdateVoiceZones();
		}

        public void AddVoiceChannel(int zone)
        {
            PlayerData.SetByte("vrpg-voiceZone", (byte)zone);

            VoiceController.UpdateVoiceZones();
        }

        public void SavePlayerData()
		{
            var VarsString = Utils.DictionaryToJson(LocalPlayerObject.VarsDict);
			PlayerData.SetString("VarsDict", VarsString);
		}

        public void LoadPlayerData()
        {
			if(LocalPlayerObject != null && PlayerData.TryGetString(Networking.LocalPlayer,"VarsDict",out string newVars))
			{
				LocalPlayerObject.DeSerializeVarsDict(newVars);
			}
        }

		public void SetTags(string tags)
		{

		}

		public void SetNameAndTitle(string nname, string ntitle)
		{
			if(Utilities.IsValid(LocalPlayerObject))
			{
				LocalPlayerObject.SetNameAndTitle(nname, ntitle);
			}
		}

        #endregion

        #region Private Methods

        #endregion

        // Test

        public void SetSpeakChannel0() => LocalPlayerObject.SetSpeakChannel(0);
        public void SetSpeakChannel1() => LocalPlayerObject.SetSpeakChannel(1);
        public void SetSpeakChannel2() => LocalPlayerObject.SetSpeakChannel(2);
        public void SetListenChannel0() => LocalPlayerObject.SetListenChannel(0);
        public void SetListenChannel1() => LocalPlayerObject.SetListenChannel(1);
        public void SetListenChannel2() => LocalPlayerObject.SetListenChannel(2);
    }
}