using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemHandler
{
    private static Dictionary<ItemInfo, int> InfoToId;
    private static Dictionary<int, ItemInfo> IdToInfo;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        Debug.Log("Initializing ItemHandler");

        IdToInfo = new Dictionary<int, ItemInfo>();
        InfoToId = new Dictionary<ItemInfo, int>();

        var infos = Resources.LoadAll<ItemInfo>("Items");

        for (int i = 0; i < infos.Length; i++)
        {
            IdToInfo.Add(i, infos[i]);
            InfoToId.Add(infos[i], i);
        }
    }

    public static int GetItemIdFor(ItemAmountPair item)
    {
        if (InfoToId.ContainsKey(item.Info))
        {
            return InfoToId[item.Info];
        }
        return -1;
    }

    public static ItemInfo GetItemInfoFor(int id)
    {
        if (IdToInfo.ContainsKey(id))
        {
            return IdToInfo[id];
        }
        return null;
    }
}
