/**
 * VRPGWorldOptions.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine.UI;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRPG2
{
	/// <summary>
	/// Options for use in various VRPG derivatives
	/// </summary>
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRPGOptions : VRPGComponent
	{
		[Header("General Options")]
		public bool GiveFirstJoinerGM;

		[Header("Graphics and Colors")]
		public Color VipLabelColor;
		public Color OocColor;
		public Color IcColor;
		public Color GmColor;
		public Sprite GameImage;

		[Header("Voice")]
		public float InVoiceZone = 25f;
		public float OutVoiceZone = 0f;

        [Button("Set Up")]
		public void SetUp()
        {
			Image logoImage = VRPG.Menu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
			logoImage.sprite = GameImage;
        }
	}
}