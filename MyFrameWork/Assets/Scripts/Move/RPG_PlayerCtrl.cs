using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG_PlayerCtrl : MonoBehaviour
{
    private Animator anim;
    public InputCtrl pi;
    private Transform model;
    public Rigidbody rigid;

    private Vector3 movingVec;
    public float movespeed = 1;
    public float isRun = 1;
    private void Awake()
    {
        model = transform.Find("Model");
        anim = model.GetComponent<Animator>();
        pi = GetComponent<InputCtrl>();
        rigid = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("forward", pi.Dmag * isRun);
        if (pi.Dmag > 0.1f)
        {
            model.transform.forward = pi.Dforward;
        }
        movingVec = pi.Dmag * model.transform.forward * movespeed;
    }
    private void FixedUpdate()
    {
        //rigid.position += Vector3.up * Time.fixedDeltaTime;
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z);
    }
}
