using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorMgr : AnimatorMgr
{
    EnemyLocomotionMgr _enemyLocomotionMgr;
    DamageCollider damageCollider;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        _enemyLocomotionMgr = GetComponentInParent<EnemyLocomotionMgr>();
        damageCollider = GetComponentInChildren<DamageCollider>();
        damageCollider.isEnemy = true;
    }
    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        if (delta == 0)
        {
            return;
        }
        _enemyLocomotionMgr.enemyRigidBody.drag = 0;
        Vector3 detaPos = anim.deltaPosition;
        detaPos.y = 0;
        Vector3 velocity = detaPos / delta;
        _enemyLocomotionMgr.enemyRigidBody.velocity = velocity;
    }

    #region 动画绑定的事件  
    public void OpenRightDamageCollider()
    {
        if (damageCollider == null)
        {
            return;
        }
        damageCollider.SetDamageColliderEnable(true);
    }
    public void OpenLeftDamageCollider()
    {
        if (damageCollider == null)
        {
            return;
        }
        damageCollider.SetDamageColliderEnable(true);
    }

    public void DisablenRightDamageCollider()
    {
        if (damageCollider == null)
        {
            return;
        }
        damageCollider.SetDamageColliderEnable(false);
    }
    public void DisableLeftDamageCollider()
    {
        if (damageCollider == null)
        {
            return;
        }
        damageCollider.SetDamageColliderEnable(false);
    }
    public void EnableCombo()
    {

    }
    public void DisableCombo()
    {

    }
    #endregion
}
