using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollMovementController : MonoBehaviourPun, IMovementStrategy
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform camera;
    [SerializeField] Transform model, rigRoot;
    [SerializeField] RagdollEffector ragdollPrefab;

    [SerializeField] float tempDuration;

    private float startStamp;
    RagdollEffector ragdollInstance;
    public string MovementName => "Ragdoll";

    private void Awake()
    {
        //Auto disable to let playerController choose
        this.enabled = false;
    }


    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (Time.time - startStamp > tempDuration)
        {
            playerController.SwitchToNormal();
        }
    }   

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            var head = ragdollInstance.GetHeadJoint();
            camera.position = head.position;
            camera.forward = head.forward;
        }
    }

    public void Activate()
    {
        photonView.RPC(nameof(RPC_RagdollActivate), RpcTarget.All);
        playerController.DetachCamera();
        startStamp = Time.time;
        playerController.DisableRigidbody();
    }

    [PunRPC]
    private void RPC_RagdollActivate()
    {
        this.enabled = true;
        model.gameObject.SetActive(false);
        ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        ragdollInstance.MatchRig(rigRoot);
    }

    public void Deactivate()
    {
        photonView.RPC(nameof(RPC_RagdollDeactivate), RpcTarget.All);
        playerController.AttachCamera();
        playerController.EnableRigidbody();
    }

    [PunRPC]
    private void RPC_RagdollDeactivate()
    {
        this.enabled = false;
        model.gameObject.SetActive(false);
        Destroy(ragdollInstance.gameObject);
    }

    public bool BlocksInteraction()
    {
        return true;
    }
}
