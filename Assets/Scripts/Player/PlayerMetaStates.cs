using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetaStates : MonoBehaviourPun, IPunObservable
{

    bool attacking;

    public bool Attacking => attacking;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(attacking);
        }
        else
        {
            attacking = (bool)stream.ReceiveNext();
        }
    }


    public void SetAttacking(bool state)
    {
        if (photonView.IsMine)
            attacking = state;
    }
}
