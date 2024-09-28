using UnityEngine;

using UnityEngine.UI;

using TMPro;

using UnityEngine.EventSystems;

using System;
using Mr3;
using UnityEngine.SceneManagement;

public class DarkSoulsGameMain : MonoBehaviour
{

    //autoStart
    private Image Img_SligerBg = null;
    private Image Img_Sliger = null;
    private GameObject Obj_GameOVer = null;
    private Text Txt_OverTips = null;
    private Button Btn_ContinueBtn = null;
    private GameObject Obj_GM = null;
    private Button Btn_CloseGm = null;
    private InputField Input_Field1 = null;
    private Button Btn_GmBtn = null;
    private InputField Input_Field2 = null;

    private void Awake()
    {
        Img_SligerBg = gameObject.transform.Find("HP/Img_SligerBg").GetComponent<Image>();
        Img_Sliger = gameObject.transform.Find("HP/Img_Sliger").GetComponent<Image>();
        Obj_GameOVer = gameObject.transform.Find("Obj_GameOVer").gameObject;
        Txt_OverTips = gameObject.transform.Find("Obj_GameOVer/Txt_OverTips").GetComponent<Text>();
        Btn_ContinueBtn = gameObject.transform.Find("Obj_GameOVer/Btn_ContinueBtn").GetComponent<Button>();
        Obj_GM = gameObject.transform.Find("Obj_GM").gameObject;
        Btn_CloseGm = gameObject.transform.Find("Obj_GM/Btn_CloseGm").GetComponent<Button>();
        Input_Field1 = gameObject.transform.Find("Obj_GM/Input_Field1").GetComponent<InputField>();
        Btn_GmBtn = gameObject.transform.Find("Obj_GM/Btn_GmBtn").GetComponent<Button>();
        Input_Field2 = gameObject.transform.Find("Obj_GM/Input_Field2").GetComponent<InputField>();

    }
    //autoEnd
    //TODO  
    private void Start()
    {
        Txt_OverTips.text = "";
        Btn_Gm = Img_Sliger.GetComponent<Button>();
        EventMgr.Instance.Init();
        AddLister();
        Btn_ContinueBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        Btn_GmBtn.onClick.AddListener(OnClick_GM);
        Btn_Gm.onClick.AddListener(() =>
        {
            Time.timeScale = 0f; // 暂停游戏
            Obj_GM.SetActive(true);
        });
        Btn_CloseGm.onClick.AddListener(() =>
        {
            Time.timeScale = 1f; // 暂停游戏
            Obj_GM.SetActive(false);
        });
        Obj_GameOVer.SetActive(false);
        Obj_GM.SetActive(false);
        CreateEnemy(20);
    }
    void OnClick_GM()
    {
        string gmStr = Input_Field1.text;
        string gmParameter = Input_Field2.text;
        switch (gmStr)
        {
            case "createenemy":
                int num = 1;
                int.TryParse(gmParameter, out num);
                CreateEnemy(num);
                break;
            case "resethp":
                int curhp = 100;
                float maxhp = 100;
                ResetHP(curhp, maxhp);
                break;
            default:
                break;
        }
    }

    public void ResetHP(int curHP, float maxHP)
    {
        PlayerStatus playerStatus = FindObjectOfType<PlayerStatus>();
        playerStatus.SetPlayerStatus(curHP, maxHP);
    }
    public GameObject enemyObj;
    public Transform enemyParent;
    private int curEnemyNum = 1;
    void CreateEnemy(int Num)
    {
        curEnemyNum = Num + 1;
        for (int i = 0; i < Num; i++)
        {
            var obj = GameObject.Instantiate(enemyObj, enemyParent);
            Vector3 pos = Vector3.zero;
            pos.x = UnityEngine.Random.Range(-30, 30);
            pos.z = UnityEngine.Random.Range(-30, 30);
            pos.y = 0;
            obj.transform.localPosition = pos;
        }
    }
    Button Btn_Gm;
    private void AddLister()
    {
        EventMgr.Instance.AddListener(DarkSoulsConst.EVENT_SETHP, SetHpValue);
        EventMgr.Instance.AddListener(DarkSoulsConst.EVENT_OVER, GameOver);
    }

    private void GameOver(string eventName, object udata)
    {
        bool isWin = (bool)udata;
        if (isWin)
        {
            curEnemyNum--;
            if (curEnemyNum > 0)
            {
                return;
            }
            Txt_OverTips.text = "You Win";
        }
        else
        {
            Txt_OverTips.text = "You Lose";
        }
        Obj_GameOVer.SetActive(true);
    }

    private void OnDisable()
    {
        EventMgr.Instance.RemoveListener(DarkSoulsConst.EVENT_SETHP, SetHpValue);
        EventMgr.Instance.RemoveListener(DarkSoulsConst.EVENT_OVER, GameOver);
    }

    private void SetHpValue(string eventName, object udata)
    {
        float value = (float)udata;
        Img_Sliger.fillAmount = value;
    }
}
