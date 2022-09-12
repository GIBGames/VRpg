using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using GIB.VRpg;
public class VoiceAmplifier : UdonSharpBehaviour
{
    [SerializeField] private CharacterHandler characterHandler;
    [SerializeField,Range(0f,10f)] private float targetNear = 0;
    [SerializeField,Range(10f,50f)] private float targetFar = 25;
    [SerializeField] private float targetGain = 15;
    [SerializeField] private bool useLowpass = true;

    private void Start()
    {
        if (characterHandler == null)
            characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        player.SetVoiceDistanceNear(targetNear);
        player.SetVoiceDistanceFar(targetFar);
        player.SetVoiceGain(targetGain);
        player.SetVoiceLowpass(useLowpass);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        player.SetVoiceDistanceNear(0);
        player.SetVoiceDistanceFar(25);
        player.SetVoiceGain(15);
        player.SetVoiceLowpass(true);
    }
}
