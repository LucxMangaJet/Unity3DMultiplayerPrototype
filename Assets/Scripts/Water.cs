using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public event System.Action EnteredWater;
    public event System.Action LeftWater;

    private void OnTriggerEnter(Collider other)
    {
        TryChangePlayerMovementStrategy(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TryChangePlayerMovementStrategy(other);
    }

    private void TryChangePlayerMovementStrategy(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            if (player.IsLocalPlayer())
            {
                //below water plane
                bool underWater = Vector3.Dot(Vector3.up, (player.transform.position - transform.position)) < 0;

                if (underWater)
                {
                    SwitchPlayerToSwim(player);
                    EnteredWater?.Invoke();
                }
                else
                {
                    SwitchPlayerToNormal(player);
                    LeftWater?.Invoke();
                }

            }
        }
    }

    private void SwitchPlayerToSwim(PlayerController pc)
    {
        pc.SwitchToSwim();
    }

    private void SwitchPlayerToNormal(PlayerController pc)
    {
        pc.SwitchToNormal();
    }
}
