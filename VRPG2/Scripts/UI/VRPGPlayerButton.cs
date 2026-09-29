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
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
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
            if (!Utilities.IsValid(target.Owner) || !Utilities.IsValid(target))
            {
                return;
            }

            ValidateButton();
            thisButton.interactable = true;
            targetPlayer = target;

            // Handle text
            if (PlayerData.TryGetString(target.Owner, "vrpg-charName", out string charName))
            {
                bool isGM = target.IsMod();

                buttonLabel.SetText(GenerateButtonContent(target, charName));

                Color labelColor = isGM ? new Color(255, 165, 0) : Color.yellow;

                buttonLabel.SetColor(labelColor);
            }
        }

        public void SelectPlayer()
        {
            //if (targetPlayer == null || !Utilities.IsValid(targetPlayer.Owner)) return;
            Debug.Log("Setting Selected player to: " + targetPlayer.Owner.displayName);
            VRPG.GetLocalPlayerObject().SetSelected(targetPlayer.Owner);
            VRPG.Social.SetSelectedPlayer(targetPlayer);
        }

        public string GenerateButtonContent(VRPGPlayerObject player, string characterName)
        {

            if (!Utilities.IsValid(player.Owner))
            {
                return "";
            }
            // Set initial player label based on ST/narrator status
            string playerSubtitle;
            string playerLabel;

            playerLabel = $"{GiveModAbbreviation(player)}{player.Owner.displayName}";

            // If player has no name set, ignore subtitle and just return player name
            if (characterName == "" || characterName == " ")
            {
                return playerLabel;
            }

            //otherwise make a new line and add player subtitle data
            playerSubtitle = "\n";

            //if subtitle starts with a < it represents a state of some kind, ignore "Playing:"
            if (characterName != null && characterName.Length > 0 && characterName[0] == '<')
                playerSubtitle += characterName;
            else
                playerSubtitle += $"As: {characterName}";

            return playerLabel + playerSubtitle;
        }

        public string GiveModAbbreviation(VRPGPlayerObject target) //Give GM Icon on Social tab
        {
            string nameString = "";
            if (target.IsJanitor())
            {
                nameString = $"[{Utils.MakeColor(VRPG.GMData.GameMasterAbv, VRPG.Options.GmColor)}] ";
            }
            else if (target.IsOperator())
            {
                nameString = $"[{Utils.MakeColor(VRPG.GMData.GameStaffAbv, VRPG.Options.StaffColor)}] ";
            }
            else if (target.IsController())
            {
                nameString = $"[{Utils.MakeColor(VRPG.GMData.GameTempAbv, VRPG.Options.TempStaffColor)}] ";
            }

            return nameString;
        }
    }
}
