using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoopCircleItem : MonoBehaviour
{
    public int DataIndex;

    public RectTransform rectTransform { get { return transform as RectTransform; } }
 
    public void InitialedView(int dex)
    {
        DataIndex = dex;
#if UNITY_EDITOR
        gameObject.name = string.Format("LoopCircleItem_{0}", DataIndex);
#endif
    }
}
