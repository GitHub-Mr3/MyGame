using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mr3;

public class CoroutineHelper : SingleMonoBase<CoroutineHelper>
{

    public void DoFunc(string str, IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
}
