/**
 * VRpgMenuCaller.cs by Toast https://github.com/dorktoast - 11/16/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
	/// <summary>
	/// Object that calls the VRpg Menu to the player.
	/// </summary>
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRpgMenuCaller : VRpgComponent
	{
		[SerializeField] private CallerPosition callerPosition;
		
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
				Interact();
		}

        private void FixedUpdate()
        {
			if (!isVRPlayer || Networking.LocalPlayer == null) return;

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

			transform.SetPositionAndRotation(callerTrack.position, transform.rotation);
		}

        public override void Interact()
        {
			VRpg.Menu.SwapMenuState();
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