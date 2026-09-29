using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRPG2
{
    public class VRPGPlayerTeleport : VRPGComponent
    {
        [SerializeField] private Animator teleAnim;
        [SerializeField] private GameObject FadeTarget;
        Vector3 targetPos;
        Quaternion targetRot;
        bool isTeleporting;

        public void TeleportPlayer(Vector3 playerPosition, Quaternion playerRotation)
        {
            if (isTeleporting) return;

            isTeleporting = true;
            FadeTarget.SetActive(true);
            FadeTarget.transform.position = playerPosition;
            VRPG.Menu.CloseMenu();
            targetPos = playerPosition;
            targetRot = playerRotation;
            teleAnim.SetBool("isDark", true);
            SendCustomEventDelayedSeconds("DoTeleport", 1f);
        }

        public void DoTeleport()
        {
            if (!isTeleporting) return;
            Networking.LocalPlayer.TeleportTo(targetPos, targetRot);
            SendCustomEventDelayedSeconds("DoEndAnim", .1f);
        }

        public void DoEndAnim()
        {
            FadeTarget.SetActive(false);
            teleAnim.SetBool("isDark", false);
            isTeleporting = false;
        }
    }
}
