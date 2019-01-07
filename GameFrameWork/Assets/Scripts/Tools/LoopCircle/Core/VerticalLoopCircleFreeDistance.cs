using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
#if  UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

/// <summary>
/// 适用于拖动控制滑动的情况
/// </summary>
public class VerticalLoopCircleFreeDistance : BaseVerticalLoopCircle, IDragHandler, IEndDragHandler, IBeginDragHandler
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

    protected float WillMoveDistance = 0;  //Update中计算得出的将要移动的距离 还需要限制到ItemSize.Height 范围

    #region 基类重写
    protected override void Start()
    {
        base.Start();
        MaxMoveVelocity = ItemSize.y * MaxMoveVelocityRate;
    }
    #endregion


    #region 接口实现
    public void OnBeginDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR
    //    ClearConsole();
#endif
        EventCamera = eventData.pressEventCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewPortRectrans, eventData.position, eventData.pressEventCamera, out LastRecordDragPos))
        {
            return;
        }
        //Debug.Log("OnBeginDrag -----" + eventData);

        MoveSpeedY = 0; //强制结束之前的滑动
        IsDraging = true;
        IsNeedDoMoreOnce = true;
        //   Direction = LoopCircleDirection.None;
    }
    /// <summary>
    /// 这里在拖拽过程中如果不移动会出问题 所以在Update中处理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraging = false;
        //      Debug.Log("OnEndDrag -----" + eventData);
        OnCompleteScrollView();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewPortRectrans, eventData.position, eventData.pressEventCamera, out LocalCursorPosition))
            return;

        if (Mathf.Abs(LocalCursorPosition.y - LastRecordDragPos.y) <= MinDragDistance) return;
        MoveDistance = LocalCursorPosition.y - LastRecordDragPos.y;
        CaculateVelocity();
        LastRecordDragPos = LocalCursorPosition;
    }


    #endregion

    private void Update()
    {
#if UNITY_EDITOR
        MaxMoveVelocity = ItemSize.y * MaxMoveVelocityRate;
#endif
        OnPointDownCaculate();
        if (MoveSpeedY == 0)
        {
            return;
        }

        if (Mathf.Abs(MoveSpeedY) < MinMoveVelocityY)
        {
            if (IsNeedDoMoreOnce == false)
                IsNeedDoMoreOnce = true;
            //     Debug.Log("----xxxxxxxxxxxxxx-----------" + "   Direction=" + Direction);
            MoveSpeedY = 0;
            AutoAdsorbentItem();
            return;
        }

        if (OnBeginScrollView() == false) return;
        WillMoveDistance = 0;

        if (IsDraging == false)
        {
            if (Direction == LoopCircleDirection.Up)
            {
                MoveSpeedY = Mathf.SmoothStep(MoveSpeedY, MoveSpeedY / VelocityAttenuationRate, Time.unscaledDeltaTime * 10); //平滑阻尼运动
            }
            else
            {
                MoveSpeedY = Mathf.SmoothStep(MoveSpeedY, MoveSpeedY / VelocityAttenuationRate, Time.unscaledDeltaTime * 10); //平滑阻尼运动
            }
            WillMoveDistance = MoveSpeedY * Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(MoveDistance) > MinDragDistance)
                WillMoveDistance = MoveDistance;
        }

        if (Mathf.Abs(WillMoveDistance) >= ViewPortRectrans.sizeDelta.y * 0.9f)
        {
            WillMoveDistance = WillMoveDistance / (Mathf.Abs(WillMoveDistance) * ViewPortRectrans.sizeDelta.y * 0.9f);
        }//***限制滑动的距离避免滑动太远超出边界

        ContentRectrans.localPosition += new Vector3(0, WillMoveDistance, 0);
        OnCompleteScrollView();  //为下一次滑动做准备

    }


    void LateUpdate()
    {
        if (MoveSpeedY == 0)
        {
            if (IsDraging == false)
            {
                if (IsNeedDoMoreOnce)
                {
                    //      Debug.Log("---------------");
                    IsNeedDoMoreOnce = false;
                    AutoAdsorbentItem();
                }
            }
            Direction = LoopCircleDirection.None;
            return;
        }
    }


    #region Unity Editor
