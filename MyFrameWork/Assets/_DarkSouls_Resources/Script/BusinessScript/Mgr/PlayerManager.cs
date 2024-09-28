using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    InputHandler _inputHandler;
    Animator _anim;
    CameraHandler cameraHandler;
    //是否触发输入事件
    public bool isInteracting;

    PlayerLocomotion playerLocomotion;
    //是否在空中
    public bool isInAir;
    ///是否 在地面
    public bool isGrounded;

    public bool canDoCobo;

    public PlayerInventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        _inputHandler = GetComponent<InputHandler>();
        _anim = GetComponentInChildren<Animator>();
        cameraHandler = FindObjectOfType<CameraHandler>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        isInteracting = _anim.GetBool(DarkSoulsConst.ISINTERACTING);
        canDoCobo = _anim.GetBool(DarkSoulsConst.CANDOCOMBO);


        float delta = Time.deltaTime;
        _inputHandler.TickInput(delta);
        //玩家移动和旋转
        playerLocomotion.HandlerMoveAndRotation(delta);
        //玩家滚动动画（前翻和后退）
        playerLocomotion.HandlerRollingAnim(delta);
        //玩家从空中 掉落动画
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        //角色跳跃
        playerLocomotion.HandlerJump();
        //角色攻击
        playerLocomotion.HandleAttackInput();

        //  角色切换武器
        playerInventory.HandleQuickSlotsInput();

        //角色凝视
        playerLocomotion.HandleLockOnInput();

    }


    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        if (cameraHandler != null)
        {
            ///相机移动跟随
            cameraHandler.FollowTarget(delta);
            //相机旋转
            cameraHandler.HandlerCameraRotation(delta, _inputHandler.mouseX, _inputHandler.mouseY);
        }
        else
        {
            cameraHandler = CameraHandler.singleton;
        }
    }
    private void LateUpdate()
    {
        _inputHandler.isRollFlag = false;
        _inputHandler.isSpritFlage = false;
        _inputHandler.rb_Input = false;
        _inputHandler.rt_Input = false;
        _inputHandler.is_Pad_Left = false;
        _inputHandler.is_Pad_Right = false;
        _inputHandler.is_Pad_Down = false;
        _inputHandler.is_Pad_Up = false;

        _inputHandler.is_Jump = false;

        //_inputHandler.isLockOnFlag = false;
        //_inputHandler.is_LockOn_Input = false;


        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }

    }

}
