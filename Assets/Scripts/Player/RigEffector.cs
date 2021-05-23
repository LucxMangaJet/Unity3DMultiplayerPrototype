using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigEffector : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Transform rootJoint, headJoint;
    [SerializeField] Transform rightArmJoint, rightArmMidJoint, rightHandJoint;


    //replicated
    Vector3 lookDir;
    bool overrideRightArm;
    Vector3 rightArmDirection;

    public void LookInDirection(Vector3 fwd)
    {
        lookDir = fwd;
    }

    public void EnableRightArmOverride()
    {
        overrideRightArm = true;
    }

    public void PointRightArmInDirection(Vector3 p)
    {
        rightArmDirection = p;
    }

    public void DisableRightArmOverride()
    {
        overrideRightArm = false;
    }

    private void LateUpdate()
    {
        headJoint.forward = lookDir;

        if (overrideRightArm)
        {
            rightArmJoint.up = rightArmDirection;
            rightArmMidJoint.localEulerAngles = Vector3.zero;
            rightHandJoint.localEulerAngles = Vector3.zero;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lookDir);
            stream.SendNext(overrideRightArm);
            stream.SendNext(rightArmDirection);
        }
        else
        {
            lookDir = (Vector3)stream.ReceiveNext();
            overrideRightArm = (bool)stream.ReceiveNext();
            rightArmDirection = (Vector3)stream.ReceiveNext();
        }
    }
}
