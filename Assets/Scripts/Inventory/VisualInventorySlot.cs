using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualInventorySlot : MonoBehaviour, IInventoryVisualSlot
{
    [SerializeField] TMPro.TextMeshProUGUI amount;
    [SerializeField] Image icon;
    [SerializeField] GameObject highlight;

    public void Dehighlight()
    {
        highlight.SetActive(false);
    }

    public void Highlight()
    {
        highlight.SetActive(true);
    }

    public void OnItemChanged(ItemAmountPair oldItem, ItemAmountPair newItem)
    {
        if (newItem.IsNothing())
        {
            amount.text = "";
            icon.sprite = null;
            icon.color = Color.clear;
        }
        else
        {
            amount.text = newItem.Amount > 1 ? newItem.Amount.ToString() : "";
            icon.sprite = newItem.Info.Icon;
            icon.color = Color.white;
        }
    }
}
