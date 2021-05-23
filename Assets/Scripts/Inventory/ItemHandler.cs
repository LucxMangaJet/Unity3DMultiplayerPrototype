using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemHandler : ResourceHandler<ItemInfo>
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        Debug.Log("Initializing ItemHandler");
        InitializeResources("Items");
    }
}

public class ResourceHandler<T> where T : UnityEngine.Object
{
    protected static Dictionary<T, int> InfoToId;
    protected static Dictionary<int, T> IdToInfo;

    public static void InitializeResources(string resourcePath)
    {
        IdToInfo = new Dictionary<int, T>();
        InfoToId = new Dictionary<T, int>();

        var infos = Resources.LoadAll<T>(resourcePath);

        for (int i = 0; i < infos.Length; i++)
        {
            IdToInfo.Add(i, infos[i]);
            InfoToId.Add(infos[i], i);
        }
    }

    public static int GetIdFor(T item)
    {
        if (InfoToId.ContainsKey(item))
        {
            return InfoToId[item];
        }
        return -1;
    }

    public static T GetObjectFor(int id)
    {
        if (IdToInfo.ContainsKey(id))
        {
            return IdToInfo[id];
        }
        return null;
    }

    public static T[] GetAllObjects()
    {
        return IdToInfo.Values.ToArray();
    }
}