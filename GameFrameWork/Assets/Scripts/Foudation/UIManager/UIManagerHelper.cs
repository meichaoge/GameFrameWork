using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 提供UIManager 常用的访问接口
    /// </summary>
    public class UIManagerHelper : Singleton_Mono_NotDestroy<UIManagerHelper>
    {

        #region  引用
        [SerializeField]
        private Camera m_UICamera;  //UI相机
        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera
        {
            get
            {
                if (m_UICamera == null)
                    m_UICamera = Camera.main;
                return m_UICamera;
            }
        }
        /// <summary>
        /// UI画布
        /// </summary>
        public Transform UICanvasTrans
        {
            get
            {
                return transform;
            }
        }

        [SerializeField]
        private Transform m_PageParentTrans;
        /// <summary>
        /// 所有的Page 都应该在这个下面
        /// </summary>
        public Transform PageParentTrans
        {
            get
            {
                if (m_PageParentTrans == null)
                    m_PageParentTrans = transform.GetChild(0);
                return m_PageParentTrans;
            }
        }

        [SerializeField]
        private Transform m_PopupParentTrans;
        /// <summary>
        /// 所有的PopUP 都应该在这个下面
        /// </summary>
        public Transform PopupParentTrans
        {
            get
            {
                if (m_PopupParentTrans == null)
                    m_PopupParentTrans = transform.GetChild(1);
                return m_PopupParentTrans;
            }
        }

        [SerializeField]
        private Transform m_TipsParentTrans;
        /// <summary>
        /// 所有的Tips 都应该在这个下面
        /// </summary>
        public Transform TipsParentTrans
        {
            get
            {
                if (m_TipsParentTrans == null)
                    m_TipsParentTrans = transform.GetChild(2);
                return m_TipsParentTrans;
            }
        }

        #endregion

    }
}