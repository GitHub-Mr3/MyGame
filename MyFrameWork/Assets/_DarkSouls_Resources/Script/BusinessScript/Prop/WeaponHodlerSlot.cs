using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHodlerSlot : MonoBehaviour
{//脚本挂 在模型左右手上

    //武器的父节点
    public Transform parentOverride;
    //是否是左手持有
    public bool isLeftHandSlot;
    //是否是右手
    public bool isRightHandSlot;

    //当前武器
    public GameObject currentWeaponModel;
    //隐藏武器
    public void UnLoadEeapon()
    {
        if (currentWeaponModel != null)
        {
            currentWeaponModel.SetActive(false);
        }
    }
    //销毁武器
    public void UnLoadWeaponAndDestory()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    /// <summary>
    /// 加载武器
    /// </summary>
    /// <param name="weaponItem"></param>
    public void LoadWeaponModel(WeaponItem weaponItem)
    {
        UnLoadWeaponAndDestory();
        if (weaponItem == null)
        {
            UnLoadEeapon();
            return;
        }

        GameObject model = Instantiate(weaponItem.modelPrefab);
        if (model != null)
        {
            if (parentOverride != null)
            {
                model.transform.parent = parentOverride;
            }
            else
            {
                model.transform.parent = transform;
            }
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

        }

        currentWeaponModel = model;
    }

}
