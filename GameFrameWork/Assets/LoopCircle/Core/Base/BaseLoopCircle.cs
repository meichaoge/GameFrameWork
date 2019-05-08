using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 循环列表中的内容显示的方向,(与滑动的方向相反)
/// </summary>
[System.Serializable]
public enum LoopViewShowDirection
{
    None = -1, //默认
    Down = 0,
    Up,
    Left = 10,
    Right
}

/// <summary>
/// 环形循环列表
/// </summary>
public abstract class BaseLoopCircle : MonoBehaviour
{

    #region UI Reference
    public RectTransform ViewPortRectrans;
    public RectTransform ContentRectrans;
    public Vector3 ContentInitialedPos = Vector3.zero;

    public GameObject mItemPrefab;
    protected Canvas mViewCanvas = null;
    #endregion

    public int ShowRectItemCount = 3;  //显示区域的元素个数
    protected bool IsVerticalLayot = true; //表示是否是垂直布局  否则水平布局

    #region Item Property
    [Space(20)]
    public Vector2 ItemSpace = new Vector2(20, 20);
    public Vector2 ItemSize = new Vector2(100, 100);

    #endregion

    #region Data 
    protected Dictionary<RectTransform, LoopCircleItem> AllLoopCircleItems = new Dictionary<RectTransform, LoopCircleItem>();
    public Dictionary<RectTransform, LoopCircleItem> mAllLoopCircleItems { get { return AllLoopCircleItems; } }


    /// <summary>
    /// 标示是否是循环模式 ，如果数据比预期的数据少则变成其他的模式
    /// </summary>
    protected bool IsLoopCircleModel { get { return DateCount > ShowRectItemCount; } }

    /// <summary>
    /// 是否是奇数个数据布局 根据不同的情况可以有不同的定义
    /// </summary>
    protected bool IsImparDataCount { get { if (IsLoopCircleModel) return ShowRectItemCount % 2 != 0; return DateCount % 2 != 0; } }

    /// <summary>
    /// /比可视区域多显示2个Item 前后各多一个  根据不同的情况可以有不同的定义
    /// </summary>
    protected int RealShowRectItemCount { get { if (IsLoopCircleModel) return ShowRectItemCount + 2; return DateCount; } }

    /// <summary>
    /// 正中间的Item 的索引  根据不同的情况可以有不同的定义
    /// </summary>
    protected int MiddleItemIndex { get { if (IsLoopCircleModel) return (RealShowRectItemCount + 1) / 2 - 1; return (DateCount + 1) / 2 - 1; } }

    public int DateCount { get; protected set; } //数据总数
    public int DateOffset { get; protected set; } //显示的数据偏移

    #endregion

    #region Other

    protected LoopViewShowDirection Direction = LoopViewShowDirection.None; //滑动的 方向 与内容现实方向相反

    /// <summary>
    /// 外界访问 用于获取之前一次操作的方向 不会再循环列表中用于具体的逻辑处理
    /// </summary>
    public LoopViewShowDirection ContentViewShowDirection = LoopViewShowDirection.None;

    public abstract float ItemDistance { get; }  //两个项之间的距离
    protected float HalfItemDistance { get { return 0.5f * ItemDistance; } }
    protected abstract float ViewRectHalfSize { get; } //ViewPortRectrans 长度/宽度的一半

    #endregion

    #region State 
    public bool IsInitialed { get; protected set; } //标示是否完成初始化
    public bool IsScrolling { get; protected set; }  //状态标示  标示是否在滑动视图 不要修改!!!!
    #endregion

    #region 事件
    public System.Action<RectTransform, LoopCircleItem> OnItemCreateAct = null;  //被创建时候调用
    public System.Action<RectTransform, LoopCircleItem> OnItemShowAct = null; //被显示时候调用
    public System.Action<RectTransform, LoopCircleItem> OnItemRemoveAct = null; //被销毁时候调用

    public System.Func<GameObject> OnItemPrefabisNull = null;  //用于外部设置关联的预制体资源

    public System.Action OnCompleteInitialedViewAct = null;  //每次重新初始化就会调用
    public System.Action OnItemMovingAct = null; //移动时候每一帧执行

    #endregion


    #region MONO
    protected virtual void Awake()
    {
        CheckViewSetting();
        IsScrolling = false;
    }
    protected virtual void Start()
    {

    }

    #endregion


    #region 对外的接口
    /// <summary>
    /// 初始化数据并显示视图的接口
    /// </summary>
    /// <param name="dataCount"></param>
    /// <param name="offset"></param>
    public virtual void RefillData(int dataCount, int offset = 0)
    {
        InitialedState();
        DateCount = dataCount;
        DateOffset = offset;
        IsInitialed = true;
        CreatePerfectShowItems();
        InitialLayoutItems();
        if (OnCompleteInitialedViewAct != null)
            OnCompleteInitialedViewAct();
    }
    #endregion


