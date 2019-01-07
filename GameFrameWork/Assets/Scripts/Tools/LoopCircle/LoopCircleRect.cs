using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
///环形循环列表
/// </summary>
public class LoopCircleRect : MonoBehaviour
{
    public RectMask2D ViewPort;
    public RectTransform ContentRectrans;
    public GameObject mItemPrefab;
    public int ShowRectItemCount = 3;  //显示区域的元素个数
    [Space(10)]
    public Vector2 ItemSpace = new Vector2(0, 20);
    public Vector3 TransParentInitialedPos;
    #region Data
    public Vector2 ItemSize = new Vector2(100, 100);
    private Dictionary<Transform, LoopCircleItem> AllLoopCircleItems = new Dictionary<Transform, LoopCircleItem>();
    private bool IsShowItemMiddle = false;  //判断是否是奇数个显示
    private int RealShowRectItemCount;  //比可视区域多显示1个Item

    private int _MiddleItemIndex = -1;
    /// <summary>
    /// 正中间的Item 的索引
    /// </summary>
    private int MiddleItemIndex
    {
        get
        {
            if (_MiddleItemIndex < 0)
            {
                if (IsShowItemMiddle)
                    _MiddleItemIndex = (RealShowRectItemCount + 1) / 2 - 1;  //正中间的Item 的索引
                else
                    _MiddleItemIndex = RealShowRectItemCount / 2;
            }
            return _MiddleItemIndex;
        }
    }

    private Vector2 lastRecordPos;

    private bool IsRollDown = true;  //表示是否显示后面的视图
    [Space(10)]
    public bool IsScrolling = false;
    public AnimationCurve mTweenAnimationCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));
    #endregion
    //****Test 
    [Space(20)]
    public int offset = 10;
    public float TweenTime = 10;
    //****Test 
    void Start()
    {

        
        TransParentInitialedPos = ContentRectrans.localPosition;
        IsShowItemMiddle = ShowRectItemCount % 2 == 0 ? false : true;

        RealShowRectItemCount = ShowRectItemCount + 1;

        for (int dex = 0; dex < RealShowRectItemCount; dex++)
        {
            GameObject go = GameObject.Instantiate(mItemPrefab, ContentRectrans);
            AllLoopCircleItems.Add(go.transform, go.GetAddComponent<LoopCircleItem>());
        }
        InitialedLayoutItems();
        lastRecordPos = ContentRectrans.localPosition;
    }

    private void Update()
    {
        if (IsScrolling) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            IsRollDown = true;
            RollView();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsRollDown = false;
            RollView();
        }
    }

    private void InitialedLayoutItems()
    {
        _MiddleItemIndex = -1; //重新计算
        ContentRectrans.localPosition = TransParentInitialedPos;// 恢复原来的位置
        for (int dex = 0; dex < RealShowRectItemCount; dex++)  //**多出一个对象
        {
            bool isVizible = (dex == 0 || dex == RealShowRectItemCount - 1) ? false : true;
            Transform itemTrans = ContentRectrans.GetChild(dex);
            AllLoopCircleItems[itemTrans].InitialedView(dex, isVizible);
            itemTrans.transform.localPosition = GetItemLocalPosByIndex(dex);
        }
    }


    public Vector3 GetItemLocalPosByIndex(int dex)
    {
        if (IsShowItemMiddle)
        {
            if (dex < MiddleItemIndex)
            {
                return new Vector3(0, (MiddleItemIndex - dex) * (ItemSize.y + ItemSpace.y), 0);
            }
            else
            {
                return new Vector3(0, -(dex - MiddleItemIndex) * (ItemSize.y + ItemSpace.y), 0);
            }
        }//显示区域奇数个
        else
        {
            if (dex < MiddleItemIndex)
            {
                return new Vector3(0, (ItemSize.y + ItemSpace.y) / 2f + (MiddleItemIndex - dex - 1) * (ItemSize.y + ItemSpace.y), 0);
            }
            else
            {
                return new Vector3(0, -(ItemSize.y + ItemSpace.y) / 2f - (dex - MiddleItemIndex) * (ItemSize.y + ItemSpace.y), 0);
            }
        }//偶数个
    }


    public void RollView()
    {
        if(IsRollDown)
        {
            if (offset > 0)
                offset = -1 * offset;
        }
        else
        {
            if (offset < 0)
                offset = -1 * offset;
        }
        if (IsScrolling) return;
        ContentRectrans.DOLocalMoveY(offset * (ItemSize.y + ItemSpace.y), TweenTime).OnUpdate(UpdateItems).OnStart(OnBeginMove).OnComplete(OnComplete).SetEase(mTweenAnimationCurve);
    }

    private void OnBeginMove()
    {
        IsScrolling = true;
        lastRecordPos = ContentRectrans.localPosition;
        Debug.Log("-----------------Begin------------------"+ IsRollDown);
        if (IsRollDown)
        {
            //**检查最后一个项是否被移动到顶部
            Transform lastItem = ContentRectrans.GetChild(ContentRectrans.childCount - 1);
            if (IsVisible(lastItem) == false)
                ScrollDown();
        }
        else
        {
            Transform lastItem = ContentRectrans.GetChild(0);
            if (IsVisible(lastItem) == false)
                ScrollUp();
        }
    }

    private void OnComplete()
    {
        Debug.Log("-----------------------Complete--------------------"+ IsRollDown);
        IsScrolling = false;
    }

    private void UpdateItems()
    {
        if (Mathf.Abs(ContentRectrans.localPosition.y - lastRecordPos.y) > (ItemSize.y + ItemSpace.y))
        {
            lastRecordPos = ContentRectrans.localPosition;
            if (IsRollDown)
                ScrollDown();
            else
                ScrollUp();
        }
    }


    private void ScrollDown()
    {
        Transform item = ContentRectrans.GetChild(ContentRectrans.childCount - 1);
        LoopCircleItem first = AllLoopCircleItems[ContentRectrans.GetChild(0)];
        Vector2 firstPos = first.transform.localPosition;
        item.SetAsFirstSibling();
        item.localPosition = new Vector2(firstPos.x, firstPos.y + ItemSize.y + ItemSpace.y);
        AllLoopCircleItems[item].InitialedView(first.Dex - 1, true);
    }

    private void ScrollUp()
    {
        Transform item = ContentRectrans.GetChild(0);
        LoopCircleItem last = AllLoopCircleItems[ContentRectrans.GetChild(ContentRectrans.childCount - 1)];
        Vector2 lastPos = last.transform.localPosition;
        item.SetAsLastSibling();
        item.localPosition = new Vector2(lastPos.x, lastPos.y - ItemSize.y - ItemSpace.y);
        AllLoopCircleItems[item].InitialedView(last.Dex +1, true);
    }

    /// <summary>
    /// 检测某一个节点是否可见
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool IsVisible(Transform item)
    {
        RectTransform rectrans = ViewPort.transform as RectTransform;
        bool IsoutsideTop = (item.localPosition.y - ItemSize.y / 2f) >= rectrans.sizeDelta.y/2f+ rectrans.localPosition.y- lastRecordPos.y;
        bool IsoutsideBottom = (item.localPosition.y + ItemSize.y / 2f) <=-1 * rectrans.sizeDelta.y/2f + rectrans.localPosition.y - lastRecordPos.y;

        return !(IsoutsideTop || IsoutsideBottom);  //注意取反操作

    }

}
