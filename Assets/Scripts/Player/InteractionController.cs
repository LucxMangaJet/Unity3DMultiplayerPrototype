using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetDescription();
    void Interact(InteractionController controller);
    bool CanInteract();

}

public class InteractionController : MonoBehaviour
{
    [SerializeField] Transform camera;
    [SerializeField] float range = 5;
    [SerializeField] PlayerController playerController;

    IInteractable currentTarget;
    PlayerInput input;

    public event System.Action<IInteractable> TargetChanged;

    public PlayerController MainController { get => playerController; }

    private void Start()
    {
        input = new PlayerInput();
        input.Enable();
        input.Player.Interact.performed += OnInteract;
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentTarget != null && !playerController.MovementStrategy.BlocksInteraction() && currentTarget.CanInteract())
        {
            currentTarget.Interact(this);
            Debug.Log($"Interacted with {currentTarget.GetDescription()}");
        }
    }

    private void Update()
    {
        IInteractable newTarget = Raycast();

        if (newTarget != currentTarget)
        {
            currentTarget = newTarget;
            TargetChanged?.Invoke(currentTarget);
        }
    }

    private IInteractable Raycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, range))
        {
            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (!interactable.CanInteract())
                    return null;

                return interactable;
            }
        }
        return null;
    }
}
