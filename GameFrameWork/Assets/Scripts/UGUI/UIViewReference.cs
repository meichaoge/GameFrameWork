using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 提供常用的UI 引用 （同步方式加载界面,主要是方便项目引用方便，对于子界面可以使用异步方式自行获取引用关系）
    /// </summary>
    public class UIViewReference : Singleton_Static<UIViewReference>
    {
        #region 常见弹框 、Widget


        private UICanvasMaskView m_UICanvasMaskView = null;
        /// <summary>
        /// 全屏幕Mask
        /// </summary>
        public UICanvasMaskView UiCanvasMaskView
        {
            get
            {
                if (m_UICanvasMaskView == null)
                    m_UICanvasMaskView = UIManager.Instance.ForceGetUISync<UICanvasMaskView>(Define_ResPath.UICanvasMaskViewPath, UIManagerHelper.Instance.WidgetParentTrans);
                return m_UICanvasMaskView;
            }
        }

        #endregion


        #region 常用的PageView
        private UIAssetUpdateView m_UIAssetUpdateView = null;
        /// <summary>
        /// 资源更新界面
        /// </summary>
        public UIAssetUpdateView UiAssetUpdateView
        {
            get
            {
                if (m_UIAssetUpdateView == null)
                    m_UIAssetUpdateView = UIManager.Instance.ForceGetUISync<UIAssetUpdateView>(Define_ResPath.UIAssetUpdateViewPath, UIManagerHelper.Instance.PageParentTrans);
                return m_UIAssetUpdateView;
            }
        }


        private UILoginPopupView m_UILoginPopupView;
        /// <summary>
        /// 登录弹窗
        /// </summary>
        public UILoginPopupView UiLoginPopupView
        {
            get
            {
                if (m_UILoginPopupView == null)
                    m_UILoginPopupView = UIManager.Instance.ForceGetUISync<UILoginPopupView>(Define_ResPath.UILoginPopupViewPath, UIManagerHelper.Instance.PopupParentTrans);
                return m_UILoginPopupView;
            }
        }


        #endregion
    }
}