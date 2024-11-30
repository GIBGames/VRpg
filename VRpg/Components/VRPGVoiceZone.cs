using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using GIB.VRPG2;
using UdonToolkit;

public class VRPGVoiceZone : VRPGComponent
{
    [SerializeField] private int zoneId;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        VRPG.HandlerLog($"Player {player.displayName} entered voice zone {zoneId}");

        if (!player.isLocal) return;

        VRPG.LocalPlayerObject.SetVoiceZone( zoneId );
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        VRPG.HandlerLog($"Player {player.displayName} exited voice zone {zoneId} into common zone.");

        if (!player.isLocal) return;

        VRPG.LocalPlayerObject.SetVoiceZone(0);
    }
}

