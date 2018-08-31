using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 所有Tips 类型的基类
    /// </summary>
    public class UIBaseTipView : UIBaseView
    {
        #region     状态

        /// <summary>
        /// 显示后多久销毁 (0 为不自动销毁)
        /// </summary>
        public float m_AutoDestroyedTime = 0;
        /// <summary>
        /// 唯一标识这个提示框是否是在自毁的过程中.如果=true 则随时可能被销毁
        /// </summary>
        public bool m_IsAvaliable { get; private set; }

        #endregion


        protected override void Awake()
        {
            m_WindowType = WindowTypeEnum.PopTip;
            base.Awake();
        }


        public override void ShowWindow(params object[] parameter)
        {
#if UNITY_EDITOR
            if(parameter==null|| parameter.Length<1)
            {
                Debug.LogError("ShowWindow  Fail,参数异常");
                return;
            }
#endif

            if (float.TryParse(parameter[0].ToString(), out m_AutoDestroyedTime) == false)
            {
                Debug.LogError("ShowWindow  Fail,错误的类型 " + parameter[0].GetType());
                m_AutoDestroyedTime = 0;
            }

            base.ShowWindow(parameter);
            AutoDestroy();
        }



        protected void AutoDestroy()
        {
            if (m_AutoDestroyedTime <= 0) return;
            m_IsAvaliable = false;
            Invoke("DelayDestroy", m_AutoDestroyedTime);
        }


        protected virtual void DelayDestroy()
        {
            UIManager.Instance.CloseTip(this);
            GameObject.Destroy(gameObject);
        }
    }
}