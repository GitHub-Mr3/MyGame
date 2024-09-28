using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class BaseHuman : MonoBehaviour
{
    /// <summary>
    /// 是否正在移动
    /// </summary>
    protected bool isMoving = false;
    /// <summary>
    /// 移动目标点
    /// </summary>
    private Vector3 targetPosition;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float speed = 1.2f;
    /// <summary>
    /// 描述
    /// </summary>
    public string desc = "";
    /// <summary>
    /// 动画组件
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 唯一ID
    /// </summary>
    public string userId = "";

    /// <summary>
    /// 移动到某处
    /// </summary>
    /// <param name="pos"></param>
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        DebugMgr.Instance.Log($"开始移动：{pos}");
        PlayAnimByTrigger(Constant.WALK);
        //播放移动的动作
    }

    public void Win()
    {
        PlayAnimByTrigger(Constant.WIN);
    }

    public void EndMove()
    {
        isMoving = false;
        PlayAnimByTrigger(Constant.IDLE);
    }
    protected void SetUserID(string id)
    {
        userId = id;

    }
    protected void SetInitInfo(Vector3 pos)
    {
        isMoving = false;
        if (currentAnimation != "")
        {
            PlayAnimByTrigger(Constant.IDLE);
        }
        targetPosition = pos;
    }

    protected void Start()
    {
        
        animator = GetComponent<Animator>();
    }
    protected void Update()
    {
        MoveUpdate();


    }
    private string currentAnimation = "";
    public void PlayAnimByTrigger(string triggerName)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string currentAnimationName = "";

        if (clipInfo.Length > 0)
        {
            // 获取当前播放的动画片段名称
            currentAnimationName = clipInfo[0].clip.name;



            if (currentAnimationName == triggerName)
            {
                return;
            }
            else
            {
                if (currentAnimationName != Constant.IDLE && triggerName != Constant.IDLE)
                {
                    return;
                }
            }

        }
        DebugMgr.Instance.LogError("当前正在播放的动画是：" + currentAnimationName + "即将要的动作：" + triggerName);

        //if (triggerName == currentAnimation)
        //{
        //    DebugMgr.Instance.Log("一样的动作，不需要播放");
        //    return;
        //}

        currentAnimation = triggerName;
        animator.SetTrigger(triggerName);

        if (triggerName == Constant.WIN)
        {
            //StartCoroutine(IEPlayAnimByTrigger(2, Constant.IDLE));
        }
    }
    IEnumerator IEPlayAnimByTrigger(float time, string triggerName)
    {
        yield return new WaitForSeconds(time);
        PlayAnimByTrigger(triggerName);
    }

    private void MoveUpdate()
    {
        if (isMoving == false)
        {
            return;
        }
        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            DebugMgr.Instance.Log("移动到目标点位");
            //PlayAnimByTrigger(Constant.IDLE);
            JsonData moveJson = new JsonData();
            moveJson["userId"] = userId;
            moveJson["movePos_x"] = targetPosition.x;
            moveJson["movePos_y"] = targetPosition.y;
            moveJson["movePos_z"] = targetPosition.z;

            NetMgr.Instance.SendMst(NetMgr.MSG_ENDMOVE, moveJson);
        }
    }


}
