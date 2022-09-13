using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    public class LarpLog : UdonSharpBehaviour
    {
        [SerializeField] private CharacterHandler characterHandler;

        [SerializeField] private GameObject LogLineParent;
        private Text[] logLines;
        [UdonSynced] public string[] logText;
        private string[] previouslogText;

        [SerializeField] private InputField logBox;

        private void Start()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("VRPG Character Handler").GetComponent<CharacterHandler>();

            logLines = LogLineParent.GetComponentsInChildren<Text>();
        }

        public override void OnDeserialization()
        {
            UpdateLogLines();
        }

        public void UpdateLogLines()
        {
            previouslogText = logText;

            for (int i = 0; i < logLines.Length; i++)
            {
                logLines[i].text = logText[i];
            }
        }

        public void AddToLog(string toLog)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            for (int i = 0; i < logLines.Length - 1; i++)
            {
                logText[i] = logText[i + 1];
            }

            logText[8] = toLog;
            UpdateLogLines();
            RequestSerialization();
        }

        public void LogBox()
        {
            string newText = logBox.text;
            logBox.text = "";
            AddToLog(Networking.LocalPlayer.displayName + ": " + newText);
        }

        public void DoRPS()
        {
            int thisRPS = Random.Range(0, 3);
            string rpsResult = string.Empty;

            switch (thisRPS)
            {
                case 0:
                    rpsResult = "ROCK";
                    break;
                case 1:
                    rpsResult = "PAPER";
                    break;
                case 2:
                    rpsResult = "SCISSORS";
                    break;
            }

            AddToLog("<color=#FFA500>" + Networking.LocalPlayer.displayName + " threw " + rpsResult + "</color>");
        }

        public void RollDie(int die)
        {
            int thisRoll = Random.Range(1, die+1);
            AddToLog($"<color=#FFA500>{Networking.LocalPlayer.displayName} rolled a d{die} and got {thisRoll}</color>");
        }

        //Add button aliases here
        public void Roll20()
        {
            RollDie(20);
        }
    }
}
