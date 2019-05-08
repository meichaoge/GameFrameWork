using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestVerticalLoopCircle : MonoBehaviour {
    public int DataCount = 10;
    public VerticalLoopCircle mVerticalLoopCircle;
    void Start () {
        mVerticalLoopCircle.OnItemCreateAct += OnCreateItem;
        mVerticalLoopCircle.OnItemShowAct += OnShowItem;
        mVerticalLoopCircle.RefillData(DataCount);
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mVerticalLoopCircle.RefillData(DataCount);
        }
    }


    private void OnCreateItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetAddComponent<TestLoopCircleItem>();
    }

    private void OnShowItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetComponent<TestLoopCircleItem>().Show(null);
    }

}
