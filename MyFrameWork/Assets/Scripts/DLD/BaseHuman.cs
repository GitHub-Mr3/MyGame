using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// 基础人类角色类，处理角色移动、动画播放和网络同步
/// </summary>
public class BaseHuman : MonoBehaviour
{
    /// <summary>
    /// 标记角色是否正在移动
    /// </summary>
    protected bool isMoving = false;
    
    /// <summary>
    /// 移动目标位置
    /// </summary>
    private Vector3 targetPosition;
    
    /// <summary>
    /// 移动速度（单位/秒）
    /// </summary>
    public float speed = 1.2f;
    
    /// <summary>
    /// 角色描述信息
    /// </summary>
    public string desc = "";
    
    /// <summary>
    /// 动画控制器组件
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 角色唯一标识符
    /// </summary>
    public string userId = "";

    /// <summary>
    /// 当前播放的动画名称
    /// </summary>
    private string currentAnimation = "";

    /// <summary>
    /// 开始移动到指定位置
    /// </summary>
    /// <param name="pos">目标位置</param>
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        DebugMgr.Instance.Log($"开始移动到{pos}");
        PlayAnimByTrigger(Constant.WALK);
    }

    /// <summary>
    /// 播放胜利动画
    /// </summary>
    public void Win()
    {
        PlayAnimByTrigger(Constant.WIN);
    }

    /// <summary>
    /// 结束移动状态
    /// </summary>
    public void EndMove()
    {
        isMoving = false;
        PlayAnimByTrigger(Constant.IDLE);
    }
    
    /// <summary>
    /// 设置角色唯一ID
    /// </summary>
    /// <param name="id">用户ID</param>
    protected void SetUserID(string id)
    {
        userId = id;
    }
    
    /// <summary>
    /// 初始化角色信息
    /// </summary>
    /// <param name="pos">初始位置</param>
    protected void SetInitInfo(Vector3 pos)
    {
        isMoving = false;
        if (currentAnimation != "")
        {
            PlayAnimByTrigger(Constant.IDLE);
        }
        targetPosition = pos;
    }

    /// <summary>
    /// 组件初始化
    /// </summary>
    protected void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// 每帧更新逻辑
    /// </summary>
    protected void Update()
    {
        MoveUpdate();
    }
    
    /// <summary>
    /// 播放指定触发器的动画
    /// </summary>
    /// <param name="triggerName">动画触发器名称</param>
    public void PlayAnimByTrigger(string triggerName)
    {
        // 获取当前动画片段信息
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string currentAnimationName = "";

        if (clipInfo.Length > 0)
        {
            // 获取当前播放的动画片段名称
            currentAnimationName = clipInfo[0].clip.name;

            // 避免重复播放相同动画
            if (currentAnimationName == triggerName)
            {
                return;
            }
            else
            {
                // 非空闲状态下不切换到其他非空闲动画
                if (currentAnimationName != Constant.IDLE && triggerName != Constant.IDLE)
                {
                    return;
                }
            }
        }
        
        DebugMgr.Instance.LogError("当前正在播放的动画是：" + currentAnimationName + "，需要播放的动画是" + triggerName);

        currentAnimation = triggerName;
        animator.SetTrigger(triggerName);
    }
    
    /// <summary>
    /// 延迟播放动画协程
    /// </summary>
    /// <param name="time">延迟时间（秒）</param>
    /// <param name="triggerName">动画触发器名称</param>
    /// <returns>IEnumerator</returns>
    IEnumerator IEPlayAnimByTrigger(float time, string triggerName)
    {
        yield return new WaitForSeconds(time);
        PlayAnimByTrigger(triggerName);
    }

    /// <summary>
    /// 移动更新逻辑
    /// </summary>
    private void MoveUpdate()
    {
        if (!isMoving)
        {
            return;
        }
        
        // 平滑移动到目标位置
        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        
        // 到达目标位置
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            DebugMgr.Instance.Log("移动到目标位置");
            
            // 发送移动结束消息到服务器
            JsonData moveJson = new JsonData();
            moveJson["userId"] = userId;
            moveJson["movePos_x"] = targetPosition.x;
            moveJson["movePos_y"] = targetPosition.y;
            moveJson["movePos_z"] = targetPosition.z;
            NetMgr.Instance.SendMst(NetMgr.MSG_ENDMOVE, moveJson);
        }
    }
}