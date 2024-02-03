/**
 * VRpgGMData.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace GIB.VRpg
{
	/// <summary>
	/// Summary of Class
	/// </summary>
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRpgGMData : VRpgComponent
	{
		[SerializeField] private GameObject[] GMObjects;
		[SerializeField] private VRC_Pickup[] GMPickups;

		[Header("Titles")]
		public string GameMasterName;
		public string GameMasterTitle;
		public string GameMasterAbv;
		public string GameStaffTitle;
		public string GameStaffAbv;

		[Header("ST Data")]
		public VRCUrl targetUrl;
		private VRCPlayerApi currentGMTemp;

		[Header("GUI")]
		[SerializeField] private VRpgTextElement gmVoiceStatus;
		[SerializeField] private VRpgTextElement gmSpeedStatus;
		[SerializeField] private Dropdown teleTarget;

		[Header("References")]
		public Transform[] TeleportPoints;
		public Transform PlayerTeleTarget;

        [Header("Mod Tools")]
        [SerializeField] private Transform noBox;

		public void OnWhitelistDownloaded()
        {
			if(VRpg.Whitelists.IsOnWhitelist("StaffList",Networking.LocalPlayer.displayName))
            {
				ActivateStaff();
            }

        }

		public bool IsOnStaffList(string userName)
        {
			return VRpg.Whitelists.IsOnWhitelist("staff", userName);

		}
		private void ActivateStaff()
        {

        }

        #region Mod tools

        public void AddSelectedToExile()
        {
            if (!Utilities.IsValid(VRpg.Social.SelectedPlayer.Owner)) return;

            VRpg.Whitelists.AddToWhitelist("ExileManifest", VRpg.Social.SelectedPlayer.Owner.displayName);
        }

        public void RemoveSelectedFromExile()
        {
            if (!Utilities.IsValid(VRpg.Social.SelectedPlayer.Owner)) return;

            VRpg.Whitelists.RemoveFromWhitelist("ExileManifest", VRpg.Social.SelectedPlayer.Owner.displayName);
        }

        private void ExileMe()
        {
            Networking.LocalPlayer.TeleportTo(noBox.position, noBox.rotation);
        }

        private void CheckForExile()
        {
            if (VRpg.Whitelists.IsOnWhitelist("ExileManifest", Networking.LocalPlayer.displayName))
            {
                ExileMe();
            }
        }

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal)
                CheckForExile();
        }

        #endregion

        #region ST Teleport

        public void TeleToSelected()
        {
            if (PlayerTeleTarget == null)
            {
                VRpg.Logger.DebugLog("PlayerTeleTarget is not assigned, cannot teleport to players.",gameObject);
                return;
            }

            if (VRpg.Social.SelectedPlayer != null)
            {
                VRCPlayerApi targetPlayer = VRpg.Social.SelectedPlayer.Owner;
                if (!targetPlayer.IsValid()) return;

                VRpg.Regions.LoadAllRegions();

                PlayerTeleTarget.SetPositionAndRotation(targetPlayer.GetPosition(), targetPlayer.GetRotation());

                // Calculate position 1 meter in front of the target
                Vector3 desiredPosition = PlayerTeleTarget.position + (PlayerTeleTarget.forward * 1.5f);
                PlayerTeleTarget.position = desiredPosition;

                // do rotation
                Vector3 directionToTarget = targetPlayer.GetPosition() - PlayerTeleTarget.position;
                Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);
                PlayerTeleTarget.rotation = desiredRotation;

                Networking.LocalPlayer.TeleportTo(PlayerTeleTarget.position, PlayerTeleTarget.rotation);
            }
        }

        public void TrySpawn0()
        {
            if (teleTarget.value == 0)
                GoSpawn0();
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GoSpawn0");
        }

        public void TrySpawn1()
        {
            if (teleTarget.value == 0)
                GoSpawn1();
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GoSpawn1");
        }

        public void TrySpawn2()
        {
            if (teleTarget.value == 0)
                GoSpawn2();
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GoSpawn2");
        }

        public void TrySpawn3()
        {
            if (teleTarget.value == 0)
                GoSpawn3();
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GoSpawn3");
        }

        public void TrySpawn4()
        {
            if (teleTarget.value == 0)
                GoSpawn4();
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GoSpawn4");
        }

        public void TrySpawn5()
        {
            if (teleTarget.value == 0)
                GoSpawn5();
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GoSpawn5");
        }

        public void GoSpawn0()
        {
            Networking.LocalPlayer.TeleportTo(TeleportPoints[0].position, TeleportPoints[0].rotation);
        }

        public void GoSpawn1()
        {
            Networking.LocalPlayer.TeleportTo(TeleportPoints[1].position, TeleportPoints[1].rotation);
        }

        public void GoSpawn2()
        {
            Networking.LocalPlayer.TeleportTo(TeleportPoints[2].position, TeleportPoints[2].rotation);
        }

        public void GoSpawn3()
        {
            Networking.LocalPlayer.TeleportTo(TeleportPoints[3].position, TeleportPoints[3].rotation);
        }

        public void GoSpawn4()
        {
            Networking.LocalPlayer.TeleportTo(TeleportPoints[4].position, TeleportPoints[4].rotation);
        }

        public void GoSpawn5()
        {
            Networking.LocalPlayer.TeleportTo(TeleportPoints[5].position, TeleportPoints[5].rotation);
        }

        #endregion

        #region ST Voice

        public void STVoiceOn()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncSTVoiceOn");
        }

        public void STVoiceOff()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncSTVoiceOff");
        }

        public void SyncSTVoiceOn()
        {
            UpdateCurrentST();
            gmVoiceStatus.Text = "GM voice <color=\"red\">ON</color>";
            currentGMTemp.SetVoiceDistanceNear(9999);
            currentGMTemp.SetVoiceDistanceFar(10000);
        }

        public void SyncSTVoiceOff()
        {
            UpdateCurrentST();
            gmVoiceStatus.Text = "GM voice OFF";
            currentGMTemp.SetVoiceDistanceNear(0);
            currentGMTemp.SetVoiceDistanceFar(25);
        }

        #endregion

        public void UpdateCurrentST()
        {
            currentGMTemp = Networking.GetOwner(gameObject);
        }
    }
}