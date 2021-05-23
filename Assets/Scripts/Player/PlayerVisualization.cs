using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualization : MonoBehaviourPun
{
    [SerializeField] Transform holdItemSocket;

    private GameObject heldItem;

    public void SwitchHeldItemTo(ItemAmountPair item)
    {
        if (!photonView.IsMine) return;

        int id = -1;
        if (!item.IsNothing())
        {
            id = ItemHandler.GetItemIdFor(item);
        }
        photonView.RPC(nameof(RPC_SwitchHeldItem), RpcTarget.All, id);
    }

    [PunRPC]
    private void RPC_SwitchHeldItem(int id)
    {
        if (heldItem != null)
            Destroy(heldItem);

        if (id < 0)
        {
            heldItem = null;
        }
        else
        {
            ItemInfo info = ItemHandler.GetItemInfoFor(id);
            if (info != null)
            {
                if (info.HoldObject != null)
                {
                    var go = Instantiate(info.HoldObject, holdItemSocket);
                    heldItem = go;
                }
                else
                {
                    heldItem = null;
                }
            }
            else
            {
                Debug.LogWarning($"Trying to switch to item with ID {id}, item not found.");
            }
        }
    }
}
