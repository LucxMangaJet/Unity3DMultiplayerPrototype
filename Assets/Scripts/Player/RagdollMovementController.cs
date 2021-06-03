using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollMovementController : MonoBehaviourPun, IMovementStrategy
{
    private static int ANIM_StandUp = Animator.StringToHash("StandUp");

    [SerializeField] Animator animator;
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform camera;
    [SerializeField] Transform model, rigRoot;
    [SerializeField] RagdollEffector ragdollPrefab;

    [SerializeField] float tempDuration;

    RagdollEffector ragdollInstance;
    public string MovementName => "Ragdoll";

    private void Awake()
    {
        //Auto disable to let playerController choose
        this.enabled = false;
    }

    private IEnumerator RagdollRoutine()
    {
        model.gameObject.SetActive(false);
        ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        ragdollInstance.MatchRig(rigRoot);

        yield return new WaitForSeconds(tempDuration);

        model.gameObject.SetActive(true);
        Debug.Log("A");
        animator.SetBool(ANIM_StandUp, true);

        if (this.enabled && photonView.IsMine)
        {
            playerController.transform.position = ragdollInstance.Root.position;
        }

        Destroy(ragdollInstance.gameObject);

        yield return new WaitForSeconds(1f);

        animator.SetBool(ANIM_StandUp, false);
        if (this.enabled && photonView.IsMine)
        {
            playerController.SwitchToNormal();
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine && ragdollInstance != null)
        {
            var head = ragdollInstance.GetHeadJoint();
            camera.position = head.position;
            camera.forward = head.forward;
        }
        //attachCam to head for standup animation
    }

    public void Activate()
    {
        photonView.RPC(nameof(RPC_RagdollActivate), RpcTarget.All);
        playerController.DetachCamera();
        playerController.DisableRigidbody();
    }

    [PunRPC]
    private void RPC_RagdollActivate()
    {
        this.enabled = true;

        StartCoroutine(RagdollRoutine());
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
        if (ragdollInstance != null)
            Destroy(ragdollInstance.gameObject);
        model.gameObject.SetActive(true);
    }

    public bool BlocksInteraction()
    {
        return true;
    }
}
