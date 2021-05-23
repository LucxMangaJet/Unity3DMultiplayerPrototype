using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingBehaviour : MonoBehaviour, IUsable
{
    private static int ANIM_MiddleFinger = Animator.StringToHash("MiddleFinger");
    private static int ANIM_Point = Animator.StringToHash("Point");

    PlayerActionHandler actionHandler;

    public void BeginPrimary()
    {
        actionHandler.Animator.SetBool(ANIM_Point, true);
    }
    public void EndPrimary()
    {
        actionHandler.Animator.SetBool(ANIM_Point, false);
    }

    public void BeginSecondary()
    {
        actionHandler.Animator.SetBool(ANIM_MiddleFinger, true);
    }

    public void EndSecondary()
    {
        actionHandler.Animator.SetBool(ANIM_MiddleFinger, false);
    }

    public void Initialize(PlayerActionHandler actionHandler)
    {
        this.actionHandler = actionHandler;
    }
}
