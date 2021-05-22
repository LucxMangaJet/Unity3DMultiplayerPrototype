using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    static int ANIM_SpeedX = Animator.StringToHash("SpeedX");
    static int ANIM_SpeedY = Animator.StringToHash("SpeedY");

    [SerializeField] Transform camera, model;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Animator animator;
    [SerializeField] Transform headJoint;

    [Header("Settings")]
    [SerializeField] Vector2 lookYMinMax;
    [SerializeField] float lookSensitivity;
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float slowdownPower;
    [SerializeField] float angleToRotate;

    Vector3 lookRotation;
    PlayerInput input;

    //replication
    Vector3 modelForward;
    Vector3 cameraForward;

    void Start()
    {
        modelForward = model.forward;

        if (!photonView.IsMine)
        {
            Destroy(camera.gameObject);
            return;
        }

        input = new PlayerInput();
        input.Player.Look.performed += OnLook;

        input.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            LocalFixedUpdate();
        }

        model.forward = modelForward;

        float speedX = Vector3.Dot(model.right, rigidbody.velocity);
        float speedY = Vector3.Dot(model.forward, rigidbody.velocity);

        animator.SetFloat(ANIM_SpeedX, Mathf.Clamp(speedX, -1, 1));
        animator.SetFloat(ANIM_SpeedY, Mathf.Clamp(speedY, -1, 1));
    }

    private void LateUpdate()
    {
        headJoint.forward = cameraForward;
    }

    private void LocalFixedUpdate()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        var camForwardFlatNormalized = camera.forward.WithY0().normalized;
        cameraForward = camera.forward;

        if (move.magnitude < 0.1f)
        {
            rigidbody.velocity = rigidbody.velocity *= slowdownPower;

            float angle = Vector3.SignedAngle(camForwardFlatNormalized, modelForward, Vector3.up);

            if (Mathf.Abs(angle) > angleToRotate)
            {
                float val = Mathf.Sign(angle) * (angleToRotate - Mathf.Abs(angle));
                modelForward = Quaternion.Euler(0, val, 0) * modelForward;
            }
        }
        else
        {
            move.Normalize();

            var force = (camForwardFlatNormalized * move.y + camera.right.WithY0().normalized * move.x) * acceleration;
            force.y = rigidbody.velocity.y;

            rigidbody.AddForce(force, ForceMode.Impulse);
            var vel = rigidbody.velocity;
            vel.y = 0;
            vel = Vector3.ClampMagnitude(vel, maxSpeed);
            vel.y = rigidbody.velocity.y;
            rigidbody.velocity = vel;

            modelForward = camForwardFlatNormalized;
        }
    }

    private void OnLook(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();

        lookRotation.y += input.x * Time.deltaTime * lookSensitivity;
        lookRotation.x += -input.y * Time.deltaTime * lookSensitivity;

        lookRotation.x = Mathf.Clamp(lookRotation.x, lookYMinMax.x, lookYMinMax.y);
        camera.eulerAngles = lookRotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(modelForward);
            stream.SendNext(cameraForward);
        }
        else
        {
            modelForward = (Vector3)stream.ReceiveNext();
            cameraForward = (Vector3)stream.ReceiveNext();
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



}
