
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRPG2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class IconController : VRPGComponent
    {
        public Image[] Icons;
        [UdonSynced,HideInInspector] public int iconIndex;
        private int oldIndex;
        public SiteRole currentIconType;

        public void ClearStates()
        {
            SetIcon(0, "");
        }

        public void SetIcon(int target,string key)
        {
            // i use this key format to deter people with modded clients

            if(target == 0)
            {
                if (target == 1 && key != "ch0c0late_milk") return;
                if (target == 2 && key != "49000042566") return;
                if (target == 3 && key != "dorktoast") return;
            }
            iconIndex = target;

            RequestSerialization();
            SyncIcons();
        }

        public void SetIconByRole(SiteRole target,string key) => SetIcon((int)target, key);

        public override void OnDeserialization()
        {
            SyncIcons();
        }

        public void SyncIcons()
        {
            if (iconIndex != oldIndex)
            {
                oldIndex = iconIndex;

                for (int i = 0; i < Icons.Length; i++)
                {
                    Icons[i].gameObject.SetActive(i == iconIndex);
                }
            }

        }
    }

    public enum SiteRole
    {
        Normal,
        Controller,
        Admin,
        Janitor
    }
}
