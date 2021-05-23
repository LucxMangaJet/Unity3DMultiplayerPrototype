using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigEffector : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Transform rootJoint, headJoint;

    //replicated
    Vector3 lookDir;

    public void LookInDirection(Vector3 fwd)
    {
        lookDir = fwd;
    }


    private void LateUpdate()
    {
        headJoint.forward = lookDir;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lookDir);
        }
        else
        {
            lookDir = (Vector3)stream.ReceiveNext();
        }
    }
}
