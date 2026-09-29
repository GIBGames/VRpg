using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UdonToolkit;
using GIB.VRPG2;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Teleporter : VRPGComponent
{
    public Transform teleportTarget;

    public override void Interact()
    {
        Trigger();
    }

    public void Trigger()
    {
        HandlePlayerTeleport();
    }

    private void HandlePlayerTeleport()
    {
        //VRPG.RegionManager.LoadAllRegions();
        VRPG.Teleporter.TeleportPlayer(teleportTarget.position, teleportTarget.rotation);
    }
}
