using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    public class STPlayerButton : UdonSharpBehaviour
    {
        public LarpPooledPlayer targetPlayer;
        [SerializeField] private Button thisButton;
        [SerializeField] private STData stData;

        private void Start()
        {
            ValidateButton();
        }

        private void ValidateButton()
        {
            if (thisButton == null)
                thisButton = GetComponent<Button>();
            if(stData == null)
                stData = GameObject.Find("VRPG STData").GetComponent<STData>();
        }

        public void NoCharacter()
        {
            ValidateButton();
            gameObject.GetComponentInChildren<Text>().text = "";
            targetPlayer = null;
            thisButton.interactable = false;
        }

        public void AssignCharacter(LarpPooledPlayer target)
        {
            ValidateButton();
            thisButton.interactable = true;
            targetPlayer = target;

            // Handle text
            Text buttonText = thisButton.GetComponentInChildren<Text>();
            buttonText.text = GenerateButtonContent(target.Owner.displayName, target.charName,target.isStoryteller);
            if (target.isStoryteller)
                buttonText.color = new Color(255, 165, 0);
            else
                buttonText.color = Color.yellow;
        }

        public void StSelect()
        {
            if (targetPlayer.Owner != null)
            {
                targetPlayer.SetSelected();
            }
        }

        public string GenerateButtonContent(string playerName, string characterName, bool isST = false)
        {
            // Set initial player label based on ST/narrator status
            string playerLabel;
            if (playerName.ToLower() == stData.GameMasterName.ToLower())
                playerLabel = $"<b>{playerName}</b> [<color=\"red\">{stData.GameMasterAbv}</color>]";
            else if (isST)
                playerLabel = $"<b>{playerName}</b> [<color=\"orange\">{stData.GameStaffAbv}</color>]";
            else
                playerLabel = $"<b>{playerName}</b>";

            // If player has no name set, ignore subtitle and just return player name
            if (characterName == "" || characterName == " ")
                return playerLabel;

            //otherwise make a new line and add player subtitle data
            string playerSubtitle = "\n";

            //if subtitle starts with a < it represents a state of some kind, ignore "Playing:"
            if (characterName[0]=='<')
                playerSubtitle += characterName;
            else
                playerSubtitle += $"Playing: {characterName}";

            return playerLabel + playerSubtitle;
        }
    }
}
