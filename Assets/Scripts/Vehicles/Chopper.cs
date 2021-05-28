using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class Chopper : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        return "Chopper";
    }

    public void Interact(InteractionController controller)
    {
        Debug.Log("interact");
    }
}
