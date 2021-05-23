using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUsable
{
    void Initialize(PlayerActionHandler actionHandler);

    void BeginPrimary();
    void EndPrimary();

    void BeginSecondary();
    void EndSecondary();
}

public class PlayerActionHandler : MonoBehaviour
{
    [SerializeField] Toolbar toolbar;
    [SerializeField] RigEffector rigEffector;
    [SerializeField] Animator animator;
    [SerializeField] Camera camera;

    private PlayerInput input;

    GameObject currentBehaviourObject;
    IUsable current;

    public RigEffector RigEffector { get => rigEffector; }
    public Animator Animator { get => animator; }

    public Transform CameraTransform { get => camera.transform; }

    private void Start()
    {
        input = new PlayerInput();
        input.Enable();

        input.Player.PrimaryAction.started += OnPrimaryStarted;
        input.Player.PrimaryAction.canceled += OnPrimaryCancelled;
        input.Player.SecondaryAction.started += OnSecondaryStarted;
        input.Player.SecondaryAction.canceled += OnSecondaryCancelled;

        toolbar.SelectedChanged += OnSelectedChanged;
    }

    private void OnSelectedChanged(int oldSelected, int newSelected)
    {

        if (current != null)
        {
            current.EndPrimary();
            current.EndSecondary();
        }

        if (currentBehaviourObject != null)
            Destroy(currentBehaviourObject);

        currentBehaviourObject = null;
        current = null;
        var item = toolbar.Inventory[newSelected];
        if (item.IsValid())
        {
            var prefab = item.Info.BehaviourObject;
            if (prefab != null)
            {
                currentBehaviourObject = Instantiate(prefab);
                current = currentBehaviourObject.GetComponent<IUsable>();
                current?.Initialize(this);
            }
        }
    }

    private void OnSecondaryCancelled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        current?.EndSecondary();
    }

    private void OnSecondaryStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        current?.BeginSecondary();
    }

    private void OnPrimaryCancelled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        current?.EndPrimary();
    }

    private void OnPrimaryStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        current?.BeginPrimary();
    }
}
