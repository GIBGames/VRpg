/**
 * VRPGMenuNavButton.cs by Toast https://github.com/dorktoast - 11/16/2023
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
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRPGMenuNavButton : VRPGComponent
	{
		[SerializeField] private int panelIndex;

		public void ShowPanel()
        {
			VRPG.Menu.ShowPanel(panelIndex);
        }
	}
}