    #region 循环列表功能实现


    #region 父类的虚实现 子类可以自行重写

    /// <summary>
    /// 初始化状态信息
    /// </summary>
    protected virtual void InitialedState()
    {
        DateCount = 0;
        IsInitialed = false;
    }

    /// <summary>
    /// 创建 RealShowRectItemCount 个显示的Item项 并保存到AllLoopCircleItems 中
    /// </summary>
    protected virtual void CreatePerfectShowItems()
    {
        for (int dex = 0; dex < RealShowRectItemCount; dex++)
        {
            CrateGameObject(dex);
        }
        ///2019/4/22 新增 删除多余的项 
        if (ContentRectrans.childCount > RealShowRectItemCount)
        {
            for (int dex = ContentRectrans.childCount - 1; dex >= RealShowRectItemCount; dex--)
            {
                RectTransform target = ContentRectrans.GetChildEX(dex);
                LoopCircleItem circleItem = AllLoopCircleItems[target];
                AllLoopCircleItems.Remove(target);
                GameObject.Destroy(target.gameObject);
                if (OnItemRemoveAct != null)
                    OnItemRemoveAct(target, circleItem);
            }
        }

    }
    /// <summary>
    /// 根据已经存在的项创建其他的项 如果项太多会删除数据
    /// </summary>
    /// <param name="dex"></param>
    /// <returns></returns>
    protected virtual RectTransform CrateGameObject(int dex)
    {
        RectTransform target = null;
        bool isCreateItem = false;  //标示是否是创建的项or 复用的项
        if (dex < ContentRectrans.childCount)
        {
            target = ContentRectrans.GetChildEX(dex);
        }
        else
        {
            target = GameObject.Instantiate(mItemPrefab, ContentRectrans).transform as RectTransform;
            isCreateItem = true;
        }
        LoopCircleItem circleItem = target.GetAddComponent<LoopCircleItem>();
        circleItem.InitialedView(dex);
        AllLoopCircleItems[target] = circleItem;

        if (isCreateItem && OnItemCreateAct != null)
            OnItemCreateAct(target, circleItem);
        return target;
    }

    /// <summary>
    /// 初始化视图
    /// </summary>
    protected virtual void InitialLayoutItems()
    {
        ContentRectrans.anchoredPosition = ContentInitialedPos;// 恢复原来的位置
        for (int dex = 0; dex < RealShowRectItemCount; dex++)  //**多出两个对象
        {
            RectTransform itemTrans = ContentRectrans.GetChildEX(dex);
            itemTrans.anchoredPosition = GetItemLocalPosByIndex(dex);

            //  Debug.Log(itemTrans.name + "  " + itemTrans.anchoredPosition);

            if (OnItemShowAct != null)
                OnItemShowAct(itemTrans, AllLoopCircleItems[itemTrans]);
        }
    }

    #endregion

    #region 子类需要实现的接口

    /// <summary>
    /// 滑动视图
    /// </summary>
    /// <param name="direction"></param>
    protected abstract void RollView(LoopViewShowDirection direction);

    /// <summary>
    /// 根据索引ID 获取默认的局部坐标
    /// </summary>
    /// <param name="dex"></param>
    /// <returns></returns>
    protected abstract Vector3 GetItemLocalPosByIndex(int dex);

    /// <summary>
    ///  每次视图改变都需要调用
    /// </summary>
    protected abstract void UpdateItemState();

    /// <summary>
    /// 检测某一个节点是否可见
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract bool IsItemVisible(RectTransform item);

    protected virtual void OnCompleteScrollView()
    {
        IsScrolling = false;
    }

    #endregion

    #endregion



