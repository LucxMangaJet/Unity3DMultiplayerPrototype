using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DefaultLookBehaviour : MonoBehaviourPun, IMovementStrategy
{
    public enum CameraMode
    {
        Normal,
        HeadBound
    }

    [SerializeField] Transform camera;

    [Header("Settings")]
    [SerializeField] Vector2 lookYMinMax;
    [SerializeField] float lookSensitivity;
    [SerializeField] Transform headJoint;

    Vector3 lookRotation;
    PlayerInput input;
    CameraMode mode;

    public string MovementName => "DefaultLook";

    private void Awake()
    {
        //Auto disable to let playerController choose
        this.enabled = false;
    }

    public void SetCameraMode(CameraMode m)
    {
        mode = m;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        input = new PlayerInput();
        input.Player.Look.performed += OnLook;

        input.Enable();
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            camera.eulerAngles = lookRotation;

            if (mode == CameraMode.HeadBound)
            {
                camera.localPosition = headJoint.position - transform.position;
            }
            else if (mode == CameraMode.Normal)
            {
                Vector3 localPos = new Vector3(0, 0, 0.2f);
                localPos = Quaternion.Euler(0, lookRotation.y, 0) * localPos;
                localPos.y = camera.localPosition.y;
                camera.localPosition = localPos;
            }
        }
    }

    private void OnLook(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();

        lookRotation.y += input.x * Time.deltaTime * lookSensitivity;
        lookRotation.x += -input.y * Time.deltaTime * lookSensitivity;

        lookRotation.x = Mathf.Clamp(lookRotation.x, lookYMinMax.x, lookYMinMax.y);

    }

    public void Activate()
    {
        this.enabled = true;
    }

    public bool BlocksInteraction()
    {
        return false;
    }

    public void Deactivate()
    {
        this.enabled = false;
    }
}
