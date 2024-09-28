using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMgr : MonoBehaviour
{
    public Animator anim;
    public void PlayerTargetAnimation(string aniName, bool isInteraction)
    {
        anim.applyRootMotion = isInteraction;
        anim.SetBool(DarkSoulsConst.ISINTERACTING, isInteraction);
        anim.CrossFade(aniName, 0.2f);
    }
}
