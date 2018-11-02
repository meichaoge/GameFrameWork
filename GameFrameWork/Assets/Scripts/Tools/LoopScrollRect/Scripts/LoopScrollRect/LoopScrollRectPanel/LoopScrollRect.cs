using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using GameFrameWork;

namespace UnityEngine.UI
{
    /// <summary>
    /// 特别说明  如果滑动时候导致列表项滑出显示区域 可以加上一个ScrollBar
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class LoopScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
    {
        public enum MovementType
        {
            Unrestricted, // Unrestricted movement -- can scroll forever
            Elastic, // Restricted but flexible -- can go past the edges, but springs back in place
            Clamped, // Restricted movement where it's not possible to go past the edges
        }

        public enum ScrollbarVisibility
        {
            Permanent,
            AutoHide,
            AutoHideAndExpandViewport,
        }

        [Tooltip("Prefab Source")]
        public GameObject m_PrefabSource;
        public int m_PoolSize = 5;
        private bool m_IsInitialed = false;  //标识是否已经初始化

     
        [Tooltip("Threshold for preloading")]
        public float threshold = 100;
        [Tooltip("Reverse direction for dragging")]
        public bool reverseDirection = false;
        [Tooltip("Rubber scale for outside")]
        public float rubberScale = 1;


        #region  **********访问获取循环列表的状态*************

        /// <summary>
        /// 当前设置的总的数据个数
        /// </summary>
        public int TotalDataCount { get; protected set; }
        /// <summary>
        /// 当前显示区域项的开始索引 (可以用于滑动到指定的位置)
        /// </summary>
        public int ShowItemStartIndex { get; protected set; }
        /// <summary>
        /// 当前显示区域项的结束索引
        /// </summary>
        public int ShowItemEndIndex { get; protected set; }
        /// <summary>
        /// 标识是否可以拖拽和滑动 
        /// </summary>
        public bool IsCanDragAndScroll = true;

        #endregion

        #region 编辑器下测试代码、显示一些字段的属性

        #if UNITY_EDITOR
        //****测试发现 ShowItemStartIndex 和 ShowItemEndIndex 就是代表当前显示区域项的开始索引和结束缩影，不过
        //需要注意的是 ShowItemEndIndex 比实际的多一个
        [CustomerHeader("编辑器下显示循环列表上最上 /左面元素索引,对应属性 ShowItemStartIndex", "#FF0000FF")]
        [SerializeField]
        private int ShowItemStartIndex_EditorShow;
        [CustomerHeader("编辑器下显示循环列表上最下/右面元素索引,对应属性 ShowItemEndIndex", "#FF0000FF")]
        [SerializeField]
        private int ShowItemEndIndex_EditorShow;

        /// <summary>
        /// 编辑器下显示实时的数据
        /// </summary>
        private void Update()
        {
            ShowItemStartIndex_EditorShow = ShowItemStartIndex;
            ShowItemEndIndex_EditorShow = ShowItemEndIndex;
        }
#endif

        #endregion


        // 获取大小
        protected abstract float GetSize(RectTransform item);
        //获取  尺寸
        protected abstract float GetDimension(Vector2 vector);
        protected abstract Vector2 GetVector(float value);

        /// <summary>
        /// 标识滑动方向的 =1标识水平方向 ,=-1 标识垂直方向 (在子类中赋值)
        /// </summary>
        protected int directionSign = 0;

        private float m_ContentSpacing = -1;
        protected GridLayoutGroup m_GridLayout = null;
        protected float contentSpacing
        {
            get
            {
                if (m_ContentSpacing >= 0)
                {
                    return m_ContentSpacing;
                }
                m_ContentSpacing = 0;
                if (content != null)
                {
                    HorizontalOrVerticalLayoutGroup layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                    if (layout1 != null)
                    {
                        m_ContentSpacing = layout1.spacing;
                    }
                    m_GridLayout = content.GetComponent<GridLayoutGroup>();
                    if (m_GridLayout != null)
                    {
                        m_ContentSpacing = GetDimension(m_GridLayout.spacing);
                    }
                }
                return m_ContentSpacing;
            }
        } //间距

        private int m_ContentConstraintCount = 0;
        /// <summary>
        /// Content 上的行数或者列数限制 (垂直布局时候限制列数，水平布局限制行数)
        /// </summary>
        protected int contentConstraintCount
        {
            get
            {
                if (m_ContentConstraintCount > 0)
                {
                    return m_ContentConstraintCount;
                }
                m_ContentConstraintCount = 1;
                if (content != null)
                {
                    GridLayoutGroup layout2 = content.GetComponent<GridLayoutGroup>();
                    if (layout2 != null)
                    {
                        if (layout2.constraint == GridLayoutGroup.Constraint.Flexible)
                        {
                            Debug.LogWarning("[LoopScrollRect] Flexible not supported yet");
                        }
                        m_ContentConstraintCount = layout2.constraintCount;
                    }
                }
                return m_ContentConstraintCount;
            }
        }


        #region  状态 
        private bool hScrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                    return m_ContentBounds.size.x > m_ViewBounds.size.x + 0.01f;
                return true;
            }
        }
        private bool vScrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                    return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
                return true;
            }
        }

        public float horizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                //==========LoopScrollRect==========
                if (TotalDataCount > 0 && ShowItemEndIndex > ShowItemStartIndex)
                {
                    //TODO: consider contentSpacing
                    float elementSize = m_ContentBounds.size.x / (ShowItemEndIndex - ShowItemStartIndex);
                    float totalSize = elementSize * TotalDataCount;
                    float offset = m_ContentBounds.min.x - elementSize * ShowItemStartIndex;

                    if (totalSize <= m_ViewBounds.size.x)
                        return (m_ViewBounds.min.x > offset) ? 1 : 0;
                    return (m_ViewBounds.min.x - offset) / (totalSize - m_ViewBounds.size.x);
                }
                else
                    return 0.5f;
                //==========LoopScrollRect==========
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }
        public float verticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                //==========LoopScrollRect==========
                if (TotalDataCount > 0 && ShowItemEndIndex > ShowItemStartIndex)
                {
                    //TODO: consider contentSpacinge
                    float elementSize = m_ContentBounds.size.y / (ShowItemEndIndex - ShowItemStartIndex);
                    float totalSize = elementSize * TotalDataCount;
                    float offset = m_ContentBounds.max.y + elementSize * ShowItemStartIndex;

                    if (totalSize <= m_ViewBounds.size.y)
                        return (offset > m_ViewBounds.max.y) ? 1 : 0;
                    return (offset - m_ViewBounds.max.y) / (totalSize - m_ViewBounds.size.y);
                }
                else
                    return 0.5f;
                //==========LoopScrollRect==========
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }
        }

        #endregion

        [Serializable]
        public class ScrollRectEvent : UnityEvent<Vector2> { }

        #region  滑动 参数数据 

        [SerializeField]
        private RectTransform m_Content;
        public RectTransform content { get { return m_Content; } set { m_Content = value; } }

        [SerializeField]
        private bool m_Horizontal = true;
        public bool horizontal { get { return m_Horizontal; } set { m_Horizontal = value; } }

        [SerializeField]
        private bool m_Vertical = true;
        public bool vertical { get { return m_Vertical; } set { m_Vertical = value; } }

        [SerializeField]
        private MovementType m_MovementType = MovementType.Elastic;
        public MovementType movementType { get { return m_MovementType; } set { m_MovementType = value; } }

        [SerializeField]
        private float m_Elasticity = 0.1f; // Only used for MovementType.Elastic
        public float elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }

        [SerializeField]
        [Tooltip("惯性,=false 时候没有惯性")]
        private bool m_Inertia = true;
        public bool inertia { get { return m_Inertia; } set { m_Inertia = value; } }

        [SerializeField]
        private float m_DecelerationRate = 0.135f; // Only used when inertia is enabled
        public float decelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }

        [SerializeField]
        private float m_ScrollSensitivity = 1.0f;
        public float scrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

        #endregion

        #region  UI 组件引用 ScrollBar

        [SerializeField]
        private RectTransform m_Viewport;
        public RectTransform viewport { get { return m_Viewport; } set { m_Viewport = value; SetDirtyCaching(); } }

        [SerializeField]
        private Scrollbar m_HorizontalScrollbar;
        public Scrollbar horizontalScrollbar
        {
            get
            {
                return m_HorizontalScrollbar;
            }
            set
            {
                if (m_HorizontalScrollbar)
                    m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
                m_HorizontalScrollbar = value;
                if (m_HorizontalScrollbar)
                    m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private Scrollbar m_VerticalScrollbar;
        public Scrollbar verticalScrollbar
        {
            get
            {
                return m_VerticalScrollbar;
            }
            set
            {
                if (m_VerticalScrollbar)
                    m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
                m_VerticalScrollbar = value;
                if (m_VerticalScrollbar)
                    m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private ScrollbarVisibility m_HorizontalScrollbarVisibility;
        public ScrollbarVisibility horizontalScrollbarVisibility { get { return m_HorizontalScrollbarVisibility; } set { m_HorizontalScrollbarVisibility = value; SetDirtyCaching(); } }

        [SerializeField]
        private ScrollbarVisibility m_VerticalScrollbarVisibility;
        public ScrollbarVisibility verticalScrollbarVisibility { get { return m_VerticalScrollbarVisibility; } set { m_VerticalScrollbarVisibility = value; SetDirtyCaching(); } }

        [SerializeField]
        private float m_HorizontalScrollbarSpacing;
        public float horizontalScrollbarSpacing { get { return m_HorizontalScrollbarSpacing; } set { m_HorizontalScrollbarSpacing = value; SetDirty(); } }

        [SerializeField]
        private float m_VerticalScrollbarSpacing;
        public float verticalScrollbarSpacing { get { return m_VerticalScrollbarSpacing; } set { m_VerticalScrollbarSpacing = value; SetDirty(); } }

        #endregion

        #region 2017/12/20 新增
        protected Vector2 offset = Vector2.zero;   //缓存需要的偏移量
        protected bool m_BeScrolledOrDraged = false;
        //[SerializeField]
        protected bool m_NeedFrusheView = false; //标识是否需要刷新 (避免不必要的刷新)

        public Action<RectTransform, int> m_OnItemRemoveEvent;  //列表象被移除
        public Action<RectTransform, int> m_OnItemCreateEvent; //列表象被创建，显示
        public System.Action<int> m_OnItemViewFlushEvent;  //刷新时候 除非主动调用RefreshCells() ，否则不会被出发
        /// <summary>
        /// 记录当前创建时候的所有Item项 销毁(放回对象池)时候会删除对象
        /// </summary>
        protected Dictionary<Transform, LoopScrollRectItemBase> m_AllShowItemsRecord = new Dictionary<Transform, LoopScrollRectItemBase>();

        [Tooltip("水平布局时候标识向左边的箭头，垂直布局时候标识向上的箭头")]
        [SerializeField]
        private GameObject m_UpArrowObj = null;  //向上的箭头
        [Tooltip("水平布局时候标识向右边的箭头，垂直布局时候标识向下的箭头")]
        [SerializeField]
        private GameObject m_DownArrowObj = null;//向下的箭头


        #endregion

        #region  LoopScrollRect 事件
        [SerializeField]
        private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();
        public ScrollRectEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }
        #endregion

        #region  数值 
        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        private Vector2 m_ContentStartPosition = Vector2.zero;

        private RectTransform m_ViewRect;

        protected RectTransform viewRect
        {
            get
            {
                if (m_ViewRect == null)
                    m_ViewRect = m_Viewport;
                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform)transform;
                return m_ViewRect;
            }
        }

        private Bounds m_ContentBounds;
        private Bounds m_ViewBounds;

        private Vector2 m_Velocity;
        public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        private bool m_Dragging;

        private Vector2 m_PrevPosition = Vector2.zero;
        private Bounds m_PrevContentBounds;
        private Bounds m_PrevViewBounds;
        //[NonSerialized]
        //private bool m_HasRebuiltLayout = false;  //2018/07/28注释掉 貌似没有必要一直重构视图 (EnsureLayoutHasRebuilt 也被注释掉了)

        private bool m_HSliderExpand;
        private bool m_VSliderExpand;
        private float m_HSliderHeight;
        private float m_VSliderWidth;

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private RectTransform m_HorizontalScrollbarRect;
        private RectTransform m_VerticalScrollbarRect;
        #endregion

        private DrivenRectTransformTracker m_Tracker;

 
        #region  接口参数
        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth { get { return -1; } }
        public virtual float preferredWidth { get { return -1; } }
        public virtual float flexibleWidth { get; private set; }

        public virtual float minHeight { get { return -1; } }
        public virtual float preferredHeight { get { return -1; } }
        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return -1; } }
        #endregion

        protected LoopScrollRect()
        {
            flexibleWidth = -1;
        }


        #region Unity Frame 

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirtyCaching();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateArrowState();
#if UNITY_EDITOR
            if (m_PrefabSource == null)
                Debug.LogError("Not Set Item Prefab to Show " + gameObject.name + "(需要在LoopScrollRect 上设置要显示的预制体)");

            HorizontalOrVerticalLayoutGroup layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layout1 != null)
            {
                LayoutElement layoutelement = m_PrefabSource.GetComponent<LayoutElement>();
                if (layoutelement = null)
                {
                    Debug.LogError("非网格布局(content 上有GriadLayout)  必须添加LayoutElement 组件 并设置合理的值" + m_PrefabSource.gameObject.name);
                }
            }
