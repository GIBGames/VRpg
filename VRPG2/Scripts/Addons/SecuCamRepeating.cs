
using GIB.VRPG2;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SecuCamRepeating : VRPGComponent
{
    [SerializeField] private GameObject targetActive;
    [SerializeField] private Camera _camera;

    [SerializeField] private float camClock;

    private void FixedUpdate()
    {
        camClock -= Time.fixedDeltaTime;
        if (camClock > 0) return;

        if (targetActive.activeInHierarchy)
            TakeShot();

        camClock = 1f;
    }

    public void TakeShot()
    {
        if(targetActive.activeInHierarchy)
            _camera.Render();
    }
}
