
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;

namespace GIB.VRpg
{
    public class VRpgRollerBase : VRpgComponent
    {
        [SerializeField] private Toggle gmToggle;

        public void SendResult(string message)
        {
            VRpg.Logger.SendLog(message, gmToggle.isOn ? LogType.GM : LogType.OOC);
        }
    }
}
