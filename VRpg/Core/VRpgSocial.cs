/**
 * VRpgSocial.cs by Toast https://github.com/dorktoast - 11/23/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
	/// <summary>
	/// Summary of Class
	/// </summary>
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRpgSocial : VRpgComponent      
	{
		//  Fields ===============

		public PlayerPooledObject SelectedPlayer;

		[SerializeField] private VRpgTextElement tagsLabel;

		[SerializeField] private VRpgTextElement playerLabel;
		[SerializeField] private VRpgTextElement nameLabel;
		[SerializeField] private VRpgTextElement titleLabel;
		[SerializeField] private VRpgTextElement descLabel;

		[Header("Player Box")]
		[SerializeField] private GameObject playerButtonParent;
		[SerializeField] private VRpgTextElement playerCountText;
		[SerializeField] private VRpgTextElement selectedPlayerName;

		private VRpgPlayerButton[] playerButtons;


		//  Properties ===========

		//  Methods ==============

		#region Unity Methods

		void Start()
		{
			playerButtons = playerButtonParent.GetComponentsInChildren<VRpgPlayerButton>();
		}
		
		#endregion
		
		#region Public Methods
		public void SetSelectedPlayer(PlayerPooledObject targetPlayer)
		{
			if (!Utilities.IsValid(targetPlayer.Owner)) return;

			string tempName = targetPlayer.VarsDict.GetString("charName", "");
			string tempTitle = targetPlayer.VarsDict.GetString("charTitle", "");
			string tempDesc = targetPlayer.VarsDict.GetString("charDesc", "");

			playerLabel.SetText(targetPlayer.Owner.displayName);
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
			foreach (VRpgPlayerButton playerButton in playerButtons)
			{
				playerButton.gameObject.SetActive(false);
			}

			Component[] poolList = VRpg.ObjectPool._GetActivePoolObjects();

			for (int i = 0; i < poolList.Length; i++)
			{
				playerButtons[i].gameObject.SetActive(true);
				PlayerPooledObject playerItem = (PlayerPooledObject)poolList[i];
				VRpgPlayerButton buttonItem = playerButtons[i];

				playerItem.SyncPoolObject();

				buttonItem.AssignCharacter(playerItem);
			}
			playerCountText.SetText("Players: " + poolList.Length.ToString());
		}

		#endregion

		#region Private Methods

		#endregion
	}
}