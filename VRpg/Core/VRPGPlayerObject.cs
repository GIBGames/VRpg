/**
 * VRPGPlayerObject.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRPG2
{
    /// <summary>
    /// An object representing a VRPG player and their variables.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VRPGPlayerObject : VRPGComponent
    {
        /// <summary>
        /// Who is the current owner of this object. Null if object is not currently in use. 
        /// </summary>
        public VRCPlayerApi Owner;
        private VRCPlayerApi _localPlayer;
        private bool loaded;

        [Header("Player Variables")]
        private DataDictionary varsDict;
        [TextArea, UdonSynced] public string VarsString;
        private string prevVarsString;

        [Header("Voice")]
        public int LocalVoiceZone;

        [Header("Labels")]
        [SerializeField] private VRPGTextElement NameLabel;
        [SerializeField] private VRPGTextElement TitleLabel;
        [SerializeField] private VRPGTextElement LevelLabel;
        [SerializeField] private VRPGTextElement TagsLabel;
        [SerializeField] private VRPGTextElement[] otherLabels;

        [Header("Icons")]
        [SerializeField] private VRPGIconElement topImage;
        [SerializeField] private VRPGIconElement MapDot;
        [SerializeField] private VRPGIconElement[] otherIcons;

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
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "AskForUpdate");
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

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!player.IsOwner(gameObject)) return;

            Owner = player;
            loaded = true;

            InitializePlayerInfo();
        }

        public override void OnDeserialization()
        {
            _OnValueChanged();
        }

        public void InitializePlayerInfo()
        {
            InitializePlayerDictionary();
            // debug log here

            if (Owner.isLocal)
            {
                VRPG.Logger.DebugLog("Local pooled object was assigned.", gameObject);
                VRPG.LocalPlayerObject = this;
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
            foreach (VRPGTextElement t in otherLabels)
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
            //foreach (VRPGPlayerObject remotePlayer in (VRPGPlayerObject[])VRPG.ObjectPool._GetActivePoolObjects())
            //{
            //    // Get current zone
            //    int remoteZone = remotePlayer.LocalVoiceZone;
            //    if (remoteZone == 0 || remoteZone == LocalVoiceZone)
            //    {
            //        // Set values for people in the same zone
            //        remotePlayer.Owner.SetVoiceDistanceFar(VRPG.Options.InVoiceZone);
            //    }
            //    else
            //    {
            //        // muffled sound for adjacent zones
            //        float targetFar = VRPG.Options.OutVoiceZone;
            //        if (System.Math.Abs(remoteZone - LocalVoiceZone) < 2)
            //            remotePlayer.Owner.SetVoiceDistanceFar(targetFar * 0.5f);
            //        else
            //            remotePlayer.Owner.SetVoiceDistanceFar(targetFar);
            //    }
            //}
        }
        #endregion

        #region Map Icon

        public void HideIcon()
        {
            if (VRPG.GMData.IsOnStaffList(Networking.LocalPlayer.displayName)
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

        public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
        {
            //if (player != Owner) return;

            VRPG.VoiceController.UpdateVoiceZones();

            UpdateValues();
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

        // Persistent Vars
        public void SetPowerLevel(int targetPowerLevel)
        {
            PlayerData.SetByte("vrpg-powerLevel", (byte)targetPowerLevel);
        }

        public void SetVoiceZone(int targetZone)
        {
            PlayerData.SetByte("vrpg-voiceZone", (byte)targetZone);
        }

        public void SetSpeakChannel(int targetZone)
        {
            PlayerData.SetByte("vrpg-speakChannel", (byte)targetZone);
        }

        public void SetListenChannel(int targetZone)
        {
            PlayerData.SetByte("vrpg-listenChannel", (byte)targetZone);
        }

        public void SetNameAndTitle(string newName, string newTitle)
        {
            PlayerData.SetString("vrpg-charName", newName);
            PlayerData.SetString("vrpg-charTitle", newTitle);
        }

        // Non-Persistent Vars
        public void SetStreaming(bool state, bool notify = true)
        {
            VarsDict.SetBool("vrpg-isStreaming", state);

            if (notify)
                NotifyValueChanged();
        }

        public void SetTags(string newTags, bool notify = true)
        {
            VarsDict.SetString("charTags", newTags);

            if (notify)
                NotifyValueChanged();
        }

        public void SetAsStoryteller(bool notify = true)
        {
            VarsDict.SetBool("isGM", true);

            if (notify)
                NotifyValueChanged();
        }

        public void SetSelected(VRCPlayerApi target, bool notify = true)
        {
            VarsDict.SetInt("selectedPlayer", target.playerId);

            if(notify)
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

        public void DeSerializeVarsDict(string varsString)
        {
            varsDict = Utils.JsonToDictionary(VarsString);
            NotifyValueChanged();
        }

        private void UpdateObjectGUI()
        {
            if (!loaded) return;
            CheckPatronColor();

            string tempName = "";
            PlayerData.TryGetString(Owner, "vrpg-charName", out tempName);
            NameLabel.SetText(tempName);

            string tempTitle = "";
            PlayerData.TryGetString(Owner, "vrpg-charTitle", out tempTitle);
            TitleLabel.SetText(tempTitle);

            string tempLevel = "";
            PlayerData.TryGetString(Owner, "vrpg-powerLevel", out tempLevel);
            LevelLabel.SetText(tempLevel, TextElementStyle.Dots_empty);

            TagsLabel.SetText(varsDict.GetString("charTags"));

            if (VarsDict.GetBool("isStreaming"))
                topImage.ResetColor();
            else
                topImage.SetTransparent();
        }

        private void CheckPatronColor()
        {
            if (Owner != null && VRPG.PatronData.IsPatron(Owner.displayName))
                NameLabel.SetColor(VRPG.Options.VipLabelColor);
            else
                NameLabel.ResetColor();
        }
        #endregion

        #region aliases

        private string GetOwnerName()
        {
            return Networking.GetOwner(gameObject).displayName;
        }

        public string GetPlayerName()
        {
            if(Utilities.IsValid(Owner) && PlayerData.TryGetString(Owner,"vrpg-charName", out string tempName))
            {
                return tempName;
            }
            else
            {
                return "";
            }
        }

        public bool IsGM()
        {
            return VarsDict.GetBool("isGM") || VRPG.GMData.IsOnStaffList(Owner.displayName);
        }

        #endregion
    }
}