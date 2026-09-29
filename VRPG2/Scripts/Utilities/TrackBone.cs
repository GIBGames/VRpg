/**
 * VRPGMenuCaller.cs by Toast https://github.com/dorktoast - 11/16/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRPG2
{
    /// <summary>
    /// Object that calls the VRPG Menu to the player.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class TrackBone : VRPGComponent
    {
        [SerializeField] private VRCPlayerApi.TrackingDataType callerPosition;

        [SerializeField] private Vector3 offset;

        #region Unity Methods



        private void FixedUpdate()
        {
            if (Networking.LocalPlayer == null) return;

            VRCPlayerApi.TrackingData callerTrack = Networking.LocalPlayer.GetTrackingData(callerPosition);

            Vector3 finalPosition = callerTrack.position + offset;

            transform.SetPositionAndRotation(finalPosition, transform.rotation);
        }

        #endregion
    }
}