using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;
using VRC.SDK3.Data;

namespace GIB.VRpg
{
    public class DiceRoller5E : VRpgRollerBase
    {
        //[SerializeField] private InputField modifierBox;
        [SerializeField] private TextMeshProUGUI dicePoolBoxText;
        [SerializeField] private TextMeshProUGUI modLabel;
        private int currentMod;
        private DataList dicePool = new DataList();

        public void Roll20()
        {
            int result = Random.Range(1, 21) + currentMod;
            SendResult($"{Networking.LocalPlayer.displayName} rolled 1d20 {GetModString()}: {result}");
        }

        public void Roll20Advantage()
        {
            int roll1 = Random.Range(1, 21);
            int roll2 = Random.Range(1, 21);
            int result = Mathf.Max(roll1, roll2) + currentMod;
            SendResult($"{Networking.LocalPlayer.displayName} rolled 1d20 with advantage {GetModString()}: {result} (Rolls: {roll1}, {roll2})");
        }

        public void Roll20Disadvantage()
        {
            int roll1 = Random.Range(1, 21);
            int roll2 = Random.Range(1, 21);
            int result = Mathf.Min(roll1, roll2) + currentMod;
            SendResult($"{Networking.LocalPlayer.displayName} rolled 1d20 with disadvantage {GetModString()}: {result} (Rolls: {roll1}, {roll2})");
        }

        private string GetModString()
        {
            return (currentMod < 0 ? $"<color=\"red\"> - {currentMod * -1}" : $"<color=\"green\"> + {currentMod}")+"</color>";
        }

        // Methods to add dice to the box
        public void BoxAddD20()
        {
            dicePool.Add(new DataToken("d20"));
            UpdateDicePoolBoxText();
        }

        public void BoxAddD12()
        {
            dicePool.Add(new DataToken("d12"));
            UpdateDicePoolBoxText();
        }

        public void BoxAddD10()
        {
            dicePool.Add(new DataToken("d10"));
            UpdateDicePoolBoxText();
        }

        public void BoxAddD8()
        {
            dicePool.Add(new DataToken("d8"));
            UpdateDicePoolBoxText();
        }

        public void BoxAddD6()
        {
            dicePool.Add(new DataToken("d6"));
            UpdateDicePoolBoxText();
        }

        public void BoxAddD4()
        {
            dicePool.Add(new DataToken("d4"));
            UpdateDicePoolBoxText();
        }

        public void ResetBox()
        {
            dicePool.Clear();
            UpdateDicePoolBoxText();
        }

        public void RollBox()
        {
            int total = 0;
            string diceResults = "";
            for (int i = 0; i <= dicePool.Count-1; i++)
            {
                int roll = 0;
                switch (dicePool[i].String)
                {
                    case "d20":
                        roll = Random.Range(1, 21);
                        break;
                    case "d12":
                        roll = Random.Range(1, 13);
                        break;
                    case "d10":
                        roll = Random.Range(1, 11);
                        break;
                    case "d8":
                        roll = Random.Range(1, 9);
                        break;
                    case "d6":
                        roll = Random.Range(1, 7);
                        break;
                    case "d4":
                        roll = Random.Range(1, 5);
                        break;
                }
                total += roll;
                diceResults += $"{dicePool[i].String}: {roll}, ";
            }
            total += currentMod;
            string modString = currentMod < 0 ? $"- {currentMod * -1}" : $"+ {currentMod}";
            SendResult($"{Networking.LocalPlayer.displayName} rolled {diceResults}Total {modString}: {total}");
        }

        // Update the dice pool box text
        private void UpdateDicePoolBoxText()
        {
            string dicePoolText = "Dice Pool: ";
            for (int i = 0; i <= dicePool.Count; i++)
            {
                dicePoolText += dicePool[i].String + " ";
            }
            modLabel.text = currentMod.ToString();
            dicePoolBoxText.text = dicePoolText.TrimEnd() + GetModString();
        }

        // Modifier box
        public void IncreaseMod() => SetMod(currentMod + 1);
        public void DecreaseMod() => SetMod(currentMod - 1);
        public void ResetMod() => SetMod(0);

        public void SetMod(int target)
        {
            currentMod = target;
            UpdateDicePoolBoxText();
        }
    }
}
