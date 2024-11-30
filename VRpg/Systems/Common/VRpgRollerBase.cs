
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;

namespace GIB.VRPG2
{
    public class VRPGRollerBase : VRPGComponent
    {
        [SerializeField] private Toggle gmToggle;

        public void SendResult(string message)
        {
            VRPG.Logger.SendLog(message, gmToggle.isOn ? VRPGLogType.GM : VRPGLogType.OOC);
        }
    }
}
