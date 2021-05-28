using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMassSetter : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Vector3 offset;

    private void Awake()
    {
        rigidbody.centerOfMass = offset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + offset, 0.2f);
    }
}
