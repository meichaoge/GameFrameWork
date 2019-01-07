using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseHorizontialLoopCircle : BaseLoopCircle
{
    protected override Vector3 GetItemLocalPosByIndex(int dex)
    {
        Debug.LogError("TODO");
        return Vector3.zero;
    }

    protected override bool IsItemVisible(Transform item)
    {
        Debug.LogError("TODO");
        return false;
    }

    protected override void UpdateItemState()
    {
        Debug.LogError("TODO");
    }
}
