using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Mono管理器
/// </summary>
public class MonoManager : SingletonBase<MonoManager>
{
    //构造方法私有化，防止外部new对象
    protected MonoManager() { }
    private MonoController monoExecuter;
    private MonoController MonoExecuter
    {
        get
        {
            if (monoExecuter == null)
            {
                GameObject obj = new GameObject(typeof(MonoController).Name);
                monoExecuter = obj.AddComponent<MonoController>();
            }
            return monoExecuter;
        }
    }
    //外部通过它来开启协程
    public Coroutine StartCoroutine(IEnumerator ie)
    {
        return MonoExecuter.StartCoroutine(ie);
    }
    /// <summary>
    /// 让外部通过它来停止协程
    /// </summary>
    /// <param name="ie"></param>
    public void StopCoroutine(IEnumerator ie)
    {
        if (ie != null)
        {
            MonoExecuter.StopCoroutine(ie);
        }
    }

    /// <summary>
    /// 让外部通过它来停止协程
    /// </summary>
    /// <param name="ie"></param>
    public void StopCoroutine(Coroutine ie)
    {
        if (ie != null)
        {
            MonoExecuter.StopCoroutine(ie);
        }
    }
    /// <summary>
    /// 让外部通过它来停止所有协程
    /// </summary>
    /// <param name="ie"></param>
    public void StopAllCoroutines()
    {
        MonoExecuter.StopAllCoroutines();
    }

    #region 添加和移除Update事件
    ///// <summary>
    ///// 添加UPdate事件 不要用Lambda表达式
    ///// </summary>
    /// <param name="call"></param>
    public void AddUpdateLister(UnityAction call)
    {
        MonoExecuter.AddUpdateLister(call);
    }
    ///// <summary>
    ///// 移除UPdate事件 不要用Lambda表达式
    ///// </summary>
    public void RemoveUpdateLister(UnityAction call)
    {
        MonoExecuter.RemoveUpdateLister(call);
    }
    ///// <summary>
    ///// 移除所有UPdate事件 不要用Lambda表达式
    ///// </summary>
    public void RemoveAllUpdateLister()
    {
        MonoExecuter.RemoveAllUpdateLister();
    }
    #endregion




    public class MonoController : MonoBehaviour
    {

        event UnityAction updateEvent;
        private void Update()
        {
            updateEvent?.Invoke();
        }
        public void AddUpdateLister(UnityAction call)
        {
            updateEvent += call;
        }
        public void RemoveUpdateLister(UnityAction call)
        {
            updateEvent -= call;
        }
        public void RemoveAllUpdateLister()
        {
            updateEvent = null;
        }
    }

}
