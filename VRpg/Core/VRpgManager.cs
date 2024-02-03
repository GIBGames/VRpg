/**
 * VRpgManager.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;
using VRC.SDK3.Data;

namespace GIB.VRpg
{
	/// <summary>
	/// The main listener and controller of the various parts of the VRpg System.
	/// </summary>
	[CustomName("VRPG Manager")]
	public class VRpgManager : UdonSharpBehaviour
	{
		[Header("Game Info")]
		public string GameName;
		public Color LabelColor = Color.yellow;

		[Header("VRPG Components")]
		[Tooltip("Game Master data component.")]
		public VRpgGMData GMData;
		[Tooltip("Menu Component.")]
		public VRpgMenu Menu;
		[Tooltip("Log Component.")]
		public VRpgLogs Logger;
		//[Tooltip("Voice Zone Controller Component.")]
		//public VRpgVoiceController VoiceController;
		[Tooltip("VIP/Patron component")]
		public VRpgPatronData PatronData;
		[Tooltip("Character component")]
		public VRpgCharacter Character;
		[Tooltip("Social component")]
		public VRpgSocial Social;
		[Tooltip("Whitelist component")]
		public VRpgWhitelists Whitelists;
		[Tooltip("Region component")]
		public VRpgRegions Regions;
		[Tooltip("World Options")]
		public VRpgOptions Options;


		[Header("ObjectPool")]
		public CyanPlayerObjectAssigner ObjectPool;
		public PlayerPooledObject LocalPoolObject;

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
				//VRpg.Logger.DebugLog("Failed to Serialize to Json. Result was:" + json.ToString(), gameObject);
				return string.Empty;
			}
		}
		#endregion

		#region Unity Methods

		void Start()
		{
		
		}
		
		#endregion
		
		#region Public Methods
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}