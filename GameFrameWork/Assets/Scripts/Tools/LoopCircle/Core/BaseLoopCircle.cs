using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 循环列表滑动方向
/// </summary>
[System.Serializable]
public enum LoopCircleDirection
{
    None=-1, //默认
    Down=0,
    Up,
    Left=10,
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
    #endregion
    public int ShowRectItemCount = 3;  //显示区域的元素个数
    public bool IsVerticalLayot = true; //表示是否是垂直布局  否则水平布局

    #region Item Property
    [Space(10)]
    public Vector2 ItemSpace = new Vector2(0, 20);
    public Vector2 ItemSize = new Vector2(100, 100);
    #endregion

    #region Data 
    protected Dictionary<Transform, LoopCircleItem> AllLoopCircleItems = new Dictionary<Transform, LoopCircleItem>();
    protected bool IsShowItemMiddle = false;  //判断是否是奇数个显示
    protected int RealShowRectItemCount;  //比可视区域多显示1个Item

    protected int _MiddleItemIndex = -1;
    /// <summary>
    /// 正中间的Item 的索引
    /// </summary>
    protected int MiddleItemIndex
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


    #endregion

    #region Other
    public AnimationCurve mTweenAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField]
    protected LoopCircleDirection Direction = LoopCircleDirection.None; //滑动的 方向 与内容现实方向相反

    protected virtual float ItemDistance { get; }  //两个项之间的距离
    protected float HalfItemDistance { get { return 0.5f * ItemDistance; } }
    #endregion

    #region State 
    [SerializeField]
    protected Vector2 lastRecordPosition;//记录上一次启动时候Content的位置

    [Header("状态标示  标示是否在滑动视图 不要修改!!!!")]
    [SerializeField]
    protected bool IsScrolling = false;
    #endregion

    protected virtual void Awake()
    {
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

        ContentRectrans.pivot = Vector2.one / 2f;
        ContentRectrans.anchorMax = Vector2.one / 2f;
        ContentRectrans.anchorMin = Vector2.one / 2f;
    }

    protected virtual void Start()
    {
        IsShowItemMiddle = ShowRectItemCount % 2 == 0 ? false : true;
        RealShowRectItemCount = ShowRectItemCount + 1;
        CreatePerfectShowItems();
        lastRecordPosition = ContentRectrans.localPosition;
        InitialLayoutItems();
    }

 

    #region 虚接口
    /// <summary>
    /// 创建 RealShowRectItemCount 个显示的Item项 并保存到AllLoopCircleItems 中
    /// </summary>
    protected virtual void CreatePerfectShowItems()
    {
        for (int dex = 0; dex < RealShowRectItemCount; dex++)
        {
            GameObject go = GameObject.Instantiate(mItemPrefab, ContentRectrans);
            AllLoopCircleItems.Add(go.transform, go.GetAddComponent<LoopCircleItem>());
        }
    }

    /// <summary>
    /// 初始化视图
    /// </summary>
    protected virtual void InitialLayoutItems()
    {
        _MiddleItemIndex = -1; //重新计算
        ContentRectrans.localPosition = ContentInitialedPos;// 恢复原来的位置
        for (int dex = 0; dex < RealShowRectItemCount; dex++)  //**多出一个对象
        {
            bool isVizible = (dex == 0 || dex == RealShowRectItemCount - 1) ? false : true;
            Transform itemTrans = ContentRectrans.GetChild(dex);
            AllLoopCircleItems[itemTrans].InitialedView(dex, isVizible);
            itemTrans.transform.localPosition = GetItemLocalPosByIndex(dex);
        }
    }

    /// <summary>
    /// 滑动视图
    /// </summary>
    /// <param name="direction"></param>
    protected virtual void RollView(LoopCircleDirection direction)
    {
        Direction = direction;
    }

    #endregion

    #region 子类需要实现的接口


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
    protected abstract bool IsItemVisible(Transform item);

    #endregion

}
