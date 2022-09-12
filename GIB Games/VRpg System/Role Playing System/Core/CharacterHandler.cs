using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    /// <summary>
    /// The main listener and controller of the various parts of the VRpg System.
    /// </summary>
    public class CharacterHandler : UdonSharpBehaviour
    {
        [Tooltip("Storyteller data component.")]
        public STData stData;
        [Tooltip("VRpg Menu Component.")]
        public LarpMenu larpMenu;
        [Tooltip("VRpg Log Component.")]
        public LarpLog larpLog;

        [Tooltip("Patron/VIP whitelist component")]
        public PatronData patronData;
        /// <summary>
        /// The object with an <see cref="AudioSource"/> that can play when the ST is called.
        /// </summary>
        public GameObject alertObject;

        [Header("Input Fields")]
        //These are the fields on the menu where the player can enter a custom name/title.
        public InputField playerCharName;
        public InputField playerCharTitle;

        /// <summary>
        /// The current Cyan object pool being used.
        /// </summary>
        [Header("Object Pool")]
        public CyanPlayerObjectAssigner objectPool;
        /// <summary>
        /// The local player's current object from the Cyan Object Pool.
        /// </summary>
        [HideInInspector]
        public LarpPooledPlayer localPoolObject;

        [Header("Voice Zone Levels")]

        [Tooltip("The player's voice when in the same Voice Zone or not in a voice zone.")]
        public float InVoiceZone = 25f;
        [Tooltip("The player's voice when in a different voice zone.")]
        public float OutVoiceZone = 0f;

        #region Object Pool


        void Start()
        {
            DisableInteractive = true;
        }

        public LarpPooledPlayer LocalPooledPlayer()
        {
            return localPoolObject;
        }

        public void _OnLocalPlayerAssigned()
        {
            Debug.Log("The local player has been assigned an object from the pool!");

            // Get the local player's pool object so we can later perform operations on it.
            localPoolObject = (LarpPooledPlayer)objectPool._GetPlayerPooledUdon(Networking.LocalPlayer);

            DisableInteractive = true;
        }

        [HideInInspector]
        public VRCPlayerApi playerAssignedPlayer;
        [HideInInspector]
        public int playerAssignedIndex;
        [HideInInspector]
        public UdonBehaviour playerAssignedPoolObject;

        public void _OnPlayerAssigned()
        {
            Debug.Log($"Object {playerAssignedIndex} assigned to player {playerAssignedPlayer.displayName} {playerAssignedPlayer.playerId}");
        }

        [HideInInspector]
        public VRCPlayerApi playerUnassignedPlayer;
        [HideInInspector]
        public int playerUnassignedIndex;
        [HideInInspector]
        public UdonBehaviour playerUnassignedPoolObject;

        public void _OnPlayerUnassigned()
        {
            Debug.Log($"Object {playerUnassignedIndex} unassigned from player {playerUnassignedPlayer.displayName} {playerUnassignedPlayer.playerId}");
        }

        #endregion

        /// <summary>
        /// Update the local player's name and title to the text in the input boxes.
        /// </summary>
        public void UpdateNameAndTitle()
        {
            if (localPoolObject != null && playerCharName.text != string.Empty)
            {
                localPoolObject.SetNameAndTitleNVC(playerCharName.text, playerCharTitle.text);
            }
        }

        /// <summary>
        /// Sets the player's name and title to a specific value.
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="newTitle"></param>
        public void SetNameAndTitle(string newName, string newTitle)
        {
            if (localPoolObject != null)
                localPoolObject.SetNameAndTitleNVC(newName, newTitle);
        }

        #region Button commands

        #region Label and icon control
        /// <summary>
        /// Hide the player's label completely.
        /// </summary>
        public void HideMyLabel()
        {
            localPoolObject.SetNameAndTitleNVC(" ", " ");
        }

        public void OOCLabel()
        {
            localPoolObject.SetNameAndTitleNVC("Out of Character", "<color=\"cyan\">" + Networking.LocalPlayer.displayName + "</color>");
        }

        public void STLabel()
        {
            localPoolObject.SetNameAndTitleNVC(Networking.LocalPlayer.displayName, "<color=\"orange\">Storyteller</color>");
        }

        // Map icons
        public void HideIcon()
        {
            localPoolObject.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "HideIcon");
        }

        public void ShowIcon()
        {
            localPoolObject.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ShowIcon");
        }

        //Use power level to represent rank insignia and more.
        public void PowerLevel0()
        {
            localPoolObject.SetPowerLevelNVC(0);
        }
        public void PowerLevel1()
        {
            localPoolObject.SetPowerLevelNVC(1);
        }
        public void PowerLevel2()
        {
            localPoolObject.SetPowerLevelNVC(2);
        }
        public void PowerLevel3()
        {
            localPoolObject.SetPowerLevelNVC(3);
        }
        public void PowerLevel4()
        {
            localPoolObject.SetPowerLevelNVC(4);
        }
        public void PowerLevel5()
        {
            localPoolObject.SetPowerLevelNVC(5);
        }

        #endregion

        #region ST Call button
        /// <summary>
        /// Alerts storytellers and logs to the VRpg Log.
        /// </summary>
        public void CallStorytellers()
        {
            Log("<color=\"red\">" + Networking.LocalPlayer.displayName + " called for an ST</color>");

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CallST");
        }

        /// <summary>
        /// Alerts storytellers and logs to the VRpg Log.
        /// </summary>
        public void CallSTEvent()
        {
            Log("<color=\"red\">" + Networking.LocalPlayer.displayName + " triggered an event!</color>");

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CallST");
        }

        /// <summary>
        /// Logs that an ST is responding to an ST call.
        /// </summary>
        public void STRespond()
        {
            Log($"{Networking.LocalPlayer.displayName} responded to ST ping.");
        }
        #endregion

        #endregion

        /// <summary>
        /// Use this in place of <see cref="Debug.Log"/> to log a special VRLarp message to the console.
        /// </summary>
        /// <param name="logText">Target log text.</param>
        public void HandlerLog(string logText)
        {
            Debug.Log($"[<color=orange>VRLarp</color>] {logText}");
        }

        /// <summary>
        /// Logs a message to the in-menu log.
        /// </summary>
        /// <param name="toLog">Target message.</param>
        public void Log(string toLog)
        {
            larpLog.AddToLog(toLog);
        }
    }
}