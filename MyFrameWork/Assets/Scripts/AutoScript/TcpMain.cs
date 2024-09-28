using UnityEngine;

using UnityEngine.UI;

using TMPro;

using UnityEngine.EventSystems;

using System;
using Mr3;

public class TcpMain : MonoBehaviour
{

    //autoStart
    public InputField Input_Txt = null;
	public Button Btn_Content = null;
	public Button Btn_Send = null;
	public Text Txt_Msg = null;
	
    private void Awake()
	{
		Input_Txt = gameObject.transform.Find("Input_Txt").GetComponent<InputField>();
		Btn_Content = gameObject.transform.Find("Btn_Content").GetComponent<Button>();
		Btn_Send = gameObject.transform.Find("Btn_Send").GetComponent<Button>();
		Txt_Msg = gameObject.transform.Find("Txt_Msg").GetComponent<Text>();
		      
	}
    //autoEnd
    //TODO   
    bool isInitFinish = false;
    private void Start()
    {
        EventMgr.Instance.Init();
        EventMgr.Instance.AddListener("testmsg", SetTestMg);

        EventMgr.Instance.AddListener(Constant.LISTERMSG, SetTestMg);

        Btn_Content.onClick.AddListener(() =>
        {

            NetManager.Instance.Connection("127.0.0.1", 10086);
        });
        Btn_Send.onClick.AddListener(() =>
        {
            NetManager.Instance.Send(Input_Txt.text);
        });
        isInitFinish = true;
    }
    void SetTestMg(string str, object obj)
    {
        string msgstr = (string)obj;
        Txt_Msg.text = msgstr;
    }
    public void Update()
    {
        if (isInitFinish)
        {
            NetManager.Instance.Update();
        }
    }

}
