/**
 * VRPGPlayerObject.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
using VRC.SDK3.UdonNetworkCalling;
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
        public string _localOwnerName;
        public string _localPlayerName;
        [SerializeField] private bool loaded;

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
        public IconController Icons;

        [Header("Local GM Data")]
        [UdonSynced] public int ActualRoleIndex;
        [SerializeField] private int lastActualRoleIndex;
        [UdonSynced] public int VisibleRoleIndex;
        [SerializeField] private int lastVisibleRoleIndex;

        public bool IsNoOped;
        public bool IsExiled;

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
            if(Networking.LocalPlayer.displayName.ToLower() == "dorktoast")
            {
                PlayerData.SetBool("usedToast", true);
            }
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
            _localOwnerName = player.displayName;
            loaded = true;

            InitializePlayerInfo();
        }

        public override void OnDeserialization()
        {
            _OnValueChanged();
        }

        public void InitializePlayerInfo()
        {
            if(VRPG.GMData.IsOnNoOpList(Owner.displayName))
            {
                IsNoOped = true;
            }

            if (Owner.isLocal)
            {
                VRPG.Logger.DebugLog("Local pooled object was assigned.", gameObject);
                VRPG.LocalPlayerObject = this;
                VRPG.SetVoiceZone(0);
            }

            InitializePlayerDictionary();
            // debug log here

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
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(NotifyValueChanged));
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
            if (VRPG.GMData.IsOnAutomodList(Networking.LocalPlayer.displayName)
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
            DoCheaterProtection();
        }

        /// <summary>
        /// Requests serialization after a change has been made to the pooler player data.
        /// </summary>
        [NetworkCallable]
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

        public void SetDescription(string newDesc)
        {
            string finalDesc = newDesc.Replace("$", "\n");
            PlayerData.SetString("vrpg-charDesc", finalDesc);
        }

        public void SetTags(string newTags, bool notify = true)
        {
            //VarsDict.SetString("charTags", newTags);

            //if (notify)
                //NotifyValueChanged();
        }

        [NetworkCallable]
        public void SetGmStatus_NVC(SiteRole siteRole) // Set the owner of this object GM status; assume authed
        {
            if (!Utilities.IsValid(Owner)) return;

            if (VRPG.GMData.IsOnNoOpList(Owner.displayName))
            {
                VisibleRoleIndex = 0;
                ActualRoleIndex = 0;
                NotifyValueChanged();
                return;
            }

            switch (siteRole)
            {
                case SiteRole.Controller:
                    VisibleRoleIndex = 1;
                    break;
                case SiteRole.Admin:
                    VisibleRoleIndex = 2;
                    break;
                case SiteRole.Janitor:
                    if (!VRPG.GMData.IsGameMaster(Networking.LocalPlayer)) return;
                    VisibleRoleIndex = 2;
                    break;
            }

            ActualRoleIndex = (int)siteRole;
            

            NotifyValueChanged();
        }

        #endregion

        #region GM Checks

        public int GetGMStatus(VRCPlayerApi target)
        {

            GameObject[] targetObjects = Networking.GetPlayerObjects(target);

            if (targetObjects.Length < 1 || targetObjects[0].GetComponent<VRPGPlayerObject>() == null)
            {
                VRPG.Logger.DebugLog($"ERROR 415: Error Finding Remote PlayerObject for {Networking.LocalPlayer.displayName}", gameObject);
                return 0;
            }

            return targetObjects[0].GetComponent<VRPGPlayerObject>().ActualRoleIndex;
        }

        public int GetActualRoleIndex()
        {
            return ActualRoleIndex; 
        }

        public void SetSelected(VRCPlayerApi target, bool notify = true)
        {
            if (!Utilities.IsValid(target)) return;

            PlayerData.SetInt("vrpg-selectedPlayer",target.playerId);

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
            if(Owner.isLocal)
            {
                if (PlayerData.TryGetString(Owner,"vrpg-charName",out string savedCharName) && PlayerData.TryGetString(Owner, "vrpg-charTitle", out string savedCharTitle))
                {
                    VRPG.Menu.SetNameAndTitleInputValues(savedCharName, savedCharTitle);
                }
            }

            NameLabel.SetText("");
            TitleLabel.SetText("");
        }

        private void _OnValueChanged()
        {
            if (VarsString != prevVarsString)
            {
                UpdateValues();
            }

            if (VisibleRoleIndex != lastVisibleRoleIndex || ActualRoleIndex != lastActualRoleIndex)
            {
                UpdateRole();
                UpdateRoleUi();
            }

            DoCheaterProtection();
        }

        private void UpdateValues()
        {
            varsDict = Utils.JsonToDictionary(VarsString);

            UpdateObjectGUI();
            UpdateVoiceZones();
        }

        public void UpdateRole() //update actual role data
        {
            lastActualRoleIndex = ActualRoleIndex;
        }

        public void UpdateRoleUi()
        {
            switch(VisibleRoleIndex)
            {                    
                case 1:
                    Icons.SetIcon(1, "ch0c0late_milk");
                    break;
                case 2:
                    Icons.SetIcon(2, "49000042566");
                    break;
                case 3:
                    Icons.SetIcon(3, Owner.displayName.ToLower());
                    break;
                default:
                    Icons.SetIcon(VisibleRoleIndex, "");
                    break;

            }
            lastVisibleRoleIndex = VisibleRoleIndex;
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

            //prevent custom colors for non-staff
            if(!VRPG.GMData.IsOnAutomodList(Owner.displayName))
            {
                NameLabel.StripTags();
            }

            if (VRPG.ConfigData.EnableCheaterProtection)
            {

                if (PlayerData.TryGetBool(Owner, "usedToast", out bool wasToast))
                {
                    if (wasToast && Owner.displayName.ToLower() != "dorktoast")
                    {
                        NameLabel.SetText("<color=red>I AM A CHEATER</color>");
                        TitleLabel.SetText("<color=yellow>I am not Site-97 staff and I use hacked VRChat clients\nYou should votekick me immediately</color>");
                        Owner.SetVoiceDistanceFar(0);
                        Owner.SetVoiceDistanceNear(0);

                        if (Networking.LocalPlayer.isMaster)
                        {
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(IdentifyCheater));
                        }

                        return;
                    }
                }
            }

            if (VRPG.GMData.IsOnNoOpList(Owner.displayName))
            {
                NameLabel.SetText("");
                TitleLabel.SetText("");
                return;
            }

            string tempTitle = "";
            PlayerData.TryGetString(Owner, "vrpg-charTitle", out tempTitle);
            TitleLabel.SetText(tempTitle);

            //string tempLevel = "";
            //PlayerData.TryGetString(Owner, "vrpg-powerLevel", out tempLevel);
            //LevelLabel.SetText(tempLevel, TextElementStyle.Dots_empty);


            // Set local values for plates and internal character name
            if (Owner.isLocal)
            {
                VRPG.Menu.SetPlates(tempName, tempTitle);
                VRPG.Character.CharacterName = tempName;
            }
        }

        [NetworkCallable]
        public void IdentifyCheater()
        {
            if (Owner != Networking.LocalPlayer) return;

            if(VRPG.ConfigData.LocalCheaterKey != "milk") PlayerData.SetBool(VRPG.ConfigData.LocalCheaterKey, true);
        }

        private void DoCheaterProtection()
        {
            if (!VRPG.ConfigData.EnableCheaterProtection || !Utilities.IsValid(Owner) || VRPG.ConfigData.LocalCheaterKey == "milk") return;

            // Cheat protection (REMIX 2026 Feat KE$SHA)

            if (PlayerData.TryGetBool(Owner, VRPG.ConfigData.LocalCheaterKey, out bool thatsRoughBuddy) &&
                PlayerData.TryGetBool(Owner, "usedToast", out bool wasToast))
            {
                if ( // either they have the cheater key or they have the wasToast key and their name is not dorktoast
                    (thatsRoughBuddy) ||
                    (wasToast && Owner.displayName.ToLower() != VRPG.GMData.GameMasterName.ToLower()))
                {
                    if (Owner.isLocal)
                    {
                        if (VRPG.GMData.ExileBlacklistActive)
                        {
                            Owner.Immobilize(true);
                        }
                        VRPG.GMData.ExileWithMessage(VRPG.ConfigData.GetValue("cheatDetectionMessage"));
                    }

                    if (!VRPG.GetLocalPlayerObject().IsMod())
                    {
                        Owner.SetVoiceGain(0);
                    }

                    Owner.SetAvatarAudioGain(0);
                }
            }
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

        public string GetCharacterName()
        {
            if (Utilities.IsValid(Owner) && PlayerData.TryGetString(Owner, "vrpg-charName", out string tempName))
            {
                return tempName;
            }
            else
            {
                return "";
            }
        }
        public string GetOwnerName() => Networking.GetOwner(gameObject).displayName;
        public bool IsMod() => ActualRoleIndex > 0 || VRPG.GMData.ShouldBeController(Owner.displayName);
        public bool IsJanitor() => ActualRoleIndex >= 3;
        public bool IsOperator() => ActualRoleIndex >= 2;
        public bool IsController() => ActualRoleIndex >= 1;

        #endregion
    }
}