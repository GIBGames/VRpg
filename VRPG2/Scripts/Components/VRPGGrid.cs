
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRPGGrid : UdonSharpBehaviour
{
    public Transform GridObject;
    public float GridOffset = -1;
    public UnityEngine.UI.Slider gridSlider;
    private void FixedUpdate()
    {
        //VRCPlayerApi myPlayerTracking = Networking.LocalPlayer
        //Vector3 headPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        //GridObject.position = headPos + (Vector3.up * GridOffset);
    }

    public void UpdateGridOffset()
    {
        //GridOffset = gridSlider.value;
    }
}
