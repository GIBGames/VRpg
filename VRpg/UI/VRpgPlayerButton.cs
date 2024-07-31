/**
 * VRpgPlayerButton.cs by Toast https://github.com/dorktoast - 11/23/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    public class VRpgPlayerButton : VRpgComponent
    {
        public PlayerPooledObject targetPlayer;

        private Button thisButton;
        private VRpgTextElement buttonLabel;

        private void Start()
        {
            ValidateButton();
        }

        private void ValidateButton()
        {
            if (thisButton == null)
                thisButton = GetComponent<Button>();

            buttonLabel = GetComponentInChildren<VRpgTextElement>();
        }

        public void NoCharacter()
        {
            ValidateButton();
            buttonLabel.SetText("");
            targetPlayer = null;
            thisButton.interactable = false;
        }

        public void AssignCharacter(PlayerPooledObject target)
        {
            if (!Utilities.IsValid(target.Owner))
            {
                return;
            }

            ValidateButton();
            thisButton.interactable = true;
            targetPlayer = target;

            // Handle text
            string charName = target.VarsDict.GetString("charName", "");
            bool isGM = target.VarsDict.GetBool("isGM", false);

            buttonLabel.SetText(GenerateButtonContent(target.Owner.displayName, charName, isGM));

            Color labelColor = isGM ? new Color(255, 165, 0) : Color.yellow;

            buttonLabel.SetColor(labelColor);
        }

        public void SelectPlayer()
        {
            if (targetPlayer == null || !Utilities.IsValid(targetPlayer.Owner)) return;

            VRpg.LocalPoolObject.SetSelectedNVC(targetPlayer.Owner);
            VRpg.Social.SetSelectedPlayer(targetPlayer);
        }

        public string GenerateButtonContent(string playerName, string characterName, bool isST = false)
        {
            // Set initial player label based on ST/narrator status
            string playerLabel;
            VRpgGMData gmData = VRpg.GMData;

            if (playerName.ToLower() == gmData.GameMasterName.ToLower())
                playerLabel = $"<b>{playerName}</b> [<color=\"red\">{gmData.GameMasterAbv}</color>]";
            else if (isST)
                playerLabel = $"<b>{playerName}</b> [<color=\"orange\">{gmData.GameStaffAbv}</color>]";
            else
                playerLabel = $"<b>{playerName}</b>";

            // If player has no name set, ignore subtitle and just return player name
            if (characterName == "" || characterName == " ")
                return playerLabel;

            //otherwise make a new line and add player subtitle data
            string playerSubtitle = "\n";

            //if subtitle starts with a < it represents a state of some kind, ignore "Playing:"
            if (characterName[0] == '<')
                playerSubtitle += characterName;
            else
                playerSubtitle += $"Playing: {characterName}";

            return playerLabel + playerSubtitle;
        }
    }
}
