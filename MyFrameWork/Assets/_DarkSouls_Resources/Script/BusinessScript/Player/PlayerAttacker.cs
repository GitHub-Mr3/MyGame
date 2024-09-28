using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler animatorHandler;
    InputHandler inputHandler;
    public string lastAttack;
    public bool comboFlag;
    // Start is called before the first frame update
    void Start()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void HandleWeaponCombo()
    {

        //if (comboFlag)
        {
            animatorHandler.anim.SetBool(DarkSoulsConst.CANDOCOMBO, false);
            if (lastAttack == DarkSoulsConst.OH_LIGHT_ATTACK_01)
            {
                animatorHandler.PlayerTargetAnimation(DarkSoulsConst.OH_LIGHT_ATTACK_02, true);
            }
        }
      
    }
    public void HandleLightAttack()
    {
        animatorHandler.PlayerTargetAnimation(DarkSoulsConst.OH_LIGHT_ATTACK_01, true);
        lastAttack = DarkSoulsConst.OH_LIGHT_ATTACK_01;
    }
    public void HandleHeavyAttack()
    {
        animatorHandler.PlayerTargetAnimation(DarkSoulsConst.OH_HEAVY_ATTACK_01, true);
        lastAttack = DarkSoulsConst.OH_HEAVY_ATTACK_01;

    }

}
