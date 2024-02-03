/**
 * VRpgIconElement.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRpg Project Repo: https://github.com/GIBGames/VRpg
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
	/// <summary>
	/// Displays and changes icon elements in VRpg.
	/// </summary>
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	[RequireComponent(typeof(Image))]
	public class VRpgIconElement : VRpgComponent
	{
		[SerializeField] private Image element;
		[SerializeField] private Color defaultColor = Color.white;
		
		private void Start() => element = GetComponent<Image>();
		public void Clear()
		{
			element.sprite = null;
			element.color = Color.clear;
		}
		public void SetTransparent() => element.color = Color.clear;
		public void SetColor(Color color) => element.color = color;
		public void ResetColor()=> element.color = defaultColor;
		public void ChangeImage(Sprite sprite)
		{
			element.sprite = sprite;
			element.color = defaultColor;
		}
	}
}