#endif


            if (m_HorizontalScrollbar)
                m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if (m_HorizontalScrollbar)
                m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);

            //   m_HasRebuiltLayout = false;
            m_Tracker.Clear();
            m_Velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        public override bool IsActive()
        {
            return base.IsActive() && m_Content != null;
        }

        protected virtual void LateUpdate()
        {

            if (m_NeedFrusheView == false) return;

#if UNITY_EDITOR
            //***8编辑器下显示状态
            ShowItemStartIndex_EditorShow = ShowItemStartIndex;
            ShowItemEndIndex_EditorShow = ShowItemEndIndex;
#endif


            if (!m_Content)
                return;

            //  EnsureLayoutHasRebuilt();
            UpdateScrollbarVisibility();
            UpdateBounds();

            DampingMoving();
            if (m_Dragging && m_Inertia)
            {
                Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / Time.unscaledDeltaTime;
                m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, Time.unscaledDeltaTime * 10);
            }

            if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
            {
                UpdateScrollbars(offset);
                m_OnValueChanged.Invoke(normalizedPosition);
                UpdatePrevData();
            }
        }

        /// <summary>
        /// 阻尼运动
        /// </summary>
        protected virtual void DampingMoving()
        {
            if (m_Dragging) return;
            Vector2 offset = CalculateOffset(Vector2.zero);
            if ((offset == Vector2.zero && m_Velocity == Vector2.zero))
            {
                m_NeedFrusheView = false;
                return;
            }

            Vector2 position = m_Content.anchoredPosition;
            for (int axis = 0; axis < 2; axis++)
            {
                // Apply spring physics if movement is elastic and content has an offset from the view.
                if (m_MovementType == MovementType.Elastic && offset[axis] != 0)
                {
                    float speed = m_Velocity[axis];
                    position[axis] = Mathf.SmoothDamp(m_Content.anchoredPosition[axis], m_Content.anchoredPosition[axis] + offset[axis], ref speed, m_Elasticity, Mathf.Infinity, Time.unscaledDeltaTime);
                    m_Velocity[axis] = speed;
                }
                // Else move content according to velocity with deceleration applied.
                else if (m_Inertia)
                {
                    m_Velocity[axis] *= Mathf.Pow(m_DecelerationRate, Time.unscaledDeltaTime);
                    if (Mathf.Abs(m_Velocity[axis]) < 1)
                        m_Velocity[axis] = 0;
                    position[axis] += m_Velocity[axis] * Time.unscaledDeltaTime;
                }
                // If we have neither elaticity or friction, there shouldn't be any velocity.
                else
                {
                    m_Velocity[axis] = 0;
                }
            }

            if (m_Velocity != Vector2.zero)
            {
                if (m_MovementType == MovementType.Clamped)
                {
                    offset = CalculateOffset(position - m_Content.anchoredPosition);
                    position += offset;
                }

                SetContentAnchoredPosition(position);
            }
        }

        #endregion


        #region  初始化/填充/刷新/清理数据 接口

        private void DeleteScrollRectItem(Transform go, int index)
        {
            m_AllShowItemsRecord.Remove(go);
            if (m_OnItemRemoveEvent != null)
                m_OnItemRemoveEvent(go as RectTransform, index);
            UIObjectPoolManager.Instance.ReturnObjectToPool(go.gameObject);
        }

        public void ClearCells()
        {
            if (Application.isPlaying)
            {
                ShowItemStartIndex = 0;
                ShowItemEndIndex = 0;
                TotalDataCount = 0;
                //             objectsToFill = null;
                for (int i = content.childCount - 1; i >= 0; i--)
                {
                    //   LoopScrollRectItemBase _scrollRectItem = content.GetChild(i).GetComponent<LoopScrollRectItemBase>();
                    LoopScrollRectItemBase _scrollRectItem = m_AllShowItemsRecord[content.GetChild(i)];
                    DeleteScrollRectItem(content.GetChild(i), _scrollRectItem.m_DataIndex);
                    //    DeleteScrollRectItem(content.GetChild(i), i);
                }
            }
        }

        public void RefreshCells()
        {
            if (Application.isPlaying && this.isActiveAndEnabled)
            {
                ShowItemEndIndex = ShowItemStartIndex;
                // recycle items if we can
                for (int i = 0; i < content.childCount; i++)
                {
                    LoopScrollRectItemBase _scrollRectItem = m_AllShowItemsRecord[content.GetChild(i)];
                    if (ShowItemEndIndex < TotalDataCount)
                    {
                        _scrollRectItem.InitialedScrollCellItem(ShowItemEndIndex);
                        //     content.GetChild(i).GetComponent<LoopScrollRectItemBase>().InitialedScrollCellItem(ShowItemEndIndex);
                        if (m_OnItemViewFlushEvent != null)
                            m_OnItemViewFlushEvent(ShowItemEndIndex);  //刷新数据视图
                        //   dataSource.ProvideData(content.GetChild(i), ShowItemEndIndex);
                        ShowItemEndIndex++;
                    }
                    else
                    {
                        //   LoopScrollRectItemBase _scrollRectItem = content.GetChild(i).GetComponent<LoopScrollRectItemBase>();
                        DeleteScrollRectItem(content.GetChild(i), _scrollRectItem.m_DataIndex);
                        //    DeleteScrollRectItem(content.GetChild(i), i);
                        i--;
                    }
                }
            }
        }

        public void RefillCellsFromEnd(int dataCount, int offset = 0)
        {
            //TODO: unsupported for Infinity or Grid yet
            if (!Application.isPlaying || dataCount < 0 || contentConstraintCount > 1 || m_PrefabSource == null)
                return;

            m_NeedFrusheView = true;
            TotalDataCount = dataCount;
            InitialedPoolManager();
            StopMovement();
            ShowItemEndIndex = reverseDirection ? offset : TotalDataCount - offset;
            ShowItemStartIndex = ShowItemEndIndex;

            for (int i = m_Content.childCount - 1; i >= 0; i--)
            {
                //   LoopScrollRectItemBase _scrollRectItem = m_Content.GetChild(i).GetComponent<LoopScrollRectItemBase>();
                LoopScrollRectItemBase _scrollRectItem = m_AllShowItemsRecord[m_Content.GetChild(i)];
                DeleteScrollRectItem(m_Content.GetChild(i), _scrollRectItem.m_DataIndex);
                //   DeleteScrollRectItem(m_Content.GetChild(i), i);
            }

            float sizeToFill = 0, sizeFilled = 0;
            if (directionSign == -1)
                sizeToFill = viewRect.rect.size.y;
            else
                sizeToFill = viewRect.rect.size.x;

            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart();
                if (size <= 0) break;
                sizeFilled += size;
            }

            Vector2 pos = m_Content.anchoredPosition;
            float dist = Mathf.Max(0, sizeFilled - sizeToFill);
            if (reverseDirection)
                dist = -dist;
            if (directionSign == -1)
                pos.y = dist;
            else if (directionSign == 1)
                pos.x = -dist;
            m_Content.anchoredPosition = pos;
        }

        public void RefillCells(int dataCount, int offset = 0)
        {
            if (!Application.isPlaying || m_PrefabSource == null)
                return;

            if (offset < 0)
            {
                Debug.LogError("设置偏移小于0不合理 ，请检查 !!! 已经自动修复为0偏移");
                offset = 0;
            }

#if UNITY_EDITOR
            if (dataCount == offset && offset != 0)
            {
                Debug.LogError("当前设置的偏移会导致无法显示UI Item  offset=" + offset);
            }
#endif

            TotalDataCount = dataCount;
            InitialedPoolManager();
            StopMovement();
            UpdateArrowState();
            ShowItemStartIndex = reverseDirection ? TotalDataCount - offset : offset;
            ShowItemEndIndex = ShowItemStartIndex;

            // Don't `Canvas.ForceUpdateCanvases();` here, or it will new/delete cells to change ShowItemStartIndex/End
            for (int i = m_Content.childCount - 1; i >= 0; i--)
            {
                //  LoopScrollRectItemBase _scrollRectItem = m_Content.GetChild(i).GetComponent<LoopScrollRectItemBase>();
                LoopScrollRectItemBase _scrollRectItem = m_AllShowItemsRecord[m_Content.GetChild(i)];
                DeleteScrollRectItem(m_Content.GetChild(i), _scrollRectItem.m_DataIndex);
                //  DeleteScrollRectItem(m_Content.GetChild(i), i);
            }

            float sizeToFill = 0, sizeFilled = 0;
            // m_ViewBounds may be not ready when RefillCells on Start
            if (directionSign == -1)
                sizeToFill = viewRect.rect.size.y;
            else
                sizeToFill = viewRect.rect.size.x;

            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
                if (size <= 0) break;
                sizeFilled += size;
            }

            Vector2 pos = m_Content.anchoredPosition;
            if (directionSign == -1)
                pos.y = 0;
            else if (directionSign == 1)
                pos.x = 0;
            m_Content.anchoredPosition = pos;
        }

        #endregion



        #region  增加/删除 一行或者一列 (调用的地方控制外层循环 )
        /// <summary>
        /// 从开始位置创建列表项这里会根据每一行/列 Item项的个数限制，创建整行或者整列)
        /// </summary>
        /// <returns>返回创建的整行或者列的  高度/长度 大小，这里不是总长度或者高度，而是当前行或者列中最大的值</returns>
        protected float NewItemAtStart()
        {
            if (TotalDataCount >= 0 && ShowItemStartIndex - contentConstraintCount < 0)
            {
                return 0;
            }
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                ShowItemStartIndex--;
                RectTransform newItem = InstantiateNextItem(ShowItemStartIndex);
                newItem.SetAsFirstSibling();
                size = Mathf.Max(GetSize(newItem), size);
            }

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }
            // protection
            if (size > 0 && threshold < size)
                threshold = size * 1.1f;
            return size;
        }

        protected float DeleteItemAtStart()
        {
            // special case: when moving or dragging, we cannot simply delete start when we've reached the end
            if (((m_Dragging || m_Velocity != Vector2.zero) && TotalDataCount >= 0 && ShowItemEndIndex >= TotalDataCount - 1)
                || content.childCount == 0)
            {
                return 0;
            }

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                RectTransform oldItem = content.GetChild(0) as RectTransform;
                size = Mathf.Max(GetSize(oldItem), size);
                //     LoopScrollRectItemBase _scrollRectItem = oldItem.GetComponent<LoopScrollRectItemBase>();
                LoopScrollRectItemBase _scrollRectItem = m_AllShowItemsRecord[oldItem];
                DeleteScrollRectItem(oldItem, _scrollRectItem.m_DataIndex);
                //      DeleteScrollRectItem(oldItem, i);

                ShowItemStartIndex++;

                if (content.childCount == 0)
                {
                    break;
                }
            }

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }
            return size;
        }


        protected float NewItemAtEnd()
        {
            if (TotalDataCount >= 0 && ShowItemEndIndex >= TotalDataCount)
            {
                return 0;
            }
            float size = 0;
            // issue 4: fill lines to end first
            int count = contentConstraintCount - (content.childCount % contentConstraintCount);
            for (int i = 0; i < count; i++)
            {
                RectTransform newItem = InstantiateNextItem(ShowItemEndIndex);
                size = Mathf.Max(GetSize(newItem), size);
                ShowItemEndIndex++;
                if (TotalDataCount >= 0 && ShowItemEndIndex >= TotalDataCount)
                {
                    break;
                }
            }

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }
            // protection
            if (size > 0 && threshold < size)
                threshold = size * 1.1f;
            return size;
        }

        protected float DeleteItemAtEnd()
        {
            if (((m_Dragging || m_Velocity != Vector2.zero) && TotalDataCount >= 0 && ShowItemStartIndex < contentConstraintCount)
                || content.childCount == 0)
            {
                return 0;
            }

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                RectTransform oldItem = content.GetChild(content.childCount - 1) as RectTransform;
                size = Mathf.Max(GetSize(oldItem), size);

                LoopScrollRectItemBase _scrollRectItem = m_AllShowItemsRecord[oldItem];
                // LoopScrollRectItemBase _scrollRectItem = oldItem.GetComponent<LoopScrollRectItemBase>();
                DeleteScrollRectItem(oldItem, _scrollRectItem.m_DataIndex);
                //   DeleteScrollRectItem(oldItem, i);

                ShowItemEndIndex--;
                if (ShowItemEndIndex % contentConstraintCount == 0 || content.childCount == 0)
                {
                    break;  //just delete the whole row
                }
            }

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }
            return size;
        }


        /// <summary>
        ///  创建一个列表项，并根据索引赋值Item项显示的内容
        /// </summary>
        /// <param name="itemIdx"></param>
        /// <returns></returns>
        private RectTransform InstantiateNextItem(int itemIdx)
        {
            GameObject go = UIObjectPoolManager.Instance.GetObjectFromPool(m_PrefabSource.name);
            if (go == null) return null;

            LoopScrollRectItemBase nextItem = go.GetComponent<LoopScrollRectItemBase>();
            if (nextItem == null)
                nextItem = go.AddComponent<LoopScrollRectItemBase>();

            nextItem.transform.SetParent(content, false);
            nextItem.gameObject.SetActive(true);

            nextItem.InitialedScrollCellItem(itemIdx);  //初始化
            if (m_AllShowItemsRecord.ContainsKey(nextItem.transform))
            {
                Debug.LogError("重复的记录  " + nextItem.transform.gameObject.name);
            }
            else
            {
                m_AllShowItemsRecord.Add(nextItem.transform, nextItem);
            }

            RectTransform rectrans = nextItem.transform as RectTransform;
            if (m_OnItemCreateEvent != null)
                m_OnItemCreateEvent(rectrans, itemIdx);
            return rectrans;
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        private void InitialedPoolManager()
        {
            if (m_IsInitialed || m_PrefabSource == null) return;
            m_IsInitialed = true;
            UIObjectPoolManager.Instance.InitPool(m_PrefabSource, m_PoolSize);
        }


        #endregion


        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                //      UpdateCachedData();
            }

            if (executing == CanvasUpdate.PostLayout)
            {
                //UpdateBounds(false);
                //UpdateScrollbars(Vector2.zero);
                //UpdatePrevData();
                //       m_HasRebuiltLayout = true;
            }
        }

        public virtual void LayoutComplete() { }
        public virtual void GraphicUpdateComplete() { }

        void UpdateCachedData()
        {
            Transform transform = this.transform;
            m_HorizontalScrollbarRect = m_HorizontalScrollbar == null ? null : m_HorizontalScrollbar.transform as RectTransform;
            m_VerticalScrollbarRect = m_VerticalScrollbar == null ? null : m_VerticalScrollbar.transform as RectTransform;

            // These are true if either the elements are children, or they don't exist at all.
            bool viewIsChild = (viewRect.parent == transform);
            bool hScrollbarIsChild = (!m_HorizontalScrollbarRect || m_HorizontalScrollbarRect.parent == transform);
            bool vScrollbarIsChild = (!m_VerticalScrollbarRect || m_VerticalScrollbarRect.parent == transform);
            bool allAreChildren = (viewIsChild && hScrollbarIsChild && vScrollbarIsChild);

            m_HSliderExpand = allAreChildren && m_HorizontalScrollbarRect && horizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
            m_VSliderExpand = allAreChildren && m_VerticalScrollbarRect && verticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
            m_HSliderHeight = (m_HorizontalScrollbarRect == null ? 0 : m_HorizontalScrollbarRect.rect.height);
            m_VSliderWidth = (m_VerticalScrollbarRect == null ? 0 : m_VerticalScrollbarRect.rect.width);
        }


        //private void EnsureLayoutHasRebuilt()
        //{
        //    if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
        //        Canvas.ForceUpdateCanvases();
        //}

        public virtual void StopMovement()
        {
            m_Velocity = Vector2.zero;
        }


        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if (!m_Horizontal)
                position.x = m_Content.anchoredPosition.x;
            if (!m_Vertical)
                position.y = m_Content.anchoredPosition.y;

            if (position != m_Content.anchoredPosition)
            {
                m_Content.anchoredPosition = position;
                UpdateBounds();
            }
        }


        protected virtual bool UpdateItems(Bounds viewBounds, Bounds contentBounds) { return false; }

        private void UpdatePrevData()
        {
            if (m_Content == null)
                m_PrevPosition = Vector2.zero;
            else
                m_PrevPosition = m_Content.anchoredPosition;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
        }

        private void UpdateScrollbars(Vector2 offset)
        {
            UpdateArrowState();
            if (m_HorizontalScrollbar)
            {
                if (m_ContentBounds.size.x > 0 && TotalDataCount > 0)
                {
                    m_HorizontalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x * (ShowItemEndIndex - ShowItemStartIndex) / TotalDataCount);
                }
                else
                    m_HorizontalScrollbar.size = 1;

                m_HorizontalScrollbar.value = horizontalNormalizedPosition;

            }

            if (m_VerticalScrollbar)
            {
                if (m_ContentBounds.size.y > 0 && TotalDataCount > 0)
                {
                    m_VerticalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y * (ShowItemEndIndex - ShowItemStartIndex) / TotalDataCount);
                }
                else
                    m_VerticalScrollbar.size = 1;

                m_VerticalScrollbar.value = verticalNormalizedPosition;
            }
        }

        /// <summary>
        /// 更新上下箭头的状态
        /// </summary>
        private void UpdateArrowState()
        {
            if (m_UpArrowObj != null)
            {
                if (ShowItemStartIndex != 0)
                    m_UpArrowObj.SetActive(true);
                else
                    m_UpArrowObj.SetActive(false);
            }


            if (m_DownArrowObj != null)
            {
                if (TotalDataCount > 5 && ShowItemEndIndex != TotalDataCount)
                    m_DownArrowObj.SetActive(true);
                else
                    m_DownArrowObj.SetActive(false);
            }
        }



        public Vector2 normalizedPosition
        {
            get
            {
                return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }



        private void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
        private void SetVerticalNormalizedPosition(float value) { SetNormalizedPosition(value, 1); }

        private void SetNormalizedPosition(float value, int axis)
        {
            if (TotalDataCount <= 0 || ShowItemEndIndex <= ShowItemStartIndex)
                return;

            //    EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector3 localPosition = m_Content.localPosition;
            float newLocalPosition = localPosition[axis];
            if (axis == 0)
            {
                float elementSize = m_ContentBounds.size.x / (ShowItemEndIndex - ShowItemStartIndex);
                float totalSize = elementSize * TotalDataCount;
                float offset = m_ContentBounds.min.x - elementSize * ShowItemStartIndex;

                newLocalPosition += m_ViewBounds.min.x - value * (totalSize - m_ViewBounds.size[axis]) - offset;
            }
            else if (axis == 1)
            {
                float elementSize = m_ContentBounds.size.y / (ShowItemEndIndex - ShowItemStartIndex);
                float totalSize = elementSize * TotalDataCount;
                float offset = m_ContentBounds.max.y + elementSize * ShowItemStartIndex;

                newLocalPosition -= offset - value * (totalSize - m_ViewBounds.size.y) - m_ViewBounds.max.y;
            }

            if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
            {
                localPosition[axis] = newLocalPosition;
                m_Content.localPosition = localPosition;
                m_Velocity[axis] = 0;
                UpdateBounds();
            }
        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();

            if (m_HSliderExpand || m_VSliderExpand)
            {
                m_Tracker.Add(this, viewRect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.SizeDelta |
                    DrivenTransformProperties.AnchoredPosition);

                // Make view full size to see if content fits.
                viewRect.anchorMin = Vector2.zero;
                viewRect.anchorMax = Vector2.one;
                viewRect.sizeDelta = Vector2.zero;
                viewRect.anchoredPosition = Vector2.zero;

                // Recalculate content layout with this size to see if it fits when there are no scrollbars.
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
            }

            // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
            if (m_VSliderExpand && vScrollingNeeded)
            {
                viewRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.sizeDelta.y);

                // Recalculate content layout with this size to see if it fits vertically
                // when there is a vertical scrollbar (which may reflowed the content to make it taller).
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
            }

            // If it doesn't fit horizontally, enable horizontal scrollbar and shrink view vertically to make room for it.
            if (m_HSliderExpand && hScrollingNeeded)
            {
                viewRect.sizeDelta = new Vector2(viewRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
            }

            // If the vertical slider didn't kick in the first time, and the horizontal one did,
            // we need to check again if the vertical slider now needs to kick in.
            // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
            if (m_VSliderExpand && vScrollingNeeded && viewRect.sizeDelta.x == 0 && viewRect.sizeDelta.y < 0)
            {
                viewRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.sizeDelta.y);
            }
        }

        public virtual void SetLayoutVertical()
        {
            UpdateScrollbarLayout();
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
        }

        void UpdateScrollbarVisibility()
        {
            if (m_VerticalScrollbar && m_VerticalScrollbarVisibility != ScrollbarVisibility.Permanent && m_VerticalScrollbar.gameObject.activeSelf != vScrollingNeeded)
                m_VerticalScrollbar.gameObject.SetActive(vScrollingNeeded);

            if (m_HorizontalScrollbar && m_HorizontalScrollbarVisibility != ScrollbarVisibility.Permanent && m_HorizontalScrollbar.gameObject.activeSelf != hScrollingNeeded)
                m_HorizontalScrollbar.gameObject.SetActive(hScrollingNeeded);
        }

        void UpdateScrollbarLayout()
        {
            if (m_VSliderExpand && m_HorizontalScrollbar)
            {
                m_Tracker.Add(this, m_HorizontalScrollbarRect,
                              DrivenTransformProperties.AnchorMinX |
                              DrivenTransformProperties.AnchorMaxX |
                              DrivenTransformProperties.SizeDeltaX |
                              DrivenTransformProperties.AnchoredPositionX);
                m_HorizontalScrollbarRect.anchorMin = new Vector2(0, m_HorizontalScrollbarRect.anchorMin.y);
                m_HorizontalScrollbarRect.anchorMax = new Vector2(1, m_HorizontalScrollbarRect.anchorMax.y);
                m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0, m_HorizontalScrollbarRect.anchoredPosition.y);
                if (vScrollingNeeded)
                    m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), m_HorizontalScrollbarRect.sizeDelta.y);
                else
                    m_HorizontalScrollbarRect.sizeDelta = new Vector2(0, m_HorizontalScrollbarRect.sizeDelta.y);
            }

            if (m_HSliderExpand && m_VerticalScrollbar)
            {
                m_Tracker.Add(this, m_VerticalScrollbarRect,
                              DrivenTransformProperties.AnchorMinY |
                              DrivenTransformProperties.AnchorMaxY |
                              DrivenTransformProperties.SizeDeltaY |
                              DrivenTransformProperties.AnchoredPositionY);
                m_VerticalScrollbarRect.anchorMin = new Vector2(m_VerticalScrollbarRect.anchorMin.x, 0);
                m_VerticalScrollbarRect.anchorMax = new Vector2(m_VerticalScrollbarRect.anchorMax.x, 1);
                m_VerticalScrollbarRect.anchoredPosition = new Vector2(m_VerticalScrollbarRect.anchoredPosition.x, 0);
                if (hScrollingNeeded)
                    m_VerticalScrollbarRect.sizeDelta = new Vector2(m_VerticalScrollbarRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
                else
                    m_VerticalScrollbarRect.sizeDelta = new Vector2(m_VerticalScrollbarRect.sizeDelta.x, 0);
            }
        }

        private void UpdateBounds(bool updateItems = true)
        {
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();

            if (m_Content == null)
                return;

            // ============LoopScrollRect============
            // Don't do this in Rebuild
            if (Application.isPlaying && updateItems && UpdateItems(m_ViewBounds, m_ContentBounds))
            {
                Canvas.ForceUpdateCanvases();
                m_ContentBounds = GetBounds();
            }
            // ============LoopScrollRect============

            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 contentSize = m_ContentBounds.size;
            Vector3 contentPos = m_ContentBounds.center;
            Vector3 excess = m_ViewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (m_Content.pivot.x - 0.5f);
                contentSize.x = m_ViewBounds.size.x;
            }
            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (m_Content.pivot.y - 0.5f);
                contentSize.y = m_ViewBounds.size.y;
            }

            m_ContentBounds.size = contentSize;
            m_ContentBounds.center = contentPos;
        }

        private readonly Vector3[] m_Corners = new Vector3[4];
        private Bounds GetBounds()
        {
            if (m_Content == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = viewRect.worldToLocalMatrix;
            m_Content.GetWorldCorners(m_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            if (m_MovementType == MovementType.Unrestricted)
                return offset;

            Vector2 min = m_ContentBounds.min;
            Vector2 max = m_ContentBounds.max;

            if (m_Horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;
                if (min.x > m_ViewBounds.min.x)
                    offset.x = m_ViewBounds.min.x - min.x;
                else if (max.x < m_ViewBounds.max.x)
                    offset.x = m_ViewBounds.max.x - max.x;
            }

            if (m_Vertical)
            {
                min.y += delta.y;
                max.y += delta.y;
                if (max.y < m_ViewBounds.max.y)
                    offset.y = m_ViewBounds.max.y - max.y;
                else if (min.y > m_ViewBounds.min.y)
                    offset.y = m_ViewBounds.min.y - min.y;
            }

            return offset;
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected void SetDirtyCaching()
        {
            if (!IsActive())
                return;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }



        #region  拖拽滑动事件处理
        public virtual void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;
            if (IsCanDragAndScroll == false) return;

            m_NeedFrusheView = true;

            //    EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector2 delta = data.scrollDelta;

            //Debug.Log("OnScroll  " + data);
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            if (vertical && !horizontal)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    delta.y = delta.x;
                delta.x = 0;
            }
            if (horizontal && !vertical)
            {
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    delta.x = delta.y;
                delta.y = 0;
            }

            Vector2 position = m_Content.anchoredPosition;
            position += delta * m_ScrollSensitivity;
            if (m_MovementType == MovementType.Clamped)
                position += CalculateOffset(position - m_Content.anchoredPosition);

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Velocity = Vector2.zero;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (IsCanDragAndScroll == false) return;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            m_NeedFrusheView = true;
            UpdateBounds();

            m_PointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
            m_ContentStartPosition = m_Content.anchoredPosition;
            m_Dragging = true;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (IsCanDragAndScroll == false) return;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (IsCanDragAndScroll == false) return;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            UpdateBounds();

            var pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 position = m_ContentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset(position - m_Content.anchoredPosition);
            position += offset;
            if (m_MovementType == MovementType.Elastic)
            {
                //==========LoopScrollRect==========
                if (offset.x != 0)
                    position.x = position.x - RubberDelta(offset.x, m_ViewBounds.size.x) * rubberScale;
                if (offset.y != 0)
                    position.y = position.y - RubberDelta(offset.y, m_ViewBounds.size.y) * rubberScale;
                //==========LoopScrollRect==========
            }

            SetContentAnchoredPosition(position);
        }

        #endregion


    }
}