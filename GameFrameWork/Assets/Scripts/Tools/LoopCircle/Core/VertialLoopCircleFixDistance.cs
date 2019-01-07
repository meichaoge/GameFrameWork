using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 适用于固定步长的Tween 动画视图
/// </summary>
public class VertialLoopCircleFixDistance : BaseVerticalLoopCircle
{

    public void RollView(LoopCircleDirection direction,float endPosY,float tweenTime)
    {
        if (IsScrolling) return;
        RollView(direction);
        if(ContentRectrans.transform.localPosition.y== endPosY)
        {
            UpdateItemState();
            OnCompleteScrollView();
            return;
        }
        OnBeginScrollView();
        ContentRectrans.DOLocalMoveY(endPosY, tweenTime).OnUpdate(UpdateItemState).OnComplete(OnCompleteScrollView).SetEase(mTweenAnimationCurve);
    }




}
