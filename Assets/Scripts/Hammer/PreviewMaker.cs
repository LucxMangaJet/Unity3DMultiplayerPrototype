using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewMaker : MonoBehaviour
{
    Collider[] colliders;
    Renderer[] renderers;


    public void Initiate(Material mat)
    {
        gameObject.layer = 2;
        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();


        foreach (var c in colliders)
        {
            c.isTrigger = true;
        }

        foreach (var rs in renderers)
        {
            rs.material = mat;
        }
    }

    public Vector3? ClosetsPoint(Vector3 p)
    {
        float minDist = float.MaxValue;
        Vector3? minVal = null;

        foreach (var collider in colliders)
        {
            var closest = collider.ClosestPoint(p);

            float dist = Vector3.Distance(closest, p);

            if (dist < minDist)
            {
                minVal = closest;
            }

        }
        return minVal;
    }

}
