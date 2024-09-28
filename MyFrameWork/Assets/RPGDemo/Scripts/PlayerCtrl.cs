using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Animator anim;
    Rigidbody rigid;
    Transform model;
    InputMgr intMgr;

    Vector3 moveDic;

    public bool isRun;
    private float speed = 2;

    private void Awake()
    {
        model = transform.Find("Model");
        anim = model.GetComponent<Animator>();
        rigid = transform.GetComponent<Rigidbody>();
        intMgr = FindObjectOfType<InputMgr>();
    }
    void Update()
    {
        //ÉèÖÃtrigger
        anim.SetFloat("forward", intMgr.Dmage * (isRun ? 2 : 1));
        if (intMgr.Dmage > 0.1f)
        {
            model.transform.forward = intMgr.Fforward;
        }
        moveDic = model.transform.forward * intMgr.Dmage * speed * (isRun ? 2 : 1);
    }
    private void LateUpdate()
    {
        rigid.velocity = new Vector3(moveDic.x, rigid.velocity.y, moveDic.z);
    }
}
