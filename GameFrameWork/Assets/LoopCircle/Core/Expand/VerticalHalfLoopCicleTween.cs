using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特殊的  适用于垂直方向上固定步长的Tween (适用于数据源个数会变化的半永循环，数据可能是nul或者不足以显示一个页面 )
/// </summary>
public class VerticalHalfLoopCicleTween : VerticalLoopCircleTween
{
    #region UIReference
    protected Tweener MoveTweenner = null;
    #endregion


    protected Vector2 mLastRecordContentAnchoePos;



    #region 接口
    public override void RefillData(int dataCount, int offset = 0)
    {
        if (MoveTweenner != null && MoveTweenner.IsPlaying())
            MoveTweenner.Kill();
        base.RefillData(dataCount, offset);
        mLastRecordContentAnchoePos = ContentRectrans.anchoredPosition;
    }


    public override void RollView(LoopViewShowDirection direction, float endPosY, float tweenTime, bool isAbsDistance, System.Action completeRoll = null)
    {
        if (direction == LoopViewShowDirection.None || (int)direction > (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }

        if (IsScrolling) return;
        ContentViewShowDirection = direction;

        if (isAbsDistance == false)
            endPosY += ContentRectrans.anchoredPosition.y;

        if (CheckContentOffsetAvailable(direction, endPosY) == false)
            return;

              //Debug.Log("------------" + endPosY + "  " + direction);
        if (IsLoopCircleModel)
        {
            #region 可以循环滑动
            RollView(direction);
            if (ContentRectrans.anchoredPosition.y == endPosY)
            {
                UpdateItemState();
                OnCompleteScrollView();
                return;
            }
            OnBeginScrollView();
            if (completeRoll == null)
                MoveTweenner = ContentRectrans.DOAnchorPosY(endPosY, tweenTime).OnUpdate(OnUpdateTweenView).OnComplete(OnCompleteScrollView).SetEase(mTweenAnimationCurve);
            else
                MoveTweenner = ContentRectrans.DOAnchorPosY(endPosY, tweenTime).OnUpdate(OnUpdateTweenView).OnComplete(() =>
                {
                    OnCompleteScrollView();
                    completeRoll();
                }).SetEase(mTweenAnimationCurve);
            #endregion

            return;
        }

        #region 数据不足以循环时候只显示一页

        float maxOffset = CaculateMaxOffsetDistance(direction);
      //  Debug.Log("maxOffset=" + maxOffset);

        if (mLastRecordContentAnchoePos.y == ContentInitialedPos.y + maxOffset)
        {
            if (completeRoll != null)
                completeRoll.Invoke();
            return; //已经达到边界
        }
        float realEndPosY = 0;

        if (Mathf.Abs(endPosY) < Mathf.Abs(maxOffset))
            realEndPosY = endPosY;
        else
            realEndPosY = maxOffset;

        if (realEndPosY == ContentRectrans.anchoredPosition.y)
        {
            if (completeRoll != null) completeRoll.Invoke();
            return;
        }

        if (CheckContentOffsetAvailable(direction, realEndPosY) == false)
            return ;

        IsScrolling = true;
        if (completeRoll != null)
        {
            MoveTweenner = ContentRectrans.DOAnchorPosY(realEndPosY, tweenTime).OnUpdate(OnUpdateTweenView).OnComplete(() =>
            {
                OnCompleteScrollView();
                completeRoll();
            }).SetEase(mTweenAnimationCurve);
        }
        else
        {
            MoveTweenner = ContentRectrans.DOAnchorPosY(realEndPosY, tweenTime).OnUpdate(OnUpdateTweenView).OnComplete(OnCompleteScrollView).SetEase(mTweenAnimationCurve);
        }
        #endregion

    }

    protected void OnUpdateTweenView()
    {
        if (IsLoopCircleModel)
            UpdateItemState();
        if (OnItemMovingAct != null)
            OnItemMovingAct();
    }

    /// <summary>
    /// 当数据不足 不能无限循环时候的滑动 只能滑动到边界
    /// </summary>
    /// <returns></returns>
    protected float CaculateMaxOffsetDistance(LoopViewShowDirection direction)
    {
        if (DateCount <= 1)
            return 0;

        float absDistance = ItemDistance * (DateCount * 0.5f);
        if (direction == LoopViewShowDirection.Up)
            absDistance= - 1 * absDistance;

        if (CheckContentOffsetAvailable(direction, absDistance) == false)
            return 0;
        return absDistance;
    }


    #endregion

    #region 基类重写
    protected override void InitialedState()
    {
        base.InitialedState();
        mLastRecordContentAnchoePos = ContentRectrans.anchoredPosition;
    }

   

    protected override void OnCompleteScrollView()
    {
        base.OnCompleteScrollView();
        if (IsLoopCircleModel)
            UpdateItemState();
        Direction = LoopViewShowDirection.None;

    }


    protected override void UpdateItemState()
    {
        AutoSetItemState();
        mLastRecordContentAnchoePos = ContentRectrans.anchoredPosition;
    }



    #endregion

    

    #region 辅助接口

    /// <summary>
    /// 主要用于删除当前数据后 为了保持现有的视图获取需要显示的数据偏移
    /// </summary>
    /// <returns></returns>
    public int GetFristVisibleLoopItemDataIndex()
    {
        RectTransform target = GetFirstVisibleTrans();
        if (target == null) return 0;
        LoopCircleItem loopCircle = GetLoopCircleItemByTrans(target);
        if (loopCircle == null) return 0;
        return loopCircle.DataIndex;
    }
    #endregion

}
