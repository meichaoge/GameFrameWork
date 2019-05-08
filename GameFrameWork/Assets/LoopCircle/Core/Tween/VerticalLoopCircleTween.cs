using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  适用于固定步长的Tween 动画视图
/// </summary>
public class VerticalLoopCircleTween : BaseVerticalLoopCircle
{
    public AnimationCurve mTweenAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    /// <summary>
    /// 当前所有可见的项 使用前确保调用了GetAllVisibleLoopItems
    /// </summary>
    private Dictionary<int, LoopCircleItem> mAllVisibleLoopItems = new Dictionary<int, LoopCircleItem>();




    /// <summary>
    /// 滑动指定固定间隔 指定偏移的距离
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="offsetItem">移动多少个ItemDistance</param>
    /// <param name="tweenTime"></param>
    /// <param name="isAbsDistance">标示是否是绝对位移</param>
    public void RollViewEx(LoopViewShowDirection direction, int offsetItem, float tweenTime, bool isAbsDistance, System.Action completeRoll = null)
    {
        if (direction == LoopViewShowDirection.None || (int)direction >= (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }

        offsetItem = GetContentOffset(direction, offsetItem);

        RollView(direction, offsetItem * ItemDistance, tweenTime, isAbsDistance, completeRoll);
    }
    public virtual void RollView(LoopViewShowDirection direction, float endPosY, float tweenTime, bool isAbsDistance = false, System.Action completeRoll = null)
    {
        if (direction == LoopViewShowDirection.None || (int)direction >= (int)LoopViewShowDirection.Left)
        {
            Debug.LogError("RollViewEx  Fail,direction= " + direction);
            return;
        }

        if (IsScrolling) return;
        if (isAbsDistance == false)
            endPosY += ContentRectrans.anchoredPosition.y;


        if (CheckContentOffsetAvailable(direction, endPosY) == false)
            return;  //2019/4/24新增 用于判断数值是否合理


        #region 检测数值  避免由于滑动的太远太快导致视图显示异常 2019/4/23 新增 
        float offset = endPosY - ContentRectrans.anchoredPosition.y;
        if (Mathf.Abs(offset) / tweenTime * Time.deltaTime >= ViewRectHalfSize)
        {
            Debug.LogError("滑动的距离太远且时间太短 导致视图异常 "+ offset);
            return;
        }
        #endregion

        base.RollView(direction);
        if (ContentRectrans.anchoredPosition.y == endPosY)
        {
            UpdateItemState();
            OnCompleteScrollView();
            if (completeRoll != null) completeRoll.Invoke();
            return;
        }
        OnBeginScrollView();
        if (completeRoll != null)
        {
            ContentRectrans.DOAnchorPosY(endPosY, tweenTime).OnUpdate(UpdateItemState).SetEase(mTweenAnimationCurve).SetUpdate(UpdateType.Late).OnComplete(() =>
            {
                OnCompleteScrollView();
                completeRoll();
            });
        }
        else
        {
            ContentRectrans.DOAnchorPosY(endPosY, tweenTime).OnUpdate(UpdateItemState).OnComplete(OnCompleteScrollView).SetEase(mTweenAnimationCurve).SetUpdate(UpdateType.Late);
        }
    }


    #region 基类实现

    /// <summary>
    /// 为了避免一开始就移动了首尾的项这里改了判断条件 2019/4/23 新增 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected override bool IsItemVisible(RectTransform item)
    {
        if (IsVerticalLayot == false)
            return true;

        return ViewPortRectrans.IsInsideRect_Vertical(item.position);
    }

    /// <summary>
    /// 每次视图改变都需要调用
    /// </summary>
    protected override void UpdateItemState()
    {
        AutoSetItemState();
    }

    #endregion


    /// <summary>
    /// 获取所有可见的Item项
    /// </summary>
    public void GetAllVisibleLoopItems()
    {
        mAllVisibleLoopItems.Clear();
        List<LoopCircleItem> temp = new List<LoopCircleItem>();
        foreach (var item in AllLoopCircleItems)
        {
            if (IsItemVisible(item.Key))
                temp.Add(item.Value);
        }

        temp.Sort((a, b) => { return a.rectTransform.anchoredPosition.y.CompareTo(b.rectTransform.anchoredPosition.y); }); //根据Y坐标转换
        temp.Reverse(); //根据Y坐标按照从大到小排序
        for (int dex = 0; dex < temp.Count; dex++)
        {
            // Debug.Log(string.Format("Name={0}   localPos={1}", temp[dex].name, temp[dex].anchoredPosition));
            mAllVisibleLoopItems.Add(dex, temp[dex]);
        }
    }
    /// <summary>
    /// 根据索引获取可见的项
    /// </summary>
    /// <param name="dex"></param>
    /// <returns></returns>
    public LoopCircleItem GetVisibleLoopItemByIndex(int dex)
    {
        LoopCircleItem result = null;
        if (mAllVisibleLoopItems.TryGetValue(dex, out result))
            return result;

        Debug.LogError(string.Format("获取可见项失败 没有这个索引:{0} 对应的项 ", dex));
        return null;
    }


}
