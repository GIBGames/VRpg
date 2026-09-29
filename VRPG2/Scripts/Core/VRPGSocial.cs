/**
 * VRPGSocial.cs by Toast https://github.com/dorktoast - 11/23/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;

namespace GIB.VRPG2
{
	/// <summary>
	/// Summary of Class
	/// </summary>
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRPGSocial : VRPGComponent
	{

		public VRCPlayerApi[] allPlayers;

		[SerializeField] private VRPGTextElement tagsLabel;

		[SerializeField] private VRPGTextElement playerLabel;
		[SerializeField] private VRPGTextElement nameLabel;
		[SerializeField] private VRPGTextElement titleLabel;
		[SerializeField] private VRPGTextElement descLabel;

		[Header("Player Box")]
		[SerializeField] private GameObject playerButtonParent;
		[SerializeField] private VRPGTextElement playerCountText;
		[SerializeField] private VRPGTextElement selectedPlayerName;

		private VRPGPlayerButton[] playerButtons;

		public VRPGPlayerObject SelectedPlayer;

		//  Properties ===========

		//  Methods ==============

		#region Unity Methods

		void Start()
		{
			playerButtons = playerButtonParent.GetComponentsInChildren<VRPGPlayerButton>();
		}
		
		#endregion
		
		#region Public Methods
		public void SetSelectedPlayer(VRPGPlayerObject targetPlayer)
		{
			//if (!Utilities.IsValid(targetPlayer.Owner)) return;

			SelectedPlayer = targetPlayer;
			SetSelectedPlayerApi(targetPlayer.Owner);
		}

        public void SetSelectedPlayerApi(VRCPlayerApi targetApi)
        {
            if (!Utilities.IsValid(targetApi)) return;

            PlayerData.TryGetString(targetApi, "vrpg-charName", out string tempName);
            PlayerData.TryGetString(targetApi, "vrpg-charTitle", out string tempTitle);
            PlayerData.TryGetString(targetApi, "vrpg-charDesc", out string tempDesc);

            playerLabel.SetText(targetApi.displayName);
            nameLabel.SetText(tempName);
            titleLabel.SetText(tempTitle);
            descLabel.SetText(tempDesc);
        }

        public void ClearSelectedPlayer()
		{
			playerLabel.Clear();
			nameLabel.Clear();
			titleLabel.Clear();
			descLabel.Clear();
		}

		public void UpdatePlayerList()
		{
			foreach (VRPGPlayerButton playerButton in playerButtons)
			{
				playerButton.gameObject.SetActive(false);
			}

			GetAllPlayers();

			for (int i = 0; i < allPlayers.Length; i++)
			{
				playerButtons[i].gameObject.SetActive(true);
				GameObject[] thisPlayerObjects = allPlayers[i].GetPlayerObjects();

				if (!Utilities.IsValid(thisPlayerObjects[0])) continue;

                VRPGPlayerObject playerItem = thisPlayerObjects[0].GetComponentInChildren<VRPGPlayerObject>();
				VRPGPlayerButton buttonItem = playerButtons[i];

				playerItem.SyncPoolObject();

				buttonItem.AssignCharacter(playerItem);
			}
			playerCountText.SetText("Players: " + allPlayers.Length.ToString());
		}

		public VRCPlayerApi[] GetAllPlayers()
		{
            allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            allPlayers = VRCPlayerApi.GetPlayers(allPlayers);

			return allPlayers;
        }

		#endregion

		#region Private Methods

		#endregion
	}
}