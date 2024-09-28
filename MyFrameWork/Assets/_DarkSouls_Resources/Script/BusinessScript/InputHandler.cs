using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    //输入的水平值
    public float horizontal;
    //输入的垂直值
    public float vertical;
    //输入的水平和垂直值的总合 
    public float moveAmount;
    //鼠标X轴
    public float mouseX;
    //鼠标Y轴
    public float mouseY;

    PlayerControl inputActions;
    //移动值 的输入 WSAD键盘输入
    Vector2 movementInput;
    //鼠标值的输入
    Vector2 cameraInput;

    public bool isInput;
    /// <summary>
    /// 是否滚动
    /// </summary>
    public bool isRollFlag;

    /// <summary>
    /// 是否为跳跃
    /// </summary>
    public bool is_Jump;

    //public bool comboFlag;

    /// <summary>
    /// 键盘上下左右键
    /// </summary>
    public bool is_Pad_Up;
    public bool is_Pad_Down;
    public bool is_Pad_Left;
    public bool is_Pad_Right;

    public bool is_LockOn_Input;

    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControl();
            //移动方向监听(WASD键盘)
            inputActions.PlayerMovement.MovementAction.performed += outputActions => movementInput = outputActions.ReadValue<Vector2>();
            //相机旋转监听(鼠标)
            inputActions.PlayerMovement.Camera.performed += cameraActions => cameraInput = cameraActions.ReadValue<Vector2>();
            ////攻击 R和E键
            inputActions.PlayerAction.RB.performed += i => rb_Input = true;
            inputActions.PlayerAction.RT.performed += i => rt_Input = true;

            //inputActions.PlayerAction.Roll.performed += i => isInput = true;

            inputActions.PlayerAction.Jump.performed += i => is_Jump = true;

            HandleQuickSlotsInput();
            //角色凝视
            HandleLockOn();

        }
        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {

        MoveInput();

        HandleRollInput(delta);


    }
    /// <summary>
    /// 监听移动和旋转的值
    /// </summary>
    public void MoveInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }
    //按下的时间
    float rollInputTimer = 0;
    /// <summary>
    ///  是否奔跑
    /// </summary>
    public bool isSpritFlage;

    //角色凝视
    public bool isLockOnFlag;

    /// <summary>
    /// 前滚动和后退
    /// </summary>
    /// <param name="delta"></param>
    private void HandleRollInput(float delta)
    {
        //如果按下shift键
        isInput = inputActions.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        //DebugMgr.Instance.Log($"{inputActions.PlayerAction.Roll.phase}  {UnityEngine.InputSystem.InputActionPhase.Started}  {isInput}");
        if (isInput)
        {
            rollInputTimer += delta;
            isSpritFlage = true;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                isSpritFlage = false;
                isRollFlag = true;
            }
            rollInputTimer = 0;
        }
    }
    
    public bool rb_Input;
    public bool rt_Input;

    private void HandleQuickSlotsInput()
    {
        inputActions.PlayerQuickSlots.DPadRight.performed += i => is_Pad_Right = true;
        inputActions.PlayerQuickSlots.DPadLeft.performed += i => is_Pad_Left = true;

    }
    private void HandleLockOn()
    {
        inputActions.PlayerAction.LockOn.performed += i => is_LockOn_Input = transform;
    }

    /// <summary>
    /// 攻击
    /// </summary>

}
