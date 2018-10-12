using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 提供常用的UI 引用
    /// </summary>
    public class UIViewReference : Singleton_Static<UIViewReference>
    {

        private UICanvasMaskView m_UICanvasMaskView = null;
        /// <summary>
        /// 全屏幕Mask
        /// </summary>
        public UICanvasMaskView UICanvasMaskView_
        {
            get
            {
                if (m_UICanvasMaskView == null)
                {
                    UIManager.Instance.ForceGetUISync<UICanvasMaskView>(Define_ResPath.UICanvasMaskViewPath, UIManagerHelper.Instance.WidgetParentTrans,
                        (view) => { m_UICanvasMaskView = view; }, true);
                }
                return m_UICanvasMaskView;
            }
        }

    }
}