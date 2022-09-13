
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VoiceZoneController : UdonSharpBehaviour
{
    [Header("Voice Zone Levels")]

    [Tooltip("The player's voice when in the same Voice Zone or not in a voice zone.")]
    public float InVoiceZone = 25f;
    [Tooltip("The player's voice when in a different voice zone.")]
    public float OutVoiceZone = 0f;

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        player.SetVoiceDistanceNear(0);
        player.SetVoiceDistanceFar(25);
        player.SetVoiceGain(15);
        player.SetVoiceLowpass(true);
    }
}