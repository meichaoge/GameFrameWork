using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
public class CustomerScrollRectItem : LoopScrollRectItemBase
{
    public Text m_DataText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void InitialedScrollCellItem(int idx)
    {
        base.InitialedScrollCellItem(idx);
        m_DataText.text = TestDataSources.Instance.GetDataById(m_DataIndex).ToString();

    }

}

#endif
