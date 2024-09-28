using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    Vector3 offset;
    private Transform playerTrs;
    // Start is called before the first frame update
    void Start()
    {
        playerTrs = GameObject.Find("Player").transform;
        offset = transform.position - playerTrs.position;
    }
    private void FixedUpdate()
    {
        transform.position = playerTrs.position + offset;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