//#if UNITY_EDITOR
//    private static void ClearConsole()
//    {
//        var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
//        var type = assembly.GetType("UnityEditorInternal.LogEntries");
//        if (type == null)
//        {
//            type = assembly.GetType("UnityEditor.LogEntries");
//        }
//        var method = type.GetMethod("Clear");
//        method.Invoke(new object(), null);
//    }
//#endif
    #endregion



    /// <summary>
    /// 计算速度
    /// </summary>
    protected void CaculateVelocity()
    {
        Vector2 Velocity = new Vector2(0, MoveDistance / Time.unscaledDeltaTime * 10);
        MoveSpeedY = Mathf.Lerp(MoveSpeedY, Velocity.y, Time.unscaledDeltaTime * 10);

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
        {
            DragingPointPosition = Input.GetTouch(0).position;
        }
        else
        {
            DragingPointPosition = Vector3.zero;
        }
#endif

        #endregion


        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewPortRectrans, DragingPointPosition, EventCamera, out LocalCursorPosition))
        {
            MoveDistance = 0;
            return;
        }
        if (LocalCursorPosition.y > LastRecordDragPos.y)
            Direction = LoopCircleDirection.Up;
        else if (LocalCursorPosition.y == LastRecordDragPos.y)
            Direction = LoopCircleDirection.None;
        else
            Direction = LoopCircleDirection.Down;

        if (Mathf.Abs(LocalCursorPosition.y - LastRecordDragPos.y) <= MinDragDistance)
        {
            MoveDistance = 0;
            MoveSpeedY = 0;
            return;
        }
        MoveDistance = LocalCursorPosition.y - LastRecordDragPos.y;
        CaculateVelocity();

        //Debug.Log("MoveDistance=" + MoveDistance + "      LocalCursorPosition=" + LocalCursorPosition + "MoveSpeedY=" + MoveSpeedY + "          Direction=" + Direction);
        LastRecordDragPos = LocalCursorPosition;
    }


    #region 扩展

    /// <summary>
    /// 自动吸附到最近的一个整数节点位置
    /// </summary>
    protected void AutoAdsorbentItem()
    {
        if (IsAdsobent == false)
        {
            OnCompleteScrollView();  //为下一次滑动做准备
            AutoSetItemState();
            return;
        }
        float distancePositionY = 0;
        if (IsShowItemMiddle)
        {
            float offset = ContentRectrans.localPosition.y % ItemDistance;
            int offsetItemDistance = (int)(ContentRectrans.localPosition.y / ItemDistance);
            if (Mathf.Abs(offset) > HalfItemDistance)
            {
                if (offsetItemDistance > 0)
                {
                    distancePositionY = (offsetItemDistance + 1) * ItemDistance;
                    Direction = LoopCircleDirection.Up;
                }
                else
                {
                    distancePositionY = (offsetItemDistance - 1) * ItemDistance;
                    Direction = LoopCircleDirection.Down;
                }
            }
            else
            {
                distancePositionY = offsetItemDistance * ItemDistance;
            }
        }//奇数个元素
        else
        {
            float offsetItemDistance = (ContentRectrans.localPosition.y / HalfItemDistance);
            int offset = (int)(ContentRectrans.localPosition.y % HalfItemDistance);
            Debug.LogError("TODO  还没有实现 位置为HalfItemDistance 的奇数倍数位置");
        } //偶数

        //   Debug.Log("distancePositionY=" + distancePositionY+"Distance="+(Mathf.Abs(ContentRectrans.localPosition.y- distancePositionY )));
         ContentRectrans.DOLocalMoveY(distancePositionY, Time.unscaledDeltaTime).OnComplete(OnCompleteAsdobent);
    }

    private void OnCompleteAsdobent()
    {
        //Debug.Log("OnCompleteAsdobent    " + Direction);
        OnCompleteScrollView();
        AutoSetItemState();
    }

    #endregion

}
