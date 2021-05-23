using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceLocalMesh : MonoBehaviourPun
{
    [SerializeField] SkinnedMeshRenderer renderer;
    [SerializeField] Mesh localMesh;

    private void Start()
    {
        if (photonView.IsMine)
            renderer.sharedMesh = localMesh;
    }
}