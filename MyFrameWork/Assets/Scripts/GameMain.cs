using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using Mr3;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{

    /********************************************************************
               _ooOoo_
              o8888888o
              88" . "88
              (| -_- |)
              O\  =  /O
           ____/`---'\____
         .'  \\|     |//  `.
        /  \\|||  :  |||//  \
       /  _||||| -:- |||||-  \
       |   | \\\  -  /// |   |
       | \_|  ''\---/''  |   |
       \  .-\__  `-`  ___/-. /
     ___`. .'  /--.--\  `. . __
  ."" '<  `.___\_<|>_/___.'  >'"".
 | | :  `- \`.;`\ _ /`;.`/ - ` : | |
 \  \ `-.   \_ __\ /__ _/   .-` /  /
======`-.____`-.___\_____/___.-`____.-'======
               `=---='
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
佛祖保佑		永无BUG		永不修改

*********************************************************************/

    public Button Btn;
    private void Awake()
    {
     
    }
    void Start()
    {


        EventMgr.Instance.Init();
        if (Btn != null)
        {

            EventMgr.Instance.AddListener("test", TestEvent);
            Btn.onClick.AddListener(() =>
            {
                EventMgr.Instance.Emit("test", "点击事件");
            });

        }

    }
    void TestEvent(string str, object obj)
    {
        string strs = (string)obj;
        Debug.Log("111111111+str" + strs);
    }

    private void OnDisable()
    {
        EventMgr.Instance.RemoveListener("test", TestEvent);
    }

}
