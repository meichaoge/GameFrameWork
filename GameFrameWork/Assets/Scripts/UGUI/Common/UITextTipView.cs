using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 飘字提示
    /// </summary>
    public class UITextTipView : UIBaseTipView
    {
        #region UI
        private RectTransform m_rtfBgImage;
        private TMPro.TextMeshProUGUI m_txtTipText;

        #endregion

        #region  Data 
        private string m_ShowTipsContent = "";  //需要显示的飘字内容
        #endregion

        #region Frame
        protected override void Awake()
        {
            base.Awake();
            this.InitView();
        }

        private void InitView()
        {
            RectTransform rtfBgImage = transform.Find("BgImage") as RectTransform;
            TMPro.TextMeshProUGUI txtTipText = transform.Find("TipText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

            //**
            m_rtfBgImage = rtfBgImage;
            m_txtTipText = txtTipText;

        }

        public override void ShowWindow(UIParameterArgs parameter)
        {
            base.ShowWindow(parameter);
            if (parameter == null || parameter.ParemeterCount < 2)
            {
                Debug.LogError("飘字弹窗不合法");
                return;
            }
            m_ShowTipsContent = parameter.GetParameterByIndex(1).ToString();

            StartCoroutine(OnEnumerateShowWindow());
        }


        private  IEnumerator OnEnumerateShowWindow()
        {
            m_txtTipText.text = m_ShowTipsContent;
            yield return null;
            AutoSetViewBgImage();

            OnCompleteShowWindow();
        }

     
        #endregion

        #region  创建视图
        /// <summary>
        /// 自动调整背景的大小
        /// </summary>
        private void AutoSetViewBgImage()
        {
            Debug.Log("AutoSetViewBgImage " + m_txtTipText.preferredWidth + "  hh" + m_txtTipText.preferredHeight);

            m_rtfBgImage.sizeDelta = new Vector2(50 + m_txtTipText.preferredWidth, 50 + m_txtTipText.preferredHeight);
        }

        #endregion

    }
}