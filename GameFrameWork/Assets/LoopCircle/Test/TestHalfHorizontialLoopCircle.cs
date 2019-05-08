using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestHalfHorizontialLoopCircle : MonoBehaviour
{
    public int DataCount;
    public int DataOffset = 0;
    public HorizontialHalfLoopCircleTween m_HorizontialHalfLoopCircleFixdistance;
    public float offset = 10;
    public float tweetime = 10;
    private float distance = 0;

    void Start()
    {
        distance = m_HorizontialHalfLoopCircleFixdistance.ItemSize.x + m_HorizontialHalfLoopCircleFixdistance.ItemSpace.x;
        m_HorizontialHalfLoopCircleFixdistance.OnItemCreateAct += OnCreateItem;
        m_HorizontialHalfLoopCircleFixdistance.OnItemShowAct += OnShowItem;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LooCircledDataManager.Instance.RefillDate(DataCount);
            m_HorizontialHalfLoopCircleFixdistance.RefillData(DataCount, DataOffset);
        }


      int  data = Random.Range(0, 3);
        offset = Random.Range(-10, 10);
        if (data >= 1)
            Left();
        else
            Right();
        return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            Left();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Right();
        }

    }


    private void Left()
    {
        if (offset < 0)
            offset = -1 * offset;
        m_HorizontialHalfLoopCircleFixdistance.RollView(LoopViewShowDirection.Left, offset * distance, tweetime, false);

    }

    private void Right()
    {
        if (offset > 0)
            offset = -1 * offset;
        m_HorizontialHalfLoopCircleFixdistance.RollView(LoopViewShowDirection.Right, offset * distance, tweetime, false);
    }



    private void OnCreateItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetAddComponent<TestLoopCircleItem>();
    }

    private void OnShowItem(Transform trans, LoopCircleItem loopItem)
    {
        trans.GetComponent<TestLoopCircleItem>().Show(OnDeleteData);
    }


    private void OnDeleteData(LoopCircleItem item)
    {
        RectTransform target = m_HorizontialHalfLoopCircleFixdistance.GetFirstVisibleTrans();
        if (target == null) return;
        LoopCircleItem loopCircle = m_HorizontialHalfLoopCircleFixdistance.GetLoopCircleItemByTrans(target);
        if (loopCircle == null) return;

        // 
        m_HorizontialHalfLoopCircleFixdistance.RefillData(LooCircledDataManager.Instance.Datas.Count, loopCircle.DataIndex);
    }
}
