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
    [SerializeField] SwimmingMovementController swimmingMovement;
    [SerializeField] RagdollMovementController ragdollMovement;
    [SerializeField] Rigidbody rigidbody;

    IMovementStrategy movementStrategy;
    Vector3 cameraLocalPosition;

    public IMovementStrategy MovementStrategy { get => movementStrategy; }
    public Transform CameraTransform => camera.transform;


    private void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(camera.gameObject);
            Destroy(localPlayerObject);
        }
        cameraLocalPosition = camera.localPosition;
        SwitchTo(normalMovement);
    }

    public void SwitchTo(IMovementStrategy strategy)
    {
        if (movementStrategy == strategy) return;

        Debug.Log($"Switching to {strategy.MovementName}");

        movementStrategy?.Deactivate();
        movementStrategy = strategy;
        strategy.Activate();
    }

    public void SwitchToSwim()
    {
        AttachCamera();
        SwitchTo(swimmingMovement);
    }

    public void SwitchToNormal()
    {
        AttachCamera();
        SwitchTo(normalMovement);
    }

    [NaughtyAttributes.Button]
    public void SwitchToRagdoll()
    {
        AttachCamera();
        SwitchTo(ragdollMovement);
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public void DetachCamera()
    {
        camera.parent = null;
    }

    public void AttachCamera()
    {
        camera.SetParent(transform);
        camera.localPosition = cameraLocalPosition;
    }

    public bool IsLocalPlayer()
    {
        return photonView.IsMine;
    }

    public void DisableRigidbody()
    {
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
    }

    public void EnableRigidbody()
    {
        rigidbody.isKinematic = false;
    }
}

public static class PlayerControllerExt
{
    public static Vector3 WithY0(this Vector3 v3)
    {
        v3.y = 0;
        return v3;
    }

    public static Vector3 WithY0Norm(this Vector3 v3)
    {
        v3.y = 0;
        return v3.normalized;
    }

    public static Vector3 OffsetBy(this Vector3 v3, float x, float y, float z)
    {
        v3 += new Vector3(x, y, z);
        return v3;
    }
}
