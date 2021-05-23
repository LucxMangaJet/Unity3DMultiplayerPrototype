using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
