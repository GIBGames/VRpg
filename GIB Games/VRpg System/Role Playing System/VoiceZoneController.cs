
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VoiceZoneController : UdonSharpBehaviour
{
    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        player.SetVoiceDistanceNear(0);
        player.SetVoiceDistanceFar(25);
        player.SetVoiceGain(15);
        player.SetVoiceLowpass(true);
    }
}