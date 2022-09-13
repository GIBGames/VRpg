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
        public STData _StData;
        [Tooltip("VRpg Menu Component.")]
        public LarpMenu _LarpMenu;
        [Tooltip("VRpg Log Component.")]
        public LarpLog _LarpLog;
        [Tooltip("Voice Zone Controller Component.")]
        public VoiceZoneController _VoiceController;

        [Tooltip("Patron/VIP whitelist component")]
        public PatronData _PatronData;
        /// <summary>
        /// The object with an <see cref="AudioSource"/> that can play when the ST is called.
        /// </summary>
        public AudioSource alertObject;

        [Header("Input Fields")]
        //These are the fields on the menu where the player can enter a custom name/title.
        [SerializeField] private InputField playerCharName;
        [SerializeField] private InputField playerCharTitle;

        /// <summary>
        /// The current Cyan object pool being used.
        /// </summary>
        [Header("Object Pool")]
        public CyanPlayerObjectAssigner ObjectPool;
        /// <summary>
        /// The local player's current object from the Cyan Object Pool.
        /// </summary>
        [HideInInspector]
        public LarpPooledPlayer LocalPoolObject;


        #region Object Pool


        void Start()
        {
            DisableInteractive = true;
        }

        public LarpPooledPlayer LocalPooledPlayer()
        {
            return LocalPoolObject;
        }

        public void _OnLocalPlayerAssigned()
        {
            Debug.Log("The local player has been assigned an object from the pool!");

            // Get the local player's pool object so we can later perform operations on it.
            LocalPoolObject = (LarpPooledPlayer)ObjectPool._GetPlayerPooledUdon(Networking.LocalPlayer);

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
            if (LocalPoolObject != null && playerCharName.text != string.Empty)
            {
                LocalPoolObject.SetNameAndTitleNVC(playerCharName.text, playerCharTitle.text);
            }
        }

        /// <summary>
        /// Sets the player's name and title to a specific value.
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="newTitle"></param>
        public void SetNameAndTitle(string newName, string newTitle)
        {
            if (LocalPoolObject != null)
                LocalPoolObject.SetNameAndTitleNVC(newName, newTitle);
        }

        #region Button commands

        #region Label and icon control
        /// <summary>
        /// Hide the player's label completely.
        /// </summary>
        public void HideMyLabel()
        {
            LocalPoolObject.SetNameAndTitleNVC(" ", " ");
        }

        public void OOCLabel()
        {
            LocalPoolObject.SetNameAndTitleNVC("Out of Character", "<color=\"cyan\">" + Networking.LocalPlayer.displayName + "</color>");
        }

        public void STLabel()
        {
            LocalPoolObject.SetNameAndTitleNVC(Networking.LocalPlayer.displayName, "<color=\"orange\">Storyteller</color>");
        }

        // Map icons
        public void HideIcon()
        {
            LocalPoolObject.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "HideIcon");
        }

        public void ShowIcon()
        {
            LocalPoolObject.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ShowIcon");
        }

        //Use power level to represent rank insignia and more.
        public void PowerLevel0()
        {
            LocalPoolObject.SetPowerLevelNVC(0);
        }
        public void PowerLevel1()
        {
            LocalPoolObject.SetPowerLevelNVC(1);
        }
        public void PowerLevel2()
        {
            LocalPoolObject.SetPowerLevelNVC(2);
        }
        public void PowerLevel3()
        {
            LocalPoolObject.SetPowerLevelNVC(3);
        }
        public void PowerLevel4()
        {
            LocalPoolObject.SetPowerLevelNVC(4);
        }
        public void PowerLevel5()
        {
            LocalPoolObject.SetPowerLevelNVC(5);
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
            _LarpLog.AddToLog(toLog);
        }
    }
}