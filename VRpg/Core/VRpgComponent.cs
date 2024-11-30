/**
 * VRPGComponent.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRPG2
{
	/// <summary>
	/// Summary of Class
	/// </summary>
	public class VRPGComponent : UdonSharpBehaviour
	{
        private VRPGManager vrpgManager;
        public VRPGManager VRPG
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
            vrpgManager = GameObject.Find("VRPG Manager").GetComponent<VRPGManager>();
        }
	}
}