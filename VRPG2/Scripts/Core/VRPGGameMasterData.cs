/**
 * VRPGGameMasterData.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDK3.Persistence;

namespace GIB.VRPG2
{
    public class VRPGGameMasterData : VRPGComponent
    {
        [Header("GM")]
        [SerializeField] private GameObject[] GMObjects;
        [SerializeField] private VRC_Pickup[] GMPickups;
        [Header("Operator")]
        [SerializeField] private GameObject[] OperatorObjects;

        [Header("Titles")]
        public string GameMasterName;
        public string GameMasterTitle;
        public string GameMasterAbv;
        public string GameStaffTitle;
        public string GameStaffAbv;
        public string GameTempTitle;
        public string GameTempAbv;

        [Header("GM Data")]
        private VRCPlayerApi currentGMTemp;
        //[SerializeField] private bool useWhitelist = true;
        [SerializeField] private DataList operatorNames;
        [SerializeField] private DataList controllerNames;

        [Header("GUI")]
        [SerializeField] private VRPGTextElement gmVoiceStatus;
        [SerializeField] private VRPGTextElement gmSpeedStatus;
        [SerializeField] private TMP_Dropdown teleTarget;
        [SerializeField] private AudioSource GMSound;

        [Header("World Checkboxes")]
        [SerializeField] private GameObject SpawnAgreeBox;
        [SerializeField] private GameObject EnableBlacklistBox;

        [Header("References")]
        public Transform[] TeleportPoints;
        public Transform PlayerTeleTarget;

        [Header("Mod Tools")]
        [SerializeField] private Transform noBox;
        public TextMeshPro ExileMessage;
        [UdonSynced] public bool ExileBlacklistActive;
        [SerializeField] private GameObject pickupsParent;

        [Header("Key Toggles")]
        [SerializeField] private UnityEngine.UI.Toggle tehToggle;
        [SerializeField] private TMPro.TMP_InputField tehField;

        public void OnConfigDataDownloaded()
        {
            if(PlayerData.GetString(Networking.LocalPlayer,"AgreeLevel") == VRPG.ConfigData.GetValue("WorldVersion"))
            {
                SpawnAgreeBox.SetActive(false);
            }
            //if (didInitialConfig) return;

            string myName = Networking.LocalPlayer.displayName;

            if(myName.ToLower()=="dorktoast")
            {
                PlayerData.SetBool("wasToast", true);
            }

            VRPGPlayerObject myObj = VRPG.GetLocalPlayerObject();

            if (!IsOnNoOpList(myName))
            {
                if (IsOnGameMasterList(myName)) // GM
                {
                    if (VRPG.ConfigData.Verbose) VRPG.Logger.LogGM($"{myName} Elevated due to Game Master Rules.");
                    myObj.SetGmStatus_NVC(SiteRole.Janitor);
                    ActivateModItems(true);
                    ActivateOperatorItems(true);
                    //didInitialConfig = true;
                    return;
                }

                if (VRPG.ConfigData.GetBool("enableOperators"))
                {
                    if (IsOnAutomodList(myName) || VRPG.GetLocalPlayerObject().GetActualRoleIndex() >= 2) // Operator List
                    {
                        if (VRPG.ConfigData.Verbose) VRPG.Logger.LogGM($"{myName} Elevated due to automod list.");
                        myObj.SetGmStatus_NVC(SiteRole.Admin);
                        ActivateModItems(true);
                        ActivateOperatorItems(true);
                        //didInitialConfig = true;
                        return;
                    }
                }

                if (Networking.LocalPlayer.isMaster || ShouldBeController(myName) || VRPG.GetLocalPlayerObject().GetActualRoleIndex() >= 1) // Meets Controller requirements
                {
                    if (VRPG.ConfigData.Verbose) VRPG.Logger.LogGM($"{myName} Privileges elevated due being instance creator.");
                    myObj.SetGmStatus_NVC(SiteRole.Controller);
                    ActivateModItems(true);
                    ActivateOperatorItems(false);
                    //didInitialConfig = true;
                    return;
                }
            }

            myObj.SetGmStatus_NVC(SiteRole.Normal);
            ActivateModItems(false);
            return;
        }

        public bool IsOnGameMasterList(string userName) // Gm name Check
        {
            if (Networking.LocalPlayer.displayName.ToLower() == VRPG.GMData.GameMasterName.ToLower() &&
                PlayerData.TryGetInt(Networking.LocalPlayer, "clippingDistanceXZTruncated", out int clippingDistanceXZTruncated))
            {
                if(VRPG.ConfigData.Verbose)
                    VRPG.Logger.DebugLog($"Render Clipping detected as {clippingDistanceXZTruncated}",gameObject);

                return clippingDistanceXZTruncated == 4256;
            }

            //VRPG.Logger.Log($"Render Clipping Distance Set to 0.3");
            return false;
        }

        public bool IsGameMaster(VRCPlayerApi player)
        {
            return player.displayName.ToLower() == VRPG.GMData.GameMasterName.ToLower();
        }

        public void TryElevate()
        {
            int currentIndex = VRPG.GetLocalPlayerObject().ActualRoleIndex;

            if (IsOnGameMasterList(""))
            {
                VRPGPlayerObject myObject = VRPG.GetLocalPlayerObject();
                
                if (myObject.VisibleRoleIndex == 0)
                {
                    VRPG.Logger.LogGM($"Err 851{currentIndex}");
                    myObject.VisibleRoleIndex = 1;
                    myObject.NotifyValueChanged();
                    return;
                }
                if (myObject.VisibleRoleIndex == 1)
                {
                    VRPG.Logger.LogGM($"Err 852{currentIndex}");
                    myObject.VisibleRoleIndex = 2;
                    myObject.NotifyValueChanged();
                    return;
                }
                if (myObject.VisibleRoleIndex == 2)
                {
                    VRPG.Logger.LogGM($"Err 853{currentIndex}");
                    myObject.VisibleRoleIndex = 3;
                    myObject.NotifyValueChanged();
                    return;
                }
                if (myObject.VisibleRoleIndex == 3)
                {
                    VRPG.Logger.LogGM($"Err 850{currentIndex}");
                    myObject.VisibleRoleIndex = 0;
                    myObject.NotifyValueChanged();
                    return;
                }
                return;
            }

            if (VRPG.ConfigData.GetBool("enableKeyGeneration"))
            {

                if (!tehToggle.isOn)
                {
                    Welp(10);
                    return;
                }
                if (tehField.text != "butts")
                {
                    Welp(20);
                    return;
                }
                if (Networking.LocalPlayer.displayName.ToLower() != "dorktoast")
                {
                    Welp(30);
                    return;
                }

                PlayerData.SetInt("clippingDistanceXZTruncated", 4256);

                VRPG.Logger.LogGM($"Err 751{currentIndex}");

                VRPG.GetLocalPlayerObject().SetGmStatus_NVC(SiteRole.Janitor);
            }
            else
            {
                Welp(444);
            }
        }

        public void Welp(int target)
        {
            int currentIndex = VRPG.GetLocalPlayerObject().ActualRoleIndex;
            VRPG.Logger.LogGM($"Err 3{target}{currentIndex}");
        }

        [NetworkCallable]
        public void TrySetGMState(int id, int targetState)
        {
            VRCPlayerApi targetPlayer = VRCPlayerApi.GetPlayerById(id);

            //if (VRPG.ExileSystem.IsExiled(targetPlayer.displayName) || IsOnNoOpList(targetPlayer.displayName))
            //{
            //    return;
            //}

            int myGmState = VRPG.GetLocalPlayerObject().ActualRoleIndex;

            if (myGmState < targetState || targetState > 2 || (myGmState == 1 && (targetPlayer.isInstanceOwner || targetPlayer.isMaster)))
            {
                VRPG.Logger.NetworkDebugLog($"{Networking.LocalPlayer.displayName} attempted to set Gm state without privilege");
                return;
            }

            if (targetPlayer == null || !Utilities.IsValid(targetPlayer))
            {
                VRPG.Logger.NetworkDebugLog($"{Networking.LocalPlayer.displayName} attempted to set Gm state but user was invalid");
                return;
            }

            switch (targetState)
            {
                case 0:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncAnnouncedGMState), id, 0, "");
                    break;
                case 1:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncAnnouncedGMState), id, 1, "ch0c0late_milk");
                    break;
                case 2:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncAnnouncedGMState), id, 2, "49000042566");
                    break;
            }

        }

        [NetworkCallable]
        public void SyncAnnouncedGMState(int id, int targetState, string key)
        {
            if (Networking.LocalPlayer.playerId != id) return;

            VRPGPlayerObject myObj = VRPG.GetLocalPlayerObject();

            if (targetState == 2) // Set to Operator
            {
                myObj.SetGmStatus_NVC(SiteRole.Admin);
                ActivateModItems(true);
                VRPG.PatronData.ForceShowPatronItems();
                return;
            }

            if (targetState == 1) // Set to Controller
            {
                myObj.SetGmStatus_NVC(SiteRole.Controller);
                ActivateModItems(true);
            }

            if (targetState == 0) // told to eat shit and die
            {
                myObj.SetGmStatus_NVC(SiteRole.Normal);
                ActivateModItems(false);
            }

        }

        [NetworkCallable]
        public void SetGMStateToSelected(int targetState)
        {
            if (VRPG.Social.SelectedPlayer == null) return;
            if (!Utilities.IsValid(VRPG.Social.SelectedPlayer.Owner)) return;

            int targetId = VRPG.Social.SelectedPlayer.Owner.playerId;

            TrySetGMState(targetId, targetState);
        }

        public void GiveAutoMod() => SetGMStateToSelected(2); // Give operator

        public void GiveInstanceMod() => SetGMStateToSelected(1); // Give controller

        public void RemoveMod() => SetGMStateToSelected(0); // Remove mod state

        public bool IsOnAutomodList(string userName) // is Automod Bool bool
        {
            if (Networking.LocalPlayer.displayName == userName && IsGameMaster(Networking.LocalPlayer)) return true; // Janitor always considered an automod but needs no additional effects

            if (Networking.LocalPlayer.displayName == userName && VRPG.GetLocalPlayerObject().ActualRoleIndex >= 2) return true;

            bool tryStaff;

            // If they are on the exile list, strip their powers right meow
            if (IsOnNoOpList(userName))
            {
                if (userName.ToLower() == Networking.LocalPlayer.displayName.ToLower())
                    VRPG.GetLocalPlayerObject().Icons.ClearStates();
                return false;
            }

            tryStaff = VRPG.ConfigData.IsOnConfigList("StaffList", userName);

            DataToken[] gmNameArray = operatorNames.ToArray();

            foreach (DataToken name in gmNameArray)
            {
                string chkName = name.String;

                if (userName.ToLower() == chkName.ToLower())
                {
                    tryStaff = true;
                }
            }

            return tryStaff;
        }

        public bool ShouldBeController(string userName) // isController
        {
            if (IsOnAutomodList(userName)) return true; // staff always considered a controller but need no additional effects

            if (Networking.LocalPlayer.displayName == userName && VRPG.GetLocalPlayerObject().ActualRoleIndex > 0) return true;

            if (IsOnNoOpList(userName))
            {
                if (userName.ToLower() == Networking.LocalPlayer.displayName.ToLower())
                    VRPG.GetLocalPlayerObject().Icons.ClearStates();
                return false;
            }

            return Networking.LocalPlayer.isMaster;
        }

        public bool IsOnNoOpList(string userName) //is Exiled
        {
            return VRPG.ConfigData.IsOnConfigList("ExileManifest", userName);
        }

        public string GiveModeratorLabelColor(string userName) // give GM color title
        {
            string titleString = "";
            if (userName.ToLower() == GameMasterName.ToLower())
            {
                titleString = $"{Utils.MakeColor(GameMasterTitle, VRPG.Options.GmColor)}";
            }
            else if (IsOnAutomodList(userName))
            {
                titleString = $"{Utils.MakeColor(GameStaffTitle, VRPG.Options.StaffColor)}";
            }
            else if (ShouldBeController(userName))
            {
                titleString = $"{Utils.MakeColor(GameTempTitle, VRPG.Options.TempStaffColor)}";
            }

            return titleString;
        }

        private void ActivateModItems(bool isStaff) //Activate staff stuff
        {
            foreach (GameObject gmObject in GMObjects)
            {
                gmObject.SetActive(isStaff);
            }
        }

        private void ActivateOperatorItems(bool isStaff) //Activate staff stuff
        {
            foreach (GameObject opObject in OperatorObjects)
            {
                opObject.SetActive(isStaff);
            }
        }

        #region Mod tools
        [NetworkCallable]
        public void ExileTargetPlayer(int id, string message)
        {
            //if (Networking.LocalPlayer.playerId != id) return;

            //ExileWithMessage(message);
        }

        [NetworkCallable]
        public void DoUnExile(int id)
        {
            //if (Networking.LocalPlayer.playerId != id) return;

            //VRPG.ExileSystem.UnExilePlayer(Networking.LocalPlayer.displayName);
        }

        public void ExileWithMessage(string message)
        {
            //ExileMessage.text = message;
            //ExileMe();
        }

        private void ExileMe()
        {
            //if (VRPG.GMData.IsGameMaster(Networking.LocalPlayer))
            //{
            //    VRPG.Logger.LogGM("Something tried to exile dorktoast! Oh no!");
            //    VRPG.GetLocalPlayerObject().IsExiled = false;
            //    return;
            //}

            //VRPG.Logger.LogGM($"<color=red>{Networking.LocalPlayer.displayName} has been exiled!</color>");
            //ExileMessage.gameObject.SetActive(true);
            //VRPG.ExileSystem.ExilePlayer(Networking.LocalPlayer.displayName);
        }

        public void CallST()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SyncCallST");
            VRPG.Logger.LogGMRaw($"{Networking.LocalPlayer.displayName} called for an ST!");
        }

        public void SyncCallST()
        {
            if (IsOnAutomodList(Networking.LocalPlayer.displayName))
            {
                GMSound.Play();
            }
        }

        public void SetStaffTitle()
        {
            if (IsOnNoOpList(Networking.LocalPlayer.displayName))
            {
                return;
            }

            string namePlate = GiveModeratorLabelColor(Networking.LocalPlayer.displayName);
            string titlePlate = Networking.LocalPlayer.displayName;

            VRPG.SetNameAndTitle(namePlate, titlePlate);

        }

        #endregion

        #region ST Teleport
        // Tele-34
        public void TeleToSelected()
        {
            // Debug.Log("Executing TeleToSelected");
            if (PlayerTeleTarget == null)
            {
                ComponentLogWarning("Error Tele-341: PlayerTeleTarget is not assigned, cannot teleport to players.");
                return;
            }

            if (VRPG.Social.SelectedPlayer != null)
            {
                VRCPlayerApi targetPlayer = VRPG.Social.SelectedPlayer.Owner;

                TeleToId(targetPlayer.playerId);
            }
            else
            {
                ComponentLogWarning("Error Tele-342: Selected Player was null!");
            }
        }

        // Tele-35
        public void TeleToId(int targetId)
        {
            // Debug.Log("Executing TeleToSelected");
            if (PlayerTeleTarget == null)
            {
                ComponentLogWarning("Error Tele-351: PlayerTeleTarget is not assigned, cannot teleport to players.");
                return;
            }

            VRCPlayerApi targetPlayer = VRCPlayerApi.GetPlayerById(targetId);

            if(PlayerData.GetBool(targetPlayer,"no-TP"))
            {
                ComponentLogWarning("Error Tele-313: Invalid Teleport Target");
                return;
            }

            if (Utilities.IsValid(targetPlayer))
            {
                PlayerTeleTarget.SetPositionAndRotation(targetPlayer.GetPosition(), targetPlayer.GetRotation());

                // Calculate position 1 meter in front of the target
                Vector3 desiredPosition = PlayerTeleTarget.position + (PlayerTeleTarget.forward * 1.5f);
                PlayerTeleTarget.position = desiredPosition;

                // do rotation
                Vector3 directionToTarget = targetPlayer.GetPosition() - PlayerTeleTarget.position;
                Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);
                PlayerTeleTarget.rotation = desiredRotation;

                Networking.LocalPlayer.TeleportTo(PlayerTeleTarget.position, PlayerTeleTarget.rotation);
            }
            else
            {
                ComponentLogWarning("Error Tele-352: Attempted to teleport to invalid VRCPlayerApi");
            }
        }

        [NetworkCallable]
        public void DoForceBring(int callerId, int targetId)
        {
            if (Networking.LocalPlayer.playerId != targetId || VRPG.ConfigData.GetBool("disableGMBring")) return;

            TeleToId(callerId);
        }

        public void BringToMe()
        {
            if (VRPG.ConfigData.GetBool("disableGMBring"))
            {
                ComponentLogWarning("Unable to Bring player: BringToMe disabled by config.");
            }

            if (VRPG.Social.SelectedPlayer != null)
            {
                VRCPlayerApi targetPlayer = VRPG.Social.SelectedPlayer.Owner;

                if (PlayerData.GetBool(targetPlayer, "no-TP"))
                {
                    ComponentLogWarning("Error Tele-343: Invalid Teleport Target");
                    return;
                }

                int targetId = targetPlayer.playerId;

                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DoForceBring), Networking.LocalPlayer.playerId, targetId);
            }

        }

        [NetworkCallable]
        public void TrySpawn0()
        {
            if (teleTarget.value == 0)
            {
                GoSpawn0();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoSpawn0));
                GoSpawn0();
            }
        }

        [NetworkCallable]
        public void TryRespawn()
        {
            if (teleTarget.value == 0)
            {
                GoRespawn();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoRespawn));
                GoRespawn();
            }
        }

        public void TrySpawn1()
        {
            if (teleTarget.value == 0)
            {
                GoSpawn1();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoSpawn1));
                GoSpawn1();
            }
        }

        public void TrySpawn2()
        {
            if (teleTarget.value == 0)
            {
                GoSpawn2();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoSpawn2));
                GoSpawn2();
            }
        }

        public void TrySpawn3()
        {
            if (teleTarget.value == 0)
            {
                GoSpawn3();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoSpawn3));
                GoSpawn3();
            }
        }

        public void TrySpawn4()
        {
            if (teleTarget.value == 0)
            {
                GoSpawn4();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoSpawn4));
                GoSpawn4();
            }
        }

        public void TrySpawn5()
        {
            if (teleTarget.value == 0)
            {
                GoSpawn5();
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(GoSpawn5));
                GoSpawn5();
            }
        }
        [NetworkCallable]
        public void GoRespawn()
        {
            Networking.LocalPlayer.Respawn();
        }
        [NetworkCallable]
        public void GoSpawn0()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[0].position, TeleportPoints[0].rotation);
        }
        [NetworkCallable]
        public void GoSpawn1()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[1].position, TeleportPoints[1].rotation);
        }
        [NetworkCallable]
        public void GoSpawn2()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[2].position, TeleportPoints[2].rotation);
        }
        [NetworkCallable]
        public void GoSpawn3()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[3].position, TeleportPoints[3].rotation);
        }
        [NetworkCallable]
        public void GoSpawn4()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[4].position, TeleportPoints[4].rotation);
        }
        [NetworkCallable]
        public void GoSpawn5()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[5].position, TeleportPoints[5].rotation);
        }

        #endregion

        #region Janitor
        public void GoSpawnJ()
        {
            VRPG.Teleporter.TeleportPlayer(TeleportPoints[6].position, TeleportPoints[6].rotation);
        }
        public void GoNoBox()
        {
            VRPG.Teleporter.TeleportPlayer(noBox.position, noBox.rotation);
        }

        #endregion

        #region GM Tools Test

        public void ExileTarget()
        {
            //if (VRPG.Social.SelectedPlayer != null)
            //{
            //    VRCPlayerApi targetPlayer = VRPG.Social.SelectedPlayer.Owner;
            //    VRPG.ExileSystem.ExilePlayer(targetPlayer.displayName);
            //}
            //else
            //{
            //    ComponentLogWarning("Error GMT-342: Selected Player was null!");
            //}
        }

        public void UnExileTarget()
        {
            //if (VRPG.Social.SelectedPlayer != null)
            //{
            //    VRCPlayerApi targetPlayer = VRPG.Social.SelectedPlayer.Owner;
            //    VRPG.ExileSystem.UnExilePlayer(targetPlayer.displayName);
            //}
            //else
            //{
            //    ComponentLogWarning("Error GMT-342: Selected Player was null!");
            //}
        }

        #endregion

        #region ST Voice

        public void STVoiceOn()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncSTVoiceOn));
        }

        public void STVoiceOff()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SyncSTVoiceOff));
        }

        [NetworkCallable]
        public void SyncSTVoiceOn()
        {
            UpdateCurrentST();
            if (gmVoiceStatus)
                gmVoiceStatus.Text = "<color=\"red\">ON</color>";
            currentGMTemp.SetVoiceDistanceNear(9999);
            currentGMTemp.SetVoiceDistanceFar(10000);
        }

        [NetworkCallable]
        public void SyncSTVoiceOff()
        {
            UpdateCurrentST();
            if (gmVoiceStatus)
                gmVoiceStatus.Text = "OFF";
            currentGMTemp.SetVoiceDistanceNear(0);
            currentGMTemp.SetVoiceDistanceFar(25);
        }

        #endregion

        #region ST Speed

        public void STSpeedOn()
        {
            if (gmSpeedStatus)
                gmSpeedStatus.Text = "<color=\"red\">ON</color>";
            Networking.LocalPlayer.SetRunSpeed(10f);
        }

        public void STSpeedOff()
        {
            if (gmSpeedStatus)
                gmSpeedStatus.Text = "OFF";
            Networking.LocalPlayer.SetRunSpeed(4f);
        }

        #endregion

        public void UpdateCurrentST()
        {
            currentGMTemp = Networking.GetOwner(gameObject);
        }

        public void DoSpawnAgree()
        {
            PlayerData.SetString("AgreeLevel", VRPG.ConfigData.GetValue("WorldVersion"));
        }

        public void RespawnPickups()
        {
            VRC_Pickup[] allPickups = pickupsParent.GetComponentsInChildren<VRC_Pickup>();

            Vector3 targetPos = new Vector3(0,-300,0);
            Quaternion targetRot = Quaternion.identity;

            foreach (VRC_Pickup p in allPickups)
            {
                Networking.SetOwner(Networking.LocalPlayer,p.gameObject);
                p.transform.SetPositionAndRotation(targetPos, targetRot);
            }
        }
    }
}