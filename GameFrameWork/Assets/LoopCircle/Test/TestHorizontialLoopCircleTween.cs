using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestHorizontialLoopCircleTween : MonoBehaviour {

    public int DataCount = 10;
    public int offset = 10;
    public float tweetime = 10;

    public HorizontialLoopCircleTween mHorizontialLoopCircleTween;


    public float data;
    void Start()
    {
        mHorizontialLoopCircleTween.OnItemCreateAct += OnCreateItem;
        mHorizontialLoopCircleTween.OnItemShowAct += OnShowItem;
        mHorizontialLoopCircleTween.OnItemRemoveAct += OnDeleteItem;

        mHorizontialLoopCircleTween.OnCompleteInitialedViewAct += OnCompleteInitialed;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mHorizontialLoopCircleTween.RefillData(DataCount);
        }


        if (mHorizontialLoopCircleTween.IsInitialed == false) return;

        offset = Random.Range(-10,10);
        data = Random.Range(0, 2f);
        if (data >= 1)
            Right();
        else
            Left();
        return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            Right();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Left();
        }


    }


    private void Right()
    {
        if (offset > 0)
            offset = -1 * offset;
        mHorizontialLoopCircleTween.RollViewEx(LoopViewShowDirection.Right, offset, tweetime, false);
    }


    private void Left()
    {
        if (offset < 0)
            offset = -1 * offset;
        mHorizontialLoopCircleTween.RollViewEx(LoopViewShowDirection.Left, offset, tweetime, false);

     
    }



    private void OnCreateItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetAddComponent<TestLoopCircleItem>();
    }

    private void OnShowItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetComponent<TestLoopCircleItem>().Show(OnDeleteData);
    }

    private void OnDeleteItem(Transform trans, LoopCircleItem loopItem)
    {
        Debug.Log("OnDeleteItem " + trans.gameObject.name + "   " + loopItem.DataIndex);

    }

    public void OnCompleteInitialed()
    {
        //foreach (var item in mHorizontialLoopCircleTween.mAllLoopCircleItems)
        //{
        //    Debug.Log(item.Key.name + "   " + item.Value.DataIndex);
        //}
    }


    private void OnDeleteData(LoopCircleItem item)
    {
        //Transform target = mHorizontialLoopCircleTween.GetFirstVisibleTrans();
        //if (target == null) return;
        //LoopCircleItem loopCircle = mHorizontialLoopCircleTween.GetLoopCircleItemByTrans(target);
        //if (loopCircle == null) return;

        //// 
        //mHorizontialLoopCircleTween.RefillData(LooCircledDataManager.Instance.Datas.Count, loopCircle.DataIndex);
    }
}
