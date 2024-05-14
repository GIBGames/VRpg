
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace GIB.VRpg
{

    public class VRpgMainMenu : VRpgComponent
    {
        [Header("References")]
        [SerializeField] private InputField nameField;
        [SerializeField] private InputField titleField;


        public void SetNameAndTitle()
        {
            VRpg.LocalPoolObject.SetNameAndTitleNVC(nameField.text, titleField.text);
        }

        public void SetOOC()
        {
            string oocColorHex = Utils.GetColorHex(VRpg.OocLabelColor);
            VRpg.LocalPoolObject.SetNameAndTitleNVC($"<color=#{oocColorHex}>Out of Character</color>", Networking.LocalPlayer.displayName);
        }

        public void SetHidden()
        {
            VRpg.LocalPoolObject.SetNameAndTitleNVC("", "");
        }
    }
}
