using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tttt : MonoBehaviour
{
    BaseHuman baseHuman;
    // Start is called before the first frame update

    USER_STATE userState = USER_STATE.STATE_NOMO;
    private enum USER_STATE
    {
        //默认值 
        STATE_NOMO = 0,
        //主持人
        STATE_HOST = 1,
        //演唱者
        STATE_PARTICIPANT = 2
    }


    void Start()
    {
        int index = 2;

        userState = (USER_STATE)index;
        switch (userState)
        {
            case USER_STATE.STATE_NOMO:
                DebugMgr.Instance.Log("nome");
                break;
            case USER_STATE.STATE_HOST:
                DebugMgr.Instance.Log("STATE_HOST");
                break;
            case USER_STATE.STATE_PARTICIPANT:
                DebugMgr.Instance.Log("STATE_PARTICIPANT");
                break;
            default:
                break;
        }
        //baseHuman = transform.GetComponent<BaseHuman>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            baseHuman.PlayAnimByTrigger(Constant.SAD);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            baseHuman.PlayAnimByTrigger(Constant.WALK);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            baseHuman.PlayAnimByTrigger(Constant.IDLE);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            baseHuman.PlayAnimByTrigger(Constant.WIN);
        }

    }
}
