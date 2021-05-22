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
    static int ANIM_Jump = Animator.StringToHash("Jump");

    [SerializeField] Transform camera, model;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Animator animator;
    [SerializeField] Transform headJoint;

    [Header("Settings")]
    [SerializeField] Vector2 lookYMinMax;
    [SerializeField] float lookSensitivity;
    [SerializeField] float walkSpeed, sprintSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float slowdownPower;
    [SerializeField] float angleToRotate;

    Vector3 lookRotation;
    PlayerInput input;

    //replication
    Vector3 modelForward;
    Vector3 cameraForward;
    bool sprint;

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
        float clampVal = sprint ? 2 : 1;

        animator.SetFloat(ANIM_SpeedX, Mathf.Clamp(speedX, -clampVal, clampVal));
        animator.SetFloat(ANIM_SpeedY, Mathf.Clamp(speedY, -clampVal, clampVal));
    }

    private void LateUpdate()
    {
        headJoint.forward = cameraForward;
    }

    private void LocalFixedUpdate()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        sprint = input.Player.Sprint.ReadValue<float>() > 0;

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
            vel = Vector3.ClampMagnitude(vel, sprint ? sprintSpeed : walkSpeed);
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
            stream.SendNext(sprint);
        }
        else
        {
            modelForward = (Vector3)stream.ReceiveNext();
            cameraForward = (Vector3)stream.ReceiveNext();
            sprint = (bool)stream.ReceiveNext();
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
