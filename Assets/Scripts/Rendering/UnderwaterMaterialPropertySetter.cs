using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterMaterialPropertySetter : MonoBehaviour
{
    private static readonly int MATID_disabled = Shader.PropertyToID("_disabled");

    [SerializeField] Material material;
    [SerializeField] Water water;
    private void OnEnable()
    {
        water.EnteredWater += OnEnteredWater;
        water.LeftWater += OnLeftWater;
    }

    private void OnDisable()
    {
        water.EnteredWater -= OnEnteredWater;
        water.LeftWater -= OnLeftWater;
    }

    private void OnLeftWater()
    {
        material.SetFloat(MATID_disabled, 1);
    }

    private void OnEnteredWater()
    {
        material.SetFloat(MATID_disabled, 0);
    }




    private void OnDestroy()
    {
        material.SetFloat(MATID_disabled, 1);
    }

}
