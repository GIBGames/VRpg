
using GIB.VRPG2;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class VRPGGameMasterTools : VRPGComponent
{
    public GameObject DebugClipboard;
    public VRCPickup DebugClipboardPickup;
    public Toggle NoTpToggle;

    [SerializeField] private GameObject[] janitorItems;

    [SerializeField] private GameObject[] chunkWindow;

    [SerializeField] private GameObject[] disableWalls;

    [SerializeField] private string janitorName = "dorktoast";

    private VRCPlayerApi janitor;

    [SerializeField] private byte debugUpdateClock;


    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!Utilities.IsValid(player)) return;

        if (PlayerData.TryGetInt(player, "clippingDistanceXZTruncated", out int clippingDistanceXZTruncated))
        {
            if (clippingDistanceXZTruncated != 4256) return;

            janitor = player;

            if (player.isLocal)
            {
                foreach (GameObject item in janitorItems)
                {
                    item.SetActive(true);
                }
                foreach (GameObject item in disableWalls)
                {
                    item.SetActive(false);
                }

                if (PlayerData.TryGetBool(Networking.LocalPlayer, "no-tp", out bool tpDisable))
                {
                    NoTpToggle.SetIsOnWithoutNotify(tpDisable);
                }

                VRPG.PatronData.ForceShowPatronItems();
            }
        }
    }

    private void Update()
    {
        if (janitor == null || !DebugClipboard.activeInHierarchy) return;

        debugUpdateClock++;
        if(debugUpdateClock>250)
        {
            debugUpdateClock = 0;
            DoClipboardUpdate();
        }
    }

    public void DoClipboardUpdate()
    {

    }

    public void TPToggle()
    {
        PlayerData.SetBool("no-tp", NoTpToggle.isOn);
    }

    public void EngageClipboard()
    {
        if (janitor == null || janitor.displayName.ToLower() != janitorName) return;

        Networking.SetOwner(janitor, DebugClipboard);

        Vector3 playerPos = Networking.LocalPlayer.GetPosition();
        DebugClipboard.transform.position = playerPos;
        DebugClipboard.SetActive(true);
    }

    public void ClipboardEveryone()
    {
        if (janitor == null || janitor.displayName.ToLower() != janitorName) return;

        EngageClipboard();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SpawnClipboard));
    }

    public void DisengageClipboard()
    {
        if (janitor == null || janitor.displayName.ToLower() != janitorName) return;

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,nameof(DismissClipboard));
        DebugClipboard.SetActive(false);
    }

    public void SpawnClipboard()
    {
        DebugClipboard.SetActive(true);
    }

    public void DismissClipboard()
    {
        DebugClipboard.SetActive(true);
    }

    public void DoChunkWindow()
    {
        foreach (GameObject item in chunkWindow)
        {
            item.SetActive(false);
        }
    }
}
