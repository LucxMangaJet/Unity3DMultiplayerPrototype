using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IMovementStrategy
{
    void Activate();
    void Deactivate();

    bool BlocksInteraction();

    string MovementName { get; }
}

public class NormalMovementController : MonoBehaviourPun, IMovementStrategy, IPunObservable
{
    static int ANIM_SpeedX = Animator.StringToHash("SpeedX");
    static int ANIM_SpeedY = Animator.StringToHash("SpeedY");
    static int ANIM_Jump = Animator.StringToHash("Jump");

    [SerializeField] Transform camera, model;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Animator animator;
    [SerializeField] Transform headJoint;
    [SerializeField] RigEffector rigEffector;

    [SerializeField] DefaultLookBehaviour lookBehaviour;

    [Header("Settings")]

    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float slowdownPower;
    [SerializeField] float angleToRotate;
    [SerializeField] float groundedAngle;
    [SerializeField] float jumpCooldown;
    [SerializeField] float jumpVelocity;

    PlayerInput input;
    float lastJumpTimestamp;

    //replicated
    Vector3 modelForward;
    bool sprint;
    float lastGroundedTimestamp;

    public string MovementName => "Walk";

    private void Awake()
    {
        //Auto disable to let playerController choose
        enabled = false;
    }

    void Start()
    {
        modelForward = model.forward;

        if (!photonView.IsMine) return;

        input = new PlayerInput();
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

    public void Activate()
    {
        photonView.RPC(nameof(RPC_Activate), RpcTarget.All);
        lookBehaviour.Activate();
    }

    [PunRPC]
    private void RPC_Activate()
    {
        this.enabled = true;
        model.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        photonView.RPC(nameof(RPC_Deactivate), RpcTarget.All);
        lookBehaviour.Deactivate();
    }

    [PunRPC]
    private void RPC_Deactivate()
    {
        this.enabled = false;
        model.gameObject.SetActive(false);
    }

    public bool BlocksInteraction()
    {
        return false;
    }
}
