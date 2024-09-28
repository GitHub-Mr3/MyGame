using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    InputHandler inputHandler;
    //移动方向
    public Vector3 moveDirection;

    public new Rigidbody rigidbody;
    //相机
    Transform cameraObj;
    //角色当前对象
    [HideInInspector]
    public Transform myTransform;

    /// <summary>
    /// 移动的速度
    /// </summary>
    float movementSpeed = 5;
    /// <summary>
    ///旋转速度
    /// </summary>
    float rotationSpeed = 10;

    [HideInInspector]
    public AnimatorHandler animatorHandler;
    /// <summary>
    ///  加速的速度
    /// </summary>
    float sprintSpeed = 7;

    float groundDetectionRayStartPoint = 0.5f;
    float minimumDistanceNeededToBeginFall = 1f;//角色需要最小距离
    float groundDirectionRayDistance = 0.2f;//到达地面所需要的长度

    LayerMask ignoreForGroundCheck;

    public float inAirTimer;//在空中的时间

    float fallingSpeed = 45;//降落的速度

    public PlayerManager playerManager;

    PlayerAttacker playerAttacker;

    CameraHandler cameraHandler;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        cameraObj = Camera.main.transform;
        myTransform = transform;
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        animatorHandler.Init();
        playerManager = GetComponent<PlayerManager>();
        playerManager.isGrounded = true;
        ignoreForGroundCheck = ~2;

        cameraHandler = FindObjectOfType<CameraHandler>();
    }
    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
    }


    //是否加速
    bool isSpriteing;
    /// <summary>
    /// 角色移动和旋转
    /// </summary>
    /// <param name="delta"></param>
    public void HandlerMoveAndRotation(float delta)
    {
        if (playerManager.isInteracting)
        {
            return;
        }
        //表示使用相机的前向向量（cameraObj.forward）乘以垂直输入（inputHandler.vertical），得到的结果代表玩家在相机前后方向上的移动方向。
        moveDirection = cameraObj.forward * inputHandler.vertical;
        //cameraObj.right * inputHandler.horizontal 表示使用相机的右侧向量（cameraObj.right）乘以水平输入（inputHandler.horizontal），得到的结果代表玩家在相机左右方向上的移动方向。
        moveDirection += cameraObj.right * inputHandler.horizontal;

        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;
        if (inputHandler.isSpritFlage && inputHandler.moveAmount > 0.5f)
        {
            speed = sprintSpeed;
            isSpriteing = true;
        }
        else
        {
            isSpriteing = false;
        }
        moveDirection *= speed;
        Vector3 projectVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectVelocity;
        //根据键盘输入的值设置行为数里面的状态
        animatorHandler.UpdateAnimatorValuser(inputHandler.moveAmount, 0, isSpriteing);
        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }
    /// <summary>
    /// 角色旋转
    /// </summary>
    /// <param name="delta"></param>
    void HandleRotation(float delta)
    {
        Vector3 targetDir = Vector3.zero;

        targetDir = cameraObj.forward * inputHandler.vertical;
        targetDir += cameraObj.right * inputHandler.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;

        //为0即代表没有输入
        if (targetDir == Vector3.zero)
            targetDir = myTransform.forward;


        Quaternion tr = Quaternion.LookRotation(targetDir);
        //、、函数在当前 Transform 的旋转和目标旋转之间进行球形插值，通过 rotationSpeed *delta 控制旋转速度，得到插值后的目标旋转 targetRotation。
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rotationSpeed * delta);

        myTransform.rotation = targetRotation;

    }
    /// <summary>
    /// 玩家滚动动画（前翻或者 后退）
    /// </summary>
    /// <param name="delta"></param>
    public void HandlerRollingAnim(float delta)
    {
        if (playerManager.isInteracting)
        {
            return;
        }
        if (animatorHandler.anim.GetBool(DarkSoulsConst.ISINTERACTING))
        {
            return;
        }
        if (inputHandler.isRollFlag)
        {
            moveDirection = cameraObj.forward * inputHandler.vertical;
            moveDirection += cameraObj.right * inputHandler.horizontal;
            if (inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayerTargetAnimation(DarkSoulsConst.ROLLING, true);
                moveDirection.y = 0;
                Quaternion rollRotationo = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotationo;
            }
            else
            {
                animatorHandler.PlayerTargetAnimation(DarkSoulsConst.BACKSTEP, true);
            }
        }
    }

    Vector3 normalVector;//角色碰到地面
    Vector3 targetPosition;//角色降落后的位置
    /// <summary>
    /// 角色降落
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="moveDirection"></param>
    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 orgin = myTransform.position;
        orgin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(orgin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;

        }

        if (playerManager.isInAir)
        {
            rigidbody.AddForce(-Vector3.up * fallingSpeed);
            rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        orgin = orgin + dir * groundDirectionRayDistance;
        targetPosition = myTransform.position;
        //使用 Debug.DrawRay 方法绘制一条射线，用于检测是否到达开始下落所需的最小距离，
        Debug.DrawRay(orgin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(orgin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {//如果检测到地面，则更新角色状态为在地面上，并调整目标位置的高度。
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;
            if (playerManager.isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    animatorHandler.PlayerTargetAnimation(DarkSoulsConst.LAND, true);
                }
                else
                {
                    animatorHandler.PlayerTargetAnimation(DarkSoulsConst.EMPTY, false);
                    inAirTimer = 0;
                }
                playerManager.isInAir = false;
            }
        }
        else
        {
            //正在下降 
            if (playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if (playerManager.isInAir == false)
            {
                if (playerManager.isInteracting == false)
                {
                    animatorHandler.PlayerTargetAnimation(DarkSoulsConst.FALLING, true);
                }
                Vector3 vel = rigidbody.velocity;
                vel.Normalize();
                rigidbody.velocity = vel * (movementSpeed / 2);
                playerManager.isInAir = true;
            }
        }
        if (playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }
    }

    public void HandlerJump()
    {
        if (inputHandler.is_Jump)
        {
            if (playerManager.isInteracting)
            {
                return;
            }
            animatorHandler.PlayerTargetAnimation(DarkSoulsConst.JUMP, false);
        }
    }
    /// <summary>
    /// 攻击
    /// </summary>
    public void HandleAttackInput()
    {
        if (playerManager.isInteracting)
        {
            return;
        }

        if (inputHandler.rb_Input)
        {
            if (playerManager.canDoCobo)
            {

                playerAttacker.HandleWeaponCombo();

            }
            else
            {
                if (playerManager.isInteracting)
                {
                    return;
                }
                if (playerManager.canDoCobo)
                {
                    return;
                }
                playerAttacker.HandleLightAttack();
            }
        }
        if (inputHandler.rt_Input)
        {
            playerAttacker.HandleHeavyAttack();
        }
    }

    public void HandleLockOnInput()
    {
        if (inputHandler.is_LockOn_Input && !inputHandler.isLockOnFlag)
        {//凝视
            cameraHandler.ClearLockOnTargets();
            inputHandler.is_LockOn_Input = false;

            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                inputHandler.isLockOnFlag = true;
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
            }
        }
        else if (inputHandler.is_LockOn_Input && inputHandler.isLockOnFlag)
        {//恢复普通视野
            inputHandler.is_LockOn_Input = false;
            inputHandler.isLockOnFlag = false;
            cameraHandler.ClearLockOnTargets();
        }
    }



}
