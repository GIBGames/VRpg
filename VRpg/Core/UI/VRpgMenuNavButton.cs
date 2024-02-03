/**
 * VRpgMenuNavButton.cs by Toast https://github.com/dorktoast - 11/16/2023
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
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRpgMenuNavButton : VRpgComponent
	{
		[SerializeField] private int panelIndex;

		public void ShowPanel()
        {
			VRpg.Menu.ShowPanel(panelIndex);
        }
	}
}