
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    public class CallSTTrigger : UdonSharpBehaviour
    {
        [SerializeField] private CharacterHandler characterHandler;

        [SerializeField] private bool triggerOnPickup;
        [SerializeField] private bool triggerOnEnter;
        private void Start()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();
        }

        public override void OnPickup()
        {
            if (triggerOnPickup)
            {
                characterHandler.CallSTEvent();
            }
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal && triggerOnEnter)
            {
                characterHandler.CallSTEvent();
            }
        }
    }
}
