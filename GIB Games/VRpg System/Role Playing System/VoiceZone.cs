using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using GIB.VRpg;

public class VoiceZone : UdonSharpBehaviour
{
    [SerializeField] private CharacterHandler characterHandler;
    [HideInInspector] public int zoneType;
    [SerializeField] private int zoneId;

    private void Start()
    {
        if (characterHandler == null)
            characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        characterHandler.HandlerLog($"Player {player.displayName} entered voice zone {zoneId}");
        LarpPooledPlayer thatPlayer = characterHandler.objectPool._GetPlayerPooledObject(player).GetComponent<LarpPooledPlayer>();
        thatPlayer.currentZone = zoneId;
        thatPlayer._UpdateVoiceZones();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        characterHandler.HandlerLog($"Player {player.displayName} exited voice zone.");
        LarpPooledPlayer thatPlayer = characterHandler.objectPool._GetPlayerPooledObject(player).GetComponent<LarpPooledPlayer>();
        thatPlayer.currentZone = 0;
        thatPlayer._UpdateVoiceZones();
    }
}
