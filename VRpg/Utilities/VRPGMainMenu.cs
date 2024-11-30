
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace GIB.VRPG2
{

    public class VRPGMainMenu : VRPGComponent
    {
        [Header("References")]
        [SerializeField] private InputField nameField;
        [SerializeField] private InputField titleField;


        public void SetNameAndTitle()
        {
            if(Utilities.IsValid(VRPG.LocalPlayerObject))
                VRPG.LocalPlayerObject.SetNameAndTitle(nameField.text, titleField.text);
        }

        public void SetOOC()
        {
            string oocColorHex = Utils.GetColorHex(VRPG.OocLabelColor);
            if (Utilities.IsValid(VRPG.LocalPlayerObject))
                VRPG.LocalPlayerObject.SetNameAndTitle($"<color=#{oocColorHex}>Out of Character</color>", Networking.LocalPlayer.displayName);
        }

        public void SetHidden()
        {
            if (Utilities.IsValid(VRPG.LocalPlayerObject))
                VRPG.LocalPlayerObject.SetNameAndTitle("", "");
        }
    }
}
