using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingPreview : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Material greenMat, redmat;

    public void SetMeshTo(Mesh m)
    {
        meshFilter.mesh = m;
    }

    public void HighlightGreen()
    {
        renderer.material = greenMat;
    }

    public void HighlightRed()
    {
        renderer.material = redmat;
    }

}
