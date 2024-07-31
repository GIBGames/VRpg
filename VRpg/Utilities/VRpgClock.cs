/**
 * VRpgClock.cs by Toast https://github.com/dorktoast - 11/16/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using System;

namespace GIB.VRpg
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class VRpgClock : UdonSharpBehaviour
	{
		public byte clockTick;

		[SerializeField] private TextMeshProUGUI dateText;
		[SerializeField] private TextMeshProUGUI timeText;

        #region Unity Methods

        private void FixedUpdate()
        {
			clockTick++;

			if (clockTick == 255)
				UpdateDateAndTime();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        public void UpdateDateAndTime()
        {
			DateTime now = DateTime.Now;

			string dateString = now.ToString("M");
			string timeString = now.ToString("HH:mm");

			dateText.text = dateString;
			timeText.text = timeString;

			clockTick = 0;
        }

		#endregion
	}
}