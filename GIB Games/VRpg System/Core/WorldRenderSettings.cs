
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class WorldRenderSettings : UdonSharpBehaviour
{
    [Header("Fog")]
    [SerializeField] private bool fogArea;

    void Start()
    {
#if UNITY_ANDORID
        fogOn = false;
#endif
        RenderSettings.fog = false;
    }

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if(player.isLocal)
            RenderSettings.fog = false;
    }

    public void EnterFog() => SetFog(true);
    public void ExitFog() => SetFog(false);

    public void SetFog(bool state)
    {
        fogArea = state;
        RenderSettings.fog = fogArea;
    }
}
