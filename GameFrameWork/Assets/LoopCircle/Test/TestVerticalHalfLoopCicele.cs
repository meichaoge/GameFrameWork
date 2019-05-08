using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestVerticalHalfLoopCicele : MonoBehaviour {

    public int DataCount;
    public int DataOffset = 0;
    public VerticalHalfLoopCicleTween m_VerticalHalfLoopCicleTween;
    public float offset = 10;
    public float tweetime = 10;
    private float distance = 0;

    void Start()
    {
        distance = m_VerticalHalfLoopCicleTween.ItemSize.x + m_VerticalHalfLoopCicleTween.ItemSpace.x;
        m_VerticalHalfLoopCicleTween.OnItemCreateAct += OnCreateItem;
        m_VerticalHalfLoopCicleTween.OnItemShowAct += OnShowItem;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LooCircledDataManager.Instance.RefillDate(DataCount);
            m_VerticalHalfLoopCicleTween.RefillData(DataCount, DataOffset);
        }

        //int data = Random.Range(0, 3);
        //offset = Random.Range(-10, 10);
        //if (data >= 1)
        //    Down();
        //else
        //    Up();
        //return;

        if (Input.GetKeyDown(KeyCode.U))
        {
            Up();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Down();
        }

    }


    private void Down()
    {
        if (offset < 0)
            offset = -1 * offset;
        m_VerticalHalfLoopCicleTween.RollView(LoopViewShowDirection.Down, offset * distance, tweetime, false);
    }

    private void Up()
    {
        if (offset > 0)
            offset = -1 * offset;
        m_VerticalHalfLoopCicleTween.RollView(LoopViewShowDirection.Up, offset * distance, tweetime, false);
    }
    private void OnCreateItem(RectTransform trans, LoopCircleItem loopItem)
    {
        Debug.Log("OnCreateItem __" + trans.name + "  DataIndex=" + loopItem.DataIndex);
        trans.GetAddComponent<TestLoopCircleItem>();
    }

    private void OnShowItem(RectTransform trans, LoopCircleItem loopItem)
    {
        Debug.Log("OnShowItem __" + trans.name + "  DataIndex=" + loopItem.DataIndex);

        trans.GetComponent<TestLoopCircleItem>().Show(OnDeleteData);
    }


    private void OnDeleteData(LoopCircleItem item)
    {
        Debug.Log("OnDeleteData __" + item.name + "  DataIndex=" + item.DataIndex);

        RectTransform target = m_VerticalHalfLoopCicleTween.GetFirstVisibleTrans();
        if (target == null) return;
        LoopCircleItem loopCircle = m_VerticalHalfLoopCicleTween.GetLoopCircleItemByTrans(target);
        if (loopCircle == null) return;

        // 
        m_VerticalHalfLoopCicleTween.RefillData(LooCircledDataManager.Instance.Datas.Count, loopCircle.DataIndex);
    }
}
