using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LocalSheetCaller : UdonSharpBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    [SerializeField] private Transform LarpMenuObject;
    private bool menuIsOpen;

    private void Start()
    {
        if (LarpMenuObject == null)
            LarpMenuObject = GameObject.Find("VRPG Local Menu").transform;

        startPos = LarpMenuObject.position;
        startRot = LarpMenuObject.rotation;
    }

    private void Update()
    {
        bool menuButtonPressed = Input.GetKeyDown(KeyCode.Tab);
        if (menuButtonPressed)
        {
            Interact();
        }
    }

    public override void Interact()
    {
        if (menuIsOpen)
            CloseLarpMenu();
        else
            OpenLarpMenu();
    }

    private void FixedUpdate()
    {
        if (Networking.LocalPlayer == null) return;

        VRCPlayerApi.TrackingData localHead = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        //if (localHead != null)
        transform.SetPositionAndRotation(localHead.position, localHead.rotation);
    }

    private void PositionLarpMenu()
    {
        Vector3 playerHeadPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Quaternion playerHeadRot = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

        LarpMenuObject.SetPositionAndRotation(playerHeadPos, playerHeadRot);

        LarpMenuObject.position += transform.forward * .9f;
    }

    public void OpenLarpMenu()
    {
        menuIsOpen = true;
        PositionLarpMenu();
    }

    public void CloseLarpMenu()
    {
        menuIsOpen = false;

        LarpMenuObject.transform.SetPositionAndRotation(startPos, startRot);
    }
}
