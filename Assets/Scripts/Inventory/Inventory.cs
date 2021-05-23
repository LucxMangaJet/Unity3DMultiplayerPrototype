using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryHolder
{
    Inventory Inventory { get; }
}

public class Inventory : IEnumerable
{
    private ItemAmountPair[] content;


    public delegate void ContentChangedDelegate(int slot, ItemAmountPair oldItem, ItemAmountPair newItem);
    public event ContentChangedDelegate ContentChanged;
    public int Length => content.Length;

    public ItemAmountPair this[int index]
    {
        get => GetContentAt(index);
        set => SetContentAt(index, value);
    }

    public Inventory(int size)
    {
        content = new ItemAmountPair[size];
    }

    private bool IndexIsValid(int i)
    {
        return i >= 0 && i < content.Length;
    }

    private ItemAmountPair GetContentAt(int index)
    {
        if (IndexIsValid(index))
        {
            return content[index];
        }

        return ItemAmountPair.Nothing;
    }

    private void SetContentAt(int index, ItemAmountPair value)
    {
        if (!IndexIsValid(index)) return;

        var oldItem = content[index];
        content[index] = value;
        ContentChanged?.Invoke(index, oldItem, value);
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < content.Length; i++)
        {
            yield return content[i];
        }
    }
}

[System.Serializable]
public struct ItemAmountPair
{
    public ItemInfo Info;
    public int Amount;

    public static ItemAmountPair Nothing { get => new ItemAmountPair(); }

    public bool IsNothing()
    {
        return Info == null || Amount <= 0;
    }

    public override string ToString()
    {
        return IsNothing() ? "Empty" : Amount > 1 ? $"{Amount} {Info.ItemName}" : Info.ItemName;
    }
}


