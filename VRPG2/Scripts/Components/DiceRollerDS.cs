using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;
using VRC.SDK3.Data;
using Unity.Collections.LowLevel.Unsafe;

namespace GIB.VRPG2
{
    public class DiceRollerDS : VRPGRollerBase
    {
        [SerializeField] private Toggle GmToggle;
        [SerializeField] private TMP_InputField attBox;
        [SerializeField] private TMP_InputField defBox;
        [SerializeField] private TMP_Dropdown attDrop;
        [SerializeField] private TMP_Dropdown defDrop;
        [SerializeField] private TextMeshProUGUI modLabel;
        private int currentMod;
        private DataList dicePool = new DataList();

        private byte diceIterations;


        public void NewRoll()
        {
            diceIterations = 0;
            Roll();
        }

        public void Roll()
        {
            // get character name
            string myName = $"{Networking.LocalPlayer.displayName}";

            // roll d10s
            int attRoll = Random.Range(1, 21);
            int defRoll = Random.Range(1, 21);

            // get entered modifiers and check if they have valid modifier values
            int attValue;
            int defValue;
            bool hasAttMod = int.TryParse(attBox.text, out attValue);
            bool hasDefMod = int.TryParse(defBox.text, out defValue);

            // calculate total
            int attTotal = attValue + attRoll;
            int defTotal = defValue + defRoll;
            bool isTie = attTotal == defTotal;

            // get +0, +1, or +2 threat
            int attThreat = attDrop.value;
            int defThreat = defDrop.value;

            // either side is exceptional if they rolled inside the threat range (10 at +0, 9 at +1, etc.)
            bool attExSuccess = attRoll + attThreat >= 20;
            bool defExSuccess = defRoll + defThreat >= 20;

            // determine winner
            bool shouldWin = attRoll != 1 && (attTotal > defTotal || defRoll == 1);
            bool fullBotch = attRoll == 1 && defRoll == 1;

            // create color string for die roll
            string attColorDie = (attExSuccess ? "<color=#00FF00>" : attRoll == 1 ? "<color=#FF5500>" : "<color=#FFFFFF>") + $"{attRoll}</color>";
            string defColorDie = (defExSuccess ? "<color=#00FF00>" : defRoll == 1 ? "<color=#FF5500>" : "<color=#FFFFFF>") + $"{defRoll}</color>";

            string successString = "";

            if (hasDefMod && fullBotch)
            {
                successString = "Both players rolled a 1! <color=#FF5500>You both suck!</color>";
            }
            else if (hasDefMod && isTie)
            {
                diceIterations++;
                successString = "Tie! Rerolling!";
                DoTieReroll();
            }
            else if (shouldWin)
            {
                successString += "<color=#00FF00>";
                if (attExSuccess)
                {
                    successString += "Exceptional ";
                }
                successString += "Success!</color>";
            }
            else
            {
                successString += "<color=#FF5500>";
                if (defExSuccess)
                {
                    successString += "Exceptional ";
                }
                successString += "Defense!</color>";
            }

            string resultString;
            if (hasDefMod) // player v player
            {
                resultString = $"{myName} rolled {attColorDie} + {attValue} = {attTotal} v. {defColorDie} + {defValue} = {defTotal}: {successString}";
            }
            else //single roll
            {
                resultString = $"{myName} rolled {attColorDie} + {attValue} = {attTotal}!";
            }

            if (GmToggle.isOn)
            {
                VRPG.Logger.LogGMRaw(resultString);
            }
            else
            {
                VRPG.Logger.LogRaw(resultString);
            }
        }

        public void ClearBox()
        {
            attBox.text = "";
            defBox.text = "";
            attDrop.value = 0;
            defDrop.value = 0;
        }

        public void DoTieReroll()
        {
            if (diceIterations > 2)
            {
                VRPG.Logger.LogRaw("Too many rerolls due to tie. Please try again!");
            }
            else
            {
                SendCustomEventDelayedSeconds("Roll", 1f);
            }
        }
    }
}
