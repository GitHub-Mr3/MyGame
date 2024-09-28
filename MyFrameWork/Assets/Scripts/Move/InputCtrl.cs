using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCtrl : MonoBehaviour
{
    public string keysUp = "w";
    public string keysDown = "s";
    public string keysLeft = "a";
    public string keysRight = "d";

    public float Dir_UP;
    public float Dir_Right;

    private float temp_up;
    private float temp_up2;
    private float temp_right;
    private float temp_right2;

    public float Dmag;
    public Vector3 Dforward;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        temp_up = (Input.GetKey(keysUp) ? 1.0f : 0) - (Input.GetKey(keysDown) ? 1.0f : 0);
        Dir_UP = Mathf.SmoothDamp(Dir_UP, temp_up, ref temp_up2, 0.1f);

        temp_right = (Input.GetKey(keysRight) ? 1.0f : 0) - (Input.GetKey(keysLeft) ? 1.0f : 0);
        Dir_Right = Mathf.SmoothDamp(Dir_Right, temp_right, ref temp_right2, 0.1f);

        Dmag = Mathf.Sqrt((Dir_UP * Dir_UP) + (Dir_Right * Dir_Right));
        Dforward = Dir_Right * transform.right + Dir_UP * transform.forward;

    }
}
