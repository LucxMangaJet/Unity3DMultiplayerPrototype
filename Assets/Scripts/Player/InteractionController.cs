using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetDescription();
    void Interact(InteractionController controller);

}

public class InteractionController : MonoBehaviour
{
    [SerializeField] Transform camera;
    [SerializeField] float range = 5;

    public event System.Action<IInteractable> TargetChanged;

    IInteractable currentTarget;
    PlayerInput input;

    private void Start()
    {
        input = new PlayerInput();
        input.Enable();
        input.Player.Interact.performed += OnInteract;
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentTarget != null)
        {
            currentTarget.Interact(this);
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
                return interactable;
            }
        }
        return null;
    }
}
