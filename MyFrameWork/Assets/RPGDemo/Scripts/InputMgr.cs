using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour
{

    private string keysUp = "w";
    private string keysDown = "s";
    private string keysLeft = "a";
    private string keysRight = "d";

    //按上下的返回值
    private float Dic_Up;
    #region 用于临时计算
    private float Dic_Up1;
    private float Dic_Up2;
    #endregion
    //按左右的返回值
    private float Dic_Rigth;
    private float Dic_Rigth1;
    private float Dic_Rigth2;

    //输入的值(上下左右的总和)
    [HideInInspector]
    public float Dmage;
    [HideInInspector]
    public Vector3 Fforward;


    private void Update()
    {
        Dic_Up1 = (Input.GetKey(keysUp) ? 1.0f : 0) - (Input.GetKey(keysDown) ? 1.0f : 0);
        Dic_Rigth1 = (Input.GetKey(keysRight) ? 1.0f : 0) - (Input.GetKey(keysLeft) ? 1.0f : 0);

        Dic_Up = Mathf.SmoothDamp(Dic_Up, Dic_Up1, ref Dic_Up2, 0.1f);
        Dic_Rigth = Mathf.SmoothDamp(Dic_Rigth, Dic_Rigth1, ref Dic_Rigth2, 0.1f);

        var point = CommonUtils.Instance.SquareToCircle(new Vector2(Dic_Up, Dic_Rigth));
        Dic_Up = point.x;
        Dic_Rigth = point.y;

        Dmage = Mathf.Sqrt((Dic_Up * Dic_Up) + (Dic_Rigth * Dic_Rigth));

        Fforward = Dic_Up * transform.forward + Dic_Rigth * transform.right;

    }

}
