
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SimpleLarpDoor : UdonSharpBehaviour
{
    [SerializeField] private Animator animator;
    [UdonSynced] public bool isOpen;
    [SerializeField] private string targetParameter = "isOpen";

    public override void Interact()
    {
        // Dear future toast
        // your code is shit

        // your code is so shit that I think it's shit while high
        // go fuck yourself

        // biscuits
        DoorCheckSynced(!isOpen);
    }

    public override void OnDeserialization()
    {
        CheckDoor();
    }

    public void TryDoor()
    {
        DoorCheckSynced(!isOpen);
    }

    public void CheckDoor()
    {
        animator.SetBool(targetParameter, isOpen);
    }

    public void DoorCheckSynced(bool targetState)
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        isOpen = targetState;
        RequestSerialization();
        CheckDoor();
    }

    public void OpenDoor()
    {
        DoorCheckSynced(true);
    }

    public void CloseDoor()
    {
        DoorCheckSynced(false);
    }
}
