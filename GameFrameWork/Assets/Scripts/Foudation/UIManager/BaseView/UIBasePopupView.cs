using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 弹窗显示的时候对其他已经弹出的弹窗的操作
    /// </summary>
    public enum PopupOpenOperateEnum
    {
        /// <summary>
        /// 保持已经打开的弹窗的状态不改变
        /// </summary>
        KeepPreviousAvailable,
        /// <summary>
        /// 隐藏所有打开的弹窗
        /// </summary>
        HideAllOpenView,
        /// <summary>
        /// 关闭当前属于界面的弹窗
        /// </summary>
        HideCurPagePopupView,
        /// <summary>
        /// 按照优先级显示 (如果已经打开的弹窗优先级更高 则不显示)
        /// </summary>
        PriorityOrder,
    }

    /// <summary>
    /// 关闭弹窗时候的操作
    /// </summary>
    public enum PopupCloseOperateEnum
    {
        /// <summary>
        /// 只关闭自己 不管其他的状态
        /// </summary>
        Simple,
        /// <summary>
        /// 关闭当前界面时候 自动弹出可以弹出的界面
        /// </summary>
        AutoPopup,
        /// <summary>
        /// 关闭当前界面所有已经打开的界面 并且清空优先级不够的未弹出界面
        /// </summary>
        CloseAndClearCurPage, 

    }



    /// <summary>
    /// 弹窗的公共父类
    /// </summary>
    public class UIBasePopupView : UIBaseView
    {
        /// <summary>
        /// 当前弹窗属于哪个界面 (可能是null)
        /// </summary>
        public UIBasePageView BelongPageView { get; protected set; }
        /// <summary>
        /// 当前弹窗的优先级 默认是0
        /// </summary>
        public uint Priority { get; protected set; }

        public object[] WillPopupParameter { get; protected set; }



        public override void ShowWindow(params object[] parameter)
        {
#if UNITY_EDITOR
            if (parameter == null || parameter.Length < 1)
            {
                Debug.LogEditorInfor("ShowWindow Fail, 没有传入当前弹窗所属的界面");
                return;
            }
#endif
            //     if (parameter[0] != null)
            BelongPageView = parameter[0] as UIBasePageView;
            //else
            //    BelongPageView = null;


            base.ShowWindow(parameter);
        }

        /// <summary>
        /// 由于某个弹窗关闭 而显示一个因为优先级关闭而不显示的界面的情况
        /// 参数已经缓存在 WillPopupParameter 中
        /// </summary>
        public virtual void DelayPopupShowWindow()
        {
            if (IsOpen == false)
                gameObject.SetActive(true);
        }


        /// <summary>
        /// 记录由于优先级关闭而不能显示的弹窗
        /// </summary>
        public virtual void FailShowByPriority(params object[] parameter)
        {
            WillPopupParameter = parameter;
            if (IsOpen)
                gameObject.SetActive(false);
        }




    }
}