using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] ToolbarVisualizer toolbarVisualizer;

    [SerializeField] float scrollPauseTime;

    private PlayerInput input;

    float lastScrollTimestamp;
    Toolbar toolbar;

    public void Initialize(Toolbar toolbar)
    {
        this.toolbar = toolbar;
        toolbarVisualizer.SetToolbar(toolbar);
    }

    private void Start()
    {
        input = new PlayerInput();
        input.Enable();

        input.UI.ScrollWheel.performed += OnScrollWheel;
    }

    private void OnScrollWheel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float val = obj.ReadValue<Vector2>().y;

        if (val != 0)
        {
            if (Time.time - lastScrollTimestamp > scrollPauseTime)
            {
                int dir = (int)Mathf.Sign(val);
                toolbar.ScrollBy(dir);
                lastScrollTimestamp = Time.time;
            }

        }
    }
}
