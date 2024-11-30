/**
 * VRPGTmpElement.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;
using TMPro;

namespace GIB.VRPG2
{
    /// <summary>
    /// Interprets and displays text in different ways.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRPGTextElement : VRPGComponent
    {
        private TextMeshProUGUI elementTMP;
        private Text elementText;

        private bool isTMP => IsTMPElement();

        private Color startColor;

        public string Text
        {
            get => GetText();
            set => SetText(value);
        }

        private void Start()
        {
            startColor = isTMP ? elementTMP.color : elementText.color;
        }

        public void Clear()
        {
            if (isTMP)
                elementTMP.text = "";
            else
                elementText.text = "";
        }

        private bool IsTMPElement()
        {
            elementTMP = GetComponent<TextMeshProUGUI>();

            if (elementTMP != null)
            {
                return true;
            }
            else
            {
                elementText = GetComponent<Text>();
                return false;
            }
        }
        public void ResetColor()
        {
            if (isTMP)
                elementTMP.color = startColor;
            else
                elementText.color = startColor;
        }

        public void SetColor(Color color)
        {
            if (isTMP)
                elementTMP.color = color;
            else
                elementText.color = color;
        }
        public void SetText(string targetText)
        {
            if (isTMP)
                elementTMP.text = targetText;
            else
                elementText.text = targetText;
        }

        public void SetText(string targetText, TextElementStyle elementStyle)
        {
            string newText = "";

            switch (elementStyle)
            {
                case TextElementStyle.Text:
                    newText = targetText;
                    break;
                case TextElementStyle.Dots_empty:
                    newText = Utils.GetDots(targetText, '\u25CB');
                    break;
                case TextElementStyle.Dots_filled:
                    newText = Utils.GetDots(targetText, '\u25CF');
                    break;
                case TextElementStyle.Squares_empty:
                    newText = Utils.GetDots(targetText, '\u25A1');
                    break;
                case TextElementStyle.Squares_filled:
                    newText = Utils.GetDots(targetText, '\u25A0');
                    break;
                default:
                    break;
            }

            if (isTMP)
                elementTMP.text = newText;
            else
                elementText.text = newText;
        }

        private string GetText()
        {
            if (isTMP)
                return elementTMP.text;
            else
                return elementText.text;
        }

    }

    public enum TextElementStyle
    {
        Text,
        Dots_empty,
        Dots_filled,
        Squares_empty,
        Squares_filled

    }
}