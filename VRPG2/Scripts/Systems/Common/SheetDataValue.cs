using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using TMPro;

namespace GIB.VRPG2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SheetDataValue : UdonSharpBehaviour
    {
        [HideInInspector] public VRPGCharacter CharacterData;
        public string TargetKey;
        private TextMeshProUGUI targetText;

        [Header("Options")]
        [SerializeField] private bool isSubhash;
        [SerializeField] private bool replaceWithDots;
        [SerializeField, HideIf(nameof(HideIfDots))] private char dot;

        public void ClearValue()
        {
            if (targetText == null) targetText = GetComponent<TextMeshProUGUI>();
            targetText.text = "";
        }

        public void AssignValue()
        {
            if (targetText == null) targetText = GetComponent<TextMeshProUGUI>();

            if (isSubhash)
            {
                SetSubhashValue(TargetKey);
            }
            else
            {
                if (replaceWithDots)
                {
                    SetDotsValue(TargetKey, dot);
                }
                else
                {
                    SetSheetValue(TargetKey);
                }
            }
        }

        [Button("Get Name")]
        public void GetValFromName()
        {
            TargetKey = gameObject.name;
        }

        private string GetDots(string targetString, char dot)
        {
            Debug.Log($"Attempting to serialize {targetString} into dots {dot}.");

            if (int.TryParse(targetString, out int value))
            {
                if (value < 1)
                {
                    return string.Empty;
                }

                string result = "";

                for (int i = 1; i <= value; i++)
                {
                    result += dot;

                    if (i % 5 == 0 && i != value)
                    {
                        result += " ";
                    }
                }

                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        private DataDictionary GetCurrentCharacter()
        {
            if (CharacterData == null)
            {
                CharacterData = GameObject.Find("VRPG Character").GetComponent<VRPGCharacter>();
            }
            return CharacterData.CurrentCharacter;
        }



        public void SetSheetValue(string valueString)
        {
            if (GetCurrentCharacter().TryGetValue(valueString, out DataToken value))
            {
                targetText.text = value.String;
            }
        }

        public void SetDotsValue(string valueString, char dot)
        {
            if (GetCurrentCharacter().TryGetValue(valueString, out DataToken value))
            {
                targetText.text = GetDots(value.String, dot);
            }
        }

        public void SetSubhashValue(string valueString)
        {
            if (GetCurrentCharacter().TryGetValue(valueString, out DataToken value))
            {
                string tempStr = value.String;
                if (replaceWithDots)
                {
                    tempStr = tempStr.Replace("1", "a ?");
                    tempStr = tempStr.Replace("2", "??");
                    tempStr = tempStr.Replace("3", "???");
                    tempStr = tempStr.Replace("4", "????");
                    tempStr = tempStr.Replace("5", "?????");
                }
                tempStr = tempStr.Replace('$', '\n');
                targetText.text = tempStr;
            }
        }

        // inspector

        private bool HideIfDots() => !replaceWithDots;
    }
}
