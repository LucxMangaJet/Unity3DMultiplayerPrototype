using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    static int ANIM_SpeedX = Animator.StringToHash("SpeedX");
    static int ANIM_SpeedY = Animator.StringToHash("SpeedY");
    static int ANIM_Jump = Animator.StringToHash("Jump");

    [SerializeField] Transform camera, model;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Animator animator;
    [SerializeField] Transform headJoint;
    [SerializeField] GameObject localPlayerObject;
    [SerializeField] RigEffector rigEffector;

    [Header("Settings")]
    [SerializeField] Vector2 lookYMinMax;
    [SerializeField] float lookSensitivity;
    [SerializeField] float walkSpeed, sprintSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float slowdownPower;
    [SerializeField] float angleToRotate;
    [SerializeField] float groundedAngle;
    [SerializeField] float jumpCooldown;
    [SerializeField] float jumpVelocity;

    Vector3 lookRotation;
    PlayerInput input;
    float lastJumpTimestamp;

    //replicated
    Vector3 modelForward;
    bool sprint;
    float lastGroundedTimestamp;

    void Start()
    {
        modelForward = model.forward;

        if (!photonView.IsMine)
        {
            Destroy(camera.gameObject);
            Destroy(localPlayerObject);
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
        if (photonView.IsMine)
        {
            camera.eulerAngles = lookRotation;

            Vector3 localPos = new Vector3(0, 0, 0.2f);
            localPos = Quaternion.Euler(0, lookRotation.y, 0) * localPos;
            localPos.y = camera.localPosition.y;
            camera.localPosition = localPos;
        }
    }

    private void LocalFixedUpdate()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        sprint = input.Player.Sprint.ReadValue<float>() > 0;
        bool jump = input.Player.Jump.ReadValue<float>() > 0;

        var camForwardFlatNormalized = camera.forward.WithY0().normalized;

        LocalMoveUpdate(move, camForwardFlatNormalized);

        if (jump && CanJump())
        {
            Jump();
        }

        rigEffector.LookInDirection(camera.forward);
    }

    private void Jump()
    {
        lastJumpTimestamp = Time.time;
        var vel = rigidbody.velocity;
        vel.y = jumpVelocity;
        rigidbody.velocity = vel;
        photonView.RPC(nameof(RPC_Jump), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Jump()
    {
        animator.SetTrigger(ANIM_Jump);
    }

    private void LocalMoveUpdate(Vector2 move, Vector3 camForwardFlatNormalized)
    {
        if (move.magnitude < 0.1f)
        {
            var vel = rigidbody.velocity;
            vel *= slowdownPower;
            vel.y = rigidbody.velocity.y;
            rigidbody.velocity = vel;

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
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(modelForward);
            stream.SendNext(sprint);
            stream.SendNext(lastGroundedTimestamp);
        }
        else
        {
            modelForward = (Vector3)stream.ReceiveNext();
            sprint = (bool)stream.ReceiveNext();
            lastGroundedTimestamp = (float)stream.ReceiveNext();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!photonView.IsMine) return;

        var contact = collision.GetContact(0);
        var offsetPos = transform.position.OffsetBy(0, 0.3f, 0);

        float angle = Vector3.Angle(contact.point - offsetPos, Vector3.down);
        if (angle < groundedAngle)
        {
            Debug.DrawLine(offsetPos, contact.point, Color.red, 0.5f);
            lastGroundedTimestamp = Time.time;
        }
    }

    private bool IsGrounded()
    {
        return Time.time - lastGroundedTimestamp < 0.1f;
    }

    private bool IsJumping()
    {
        return Time.time - lastJumpTimestamp < jumpCooldown;
    }

    private bool CanJump()
    {
        return IsGrounded() && !IsJumping();
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
