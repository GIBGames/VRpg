/**
 * VRPGPlayerButton.cs by Toast https://github.com/dorktoast - 11/23/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
namespace GIB.VRPG2
{
    public class VRPGPlayerButton : VRPGComponent
    {
        public VRPGPlayerObject targetPlayer;

        private Button thisButton;
        private VRPGTextElement buttonLabel;

        private void Start()
        {
            ValidateButton();
        }

        private void ValidateButton()
        {
            if (thisButton == null)
                thisButton = GetComponent<Button>();

            buttonLabel = GetComponentInChildren<VRPGTextElement>();
        }

        public void NoCharacter()
        {
            ValidateButton();
            buttonLabel.SetText("");
            targetPlayer = null;
            thisButton.interactable = false;
        }

        public void AssignCharacter(VRPGPlayerObject target)
        {
            if (!Utilities.IsValid(target.Owner))
            {
                return;
            }

            ValidateButton();
            thisButton.interactable = true;
            targetPlayer = target;

            // Handle text
            PlayerData.TryGetString(target.Owner,"vrpg-charName", out string charName);
            bool isGM = target.VarsDict.GetBool("isGM", false);

            buttonLabel.SetText(GenerateButtonContent(target.Owner.displayName, charName, isGM));

            Color labelColor = isGM ? new Color(255, 165, 0) : Color.yellow;

            buttonLabel.SetColor(labelColor);
        }

        public void SelectPlayer()
        {
            if (targetPlayer == null || !Utilities.IsValid(targetPlayer.Owner)) return;

            VRPG.LocalPlayerObject.SetSelected(targetPlayer.Owner);
            VRPG.Social.SetSelectedPlayer(targetPlayer);
        }

        public string GenerateButtonContent(string playerName, string characterName, bool isST = false)
        {
            // Set initial player label based on ST/narrator status
            string playerLabel;
            VRPGGameMasterData gmData = VRPG.GMData;

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
