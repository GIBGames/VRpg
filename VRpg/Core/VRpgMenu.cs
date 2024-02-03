/**
 * VRpgLocalMenu.cs by Toast https://github.com/dorktoast - 11/12/2023
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
    /// A handler for management of the VRpg Menu
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRpgMenu : VRpgComponent
    {
        [Header("Panels")]
        [SerializeField] private GameObject[] panels;

        private bool menuIsOpen;
        private Vector3 menuStartPosition;
        private Quaternion menuStartRotation;

        #region Unity/Udon

        private void Start()
        {
            menuStartPosition = transform.position;
            menuStartRotation = transform.rotation;
        }

        public void SwapMenuState()
        {
            if (menuIsOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        #endregion

        public void OpenMenu()
        {
            menuIsOpen = true;
            PositionMenu();
        }

        public void CloseMenu()
        {
            menuIsOpen = false;
            transform.SetPositionAndRotation(menuStartPosition, menuStartRotation);
        }

        public void PositionMenu()
        {
            Vector3 playerHeadPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Quaternion playerHeadRot = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

            transform.SetPositionAndRotation(playerHeadPos, playerHeadRot);

            transform.position += transform.forward * .9f;
        }

        #region panels

        public void ShowPanel(int index)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                if (i == index)
                {
                    panels[i].SetActive(true);
                }
                else
                {
                    panels[i].SetActive(false);
                }
            }

            panels[index].SetActive(true);
        }

        #endregion

    }

}