using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;
using VRC.SDK3.Persistence;
using VRC.SDK3.Data;

namespace GIB.VRPG2
{
    [CustomName("VRPG Voice Zone Controller")]
    public class VRPGVoiceController : VRPGComponent
    {
        [Header("Voice Zone Levels")]

        [Tooltip("The player's voice when in the same Voice Zone or not in a voice zone.")]
        public float InZone = 25f;
        [Tooltip("The player's voice when in a different voice zone.")]
        public float OutZone = 0f;
        [Tooltip("The player's voice when in the same zone but a different channel.")]
        public float OutChannel = 1f;

        public bool ListenAllChannels;
        public UnityEngine.UI.Toggle allChannelsToggle;
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            player.SetVoiceDistanceNear(0);
            player.SetVoiceDistanceFar(25);
            player.SetVoiceLowpass(true);
        }

        public void UpdateVoiceZones()
        {
            // If players are in the same zone and channel, act as normal (InZone).
            // If they are in the same zone and the player is on the same channel the speaker is speaking on OR the speaker is on channel 0,
            // act as normal (InZone).
            // If they are in the same zone and the player is not on the same channel, set the speaker voice to low (OutChannel).
            // Otherwise, mute the speaker (OutZone).

            byte myZone = PlayerData.GetByte(Networking.LocalPlayer, "vrpg-voiceZone");
            byte myListenChannels = PlayerData.GetByte(Networking.LocalPlayer, "vrpg-listenChannel");

            var allPlayers = VRPG.Social.GetAllPlayers();

            for (int i = 0; i < allPlayers.Length; i++)
            {
                var targetZone = PlayerData.GetByte(allPlayers[i], "vrpg-voiceZone");
                var targetSpeakChannel = PlayerData.GetByte(allPlayers[i], "vrpg-speakChannel");

                bool zoneMatch = targetZone == 0 || myZone == targetZone;
                bool channelMatch = ListenAllChannels || targetSpeakChannel == 0 || myListenChannels == targetSpeakChannel;

                if (zoneMatch)
                {
                    if(channelMatch)
                    {
                        allPlayers[i].SetVoiceDistanceFar(InZone);
                    }
                    else
                    {
                        allPlayers[i].SetVoiceDistanceFar(OutChannel);
                    }
                }
                else
                {
                    allPlayers[i].SetVoiceDistanceFar(OutZone);
                }
            }
        }

        public void SetListenAllChannels()
        {
            if(allChannelsToggle != null)
            {
                ListenAllChannels = allChannelsToggle.isOn;
            }
        }
    }
}