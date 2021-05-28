using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class PlayerController : MonoBehaviourPun
{
    [SerializeField] Transform camera;
    [SerializeField] GameObject localPlayerObject;
    [SerializeField] NormalMovementController normalMovement;

    IMovementStrategy movementStrategy;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(camera.gameObject);
            Destroy(localPlayerObject);
        }

        SwitchTo(normalMovement);
    }

    public void SwitchTo(IMovementStrategy strategy)
    {
        if (movementStrategy != null)
        {
            movementStrategy.Deactivate();
        }
        else
        {
            movementStrategy = strategy;
            strategy.Activate();
        }
    }
}

public static class PlayerControllerExt
{
    public static Vector3 WithY0(this Vector3 v3)
    {
        v3.y = 0;
        return v3;
    }

    public static Vector3 OffsetBy(this Vector3 v3, float x, float y, float z)
    {
        v3 += new Vector3(x, y, z);
        return v3;
    }
}
