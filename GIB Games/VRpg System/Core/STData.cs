using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    public class STData : UdonSharpBehaviour
    {
        [SerializeField] private CharacterHandler characterHandler;

        public GameObject[] STObjects;
        public VRC_Pickup[] NpcPickups;

        [Header("Current ST Data")]
        private VRCPlayerApi currentST;
        [SerializeField] private string[] STWhitelist;

        [Header("ST Voice")]
        [SerializeField] private Text stVoiceStatus;

        [Header("Teleport Control")]
        [SerializeField] private Dropdown teleTarget;
        public Transform[] TeleportPoints;

        [Header("Player Box")]
        [SerializeField] private GameObject playerButtonParent;
        [SerializeField] private STPlayerButton[] playerButtons;

        [Header("Cell Phone")]
        public LarpPooledPlayer selectedPlayer;
        [SerializeField] private Text selectedPlayerName;

        private void Start()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();

            playerButtons = playerButtonParent.GetComponentsInChildren<STPlayerButton>();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer && (player.isMaster || OnStorytellerList(player.displayName.ToLower()) || player.displayName.ToLower() == "dorktoast"))
            {
                BecomeCurrentST();
            }
        }

        public bool OnStorytellerList(string target)
        {
            foreach(string s in STWhitelist)
            {
                if (s.ToLower() == target)
                    return true;
            }

            return false;
        }

        public void BecomeCurrentST()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            DoStoryteller();
        }

        public void SyncCurrentST()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateCurrentST");
        }

        public void DoStoryteller()
        {
            foreach (GameObject g in STObjects)
            {
                g.SetActive(true);
            }
            if (characterHandler.LocalPooledPlayer() != null)
            {
                characterHandler.LocalPooledPlayer().isStoryteller = true;
            }
            foreach (VRC_Pickup p in NpcPickups)
            {
                p.pickupable = true;
            }
        }

        public void SetSelectedPlayer(LarpPooledPlayer remPlayer)
        {
            selectedPlayer = remPlayer;
            selectedPlayerName.text = remPlayer.Owner.displayName;
            characterHandler.LocalPoolObject.targetPlayerId = remPlayer.Owner.playerId;
        }

        public void UpdateCurrentST()
        {
            currentST = Networking.GetOwner(gameObject);
        }

        public void UpdateStPlayerList()
        {
            foreach (STPlayerButton stButton in playerButtons)
            {
                stButton.NoCharacter();
            }

            Component[] poolList = characterHandler.ObjectPool._GetActivePoolObjects();

            for (int i = 0; i < poolList.Length; i++)
            {
                LarpPooledPlayer playerItem = (LarpPooledPlayer)poolList[i];
                STPlayerButton buttonItem = playerButtons[i];

                buttonItem.AssignCharacter(playerItem);
            }
        }

        private void ShowPlayerButtonPage(int pageNumber)
        {
            foreach (STPlayerButton s in playerButtons)
            {
                s.gameObject.SetActive(false);
            }

            for (int i = 20 * (pageNumber - 1); i < 20 * pageNumber; i++)
            {
                playerButtons[i].gameObject.SetActive(true);
            }
        }

        public void PlayerPage1()
        {
            ShowPlayerButtonPage(1);
        }

        public void PlayerPage2()
        {
            ShowPlayerButtonPage(2);
        }

        public void PlayerPage3()
        {
            ShowPlayerButtonPage(3);
        }

        public void PlayerPage4()
        {
            ShowPlayerButtonPage(4);
        }

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
            stVoiceStatus.text = "ST voice <color=\"red\">ON</color>";
            currentST.SetVoiceDistanceNear(9999);
            currentST.SetVoiceDistanceFar(10000);
        }

        public void SyncSTVoiceOff()
        {
            UpdateCurrentST();
            stVoiceStatus.text = "ST voice OFF";
            currentST.SetVoiceDistanceNear(0);
            currentST.SetVoiceDistanceFar(25);
        }

        #endregion

        #region ST Teleport

        public void TeleToSelected()
        {
            if (selectedPlayer != null)
            {
                Networking.LocalPlayer.TeleportTo(selectedPlayer.Owner.GetPosition(), transform.rotation);
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

    }
}
