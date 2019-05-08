using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 适用于拖动控制滑动的情况
/// </summary>
public class VerticalLoopCircle : BaseVerticalLoopCircle, IDragHandler, IEndDragHandler, IBeginDragHandler
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


    protected float MinMoveVelocityY = 5;  //最小的速度
    [SerializeField]
    [Range(1, 50)]
    protected float MaxMoveVelocityRate = 20;   //最大的移动速度率
    protected float MaxMoveVelocity = 0;  //最大的移动速度 ItemSize*MaxMoveVelocityRate;
    [SerializeField]
    protected float MoveSpeedY = 0; //移动速度
    protected bool IsNeedDoMoreOnce = false;

    protected Camera EventCamera;
    protected Vector3 DragingPointPosition = Vector3.zero;  //拖拽时候手指的位置

    protected Vector2 LocalCursorPosition;//点击时候光标位置
    protected float VelocityAttenuationRate = 2.5f;  //自由滑动时候速度衰减l率

    protected float ContentRectWillMoveDistance = 0;  //Update中计算得出的将要移动的距离 还需要限制到ItemSize.Height 范围




    #region MONO
    protected override void Awake()
    {
        base.Awake();
        MaxMoveVelocity = ItemSize.y * MaxMoveVelocityRate;
    }

    private void Update()
    {
#if UNITY_EDITOR
        MaxMoveVelocity = ItemSize.y * MaxMoveVelocityRate;
#endif
        OnPointDownCaculate();
        if (MoveSpeedY == 0)
            return;

        //Debug.Log("MoveSpeedX=" + MoveSpeedY + "   " + MaxMoveVelocity);
        if (Mathf.Abs(MoveSpeedY) < MinMoveVelocityY)
        {
            if (IsNeedDoMoreOnce == false)
                IsNeedDoMoreOnce = true;
            MoveSpeedY = 0;
            AutoAdsorbentItem();
            return;
        }

        if (OnBeginScrollView() == false)
            return;
        ContentRectWillMoveDistance = 0;

        if (IsDraging == false)
        {
            MoveSpeedY = Mathf.SmoothStep(MoveSpeedY, MoveSpeedY / VelocityAttenuationRate, Time.unscaledDeltaTime * 10); //平滑阻尼运动
            ContentRectWillMoveDistance = MoveSpeedY * Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(MoveDistance) > MinDragDistance)
                ContentRectWillMoveDistance = MoveDistance;
        }

        if (Mathf.Abs(ContentRectWillMoveDistance) >= ViewPortRectrans.rect.height * 0.5f)
        {
            ContentRectWillMoveDistance = ContentRectWillMoveDistance / (Mathf.Abs(ContentRectWillMoveDistance) * ViewPortRectrans.rect.height * 0.5f);
        }//***限制滑动的距离避免滑动太远超出边界

        ContentRectrans.anchoredPosition += new Vector2(0, ContentRectWillMoveDistance);
        OnCompleteScrollView();  //为下一次滑动做准备
        if (OnItemMovingAct != null)
            OnItemMovingAct();
    }

    void LateUpdate()
    {
        if (MoveSpeedY == 0)
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

        if (Mathf.Abs(LocalCursorPosition.y - LastRecordDragPos.y) <= MinDragDistance)
            return;
        MoveDistance = LocalCursorPosition.y - LastRecordDragPos.y;
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
    /// 计算速度
    /// </summary>
    protected void CaculateVelocity()
    {
        float VelocityY = MoveDistance / Time.unscaledDeltaTime * 10;
        //MoveSpeedY = Mathf.Lerp(MoveSpeedY, VelocityY, Time.unscaledDeltaTime * 10);
        Mathf.SmoothDamp(MoveSpeedY, VelocityY, ref MoveSpeedY, Time.unscaledDeltaTime * 10, MaxMoveVelocity);


        if (Mathf.Abs(MoveSpeedY) > MaxMoveVelocity)
        {
            MoveSpeedY = MoveSpeedY / Mathf.Abs(MoveSpeedY) * MaxMoveVelocity;
        }
    }

    /// <summary>
    /// 开始拖拽到结束拖拽过程中计算速度(可以处理拖拽过程中在一个点不移动情况)
    /// </summary>
    private void OnPointDownCaculate()
    {
        if (IsDraging == false) return;

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
        if (LocalCursorPosition.y > LastRecordDragPos.y)
            Direction = LoopViewShowDirection.Down;
        //else if (LocalCursorPosition.y == LastRecordDragPos.y)
        //    Direction = LoopViewShowDirection.None;  //这里不能设置这个 否在会导致AutoSetItemState 计算异常
        else if (LocalCursorPosition.y < LastRecordDragPos.y)
            Direction = LoopViewShowDirection.Up;



        if (Mathf.Abs(LocalCursorPosition.y - LastRecordDragPos.y) <= MinDragDistance)
        {
            MoveDistance = 0;
            MoveSpeedY = 0;
            //Debug.Log("Stop");
            return;
        }
        MoveDistance = LocalCursorPosition.y - LastRecordDragPos.y;
        CaculateVelocity();

        //    Debug.Log("MoveDistance=" + MoveDistance + "      LocalCursorPosition=" + LocalCursorPosition + "MoveSpeedY=" + MoveSpeedY + "          Direction=" + Direction);
        LastRecordDragPos = LocalCursorPosition;
    }


    #region 扩展

    /// <summary>
    /// 自动吸附到最近的一个整数节点位置
    /// </summary>
    protected void AutoAdsorbentItem()
    {
        if (IsAdsobent == false || ContentRectrans.anchoredPosition.y == 0)
        {
            OnCompleteScrollView();  //为下一次滑动做准备
            AutoSetItemState();
            return;
        }
        float distancePositionY = 0;
        float offsetItemDistance = 0, offset = 0;
        offsetItemDistance = Mathf.FloorToInt(ContentRectrans.anchoredPosition.y / ItemDistance);  //偏移多少个位置，注意负数时候的取整
        offset = (ContentRectrans.anchoredPosition.y) % ItemDistance;

        distancePositionY = offsetItemDistance * ItemDistance;
        if (ContentRectrans.anchoredPosition.y > 0)
        {
            Direction = LoopViewShowDirection.Up;//这里赋值是为了动画处理完成后的AutoSetItemState 
            if (Mathf.Abs(offset) > HalfItemDistance)
                distancePositionY += ItemDistance;
            //Debug.Log("Up " + offsetItemDistance);
        }
        else
        {
            Direction = LoopViewShowDirection.Down;
            if (Mathf.Abs(offset) < HalfItemDistance)
                distancePositionY += ItemDistance;  //由于负数FloorToInt是向下取整的，所以这里要特殊处理
            //    Debug.Log("Down " + offsetItemDistance);
        }

        if (OnItemMovingAct == null)
        {
            ContentRectrans.DOLocalMoveY(distancePositionY, Time.unscaledDeltaTime * 5).OnComplete(OnCompleteAsdobent);
        }
        else
        {
            ContentRectrans.DOLocalMoveY(distancePositionY, Time.unscaledDeltaTime * 5).OnComplete(OnCompleteAsdobent).OnUpdate(OnContentViewTweenUpdate);
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
        //Debug.Log("OnCompleteAsdobent    " + Direction);
        OnCompleteScrollView();
        AutoSetItemState();
    }

    #endregion

}
