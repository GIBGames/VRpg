using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace GIB.VRpg
{
    /// <summary>
    /// An in-game menu that allows players and STs to perform special functions.
    /// </summary>
    public class LarpMenu : UdonSharpBehaviour
    {
        [SerializeField] private CharacterHandler characterHandler;
        public bool menuIsOpen;

        [Header("Panels")]
        [SerializeField] private GameObject[] panels;

        /// <summary>
        /// The panel containing ST functions.
        /// </summary>
        [Header("ST Panel")]
        [SerializeField] private GameObject stFunctions;

        private void Start()
        {
            if (characterHandler == null)
                characterHandler = GameObject.Find("CharacterHandler").GetComponent<CharacterHandler>();
        }

        /// <summary>
        /// Show a target panel in the menu.
        /// </summary>
        /// <param name="index"></param>
        public void ShowPanel(int index)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                if (i == index)
                {
                    panels[i].SetActive(true);
                }
                else
                {
                    panels[i].SetActive(false);
                }
            }

            panels[index].SetActive(true);
        }

        #region Button references

        //You can rename and change these as you need.
        public void ShowMain()
        {
            ShowPanel(0);
        }

        public void ShowPower()
        {
            ShowPanel(1);
        }

        public void ShowChar()
        {
            ShowPanel(2);
        }

        public void ShowMap()
        {
            ShowPanel(3);
        }

        public void ShowST()
        {
            stFunctions.SetActive(characterHandler.LocalPoolObject.isStoryteller);
            ShowPanel(4);
        }
        #endregion
    }
}
