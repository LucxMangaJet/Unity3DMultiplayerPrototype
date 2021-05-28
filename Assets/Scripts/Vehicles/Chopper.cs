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

    [SerializeField] float movementForce;
    [SerializeField] float upForce;
    [SerializeField] float downForce;

    PlayerInput input;
    PlayerController playerController;
    Transform camera;

    bool localActive;

    //rpc replicated
    bool used;

    //replicated
    float rotatorSpeed;

    public void Activate()
    {
        RPC_Activate();
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
        if (!localActive) return;


        camera.position = cameraTarget.position;
        camera.forward = cameraTarget.forward;

        rotatorSpeed = Mathf.Max(0, 0.1f + rigidbody.velocity.y * 0.01f);
        animator.SetFloat(ANIM_RotatorSpeed, rotatorSpeed);

        LocalLateUpdate();
    }

    private void LocalLateUpdate()
    {
        Vector2 i_mov = input.Chopper.Movement.ReadValue<Vector2>();
        float i_up = input.Chopper.MoveUp.ReadValue<float>();
        float i_down = input.Chopper.MoveDown.ReadValue<float>();

        Vector3 movement = new Vector3(i_mov.x, 0, i_mov.y) * Time.deltaTime * movementForce;
        rigidbody.AddForce(movement, ForceMode.Impulse);
        rigidbody.AddForce(Vector3.up * i_up * Time.deltaTime * upForce, ForceMode.Impulse);
        rigidbody.AddForce(Vector3.down * i_down * Time.deltaTime * downForce, ForceMode.Impulse);
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
        RPC_Deactivate();
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
