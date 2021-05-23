using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogHandler : ResourceHandler<CatalogObjectInfo>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        Debug.Log("Initializing CatalogHandler");
        InitializeResources("CatalogObjects");
    }
}
