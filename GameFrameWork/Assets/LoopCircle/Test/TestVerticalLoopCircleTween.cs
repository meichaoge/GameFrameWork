using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestVerticalLoopCircleTween : MonoBehaviour
{
    public int DataCount = 10;

    public int offset = 10;
  //  private int offset2 = 0;
    public float tweetime = 10;
 //   public int Count = 3;  //转动的圈数

    private float distance = 0;
    private int data = 0;
    public VerticalLoopCircleTween mVerticalLoopCircleTween;


    void Start()
    {
        distance = mVerticalLoopCircleTween.ItemSize.y + mVerticalLoopCircleTween.ItemSpace.y;
        mVerticalLoopCircleTween.OnItemCreateAct += OnCreateItem;
        mVerticalLoopCircleTween.OnItemShowAct += OnShowItem;
        mVerticalLoopCircleTween.OnItemRemoveAct += OnDeleteItem;

        mVerticalLoopCircleTween.OnCompleteInitialedViewAct += OnCompleteInitialed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            mVerticalLoopCircleTween.RefillData(DataCount);
        }
        if (mVerticalLoopCircleTween.IsInitialed == false) return;

        data = Random.Range(0, 3);
        offset = Random.Range(-10, 10);
        //if (data >= 1)
        //    Down();
        //else
        //    Up();
        //return;
        if (Input.GetKeyDown(KeyCode.D))
        {
            Down();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Up();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            mVerticalLoopCircleTween.GetAllVisibleLoopItems();
        }
    }


    private void Down()
    {
        //   offset2 = DataCount;
        if (offset < 0)
            offset = -1 * offset;
        mVerticalLoopCircleTween.RollViewEx(LoopViewShowDirection.Down, offset, tweetime, false);
    }


    private void Up()
    {
        //    offset2 = DataCount;
        if (offset > 0)
            offset = -1 * offset;
        mVerticalLoopCircleTween.RollViewEx(LoopViewShowDirection.Up, offset, tweetime, false);
    }


    private void OnCreateItem(Transform trans, LoopCircleItem loopItem)
    {
        Debug.Log("OnCreateItem " + trans.gameObject.name + "   " + loopItem.DataIndex);
        trans.GetAddComponent<TestLoopCircleItem>();
    }

    private void OnShowItem(Transform trans, LoopCircleItem loopItem)
    {
        Debug.Log("OnShowItem " + trans.gameObject.name + "   " + loopItem.DataIndex);
        trans.GetComponent<TestLoopCircleItem>().Show(OnDeleteData);
    }

    private void OnDeleteItem(Transform trans, LoopCircleItem loopItem)
    {
        Debug.Log("OnDeleteItem " + trans.gameObject.name + "   " + loopItem.DataIndex);

    }

    public void OnCompleteInitialed()
    {
        foreach (var item in mVerticalLoopCircleTween.mAllLoopCircleItems)
        {
            //Debug.Log(item.Key.name + "   " + item.Value.DataIndex);
        }
    }
    private void OnDeleteData(LoopCircleItem item)
    {
        //Transform target = mVerticalLoopCircleTween.GetFirstVisibleTrans();
        //if (target == null) return;
        //LoopCircleItem loopCircle = mVerticalLoopCircleTween.GetLoopCircleItemByTrans(target);
        //if (loopCircle == null) return;
        //// 
        //mVerticalLoopCircleTween.RefillData(LooCircledDataManager.Instance.Datas.Count, loopCircle.DataIndex);
    }

}
