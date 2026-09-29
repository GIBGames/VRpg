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
using VRC.SDK3.Persistence;

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
        public Color StaffColor;
        public Color TempStaffColor;
        public Sprite GameImage;

		[Header("Voice")]
		public float InVoiceZone = 25f;
		public float OutVoiceZone = 0f;

		[Header("Persistent Options")]

		public Toggle[] optionToggles;
		public Slider[] optionSliders;


        public override void OnPlayerRestored(VRCPlayerApi player)
        {
			if (player.isLocal)
				RestoreOptions(player);
        }

		private void RestoreOptions(VRCPlayerApi player)
		{
			foreach (var toggle in optionToggles)
			{
				string keyVal = "vrpg-options-" + toggle.name;
				bool optionValue = !PlayerData.HasKey(player, keyVal) || PlayerData.GetBool(player, keyVal);
				toggle.isOn = optionValue;
			}

            foreach (var slider in optionSliders)
            {
                string keyValS = "vrpg-options-" + slider.name;
				if (PlayerData.HasKey(player, keyValS))
					slider.value = PlayerData.GetFloat(player, keyValS);
            }
        }

		public void SaveOptions()
		{
            foreach (var toggle in optionToggles)
            {
                PlayerData.SetBool("vrpg-options-" + toggle.name,toggle.isOn);
            }

            foreach (var slider in optionSliders)
            {
                PlayerData.SetFloat("vrpg-options-" + slider.name, slider.value);
            }
        }


        [Button("Set Up")]
		public void SetUp()
        {
			Image logoImage = VRPG.Menu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
			logoImage.sprite = GameImage;
        }
	}
}