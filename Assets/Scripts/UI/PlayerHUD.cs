using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] ToolbarVisualizer toolbarVisualizer;
    [SerializeField] float scrollPauseTime;
    [SerializeField] TMPro.TextMeshProUGUI interactText;

    private PlayerInput input;

    float lastScrollTimestamp;
    Toolbar toolbar;
    InteractionController interactionController;

    public void Initialize(Toolbar toolbar, InteractionController interactionController)
    {
        this.toolbar = toolbar;
        this.interactionController = interactionController;

        toolbarVisualizer.SetToolbar(toolbar);
        interactionController.TargetChanged += OnInteractionTargetChanged;

        interactText.enabled = false;
    }

    private void OnInteractionTargetChanged(IInteractable obj)
    {
        if (obj == null)
        {
            interactText.enabled = false;
        }
        else
        {
            interactText.enabled = true;
            interactText.text = "Press E - " + obj.GetDescription();
        }
    }

    private void Start()
    {
        input = new PlayerInput();
        input.Enable();

        input.UI.ScrollWheel.performed += OnScrollWheel;

    }

    private void Update()
    {
        //Check toolbar keyboard shortcuts
        int baseKey = (int)Key.Digit1;
        for (int i = 0; i < 10; i++)
        {
            Key key = (Key)baseKey + i;
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                toolbar.Select(i);
            }
        }
    }

    private void OnScrollWheel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float val = obj.ReadValue<Vector2>().y;

        if (val != 0)
        {
            if (Time.time - lastScrollTimestamp > scrollPauseTime)
            {
                int dir = -1 * (int)Mathf.Sign(val);
                toolbar.ScrollBy(dir);
                lastScrollTimestamp = Time.time;
            }

        }
    }
}
