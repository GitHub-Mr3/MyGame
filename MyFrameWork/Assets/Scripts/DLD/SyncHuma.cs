using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncHuma : BaseHuman
{

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
    public void SetUserID(string id)
    {
        base.Start();
        base.SetUserID(id);
    }
    public void SetInitInfos(Vector3 pos)
    {
        base.SetInitInfo(pos);
    }
}
