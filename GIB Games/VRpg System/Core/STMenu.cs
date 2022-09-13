
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace GIB.VRpg
{
    public class STMenu : UdonSharpBehaviour
    {
        public STData sTData;

        public GameObject characterSelectPanel;
        public GameObject characterSheetPanel;
        public GameObject worldPanel;

        public void ShowCharacterSelectPanel()
        {
            characterSelectPanel.SetActive(true);

            worldPanel.SetActive(false);
            characterSheetPanel.SetActive(false);
        }

        public void ShowWorldPanel()
        {
            worldPanel.SetActive(true);

            characterSelectPanel.SetActive(false);
            characterSheetPanel.SetActive(false);
        }

        public STData GetSTData()
        {
            if (sTData == null)
                sTData = GameObject.Find("ST Data").GetComponent<STData>();

            return sTData;
        }
    }
}
