
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SimpleLarpDoor : UdonSharpBehaviour
{
    [SerializeField] private Animator animator;
    private bool isOpen;
    [SerializeField] private bool isNetworked = true;
    [SerializeField] private string targetParameter = "isOpen";

    public override void Interact()
    {
        isOpen = !isOpen;

        if(isNetworked)
        {
            if (isOpen)
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OpenDoor");
            else
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CloseDoor");
        }
        else
        {
            animator.SetBool(targetParameter, isOpen);
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        animator.SetBool(targetParameter, isOpen);
    }

    public void CloseDoor()
    {
        isOpen = false;
        animator.SetBool(targetParameter, isOpen);
    }
}
