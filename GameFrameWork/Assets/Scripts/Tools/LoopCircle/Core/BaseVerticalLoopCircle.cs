using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 只在垂直方向上布局 且上下滑动 
/// </summary>
public class BaseVerticalLoopCircle : BaseLoopCircle
{
    protected float mItemDistance = 0;
    protected override float ItemDistance
    {
        get
        {
            if (mItemDistance == 0)
            {
                mItemDistance = ItemSpace.y + ItemSize.y;
            }
            return mItemDistance;
        }
    }


    #region 重写基类实现
    protected override void Awake()
    {
        base.Awake();
        if (IsVerticalLayot == false)
        {
            Debug.LogError("BaseVerticalLoopCircle Can Only Use For Vertical Layout");
        }
    }

    protected override void RollView(LoopCircleDirection direction)
    {
        if ((int)direction > 1) return;
        base.RollView(direction);

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
        if (IsVerticalLayot == false) return Vector3.zero;

        if (IsShowItemMiddle)
        {
            if (dex < MiddleItemIndex)
            {
                return new Vector3(0, (MiddleItemIndex - dex) * ItemDistance, 0);
            }
            else
            {
                return new Vector3(0, -(dex - MiddleItemIndex) * ItemDistance, 0);
            }
        }//显示区域奇数个
        else
        {
            if (dex < MiddleItemIndex)
            {
                return new Vector3(0, HalfItemDistance + (MiddleItemIndex - dex - 1) * (ItemSize.y + ItemSpace.y), 0);
            }
            else
            {
                return new Vector3(0, -HalfItemDistance - (dex - MiddleItemIndex) * (ItemSize.y + ItemSpace.y), 0);
            }
        }//偶数个
    }


    protected override bool IsItemVisible(Transform item)
    {
        if (IsVerticalLayot == false) return true;

        bool IsoutsideTop = (item.localPosition.y - ItemSize.y / 2f) >= ViewPortRectrans.sizeDelta.y / 2f + ViewPortRectrans.localPosition.y - lastRecordPosition.y;
        bool IsoutsideBottom = (item.localPosition.y + ItemSize.y / 2f) <= -1 * ViewPortRectrans.sizeDelta.y / 2f + ViewPortRectrans.localPosition.y - lastRecordPosition.y;
        //Debug.Log("lastRecordPosition=" + lastRecordPosition + "   ViewsizeDelta=" + ViewPortRectrans.sizeDelta + "ViewocalPosition=" + ViewPortRectrans.localPosition+ "  itemlocalPosition=" + item.localPosition+
        //    "   IsoutsideBottom="+ IsoutsideBottom+ "    IsoutsideTop="+ IsoutsideTop+ "   Direction="+ Direction);
        return !(IsoutsideTop || IsoutsideBottom);  //注意取反操作

    }

    #endregion

    #region 垂直布局滑动
    protected virtual bool OnBeginScrollView()
    {
        IsScrolling = true;
        if ((int)Direction < 0 || (int)Direction >= 10)
        {
     //       Debug.LogError("没有定义的类型 " + Direction);
            return false;
        }
        //Debug.Log("-----------------Begin------------------" );
        AutoSetItemState();
        return true;
    }
    protected int LoopTime = 0;
    /// <summary>
    /// 自动调整多出的那个项的位置
    /// </summary>
    protected virtual void AutoSetItemState()  
    {
        //  Debug.Log("AutoSetItemState");
        LoopTime = 0;
        if (Direction == LoopCircleDirection.Down)
        {
            //**检查最后一个项是否可见    如果不可见则移动到顶部
            Transform lastItem = ContentRectrans.GetChild(ContentRectrans.childCount - 1);
            while (IsItemVisible(lastItem) == false)
            {
                //Debug.Log("ccccc");
                ScrollDownDirection(lastItem);
                lastItem = ContentRectrans.GetChild(ContentRectrans.childCount - 1);
                ++LoopTime;
                if (LoopTime >= ContentRectrans.childCount)
                    break;  //防止为止原因导致死循环
            }//由于快读滑动时候可能一下子滑出超过多个Item项的距离
        }
        else
        {
            //***检测第一个项是否可见 如果不可见则移动到末尾
            Transform firstItem = ContentRectrans.GetChild(0);
            while (IsItemVisible(firstItem) == false)
            {
                ScrollUpDirection(firstItem);
                firstItem = ContentRectrans.GetChild(0);
                ++LoopTime;
                if (LoopTime >= ContentRectrans.childCount)
                    break;  //防止为止原因导致死循环
            }
        }
    }

    protected virtual void OnCompleteScrollView()
    {
        IsScrolling = false;
        lastRecordPosition = ContentRectrans.localPosition;
        //Debug.Log("-----------------Complete------------------");
    }

    /// <summary>
    /// 每次视图改变都需要调用
    /// </summary>
   protected override  void UpdateItemState()
    {
        if (Mathf.Abs(ContentRectrans.localPosition.y - lastRecordPosition.y) > (ItemSize.y + ItemSpace.y))
        {
            lastRecordPosition = ContentRectrans.localPosition;
            AutoSetItemState();
        }
    }







    /// <summary>
    /// 将最后一个不可见得项移动到开头 
    /// </summary>
    /// <param name="operateItem"></param>
    protected virtual void ScrollDownDirection(Transform lastItem)
    {
        if (lastItem == null)
            lastItem = ContentRectrans.GetChild(ContentRectrans.childCount - 1);
        LoopCircleItem first = AllLoopCircleItems[ContentRectrans.GetChild(0)];
        Vector2 firstPos = first.transform.localPosition;
        lastItem.SetAsFirstSibling();
        lastItem.localPosition = new Vector2(firstPos.x, firstPos.y + ItemSize.y + ItemSpace.y);
        AllLoopCircleItems[lastItem].InitialedView(first.Dex - 1, true);
    }

    /// <summary>
    /// 将第一个不可见的项移动到末尾
    /// </summary>
    /// <param name="firstItem"></param>
    protected virtual void ScrollUpDirection(Transform firstItem)
    {
        if (firstItem == null)
            firstItem = ContentRectrans.GetChild(0);
        LoopCircleItem last = AllLoopCircleItems[ContentRectrans.GetChild(ContentRectrans.childCount - 1)];
        Vector2 lastPos = last.transform.localPosition;
        firstItem.SetAsLastSibling();
        firstItem.localPosition = new Vector2(lastPos.x, lastPos.y - ItemSize.y - ItemSpace.y);
        AllLoopCircleItems[firstItem].InitialedView(last.Dex + 1, true);
    }

    #endregion



}
