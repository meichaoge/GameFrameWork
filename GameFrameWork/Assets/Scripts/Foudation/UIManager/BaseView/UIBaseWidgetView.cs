﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.UGUI
{

    /// <summary>
    /// Widget 组件父类
    /// </summary>
    public class UIBaseWidgetView : UIBaseView
    {
        /// <summary>
        /// 当前弹窗属于哪个视图(可能是null)
        /// </summary>
        public Transform BelongParent { get; protected set; }

        /// <summary>
        /// 显示后多久自动隐藏 0 标识不隐藏
        /// </summary>
        public float HideDelayTime = 0.5f;

        /// <summary>
        /// 标识是否是单例的组件
        /// </summary>
        public bool IsSinglenton = false;

        protected bool m_IsDestroyedHide = false;


        public override void ShowWindow(params object[] parameter)
        {
#if UNITY_EDITOR
            if (parameter == null || parameter.Length < 1)
            {
                Debug.LogEditorInfor("ShowWindow Fail, 没有传入当前弹窗所属的界面");
                return;
            }
#endif
            BelongParent = parameter[0] as Transform;
            HideDelayTime = float.Parse(parameter[1].ToString());
            IsSinglenton = bool.Parse(parameter[2].ToString());

            base.ShowWindow(parameter);
        }


        public override void HideWindow(params object[] parameter)
        {
            m_IsDestroyedHide = bool.Parse(parameter[0].ToString());
            base.HideWindow(parameter);
        }


        protected override void OnCompleteHideWindow()
        {
            base.OnCompleteHideWindow();

            if (m_IsDestroyedHide)
            {
                Debug.LogEditorInfor("当前组件在隐藏时候自动销毁自己");
                GameObject.Destroy(gameObject);
            }
        }

    }
}