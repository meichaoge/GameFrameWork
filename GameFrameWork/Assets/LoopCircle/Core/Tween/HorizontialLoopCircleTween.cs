using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 适用于水平方向上固定步长的Tween 
/// </summary>
public class HorizontialLoopCircleTween : BaseHorizontialLoopCircle
{
    public AnimationCurve mTweenAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    /// <summary>
    /// 滑动指定固定间隔 指定偏移的距离
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="offsetItem">移动多少个ItemDistance</param>
    /// <param name="tweenTime"></param>
    /// <param name="isAbsDistance"></param>
    public void RollViewEx(LoopViewShowDirection direction, int offsetItem, float tweenTime, bool isAbsDistance, System.Action completeRoll = null)
    {
        if (direction == LoopViewShowDirection.None || (int)direction < (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }

        offsetItem = GetContentOffset(direction, offsetItem);
        RollView(direction, offsetItem * ItemDistance, tweenTime, isAbsDistance, completeRoll);
    }

    public virtual void RollView(LoopViewShowDirection direction, float endPosX, float tweenTime, bool isAbsDistance = false, System.Action completeRoll = null)
    {
        if (direction == LoopViewShowDirection.None || (int)direction < (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }

        if (IsScrolling) return;
        if (isAbsDistance == false)
            endPosX += ContentRectrans.anchoredPosition.x;

        #region 检测数值  避免由于滑动的太远太快导致视图显示异常 2019/4/23 新增 
        float offset = endPosX - ContentRectrans.anchoredPosition.x;
        if (Mathf.Abs(offset) / tweenTime * Time.deltaTime >= ViewRectHalfSize)
        {
            Debug.LogError("滑动的距离太远且时间太短 导致视图异常 "+ offset);
            return;
        }
        #endregion


        if (CheckContentOffsetAvailable(direction, endPosX) == false)
            return;

        RollView(direction);
        if (ContentRectrans.anchoredPosition.x == endPosX)
        {
            UpdateItemState();
            OnCompleteScrollView();
            return;
        }
        OnBeginScrollView();
        if (completeRoll != null)
        {
            ContentRectrans.DOAnchorPosX(endPosX, tweenTime).OnUpdate(UpdateItemState).OnComplete(() =>
            {
                OnCompleteScrollView();
                if (completeRoll != null) completeRoll.Invoke();
            }).SetEase(mTweenAnimationCurve);
        }
        else
        {
            ContentRectrans.DOAnchorPosX(endPosX, tweenTime).OnUpdate(UpdateItemState).OnComplete(OnCompleteScrollView).SetEase(mTweenAnimationCurve);
        }
    }

    /// <summary>
    /// 为了避免一开始就移动了首尾的项这里改了判断条件 2019/4/23 新增 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected override bool IsItemVisible(RectTransform item)
    {
        if (IsVerticalLayot) return true;

        return ViewPortRectrans.IsInsideRect_Horizontial(item.position);
    }


}
