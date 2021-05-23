using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar : MonoBehaviour, IInventoryHolder
{
    [SerializeField] int size;
    [SerializeField] ItemAmountPair[] initialContent;

    private int selected;
    private Inventory inventory;

    public delegate void SelectedChangedDelegate(int oldSelected, int newSelected);
    public event SelectedChangedDelegate SelectedChanged;
    public Inventory Inventory => inventory;


    private void Awake()
    {
        inventory = new Inventory(size);

        for (int i = 0; i < size && i < initialContent.Length; i++)
        {
            inventory[i] = initialContent[i];
        }
    }

    private void Select(int index)
    {
        if (index < 0 || index >= size) return;

        var old = selected;
        selected = index;
        SelectedChanged?.Invoke(old, index);
    }

    public void ScrollBy(int amount)
    {
        if (amount == 0) return;
        int newSelection = (size + selected + amount) % size;
        Select(newSelection);
    }

}
