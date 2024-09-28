using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRPGMove : MonoBehaviour
{
    public string keysUp = "w";
    public string keysDown = "s";
    public string keysLeft = "a";
    public string keysRight = "d";


    private float dUP;
    private float dUP1;
    private float dUP2;
    private float dRIght;
    private float dRIght1;
    private float dRIght2;

    private float dmage;

    Vector3 forward;
    Rigidbody rigid;
    Animator anim;
    Transform model;
    private void Awake()
    {
        model = transform.Find("Model");
        rigid = GetComponent<Rigidbody>();
        anim = model.GetComponent<Animator>();
    }
    Vector3 moveVec;
    private void Update()
    {
        dUP1 = (Input.GetKey(keysUp) ? 1 : 0) - (Input.GetKey(keysDown) ? 1 : 0);

        dRIght1 = (Input.GetKey(keysRight) ? 1 : 0) - (Input.GetKey(keysLeft) ? 1 : 0);

        dUP = Mathf.SmoothDamp(dUP, dUP1, ref dUP2, 0.1f);

        dRIght = Mathf.SmoothDamp(dRIght, dRIght1, ref dRIght2, 0.1f);

        dmage = Mathf.Sqrt((dUP * dUP) + (dRIght * dRIght));

        anim.SetFloat("forward", dmage);
        forward = dUP * transform.forward + dRIght * transform.right;
        if (dmage > 0.1f)
        {
            model.transform.forward = forward;
        }
        moveVec = forward * dmage;

    }
    private void LateUpdate()
    {
        rigid.velocity = new Vector3(moveVec.x, rigid.velocity.y, moveVec.z);
    }

}
