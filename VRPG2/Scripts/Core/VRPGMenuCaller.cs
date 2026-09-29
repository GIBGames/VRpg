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
	public class VRPGMenuCaller : VRPGComponent
	{
		[SerializeField] private CallerPosition callerPosition;

		[SerializeField] private Vector3 offset;
		
		private bool isVRPlayer;

		#region Unity Methods

		void Start()
		{
			if (Networking.LocalPlayer.IsUserInVR())
			{
				isVRPlayer = true;
			}
		}

		private void Update()
		{
			if (isVRPlayer) return;

			if (Input.GetKeyDown(KeyCode.Q))
                VRPG.Menu.SwapMenuState();
        }

        private void FixedUpdate()
        {
			if (Networking.LocalPlayer == null) return;

			VRCPlayerApi.TrackingData callerTrack = new VRCPlayerApi.TrackingData();
			
			switch (callerPosition)
            {
				case CallerPosition.Head:
					callerTrack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
					break;
				case CallerPosition.LeftHand:
					callerTrack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);
					break;
                case CallerPosition.RightHand:
					callerTrack = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand);
					break;
                default:
                    break;
            }
			Vector3 finalPosition = callerTrack.position + offset;

			transform.SetPositionAndRotation(finalPosition, transform.rotation);
		}

        public override void Interact()
        {
            VRPG.Menu.SwapMenuState();
        }

        #endregion
    }

	public enum CallerPosition
    {
		Head,
		LeftHand,
		RightHand
    }
}