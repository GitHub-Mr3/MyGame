using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏背包相关
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager _weaponSlotManager;
    public WeaponItem leftWeapon;
    public WeaponItem rightWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponInRightHandSlots = new WeaponItem[1];
    public WeaponItem[] weaponInLeftHandSlots = new WeaponItem[1];
    public int currendRightWeaponIndex = -1;
    public int currendLeftWeaponIndex = -1;
    InputHandler inputHandler;

    private void Awake()
    {
        _weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        inputHandler = GetComponent<InputHandler>();
    }
    private void Start()
    {

        leftWeapon = unarmedWeapon;
        rightWeapon = unarmedWeapon;
        return;
        rightWeapon = weaponInRightHandSlots[currendRightWeaponIndex];
        leftWeapon = weaponInLeftHandSlots[currendLeftWeaponIndex];

        _weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        _weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }


    public void ChangeRightWeapon()
    {
        currendRightWeaponIndex = currendRightWeaponIndex + 1;
        if (currendRightWeaponIndex == 0 && weaponInRightHandSlots[0] != null)
        {
            rightWeapon = weaponInRightHandSlots[currendRightWeaponIndex];
            _weaponSlotManager.LoadWeaponOnSlot(weaponInRightHandSlots[currendRightWeaponIndex], false);
        }
        else if (currendRightWeaponIndex == 0 && weaponInRightHandSlots[0] == null)
        {
            currendRightWeaponIndex = currendRightWeaponIndex + 1;
        }
        else if (currendRightWeaponIndex == 1 && weaponInRightHandSlots[1] != null)
        {
            rightWeapon = weaponInRightHandSlots[currendRightWeaponIndex];
            _weaponSlotManager.LoadWeaponOnSlot(weaponInRightHandSlots[currendRightWeaponIndex], false);
        }
        else if (currendRightWeaponIndex == 1 && weaponInRightHandSlots[1] == null)
        {
            currendRightWeaponIndex = currendRightWeaponIndex + 1;
        }
        if (currendRightWeaponIndex > weaponInRightHandSlots.Length - 1)
        {
            currendRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        }
    }

    public void ChangeleftWeapon()
    {
        currendLeftWeaponIndex = currendLeftWeaponIndex + 1;
        if (currendLeftWeaponIndex == 0 && weaponInLeftHandSlots[0] != null)
        {
            leftWeapon = weaponInLeftHandSlots[currendLeftWeaponIndex];
            _weaponSlotManager.LoadWeaponOnSlot(weaponInLeftHandSlots[currendLeftWeaponIndex], true);
        }
        else if (currendLeftWeaponIndex == 0 && weaponInLeftHandSlots[0] == null)
        {
            currendLeftWeaponIndex = currendLeftWeaponIndex + 1;
        }
        else if (currendLeftWeaponIndex == 1 && weaponInLeftHandSlots[1] != null)
        {
            leftWeapon = weaponInLeftHandSlots[currendLeftWeaponIndex];
            _weaponSlotManager.LoadWeaponOnSlot(weaponInLeftHandSlots[currendLeftWeaponIndex], true);
        }
        else if (currendLeftWeaponIndex == 1 && weaponInLeftHandSlots[1] == null)
        {
            currendLeftWeaponIndex = currendLeftWeaponIndex + 1;
        }
        if (currendLeftWeaponIndex > weaponInLeftHandSlots.Length - 1)
        {
            currendLeftWeaponIndex = -1;
            leftWeapon = unarmedWeapon;
            _weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
        }
    }
    /// <summary>
    /// 武器切换
    /// </summary>
    public void HandleQuickSlotsInput()
    {

        if (inputHandler.is_Pad_Left)
        {
            ChangeleftWeapon();
        }
        if (inputHandler.is_Pad_Right)
        {
            ChangeRightWeapon();
        }
    }
}
