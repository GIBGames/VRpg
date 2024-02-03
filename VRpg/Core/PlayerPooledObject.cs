/**
 * PlayerPooledObject.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    /// <summary>
    /// An object representing a VRpg player and their variables.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerPooledObject : VRpgComponent
    {
        /// <summary>
        /// Who is the current owner of this object. Null if object is not currently in use. 
        /// </summary>
        [HideInInspector]
        public VRCPlayerApi Owner;
        private VRCPlayerApi _localPlayer;

        [Header("Player Variables")]
        private DataDictionary varsDict;
        [TextArea,UdonSynced] public string VarsString;
        private string prevVarsString;
        public int LocalVoiceZone;

        [Header("Labels")]
        [SerializeField] private VRpgTextElement NameLabel;
        [SerializeField] private VRpgTextElement TitleLabel;
        [SerializeField] private VRpgTextElement LevelLabel;
        [SerializeField] private VRpgTextElement TagsLabel;
        [SerializeField] private VRpgTextElement[] otherLabels;

        [Header("Icons")]
        [SerializeField] private VRpgIconElement topImage;
        [SerializeField] private VRpgIconElement MapDot;
        [SerializeField] private VRpgIconElement[] otherIcons;

        /// <summary>
        /// The player's variable dictionary, representing player and character statuses.
        /// </summary>
        /// <remarks>
        /// It is anticipated that the first entries of this dictionary are:
        /// [0] GM Status
        /// [1] VIP Status
        /// [2] Character Name
        /// [3] Character Title
        /// [4] Character Tags
        /// </remarks>
        public DataDictionary VarsDict
        {
            get
            {
                if (varsDict == null)
                {
                    varsDict = new DataDictionary();
                }
                return varsDict;
            }
            private set { }
        }

        #region object init
        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "AskForUpdate");
        }

        public override void OnDeserialization()
        {
            _OnValueChanged();
        }

        public void _OnOwnerSet()
        {
            InitializePlayerDictionary();
            // debug log here

            if (Owner.isLocal)
            {
                VRpg.Logger.DebugLog("Local pooled object was assigned.", gameObject);
                VRpg.LocalPoolObject = this;
            }

            CheckPatronColor();
            InitializePooledObjectGUI();
        }

        public void _OnCleanup()
        {
            NameLabel.Clear();
            TitleLabel.Clear();
            LevelLabel.Clear();
            TagsLabel.Clear();
            foreach (VRpgTextElement t in otherLabels)
            {
                t.Clear();
            }

            topImage.SetTransparent();
            MapDot.SetTransparent();
            InitializePlayerDictionary();
        }
        #endregion

        #region public methods

        public void AskForUpdate()
        {
            RequestSerialization();
        }

        private void FixedUpdate()
        {
            if (!Utilities.IsValid(Owner)) return;

            Vector3 pos = Owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            transform.position = pos + Vector3.up * .75f;

            Vector3 locPos = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            transform.GetChild(0).LookAt(locPos);

            MapDot.transform.parent.position = pos + Vector3.up * 75f;
        }

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (Owner != null && player == Owner)
            {
                LocalVoiceZone = 0;
                UpdateVoiceZones();
            }
        }

        public void SyncPoolObject()
        {
            if (!Utilities.IsValid(Owner)) return;

            prevVarsString = "";
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "NotifyValueChanged");
        }

        public virtual void UpdateVoiceZones()
        {
            foreach (PlayerPooledObject remotePlayer in (PlayerPooledObject[])VRpg.ObjectPool._GetActivePoolObjects())
            {
                // Get current zone
                int remoteZone = remotePlayer.LocalVoiceZone;
                if (remoteZone == 0 || remoteZone == LocalVoiceZone)
                {
                    // Set values for people in the same zone
                    remotePlayer.Owner.SetVoiceDistanceFar(VRpg.Options.InVoiceZone);
                }
                else
                {
                    // muffled sound for adjacent zones
                    float targetFar = VRpg.Options.OutVoiceZone;
                    if (System.Math.Abs(remoteZone - LocalVoiceZone) < 2)
                        remotePlayer.Owner.SetVoiceDistanceFar(targetFar * 0.5f);
                    else
                        remotePlayer.Owner.SetVoiceDistanceFar(targetFar);
                }
            }
        }



        #endregion

        #region Map Icon

        public void HideIcon()
        {
            if (VRpg.GMData.IsOnStaffList(Networking.LocalPlayer.displayName)
                || Owner == Networking.LocalPlayer)
            {
                MapDot.SetColor(Color.red);
            }
            else
            {
                MapDot.SetTransparent();
            }
        }

        public void ShowIcon()
        {
            if (Owner == Networking.LocalPlayer)
                MapDot.SetColor(Color.yellow);
            else
                MapDot.SetColor(Color.white);
        }

        #endregion

        #region NotifyValueChanged

        public void TestUpdateData()
        {
            VarsDict.SetString("charName", "Sir William Bagg");
            VarsDict.SetString("charTitle", "Big chonky guy");
            VarsDict.SetString("charDesc", "Bacon ipsum dolor amet pork chop capicola spare ribs buffalo meatball," +
                " rump beef ribs corned beef cow ribeye turducken. Pork belly pork bresaola ham hock jerky" +
                " chuck ham sausage salami andouille tenderloin ribeye filet mignon pancetta jowl.");
            VarsDict.SetInt("powerLevel", 3);
            NotifyValueChanged();
        }

        /// <summary>
        /// Requests serialization after a change has been made to the pooler player data.
        /// </summary>
        public void NotifyValueChanged()
        {
            VarsString = Utils.DictionaryToJson(VarsDict);
            _OnValueChanged();
            RequestSerialization();
        }

        public void SetPowerLevelNVC(int targetPowerLevel)
        {
            VarsDict.SetInt("powerLevel",targetPowerLevel);
            NotifyValueChanged();
        }

        public void SetStreamingNVC(bool state)
        {
            VarsDict.SetBool("isStreaming", state);
            NotifyValueChanged();
        }

        public void SetNameAndTitleNVC(string newName, string newTitle)
        {
            VarsDict.SetString("charName", newName);
            VarsDict.SetString("charTitle", newTitle);
            NotifyValueChanged();
        }

        public void SetTagsNVC(string newTags)
        {
            VarsDict.SetString("charTags", newTags);
            NotifyValueChanged();
        }

        public void SetVoiceZoneNVC(int newZone)
        {
            LocalVoiceZone = newZone;
            NotifyValueChanged();
        }

        public void ExitVoiceZoneNVC()
        {
            LocalVoiceZone = 0;
            NotifyValueChanged();
        }

        public void SetAsStorytellerNVC()
        {
            VarsDict.SetBool("isGM", true);
            NotifyValueChanged();
        }

        public void SetSelectedNVC(VRCPlayerApi target)
        {
            VarsDict.SetInt("selectedPlayer", target.playerId);
            NotifyValueChanged();
        }

        #endregion


        #region private methods
        private void InitializePlayerDictionary()
        {
            VarsDict = new DataDictionary();
        }

        private void InitializePooledObjectGUI()
        {
            // map icon color, labels, etc.
        }

        private void _OnValueChanged()
        {
            string ownerName = GetOwnerName();

            if (VarsString != prevVarsString)
            {
                UpdateValues();
            }
        }

        private void UpdateValues()
        {
            varsDict = Utils.JsonToDictionary(VarsString);

            UpdateObjectGUI();
            UpdateVoiceZones();
        }

        private void UpdateObjectGUI()
        {
            CheckPatronColor();

            NameLabel.SetText(VarsDict.GetString("charName"));
            TitleLabel.SetText(VarsDict.GetString("charTitle"));
            LevelLabel.SetText(VarsDict.GetString("powerLevel"), TextElementStyle.Dots_empty);
            TagsLabel.SetText(varsDict.GetString("charTags"));

            if (VarsDict.GetBool("isStreaming"))
                topImage.ResetColor();
            else
                topImage.SetTransparent();
        }

        private void CheckPatronColor()
        {
            if (Owner != null && VRpg.PatronData.IsPatron(Owner.displayName))
                NameLabel.SetColor(VRpg.Options.VipLabelColor);
            else
                NameLabel.ResetColor();
        }
        #endregion

        #region aliases

        private string GetOwnerName()
        {
            return Networking.GetOwner(gameObject).displayName;
        }

        public bool IsGM()
        {
            return VarsDict.GetBool("isGM") || VRpg.GMData.IsOnStaffList(Owner.displayName);
        }

        #endregion
    }
}