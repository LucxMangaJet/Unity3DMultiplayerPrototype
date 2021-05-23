using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemInfo : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public GameObject HoldObject;
    public GameObject BehaviourObject;
}
