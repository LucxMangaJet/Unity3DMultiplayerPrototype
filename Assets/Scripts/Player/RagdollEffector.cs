using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEffector : MonoBehaviour
{

    [SerializeField] Transform rootBone;
    [SerializeField] Transform headBone;

    [SerializeField] bool debug;
    [NaughtyAttributes.ShowIf("debug")]
    [SerializeField] Transform debugPoint;
    [NaughtyAttributes.ShowIf("debug")]
    [SerializeField] Vector3 debugForce;
    [NaughtyAttributes.ShowIf("debug")]
    [SerializeField] float falloffDistance;

    Rigidbody[] rigidbodies;

    public Transform Root => rootBone;

    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        if (debug)
            Push();
    }

    [NaughtyAttributes.Button]
    public void Push()
    {
        AddForceAtPoint(debugPoint.position, debugForce.normalized, debugForce.magnitude, falloffDistance);
    }

    public void AddForceAtPoint(Vector3 point, Vector3 direction, float force, float fallOffDistance)
    {
        Rigidbody rb = GetClosestRigidbody(point);
        rb.AddForceAtPosition(direction * force, point, ForceMode.Impulse);
    }

    public Transform GetHeadJoint()
    {
        return headBone;
    }

    public void MatchRig(Transform root)
    {
        Dictionary<string, Transform> nameBoneMap = new Dictionary<string, Transform>();
        Stack<Transform> unprocessed = new Stack<Transform>();
        unprocessed.Push(root);

        while (unprocessed.Count > 0)
        {
            Transform t = unprocessed.Pop();

            nameBoneMap.Add(t.name, t);
            for (int i = 0; i < t.childCount; i++)
            {
                unprocessed.Push(t.GetChild(i));
            }
        }

        unprocessed.Clear();
        unprocessed.Push(rootBone);

        while (unprocessed.Count > 0)
        {
            Transform t = unprocessed.Pop();

            if (nameBoneMap.ContainsKey(t.name))
            {

                Transform refT = nameBoneMap[t.name];
                t.position = refT.position;
                t.rotation = refT.rotation;
            }

            for (int i = 0; i < t.childCount; i++)
            {
                unprocessed.Push(t.GetChild(i));
            }
        }
    }

    private Rigidbody GetClosestRigidbody(Vector3 p)
    {
        float minDist = float.MaxValue;
        Rigidbody closest = null;
        foreach (var r in rigidbodies)
        {
            float dist = (r.worldCenterOfMass - p).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = r;
            }
        }
        return closest;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (debugPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(debugPoint.position, 0.1f);

            UnityEditor.Handles.ArrowHandleCap(0, debugPoint.position, Quaternion.FromToRotation(Vector3.forward, debugForce), 1, EventType.Repaint);
        }
#endif
    }

}
