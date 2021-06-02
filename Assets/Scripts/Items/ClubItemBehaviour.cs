using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubItemBehaviour : MonoBehaviour, IUsable
{
    private static int ANIM_WeaponSwing = Animator.StringToHash("WeaponSwing");

    PlayerActionHandler actionHandler;

    public void Initialize(PlayerActionHandler actionHandler)
    {
        this.actionHandler = actionHandler;
    }

    public void BeginPrimary()
    {
        actionHandler.Animator.SetBool(ANIM_WeaponSwing, true);
    }

    public void EndPrimary(bool cancelled)
    {
        actionHandler.Animator.SetBool(ANIM_WeaponSwing, false);
    }

    public void EndSecondary(bool cancelled)
    {
    }

    public void BeginSecondary()
    {
    }
}
