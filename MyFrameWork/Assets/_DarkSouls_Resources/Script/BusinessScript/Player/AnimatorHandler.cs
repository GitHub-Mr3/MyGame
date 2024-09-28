using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : AnimatorMgr
{
    int vertical;
    int horizontal;
    public bool canRotate;

    public PlayerLocomotion playerLocomotion;
    public PlayerManager playerManager;
    public void Init()
    {
        anim = GetComponent<Animator>();
        //它接受一个字符串参数并返回一个对应的哈希码
        vertical = Animator.StringToHash(DarkSoulsConst.VERTICAL);
        horizontal = Animator.StringToHash(DarkSoulsConst.HORIZONTAL);


        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        playerManager = GetComponentInParent<PlayerManager>();
    }
    /// <summary>
    /// 设置玩家动作值
    /// </summary>
    /// <param name="_vertical">锤子值 </param>
    /// <param name="_horizontal">水平值</param>
    /// <param name="isSpritFlage">是否奔跑</param>
    public void UpdateAnimatorValuser(float _vertical, float _horizontal, bool isSpritFlage)
    {
        #region _vertical 值 
        float v = 0;
        if (_vertical > 0 && _vertical < 0.55f)
        {
            v = 0.5f;
        }
        else if (_vertical > 0.55f)
        {
            v = 1;
        }
        else if (_vertical < 0 && v > -0.55f)
        {
            v = -0.5f;
        }
        else if (_vertical < 0.05f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }
        #endregion
        #region _horizontal 值
        float h = 0;
        if (_horizontal > 0 && _horizontal < 0.55f)
        {
            h = 0.5f;
        }
        else if (_horizontal > 0.55f)
        {
            h = 1;
        }
        else if (_horizontal < 0 && v > -0.55f)
        {
            h = -0.5f;
        }
        else if (_horizontal < 0.05f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }
        #endregion
        //如果是奔跑
        if (isSpritFlage)
        {
            //2对应 的值 为行为树里面的奔跑
            v = 2;
            h = _horizontal;

        }
        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void CanRotate()
    {
        canRotate = true;
    }
    public void StopRotation()
    {
        canRotate = false;
    }

    //public void PlayerTargetAnimation(string aniName, bool isInteraction)
    //{
    //    anim.applyRootMotion = isInteraction;
    //    anim.SetBool(DarkSoulsConst.ISINTERACTING, isInteraction);
    //    anim.CrossFade(aniName, 0.2f);
    //}

    private void OnAnimatorMove()
    {
        if (playerManager.isInteracting == false)
        {
            return;
        }
        float delta = Time.deltaTime;
        if (delta == 0)
        {
            return;
        }
        playerLocomotion.rigidbody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.rigidbody.velocity = velocity;
    }
    #region 动画绑定的事件，不能删
    public void EnableCombo()
    {
        anim.SetBool(DarkSoulsConst.CANDOCOMBO, true);
    }
    public void DisableCombo()
    {
        anim.SetBool(DarkSoulsConst.CANDOCOMBO, false);
    }
    #endregion
}
