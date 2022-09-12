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

        private void Start()
        {
            if (thisButton == null)
                thisButton = GetComponent<Button>();
        }

        public void NoCharacter()
        {
            thisButton.GetComponentInChildren<Text>().text = "";
            targetPlayer = null;
            thisButton.interactable = false;
        }

        public void AssignCharacter(LarpPooledPlayer target)
        {
            thisButton.interactable = true;
            targetPlayer = target;

            // Handle text
            Text buttonText = thisButton.GetComponentInChildren<Text>();
            buttonText.text = target.Owner.displayName;
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
    }
}
