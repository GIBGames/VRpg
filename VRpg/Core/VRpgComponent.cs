/**
 * VRpgComponent.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
	/// <summary>
	/// Summary of Class
	/// </summary>
	public class VRpgComponent : UdonSharpBehaviour
	{
        private VRpgManager vrpgManager;
        public VRpgManager VRpg
        {
            get
            {
                if (vrpgManager == null)
                    CacheManager();
                return vrpgManager;
            }
            private set { }
        }

        public virtual void CacheManager()
        {
            vrpgManager = GameObject.Find("VRpg Manager").GetComponent<VRpgManager>();
        }
	}
}