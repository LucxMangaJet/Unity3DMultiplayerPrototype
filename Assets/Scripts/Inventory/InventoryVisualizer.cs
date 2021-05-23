using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryVisualSlot
{
    void OnItemChanged(ItemAmountPair oldItem, ItemAmountPair newItem);
    void Highlight();
    void Dehighlight();
}

public class InventoryVisualizer : MonoBehaviour
{
    [SerializeField] Component inventoryHolderComponent;

    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotParent;

    protected Inventory inventory;
    protected IInventoryHolder holder;

    protected IInventoryVisualSlot[] visualSlots;


    private void Start()
    {
        if (inventoryHolderComponent is IInventoryHolder)
        {
            holder = inventoryHolderComponent as IInventoryHolder;
            Initialize();
        }
    }

    protected virtual void Initialize()
    {
        inventory = holder.Inventory;
        inventory.ContentChanged += OnInventoryChanged;

        visualSlots = new IInventoryVisualSlot[inventory.Length];

        for (int i = 0; i < visualSlots.Length; i++)
        {
            visualSlots[i] = Instantiate(slotPrefab, slotParent).GetComponent<IInventoryVisualSlot>();
            //initialize slot
            visualSlots[i]?.OnItemChanged(ItemAmountPair.Nothing, inventory[i]);
        }
    }

    protected virtual void OnDestroy()
    {
        if (inventory != null)
            inventory.ContentChanged -= OnInventoryChanged;
    }

    protected virtual void OnInventoryChanged(int slot, ItemAmountPair oldItem, ItemAmountPair newItem)
    {
        if (visualSlots[slot] != null)
        {
            visualSlots[slot].OnItemChanged(oldItem, newItem);
        }
    }
}
