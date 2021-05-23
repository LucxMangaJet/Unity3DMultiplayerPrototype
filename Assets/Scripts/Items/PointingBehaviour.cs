using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingBehaviour : MonoBehaviour, IUsable
{
    PlayerActionHandler actionHandler;

    bool inSecondary;

    public void BeginPrimary()
    {
    }
    public void EndPrimary()
    {
    }

    public void BeginSecondary()
    {
        actionHandler.RigEffector.EnableRightArmOverride();
    }

    public void EndSecondary()
    {
        actionHandler.RigEffector.DisableRightArmOverride();
    }

    private void Update()
    {
        actionHandler.RigEffector.PointRightArmInDirection(actionHandler.CameraTransform.forward);
    }

    public void Initialize(PlayerActionHandler actionHandler)
    {
        this.actionHandler = actionHandler;
    }
}
