
using GIB.VRPG2;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Enums;
using VRC.Udon.Common.Interfaces;

public class WorldVersionCheck : VRPGComponent
{
    public VRCUrl targetUrl;
    public string ConfigWorldVersion;
    public string CheckedWorldVersion;

    [SerializeField] private float delaySeconds = 90f;
    [SerializeField] private float delayClock;

    [SerializeField] private AudioSource alertSource;
    [SerializeField] private AudioClip alertClip;
    [SerializeField] private string noticeText;

    private bool gotConfig;

    public void OnConfigDataDownloaded()
    {
        ConfigWorldVersion = VRPG.ConfigData.GetValue("WorldVersion");
        noticeText = VRPG.ConfigData.GetValue("VersionChangeNotice");
        gotConfig = true;
    }

    public void CheckVersion()
    {
        VRCStringDownloader.LoadUrl(targetUrl, (IUdonEventReceiver)this);
    }

    private void FixedUpdate()
    {
        if (!gotConfig) return;

        delayClock -= Time.fixedDeltaTime;
        if (delayClock < 0)
        {
            delayClock = delaySeconds;
            CheckVersion();
        }
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        DoWorldVersion(result.Result);
    }

    public void DoWorldVersion(string version)
    {
        if (ConfigWorldVersion == "" || !gotConfig) return;

        CheckedWorldVersion = version;

        if (CheckedWorldVersion != ConfigWorldVersion)
        {
            alertSource.PlayOneShot(alertClip);

            if (Networking.LocalPlayer.isMaster)
            {
                VRPG.Logger.SendLog(noticeText, VRPGLogType.OOC, true);
            }

        }
    }
}
