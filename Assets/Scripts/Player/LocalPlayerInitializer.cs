using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerInitializer : MonoBehaviour
{
    [SerializeField] GameObject playerHUDPrefab;
    [SerializeField] Toolbar toolbar;

    void Start()
    {
        Debug.Log("Initializing Local Player");

        var hudObject = Instantiate(playerHUDPrefab);
        if(hudObject.TryGetComponent(out PlayerHUD hud))
        {
            hud.Initialize(toolbar);
        }
    }
}
