using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun
{
    static int ANIM_SpeedX = Animator.StringToHash("SpeedX");
    static int ANIM_SpeedY = Animator.StringToHash("SpeedY");

    [SerializeField] Transform camera, model;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Animator animator;

    [Header("Settings")]
    [SerializeField] Vector2 lookYMinMax;
    [SerializeField] float lookSensitivity;
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float slowdownPower;

    Vector3 lookRotation;
    PlayerInput input;

    void Start()
    {
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
        Vector2 move = input.Player.Move.ReadValue<Vector2>();

        if (move.magnitude < 0.1f)
        {
            rigidbody.velocity = rigidbody.velocity *= slowdownPower;
        }
        else
        {
            move.Normalize();

            var camForwardFlatNormalized = camera.forward.WithY0().normalized;

            var force = (camForwardFlatNormalized * move.y + camera.right.WithY0().normalized * move.x) * acceleration;
            force.y = rigidbody.velocity.y;

            rigidbody.AddForce(force, ForceMode.Impulse);
            var vel = rigidbody.velocity;
            vel.y = 0;
            vel = Vector3.ClampMagnitude(vel, maxSpeed);
            vel.y = rigidbody.velocity.y;
            rigidbody.velocity = vel;

            model.forward = camForwardFlatNormalized;
        }

        float speedX = Vector3.Dot(model.right, rigidbody.velocity);
        float speedY = Vector3.Dot(model.forward, rigidbody.velocity);

        animator.SetFloat(ANIM_SpeedX, Mathf.Clamp(speedX, -1, 1));
        animator.SetFloat(ANIM_SpeedY, Mathf.Clamp(speedY, -1, 1));
    }

    private void OnLook(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();

        lookRotation.y += input.x * Time.deltaTime * lookSensitivity;
        lookRotation.x += -input.y * Time.deltaTime * lookSensitivity;

        lookRotation.x = Mathf.Clamp(lookRotation.x, lookYMinMax.x, lookYMinMax.y);
        camera.eulerAngles = lookRotation;
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
