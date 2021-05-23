using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarVisualizer : InventoryVisualizer
{
    [SerializeField] TMPro.TextMeshProUGUI displayText;

    public void SetToolbar(Toolbar t)
    {
        holder = t;
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (holder is Toolbar toolbar)
        {
            toolbar.SelectedChanged += OnSelectedChanged;
            //select 0 by default
            OnSelectedChanged(0, 0);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (holder is Toolbar toolbar)
        {
            toolbar.SelectedChanged -= OnSelectedChanged;
        }
    }


    private void OnSelectedChanged(int oldSelected, int newSelected)
    {
        visualSlots[oldSelected].Dehighlight();
        visualSlots[newSelected].Highlight();

        displayText.text = inventory[newSelected].ToString();
    }
}
