using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestMain : MonoBehaviour
{
    // Start is called before the first frame update
    Button Btn_T;
    private void Awake()
    {
        Btn_T = transform.Find("Btn_T").GetComponent<Button>();
    }
    void Start()
    {
        Btn_T.onClick.AddListener(OnClick_Btn);
    }
    public void OnClick_Btn()
    {
        Debug.LogError("点击了测试 按钮");
        EventCenter.Broadcast(EventType.ShowText, 1, 2);
    }
    private void OnEnable()
    {
        EventCenter.AddListener<int,int>(EventType.ShowText, OnNotify_T);
    }

    public void OnNotify_T(int a,int b)
    {
        Debug.LogError("事件消息"+a+"   "+b);
    }
    private void OnDisable()
    {
        EventCenter.RemoveListener<int,int>(EventType.ShowText, OnNotify_T);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
