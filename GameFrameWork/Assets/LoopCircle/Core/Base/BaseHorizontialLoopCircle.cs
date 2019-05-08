using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 只在水平方向上布局 且左右滑动
/// </summary>
public abstract class BaseHorizontialLoopCircle : BaseLoopCircle
{
    public override float ItemDistance { get { return ItemSpace.x + ItemSize.x; } }
    protected override float ViewRectHalfSize { get { return ViewPortRectrans.rect.width / 2f; } }


    protected int LoopTime = 0;  //避免死循环记录循环次数





    #region 重写基类实现
    protected override void Awake()
    {
        IsVerticalLayot = false;
        base.Awake();
    }

    protected override void RollView(LoopViewShowDirection direction)
    {
        if (direction == LoopViewShowDirection.None || (int)direction < (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }

        ContentViewShowDirection = Direction = direction;
    }


    #endregion


    #region 基类实现
    /// <summary>
    /// 获取水平布局时候 每个Item默认的位置坐标
    /// </summary>
    /// <param name="dex"></param>
    /// <returns></returns>
    protected override Vector3 GetItemLocalPosByIndex(int dex)
    {
        if (IsImparDataCount)
        {
            if (dex < MiddleItemIndex)
                return new Vector3(-(MiddleItemIndex - dex) * ItemDistance, 0, 0);
            else
                return new Vector3((dex - MiddleItemIndex) * ItemDistance, 0, 0);
        }//显示区域奇数个
        else
        {
            if (dex < MiddleItemIndex)
                return new Vector3(-HalfItemDistance - (MiddleItemIndex - dex) * ItemDistance, 0, 0);
            else
                return new Vector3(HalfItemDistance + (dex - 1 - MiddleItemIndex) * ItemDistance, 0, 0);
        }// 偶数个
    }

    //protected override bool IsItemVisible(RectTransform item)
    //{
    //    if (IsVerticalLayot)
    //        return true;
    //    return ViewPortRectrans.IsInsideRect_Horizontial(item);
    //}

    #endregion

    #region 水平布局滑动
    protected virtual bool OnBeginScrollView()
    {
        if (Direction == LoopViewShowDirection.None)
            return false;

        IsScrolling = true;
        if ((int)Direction < 10)
        {
            Debug.LogError("没有定义的类型 " + Direction);
            return false;
        }
        AutoSetItemState();
        return true;
    }

    /// <summary>
    /// 自动调整多出的那个项的位置
    /// </summary>
    protected virtual void AutoSetItemState()
    {
        LoopTime = 0;
        RectTransform lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
        RectTransform firstItem = ContentRectrans.GetChildEX(0);
        if (Direction == LoopViewShowDirection.Left)
        {
            //***如果第一个项可见且最后一个项不可见 则移动最后一个到第一个
            while (LoopTime < ContentRectrans.childCount && IsItemVisible(firstItem) && IsItemVisible(lastItem) == false)
            {
                ScrollLeftDirection(lastItem, firstItem);
                lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
                firstItem = ContentRectrans.GetChildEX(0);
                ++LoopTime;
            }
        }
        else
        {
            //**如果最后一个项可见且第一一个项不可见 则移动第一个到最后以个
            while (LoopTime < ContentRectrans.childCount && IsItemVisible(lastItem) && IsItemVisible(firstItem) == false)
            {
                ScrollRightDirection(firstItem, lastItem);
                lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
                firstItem = ContentRectrans.GetChildEX(0);
                ++LoopTime;
            }//由于快读滑动时候可能一下子滑出超过多个Item项的距离 所以这里是一个while 循环
        }
    }



    /// <summary>
    /// 每次视图改变都需要调用
    /// </summary>
    protected override void UpdateItemState()
    {
        AutoSetItemState();
    }



    /// <summary>
    ///将最后一个不可见得项移动到开头  
    /// </summary>
    /// <param name="operateItem"></param>
    protected virtual void ScrollLeftDirection(RectTransform lastItem, RectTransform firstItem)
    {
        //if (lastItem == null)
        //    lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
        lastItem.SetAsFirstSibling();
        lastItem.anchoredPosition = new Vector2(firstItem.anchoredPosition.x - ItemDistance, firstItem.anchoredPosition.y);

        LoopCircleItem lastloopCircle = AllLoopCircleItems[lastItem];
        LoopCircleItem firstloopCircle = AllLoopCircleItems[firstItem];

        lastloopCircle.InitialedView(firstloopCircle.DataIndex - 1);
        if (OnItemShowAct != null)
            OnItemShowAct.Invoke(lastItem, lastloopCircle);
    }

    /// <summary>
    /// 将第一个不可见的项移动到末尾
    /// </summary>
    /// <param name="firstItem"></param>
    protected virtual void ScrollRightDirection(RectTransform firstItem, RectTransform lastItem)
    {
        //if (firstItem == null)
        //    firstItem = ContentRectrans.GetChildEX(0);

        firstItem.SetAsLastSibling();
        firstItem.anchoredPosition = new Vector2(lastItem.anchoredPosition.x + ItemDistance, lastItem.anchoredPosition.y);
        LoopCircleItem firstloopCircle = AllLoopCircleItems[firstItem];
        LoopCircleItem lastloopCircle = AllLoopCircleItems[lastItem];

        firstloopCircle.InitialedView(lastloopCircle.DataIndex + 1);
        if (OnItemShowAct != null)
            OnItemShowAct.Invoke(firstItem, firstloopCircle);
    }

    #endregion

}
