/**
 * VRPGRegions.cs by Toast https://github.com/dorktoast - 11/23/2023
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
    public class VRPGRegions : VRPGComponent
    {
        public GameObject[] Regions;

        public void LoadSingleRegion(int regionIndex)
        {
            for (int i = 0; i < Regions.Length; i++)
            {
                Regions[i].SetActive(i == regionIndex);
            }
        }

        public void LoadAdditionalRegion(int regionIndex)
        {
            Regions[regionIndex].SetActive(true);
        }

        public void LoadAllRegions()
        {
            foreach (GameObject g in Regions)
            {
                g.SetActive(true);
            }
        }

        public void LoadTargetRegion(int region)
        {
            if (region == 0)
                LoadAllRegions();
            else
                LoadSingleRegion(region);
        }
    }
}
