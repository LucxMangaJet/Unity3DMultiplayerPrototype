using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingMovementController : MonoBehaviourPun, IMovementStrategy, IPunObservable
{
    static int ANIM_IsSwimming = Animator.StringToHash("IsSwimming");

    [SerializeField] Transform camera;
    [SerializeField] Transform model;
    [SerializeField] DefaultLookBehaviour lookBehaviour;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rigidbody;

    [Header("Settings")]
    [SerializeField] float acceleration;
    [SerializeField] float slowdownPower;
    [SerializeField] float swimSpeed;

    PlayerInput input;

    //replicated
    Vector3 modelForward;

    public string MovementName => "Swim";

    private void Awake()
    {
        //Auto disable to let playerController choose
        this.enabled = false;
    }

    void Start()
    {
        modelForward = model.transform.forward;

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

    }

    private void LocalFixedUpdate()
    {
        Vector2 movement = input.Player.Move.ReadValue<Vector2>();

        LocalMoveUpdate(movement);

        modelForward = camera.forward;
    }

    private void LocalMoveUpdate(Vector2 move)
    {
        if (move.magnitude < 0.1f)
        {
            rigidbody.velocity *= slowdownPower;
        }
        else
        {
            move.Normalize();

            var force = (camera.forward * move.y + camera.right * move.x) * acceleration;

            rigidbody.AddForce(force, ForceMode.Impulse);
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, swimSpeed);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(modelForward);
        }
        else
        {
            modelForward = (Vector3)stream.ReceiveNext();
        }
    }

    public void Activate()
    {
        photonView.RPC(nameof(RPC_SwimActivate), RpcTarget.All);
        lookBehaviour.SetCameraMode(DefaultLookBehaviour.CameraMode.HeadBound);
        lookBehaviour.Activate();
    }

    [PunRPC]
    private void RPC_SwimActivate()
    {
        this.enabled = true;
        model.gameObject.SetActive(true);
        animator.SetBool(ANIM_IsSwimming, true);
        rigidbody.useGravity = false;
    }

    public void Deactivate()
    {
        photonView.RPC(nameof(RPC_SwimDeactivate), RpcTarget.All);
        lookBehaviour.SetCameraMode(DefaultLookBehaviour.CameraMode.Normal);
        lookBehaviour.Deactivate();
    }

    [PunRPC]
    private void RPC_SwimDeactivate()
    {
        this.enabled = false;
        model.gameObject.SetActive(false);
        animator.SetBool(ANIM_IsSwimming, false);
        rigidbody.useGravity = true;
    }

    public bool BlocksInteraction()
    {
        return false;
    }
}
