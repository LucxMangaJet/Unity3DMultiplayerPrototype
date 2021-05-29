using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class Chopper : MonoBehaviourPun, IInteractable, IMovementStrategy, IPunObservable
{
    static int ANIM_RotatorSpeed = Animator.StringToHash("RotatorSpeed");

    [SerializeField] Transform exitLocation;
    [SerializeField] Transform cameraTarget;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Animator animator;

    [SerializeField] float movementVelocity;
    [SerializeField] float upVelocity;
    [SerializeField] float pitchFactor;
    [SerializeField] float rollFactor;
    [SerializeField] float chopperRotationLerpFactor;
    [SerializeField] float rotatorMaxSpeed;
    [SerializeField] float rotatorMinSpeed;
    [SerializeField] float rotatorAcceleration;

    PlayerInput input;
    PlayerController playerController;
    Transform camera;

    bool localActive;
    Vector3 cameraOffset;
    Vector3 currentRotation;

    //rpc replicated
    bool used;

    //replicated
    float rotatorSpeed;

    private void Awake()
    {
        cameraOffset = cameraTarget.localPosition;
    }

    public void Activate()
    {
        photonView.RPC(nameof(RPC_Activate), RpcTarget.All);

        localActive = true;
        camera = playerController.CameraTransform;

        input = new PlayerInput();
        input.Chopper.Exit.performed += OnExit;
        input.Enable();

        playerController.DetachCamera();
    }


    [PunRPC]

    private void RPC_Activate()
    {
        used = true;
    }

    [PunRPC]
    private void RPC_Deactivate()
    {
        used = false;
    }


    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            float delta = (localActive ? rotatorAcceleration : -rotatorAcceleration) * Time.deltaTime;

            rotatorSpeed = Mathf.Clamp(rotatorSpeed + delta, rotatorMinSpeed, rotatorMaxSpeed);
        }

        if (localActive)
        {
            camera.position = transform.position + (Quaternion.Euler(0, currentRotation.y, 0) * cameraOffset);
            camera.forward = transform.position - camera.position;

            LocalLateUpdate();
        }

        animator.SetFloat(ANIM_RotatorSpeed, rotatorSpeed);
    }

    private void LocalLateUpdate()
    {
        Vector2 i_mov = input.Chopper.Movement.ReadValue<Vector2>();
        float i_up = input.Chopper.MoveUp.ReadValue<float>();
        float i_down = input.Chopper.MoveDown.ReadValue<float>();
        float i_lookX = input.Chopper.Look.ReadValue<Vector2>().x;

        Vector3 movement = (i_mov.x * transform.right.WithY0Norm() + i_mov.y *
                            transform.forward.WithY0Norm()) * movementVelocity;

        rigidbody.velocity = new Vector3(movement.x, (i_up + i_down * -1) * upVelocity, movement.z);

        Vector3 rotation = new Vector3();
        rotation.x = Vector3.Dot(rigidbody.velocity, transform.forward.WithY0().normalized) * pitchFactor;
        rotation.y = currentRotation.y;
        rotation.z = Vector3.Dot(rigidbody.velocity, transform.right.WithY0().normalized) * -rollFactor;

        currentRotation = Vector3.Lerp(currentRotation, rotation, chopperRotationLerpFactor * Time.deltaTime);
        currentRotation.y += i_lookX;


        transform.rotation = Quaternion.Euler(currentRotation);
    }


    private void OnExit(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.MoveTo(exitLocation.position);
        playerController.SwitchToNormal();
    }

    public bool BlocksInteraction()
    {
        return true;
    }

    public void Deactivate()
    {
        photonView.RPC(nameof(RPC_Deactivate), RpcTarget.All);

        input.Disable();
        input.Dispose();
        input = null;
        localActive = false;
    }

    public string GetDescription()
    {
        return "Chopper";
    }

    public void Interact(InteractionController controller)
    {
        playerController = controller.MainController;
        controller.MainController.SwitchTo(this);

        photonView.RequestOwnership();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rotatorSpeed);
        }
        else
        {
            rotatorSpeed = (float)stream.ReceiveNext();
        }
    }

    public bool CanInteract()
    {
        return !used;
    }
}
