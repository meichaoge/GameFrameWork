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
        #region Tips Frame的公共操作
        private static List<UIBaseTipView> m_AllRecordViewTips = new List<UIBaseTipView>();//按照打开的事件顺序记录了所有打开的飘窗提示UI

        #endregion


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

        #region Tips Frame

        protected override void Awake()
        {
            m_WindowType = WindowTypeEnum.PopTip;
            base.Awake();
        }

        public override void ShowWindow(UIParameterArgs parameter)
        {
#if UNITY_EDITOR
            if (parameter == null || parameter.ParemeterCount < 1)
            {
                Debug.LogError("ShowWindow  Fail,参数异常");
                return;
            }
#endif

            if (float.TryParse(parameter.GetParameterByIndex(0).ToString(), out m_AutoDestroyedTime) == false)
            {
                Debug.LogError("ShowWindow  Fail,错误的类型 " + parameter.GetParameterByIndex(0).GetType());
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
            CloseTip();
            GameObject.Destroy(gameObject);
        }

        #endregion


        #region Tips 类型的公共界面
        /// <summary>
        /// 打开弹窗 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="autoDestroyTime">自动销毁时间 为0标识不会自毁</param>
        /// <param name="parameter"></param>
        public void OpenTip(UIParameterArgs parameter, float autoDestroyTime = 0f)
        {
            if (parameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }
#if UNITY_EDITOR
            if (this.WindowType != WindowTypeEnum.PopTip)
            {
                Debug.LogError("OpenTip Fail,Not Page Window " + this.name);
                return;
            }
#endif
            parameter.InsertParameter(autoDestroyTime, 0);
            this.ShowWindow(parameter);
            m_AllRecordViewTips.Add(this);
        }

        /// <summary>
        /// 关闭界面 (包括自动销毁了)
        /// </summary>
        /// <param name="view"></param>
        public void CloseTip()
        {
            for (int dex = m_AllRecordViewTips.Count - 1; dex >= 0; --dex)
            {
                if (m_AllRecordViewTips[dex] == this)
                {
                    m_AllRecordViewTips.RemoveAt(dex);
                    break;
                }
            }
            Debug.LogError("CloseTip Fail  Not Exit " + this.name);
        }

        #endregion


    }
}