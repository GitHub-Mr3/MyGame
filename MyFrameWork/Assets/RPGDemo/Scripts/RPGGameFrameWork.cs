using Mr3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGGameFrameWork : MonoBehaviour
{
    private bool isAddNetLister;

    private void Awake()
    {

    }
    private void Start()
    {
        InitGame();
        gameObject.AddComponent<RPGMain>();
    }
    void InitGame()
    {
        EventMgr.Instance.Init();
        RPGNetMgr.Instance.AddNetListener();
        isAddNetLister = true;
        InitNetModule();
    }
    void InitNetModule()
    {
        NetManager.Instance.Connection("127.0.0.1", 10086);
    }
    private void Update()
    {
        if (isAddNetLister)
        {
            NetManager.Instance.Update();
        }
    }
}
