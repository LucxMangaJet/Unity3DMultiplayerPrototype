using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamager
{
    PlayerMetaStates GetMetaStates();
}

public class Club : MonoBehaviour, IDamager
{
    public PlayerMetaStates GetMetaStates()
    {
        return GetComponentInParent<PlayerMetaStates>();
    }
}