    #region 辅助接口
    /// <summary>
    /// 检测视图配置是否正确
    /// </summary>
    protected void CheckViewSetting()
    {
        if (mViewCanvas == null)
        {
            mViewCanvas = transform.GetComponentInParent<Canvas>();
        }

        if (ViewPortRectrans == null)
        {
            Debug.LogError("ViewPortRectrans Can not Be Null");
            return;
        }

        if (ViewPortRectrans == null)
        {
            Debug.LogError("ViewPortRectrans Can not Be Null");
            return;
        }
        if (mItemPrefab == null)
        {
            if (OnItemPrefabisNull != null)
                mItemPrefab = OnItemPrefabisNull();

            if (mItemPrefab == null)
                Debug.LogError("CrateGameObject Fail");
        }

        if (ContentRectrans != null)
        {
            ContentInitialedPos = ContentRectrans.anchoredPosition;
        }

#if UNITY_EDITOR
        //****为了检测到正确的边界
        if (IsVerticalLayot)
        {
            if (ViewPortRectrans.rect.height < ShowRectItemCount * ItemDistance)
                Debug.LogError(string.Format("垂直方向上{0} 的{1} 高度不是与显示元素的视口大小不匹配，请至少设置为{2}", gameObject.name, ViewPortRectrans.name, ShowRectItemCount * ItemDistance));
        }
        else
        {
            if (ViewPortRectrans.rect.width < ShowRectItemCount * ItemDistance)
                Debug.LogError(string.Format("水平方向上{0} 的{1} 宽度不是与显示元素的视口大小不匹配，请至少设置为{2}", gameObject.name, ViewPortRectrans.name, ShowRectItemCount * ItemDistance));
        }
#endif

    }


    /// <summary>
    /// 获取ContentRectrans 下最后面一个可见项 
    /// </summary>
    /// <returns></returns>
    public virtual RectTransform GetLastVisibleTrans()
    {
        for (int dex = ContentRectrans.childCount - 1; dex >= 0; --dex)
        {
            var item = ContentRectrans.GetChildEX(dex);
            if (item.gameObject.activeSelf)
                return item;
        }
        return null;
    }
    /// <summary>
    /// 获取ContentRectrans 下第一个可见项 
    /// </summary>
    /// <returns></returns>
    public virtual RectTransform GetFirstVisibleTrans()
    {
        for (int dex = 0; dex < ContentRectrans.childCount; ++dex)
        {
            var item = ContentRectrans.GetChildEX(dex);
            if (item.gameObject.activeSelf)
                return item;
        }
        return null;
    }

    public LoopCircleItem GetLoopCircleItemByTrans(RectTransform target)
    {
        LoopCircleItem result = null;
        if (AllLoopCircleItems.TryGetValue(target, out result))
        {
            return result;
        }

        Debug.LogError("GetLoopCircleItemByTrans Fail");
        return null;
    }

    /// <summary>
    /// 检测偏移的参数是否合法 2019/4/24 新增
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    protected bool CheckContentOffsetAvailable(LoopViewShowDirection direction, float endPos)
    {
        float offset = 0;
        switch (direction)
        {
            case LoopViewShowDirection.Down:
                offset = endPos - ContentRectrans.anchoredPosition.y;
                if (offset < 0)
                {
                    Debug.LogError("显示下面的内容的偏移要大于0");
                    return false;
                }
                break;
            case LoopViewShowDirection.Up:
                offset = endPos - ContentRectrans.anchoredPosition.y;
                if (offset > 0)
                {
                    Debug.LogError("显示上面的内容的偏移要小于0");
                    return false;
                }
                break;
            case LoopViewShowDirection.Left:
                offset = endPos - ContentRectrans.anchoredPosition.x;
                if (offset < 0)
                {
                    Debug.LogError("显示左边的内容的偏移要大于0");
                    return false;
                }
                break;
            case LoopViewShowDirection.Right:
                offset = endPos - ContentRectrans.anchoredPosition.x;
                if (offset > 0)
                {
                    Debug.LogError("显示右边的内容的偏移要小于0");
                    return false;
                }
                break;
            default:
                Debug.LogError("没有定义视图显示方向" + direction);
                return false;
        }
        return true;
    }

    /// <summary>
    /// 根据视图显示的方向修正数据  (修正数据 2019/4/23 新增 )
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="offsetItem"></param>
    /// <returns></returns>
    protected int GetContentOffset(LoopViewShowDirection direction, int offsetItem)
    {
        int realOffset = offsetItem;
        switch (direction)
        {
            case LoopViewShowDirection.Down:
                if (realOffset < 0)
                {
                    Debug.LogError("显示下面的视图 偏移应该为大于0");
                    realOffset *= -1;
                }
                break;
            case LoopViewShowDirection.Up:
                if (realOffset > 0)
                {
                    Debug.LogError("显示上面的视图 偏移应该为小于0");
                    realOffset *= -1;
                }
                break;
            case LoopViewShowDirection.Left:
                if (realOffset < 0)
                {
                    Debug.LogError("显示左边的视图 偏移应该为大于0");
                    realOffset *= -1;
                }
                break;
            case LoopViewShowDirection.Right:
                if (realOffset > 0)
                {
                    Debug.LogError("显示右边的视图 偏移应该为小于0");
                    realOffset *= -1;
                }
                break;
            default:
                Debug.LogError("没有定义的类型 " + direction);
                break;
        }

        return realOffset;
    }

    #endregion



}
