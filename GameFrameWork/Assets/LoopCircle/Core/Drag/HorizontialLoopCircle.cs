using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// 适用于拖动控制滑动的情况(必须加上IDragHandler接口，否则无法拖拽)
/// </summary>
public class HorizontialLoopCircle : BaseHorizontialLoopCircle, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    [Space(10)]
    [Header("默认关闭自动吸附功能")]
    [SerializeField]
    protected bool IsAdsobent = false;

    [SerializeField]
    protected bool IsDraging = false;  //标示是否正在拖拽过程中
    protected Vector2 LastRecordDragPos = Vector2.zero;

    [SerializeField]
    [Range(0.1f, 20f)]
    protected float MinDragDistance = 3f; //当拖拽的距离小于这个值得时候当做没有拖动
    protected float MoveDistance = 0;  //拖拽的距离


    protected float MinMoveVelocityX = 5;  //最小的速度
    [SerializeField]
    [Range(1, 50)]
    protected float MaxMoveVelocityRate = 20;   //最大的移动速度率
    protected float MaxMoveVelocity = 0;  //最大的移动速度 ItemSize*MaxMoveVelocityRate;
    [SerializeField]
    protected float MoveSpeedX = 0; //移动速度
    protected bool IsNeedDoMoreOnce = false;

    protected Camera EventCamera;
    protected Vector3 DragingPointPosition = Vector3.zero;  //拖拽时候手指的位置

    protected Vector2 LocalCursorPosition;//点击时候光标位置
    protected float VelocityAttenuationRate = 2.5f;  //自由滑动时候速度衰减l率

    protected float ContentRectWillMoveDistance = 0;  //Update中计算得出的将要移动的距离 还需要限制到ItemSize.Height 范围

    #region 基类重写
    protected override void Awake()
    {
        base.Awake();
        MaxMoveVelocity = ItemSize.x * MaxMoveVelocityRate;
    }


    //protected override void OnCompleteScrollView()
    //{
    //    IsScrolling = false;
    //    //   Direction = LoopViewShowDirection.None; //****注意这里不能调用这句话
    //}
    #endregion


    #region MONO
    private void Update()
    {
#if UNITY_EDITOR
        MaxMoveVelocity = ItemSize.x * MaxMoveVelocityRate;
#endif
        OnPointDownCaculate();
        if (MoveSpeedX == 0)
            return;

        //Debug.Log("MoveSpeedX=" + MoveSpeedX + "   " + MaxMoveVelocity);
        if (Mathf.Abs(MoveSpeedX) < MinMoveVelocityX)
        {
            if (IsNeedDoMoreOnce == false)
                IsNeedDoMoreOnce = true;
            MoveSpeedX = 0;
            AutoAdsorbentItem();
            return;
        }

        if (OnBeginScrollView() == false)
            return;
        ContentRectWillMoveDistance = 0;

        if (IsDraging == false)
        {
            MoveSpeedX = Mathf.SmoothStep(MoveSpeedX, MoveSpeedX / VelocityAttenuationRate, Time.unscaledDeltaTime * 10); //平滑阻尼运动
            ContentRectWillMoveDistance = MoveSpeedX * Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(MoveDistance) > MinDragDistance)
                ContentRectWillMoveDistance = MoveDistance;
        }

        if (Mathf.Abs(ContentRectWillMoveDistance) >= ViewPortRectrans.rect.width * 0.5f)
        {
            ContentRectWillMoveDistance = ContentRectWillMoveDistance / (Mathf.Abs(ContentRectWillMoveDistance) * ViewPortRectrans.rect.width * 0.5f);
        }//***限制滑动的距离避免滑动太远超出边界

        ContentRectrans.anchoredPosition += new Vector2(ContentRectWillMoveDistance, 0);
        OnCompleteScrollView();  //为下一次滑动做准备
        if (OnItemMovingAct != null)
            OnItemMovingAct();
    }


    void LateUpdate()
    {
        if (MoveSpeedX == 0)
        {
            if (IsDraging == false)
            {
                if (IsNeedDoMoreOnce)
                {
                    IsNeedDoMoreOnce = false;
                    AutoAdsorbentItem();
                }
            }
            Direction = LoopViewShowDirection.None;
            return;
        }
    }
    #endregion

    #region 接口实现
    public void OnBeginDrag(PointerEventData eventData)
    {
        EventCamera = eventData.pressEventCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewPortRectrans, eventData.position, eventData.pressEventCamera, out LastRecordDragPos))
        {
            return;
        }
        //Debug.Log("OnBeginDrag -----" + eventData);
        IsDraging = true;
        IsNeedDoMoreOnce = true;
    }
    /// <summary>
    /// 这里在拖拽过程中如果不移动会出问题 所以在Update中处理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraging = false;
        //Debug.Log("OnEndDrag -----" + eventData);
        OnCompleteScrollView();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewPortRectrans, eventData.position, eventData.pressEventCamera, out LocalCursorPosition))
            return;

        if (Mathf.Abs(LocalCursorPosition.x - LastRecordDragPos.x) <= MinDragDistance)
            return;
        MoveDistance = LocalCursorPosition.x - LastRecordDragPos.x;
        CaculateVelocity();
        LastRecordDragPos = LocalCursorPosition;
    }

    #endregion





    #region 基类重写

    /// <summary>
    /// 由于滑动的距离不确定 所以这里是按照中心点的坐标来 计算而不是边界(按照边界会导致滑动一点点但是一个上下不一样平衡)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected override bool IsItemVisible(RectTransform item)
    {
        if (IsVerticalLayot)
            return true;
        return ViewPortRectrans.IsInsideRect_Horizontial(item.position);
    }

    #endregion



    /// <summary>
    /// 计算速度
    /// </summary>
    protected void CaculateVelocity()
    {
        float VelocityX = MoveDistance / Time.unscaledDeltaTime * 10;
        //MoveSpeedX = Mathf.Lerp(MoveSpeedX, VelocityX, 0.5f);
        Mathf.SmoothDamp(MoveSpeedX, VelocityX, ref MoveSpeedX, Time.unscaledDeltaTime * 10, MaxMoveVelocity);

        if (Mathf.Abs(MoveSpeedX) > MaxMoveVelocity)
        {
            MoveSpeedX = MoveSpeedX / Mathf.Abs(MoveSpeedX) * MaxMoveVelocity;
        }
    }

    /// <summary>
    /// 开始拖拽到结束拖拽过程中计算速度(可以处理拖拽过程中在一个点不移动情况)
    /// </summary>
    private void OnPointDownCaculate()
    {
        if (IsDraging == false)
            return;

        #region 获取不同平台下光标位置

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        DragingPointPosition = Input.mousePosition;
#endif

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
          if (Input.touchCount == 1)
            DragingPointPosition = Input.GetTouch(0).position;
        else
            DragingPointPosition = Vector3.zero;
#endif

        #endregion


        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewPortRectrans, DragingPointPosition, EventCamera, out LocalCursorPosition))
        {
            MoveDistance = 0;
            return;
        }
        if (LocalCursorPosition.x > LastRecordDragPos.x)
            Direction = LoopViewShowDirection.Left;
        //else if (LocalCursorPosition.y == LastRecordDragPos.y)
        //    Direction = LoopViewShowDirection.None; //这里不能设置这个  否在会导致AutoSetItemState 计算异常
        else if (LocalCursorPosition.x < LastRecordDragPos.x)
            Direction = LoopViewShowDirection.Right;

        if (Mathf.Abs(LocalCursorPosition.x - LastRecordDragPos.x) <= MinDragDistance)
        {
            MoveDistance = 0;
            MoveSpeedX = 0;
            return;
        }
        MoveDistance = LocalCursorPosition.x - LastRecordDragPos.x;
        CaculateVelocity();

        //Debug.Log("MoveDistance=" + MoveDistance + "      LocalCursorPosition=" + LocalCursorPosition + "MoveSpeedX=" + MoveSpeedX + "          Direction=" + Direction);
        LastRecordDragPos = LocalCursorPosition;
    }


    #region 扩展

    /// <summary>
    /// 自动吸附到最近的一个整数节点位置
    /// </summary>
    protected void AutoAdsorbentItem()
    {
        if (IsAdsobent == false || ContentRectrans.anchoredPosition.x == 0)
        {
            OnCompleteScrollView();  //为下一次滑动做准备
            AutoSetItemState();
            return;
        }
        float distancePositionX = 0;

        float offsetItemDistance = 0, offset = 0;
        offsetItemDistance = Mathf.FloorToInt(ContentRectrans.anchoredPosition.x / ItemDistance);  //偏移多少个位置，注意负数时候的取整
        offset = (ContentRectrans.anchoredPosition.x) % ItemDistance;


        distancePositionX = offsetItemDistance * ItemDistance;
        if (ContentRectrans.anchoredPosition.x > 0)
        {
            Direction = LoopViewShowDirection.Left;//这里赋值是为了动画处理完成后的AutoSetItemState 
            if (Mathf.Abs(offset) > HalfItemDistance)
                distancePositionX += ItemDistance;
            //Debug.Log("Left " + offsetItemDistance);
        }
        else
        {
            Direction = LoopViewShowDirection.Right;
            if (Mathf.Abs(offset) < HalfItemDistance)
                distancePositionX += ItemDistance;  //由于负数FloorToInt是向下取整的，所以这里要特殊处理
                                                    //    Debug.Log("Right " + offsetItemDistance);
        }

        if (OnItemMovingAct == null)
        {
            ContentRectrans.DOAnchorPosX(distancePositionX, Time.unscaledDeltaTime * 5).OnComplete(OnCompleteAsdobent);
        }
        else
        {
            ContentRectrans.DOAnchorPosX(distancePositionX, Time.unscaledDeltaTime * 5).OnComplete(OnCompleteAsdobent).OnUpdate(OnContentViewTweenUpdate);
        }
    }


    /// <summary>
    /// Tween动画时候执行的事件
    /// </summary>
    protected virtual void OnContentViewTweenUpdate()
    {
        if (OnItemMovingAct != null)
            OnItemMovingAct();
    }

    private void OnCompleteAsdobent()
    {
        OnCompleteScrollView();
        AutoSetItemState();
    }

    #endregion


}
