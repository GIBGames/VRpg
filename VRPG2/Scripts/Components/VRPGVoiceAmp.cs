
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRPGVoiceAmp : UdonSharpBehaviour
{
    [SerializeField] private float targetGain = 15;
    [SerializeField] private float targetFar = 25;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        player.SetVoiceDistanceFar(targetFar);
        player.SetVoiceGain(targetGain);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        player.SetVoiceDistanceFar(25);
        player.SetVoiceGain(15);
    }
}
