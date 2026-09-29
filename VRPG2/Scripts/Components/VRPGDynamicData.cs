/**
 * VRPGManager.cs by Toast https://github.com/dorktoast
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using GIB.VRPG2;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class VRPGDynamicData : VRPGComponent
{
    [SerializeField] private string targetKey;
    public VRPGTextElement text;

    public void OnConfigDataDownloaded()
    {
        DataToken token = new DataToken();

        if (VRPG.ConfigData.ConfigData.TryGetValue(targetKey, out token))
        {
            text.SetText(token.String);
        }
    }
}
