using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    //是否持有
    public bool isUnarmed;
    //[Header("One Handed Attack Animations")]
    ////轻攻击
    //public string OH_Light_Attack_01;
    ////生攻击
    //public string OH_Heavy_Attack_01;
}
