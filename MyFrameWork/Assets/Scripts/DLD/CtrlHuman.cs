using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHuman : BaseHuman
{
    // Start is called before the first frame update

    public void SetUserID(string id)
    {
        base.Start();
        DebugMgr.Instance.Log($"CtrlHuman SetUserID {id} ");
        base.SetUserID(id);
    }

    public void SetInitInfos(Vector3 pos)
    {
        base.SetInitInfo(pos);
    }


    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider != null)
            {
                if (hit.collider.tag == Constant.TERRAIN)
                {
                    //MoveTo(hit.point);
                    JsonData moveJson = new JsonData();
                    moveJson["userId"] = userId;
                    moveJson["movePos_x"] = hit.point.x;
                    moveJson["movePos_y"] = hit.point.y;
                    moveJson["movePos_z"] = hit.point.z;
                    NetMgr.Instance.SendMst(NetMgr.MSG_MOVE, moveJson);
                }
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            PlayAnimByTrigger(Constant.SAD);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Win();
            return;
            RaycastHit hit;
            Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
            Vector3 lineStart = lineEnd + 20 * transform.forward;
            if (Physics.Linecast(lineStart, lineEnd, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj == gameObject)
                {
                    return;
                }
                SyncHuma h = hitObj.GetComponent<SyncHuma>();
                if (h == null)
                {
                    return;
                }
                Win();
                DebugMgr.Instance.LogError("¹¥»÷µ½ÁË");
            }

        }
    }
}
