using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 只在垂直方向上布局 且上下滑动 
/// </summary>
public abstract class BaseVerticalLoopCircle : BaseLoopCircle
{
    public override float ItemDistance { get { return ItemSpace.y + ItemSize.y; } }
    protected override float ViewRectHalfSize { get { return ViewPortRectrans.rect.height / 2f; } }



    protected int LoopTime = 0;  //避免死循环记录循环次数

    #region 重写基类实现
    protected override void Awake()
    {
        base.Awake();
        IsVerticalLayot = true;
    }

    protected override void RollView(LoopViewShowDirection direction)
    {
        if (direction == LoopViewShowDirection.None || (int)direction > (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }
        ContentViewShowDirection = Direction = direction;
    }



    #endregion

    #region 基类实现

    /// <summary>
    /// 获取垂直布局时候 每个Item默认的位置坐标
    /// </summary>
    /// <param name="dex"></param>
    /// <returns></returns>
    protected override Vector3 GetItemLocalPosByIndex(int dex)
    {
        if (IsImparDataCount)
        {
            if (dex < MiddleItemIndex)
                return new Vector3(0, (MiddleItemIndex - dex) * ItemDistance, 0);
            else
                return new Vector3(0, -(dex - MiddleItemIndex) * ItemDistance, 0);
        }//显示区域奇数个
        else
        {
            if (dex < MiddleItemIndex)
                return new Vector3(0, HalfItemDistance + (MiddleItemIndex - dex) * ItemDistance, 0);
            else
                return new Vector3(0, -HalfItemDistance - (dex - MiddleItemIndex - 1) * ItemDistance, 0);
        }//偶数个
    }



    #endregion

    #region 垂直布局滑动
    protected virtual bool OnBeginScrollView()
    {
        if (Direction == LoopViewShowDirection.None)
            return false;

        IsScrolling = true;
        if ((int)Direction >= (int)LoopViewShowDirection.Left)
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
        LoopTime = 0;  //防止While循环条件一直成立导致导致死循环
        RectTransform lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
         RectTransform firstItem = ContentRectrans.GetChildEX(0);
        if (Direction == LoopViewShowDirection.Up)
        {
            //**检查最后一个项是否可见    如果不可见则移动到顶部
            while (LoopTime < ContentRectrans.childCount &&IsItemVisible(lastItem) == false && IsItemVisible(firstItem))
            {
                ScrollUpDirection(lastItem, firstItem);
                firstItem = ContentRectrans.GetChildEX(0);
                lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
                ++LoopTime;
            }//由于快读滑动时候可能一下子滑出超过多个Item项的距离
        }
        else
        {
            //***检测第一个项是否可见 如果不可见则移动到末尾
            while (LoopTime < ContentRectrans.childCount && IsItemVisible(firstItem) == false && IsItemVisible(lastItem)) 
            {
                ScrollDownDirection(firstItem, lastItem);
                lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);
                firstItem = ContentRectrans.GetChildEX(0);
                ++LoopTime;
            }
        }
    }


    /// <summary>
    /// 将第一个不可见得项移动到最后
    /// </summary>
    /// <param name="operateItem"></param>
    protected virtual void ScrollDownDirection(RectTransform firstItem, RectTransform lastItem)
    {
        //if (firstItem == null)
        //    firstItem = ContentRectrans.GetChildEX(0);
        //if (lastItem == null)
        //    lastItem = ContentRectrans.GetChildEX(ContentRectrans.childCount - 1);

        firstItem.SetAsLastSibling();
        firstItem.anchoredPosition = new Vector2(lastItem.anchoredPosition.x, lastItem.anchoredPosition.y - ItemDistance);
        LoopCircleItem firstloopCircle = AllLoopCircleItems[firstItem];
        LoopCircleItem lastloopCircle = AllLoopCircleItems[lastItem];

        firstloopCircle.InitialedView(lastloopCircle.DataIndex + 1); //重新初始化赋值

        if (OnItemShowAct != null)
            OnItemShowAct.Invoke(firstItem, firstloopCircle);
    }

    /// <summary>
    /// 将最后不可见的项移动到第一个
    /// </summary>
    /// <param name="firstItem"></param>
    protected virtual void ScrollUpDirection(RectTransform lastItem, RectTransform firstItem)
    {
        lastItem.SetAsFirstSibling();
        lastItem.anchoredPosition = new Vector2(firstItem.anchoredPosition.x, firstItem.anchoredPosition.y + ItemDistance);

        LoopCircleItem lastloopCircle = AllLoopCircleItems[lastItem];
        LoopCircleItem firstloopCircle = AllLoopCircleItems[firstItem];
        lastloopCircle.InitialedView(firstloopCircle.DataIndex - 1);

        if (OnItemShowAct != null)
            OnItemShowAct.Invoke(lastItem, lastloopCircle);
    }

    #endregion



}
