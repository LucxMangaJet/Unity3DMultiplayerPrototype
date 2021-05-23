using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingBehaviour : MonoBehaviour, IUsable
{
    public static int ANIM_MiddleFinger = Animator.StringToHash("MiddleFinger");
    public static int ANIM_Point = Animator.StringToHash("Point");

    PlayerActionHandler actionHandler;

    public void BeginPrimary()
    {
        actionHandler.Animator.SetBool(ANIM_Point, true);
    }
    public void EndPrimary(bool cancelled)
    {
        actionHandler.Animator.SetBool(ANIM_Point, false);
    }

    public void BeginSecondary()
    {
        actionHandler.Animator.SetBool(ANIM_MiddleFinger, true);
    }

    public void EndSecondary(bool cancelled)
    {
        actionHandler.Animator.SetBool(ANIM_MiddleFinger, false);
    }

    public void Initialize(PlayerActionHandler actionHandler)
    {
        this.actionHandler = actionHandler;
    }
}
