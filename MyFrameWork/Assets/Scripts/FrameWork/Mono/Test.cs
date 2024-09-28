using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MonoManager.Instance.StartCoroutine(abc());
    }
    IEnumerator abc()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            DebugMgr.Instance.Log("Ð­³Ì");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
