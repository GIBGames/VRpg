using Cyan.PlayerObjectPool;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    public class LarpStreamIcon : UdonSharpBehaviour
    {
        [SerializeField] private CharacterHandler characterHandler;
        public Toggle toggle;

        private void Start()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();
        }

        public void CheckStreamToggle()
        {
            characterHandler.SetStreaming(toggle.isOn);
        }

    }
}
