using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using GIB.VRpg;

namespace Cyan.PlayerObjectPool
{
    public class LarpPoolEventListener : UdonSharpBehaviour
    {
        public CyanPlayerObjectAssigner objectPool;
        private LarpPooledPlayer _localPoolObject;

        void Start()
        {
            DisableInteractive = true;
        }

        [PublicAPI]
        public void _OnLocalPlayerAssigned()
        {
            Debug.Log("The local player has been assigned an object from the pool!");

            // Get the local player's pool object so we can later perform operations on it.
            _localPoolObject = (LarpPooledPlayer)objectPool._GetPlayerPooledUdon(Networking.LocalPlayer);

            // Allow the user to interact with this object.
            DisableInteractive = false;
        }

        [PublicAPI, HideInInspector]
        public VRCPlayerApi playerAssignedPlayer;
        [PublicAPI, HideInInspector]
        public int playerAssignedIndex;
        [PublicAPI, HideInInspector]
        public UdonBehaviour playerAssignedPoolObject;
        [PublicAPI]
        public void _OnPlayerAssigned()
        {
            Debug.Log($"Object {playerAssignedIndex} assigned to player {playerAssignedPlayer.displayName} {playerAssignedPlayer.playerId}");
        }

        [PublicAPI, HideInInspector]
        public VRCPlayerApi playerUnassignedPlayer;
        [PublicAPI, HideInInspector]
        public int playerUnassignedIndex;
        [PublicAPI, HideInInspector]
        public UdonBehaviour playerUnassignedPoolObject;
        [PublicAPI]
        public void _OnPlayerUnassigned()
        {
            Debug.Log($"Object {playerUnassignedIndex} unassigned from player {playerUnassignedPlayer.displayName} {playerUnassignedPlayer.playerId}");
        }
    }
}