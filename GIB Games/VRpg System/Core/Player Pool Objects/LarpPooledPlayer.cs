using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace GIB.VRpg
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LarpPooledPlayer : UdonSharpBehaviour
    {
        [SerializeField] private CharacterHandler characterHandler;
        /// <summary>
        /// Who is the current owner of this object. Null if object is not currently in use. 
        /// </summary>
        [HideInInspector]
        public VRCPlayerApi Owner;
        private VRCPlayerApi _localPlayer;

        [Header("Player Variables")]
        [UdonSynced] public string charName = " ";
        [UdonSynced] public string charTitle = " ";
        [UdonSynced] public int powerLevel;

        public bool isStoryteller;

        [Header("References")]
        public Text nameLabel;
        public Text titleLabel;
        public GameObject streamIcon;

        [Header("Voice Zones")]
        public int currentZone;

        [Header("Remote Helper")]
        [UdonSynced] public int targetPlayerId;
        public VRCPlayerApi targetPlayer;

        //[SerializeField] private GameObject mapIcon;
        [SerializeField] private Image mapDot;

        public GameObject[] powerLevels;

        // private syncing variables
        private string prevCharName;
        private string prevCharTitle;
        private int prevPowerLevel;
        public int prevPlayerId;

        #region aliases

        public CharacterHandler GetCharacterHandler()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("CharacterHandler").GetComponent<CharacterHandler>();

            return characterHandler;
        }

        private string GetOwnerName()
        {
            return Networking.GetOwner(gameObject).displayName;
        }

        #endregion

        #region Cyan Object Init
        private void Start()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();

            _localPlayer = Networking.LocalPlayer;
        }

        public void _OnOwnerSet()
        {
            // Initialize the object here
            charName = "";
            charTitle = "";
            prevCharName = "";
            prevCharTitle = "";
            currentZone = 0;
            powerLevel = 0;
            prevPowerLevel = 0;
            isStoryteller = false;

            _UpdateNameLabel();

            if (Owner.isLocal)
            {
                characterHandler.HandlerLog($"Pooled object assigned to {Networking.LocalPlayer.displayName}");
                characterHandler.LocalPoolObject = this;
                //mapIcon.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
                mapDot.color = Color.yellow;
            }

            // replace this name if desired, this person will always be made ST on join
            if (Owner.isMaster || Owner.displayName.ToLower() == "dorktoast")
            {
                isStoryteller = true;
            }

            if (GetCharacterHandler()._PatronData.isPatron(Owner.displayName))
            {
                nameLabel.color = new Color(255f, 251f, 0f);
            }
        }

        
        public void _OnCleanup()
        {
            // Cleanup the object here
            charName = "";
            charTitle = "";
            prevCharName = "";
            prevCharTitle = "";
            currentZone = 0;
            powerLevel = 0;
            prevPowerLevel = 0;
            _UpdateNameLabel();

            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        #endregion

        #region Messages / Events

        private void FixedUpdate()
        {
            if (!Utilities.IsValid(Owner))
            {
                return;
            }

            Vector3 pos = Owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            transform.position = pos + Vector3.up * .75f;

            Vector3 locPos = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            transform.LookAt(locPos);

            mapDot.transform.parent.position = pos + Vector3.down * 20f;
        }

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            // Reset player voice zones on respawn.
            characterHandler.HandlerLog($"Player {player.displayName} Respawned: Resetting voice zone");
            if (Owner != null && player == Owner)
            {
                currentZone = 0;
                _UpdateVoiceZones();
            }
        }

        /// <summary>
        /// Requests serialization after a change has been made to the pooler player data.
        /// </summary>
        public void NotifyValueChanged()
        {
            _OnValueChanged();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            _OnValueChanged();
        }

        #endregion

        #region local methods

        //These should absolutely NOT call NotifyValueChanged or any method ending in NVC because they are called by NVC
        private void _OnValueChanged()
        {
            if (charTitle != prevCharTitle || charName != prevCharName)
            {
                characterHandler.HandlerLog($"Title/Name Change: {GetOwnerName()}");
                prevCharName = charName;
                prevCharTitle = charTitle;

                _UpdateNameLabel();
            }

            if (powerLevel != prevPowerLevel)
            {
                characterHandler.HandlerLog($"PowerLevel Change: {GetOwnerName()}");
                prevPowerLevel = powerLevel;
                _UpdatePowerLevel();
            }
        }

        public void _UpdateVoiceZones()
        {
            foreach (LarpPooledPlayer c in (LarpPooledPlayer[])GetCharacterHandler().ObjectPool._GetActivePoolObjects())
            {
                if (c.currentZone == 0 || c.currentZone == characterHandler.LocalPoolObject.currentZone)
                {
                    c.Owner.SetVoiceDistanceFar(GetCharacterHandler()._VoiceController.InVoiceZone);
                }
                else
                {
                    c.Owner.SetVoiceDistanceFar(GetCharacterHandler()._VoiceController.OutVoiceZone);
                }
            }
        }

        public void _UpdatePowerLevel()
        {
            foreach (GameObject g in powerLevels)
            {
                g.SetActive(false);
            }
            if (powerLevel == 0) return;
            powerLevels[powerLevel].SetActive(true);
        }

        private void _UpdateNameLabel()
        {
            if (Owner != null && GetCharacterHandler()._PatronData.isPatron(Owner.displayName))
            {
                nameLabel.text = "<color=\"yellow\">" + charName + "</color>";
            }
            else
            {
                nameLabel.text = charName;
            }

            titleLabel.text = charTitle;
        }

        public void SetSelected()
        {
            GetCharacterHandler()._StData.SetSelectedPlayer(gameObject.GetComponent<LarpPooledPlayer>());
        }

        public void HideIcon()
        {
            mapDot.gameObject.SetActive(false);
        }

        public void ShowIcon()
        {
            mapDot.gameObject.SetActive(true);
        }

        public VRCPlayerApi GetTargetPlayer()
        {
            return targetPlayer;
        }

        #endregion

        #region Notify Value Changed

        /// <summary>
        /// These methods all call Notify Value changes.
        /// </summary>
        /// <param name="targetPowerLevel"></param>

        public void SetPowerLevelNVC(int targetPowerLevel)
        {
            powerLevel = targetPowerLevel;

            NotifyValueChanged();
        }

        public void SetNameAndTitleNVC(string newName, string newTitle)
        {
            charName = newName;
            charTitle = newTitle;
            characterHandler.HandlerLog("Vars set, requesting serialization");
            NotifyValueChanged();
        }
        public void SetVoiceZoneNVC(int newZone)
        {
            currentZone = newZone;
            NotifyValueChanged();
        }

        public void ExitVoiceZoneNVC()
        {
            currentZone = 0;
            NotifyValueChanged();
        }
        #endregion
    }
}