using UnityEngine;
using UnityEngine.UI;
using System.Collections;


#if UNITY_EDITOR
public class ScrollIndexCallback3 : LoopScrollRectItemBase
{
    public Text text;

    public override void InitialedScrollCellItem(int idx)
    {
        base.InitialedScrollCellItem(idx);
        if (text != null)
        {
            text.text = name;
        }
    }
}



#endif