using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHodlerSlot leftHandSlot;
    WeaponHodlerSlot rightHandSlot;

    DamageCollider leftHandDamageCollider;
    DamageCollider rightHandDamageCollider;

    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        WeaponHodlerSlot[] weaponHandlerSlots = GetComponentsInChildren<WeaponHodlerSlot>();

        foreach (var item in weaponHandlerSlots)
        {
            if (item.isLeftHandSlot)
            {
                leftHandSlot = item;
            }
            else if (item.isRightHandSlot)
            {
                rightHandSlot = item;
            }
        }

    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
            if (weaponItem != null)
            {
                animator.CrossFade(DarkSoulsConst.LEFT_IDLE_01, 0.2f);
            }
            else
            {
                animator.CrossFade(DarkSoulsConst.LEFTARMEMPT, 0.2f);
            }
        }
        else
        {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();

            if (weaponItem != null)
            {
                animator.CrossFade(DarkSoulsConst.RIGHT_IDLE_01, 0.2f);
            }
            else
            {
                animator.CrossFade(DarkSoulsConst.RIGHTARMEMPT, 0.2f);
            }

        }
    }

    void LoadLeftWeaponDamageCollider()
    {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        leftHandDamageCollider.isEnemy = false;
    }

    void LoadRightWeaponDamageCollider()
    {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        rightHandDamageCollider.isEnemy = false;
    }

    #region 动画绑定的事件 不能删除
    public void OpenRightDamageCollider()
    {
        if (rightHandDamageCollider == null)
        {
            return;
        }
        rightHandDamageCollider.SetDamageColliderEnable(true);
    }
    public void OpenLeftDamageCollider()
    {
        if (leftHandDamageCollider == null)
        {
            return;
        }
        leftHandDamageCollider.SetDamageColliderEnable(true);
    }

    public void DisablenRightDamageCollider()
    {
        if (rightHandDamageCollider == null)
        {
            return;
        }
        rightHandDamageCollider.SetDamageColliderEnable(false);
    }
    public void DisableLeftDamageCollider()
    {
        if (leftHandDamageCollider == null)
        {
            return;
        }
        leftHandDamageCollider.SetDamageColliderEnable(false);
    }
    #endregion

}
