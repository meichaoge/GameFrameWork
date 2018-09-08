using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 登录弹窗界面
    /// </summary>
    public class UILoginView : UIBasePopupView
    {
        #region UI
        private TMPro.TextMeshProUGUI m_txtTitleText;
        private TMPro.TextMeshProUGUI m_txtNameTipText;
        private InputField m_inputNameInputField;
        private TMPro.TextMeshProUGUI m_txtPasswordTipText;
        private TMPro.TMP_InputField m_InputFieldProPasswordInputField;
        private Button m_btnLoginButton;

        #endregion

        #region Frame
        protected override void Awake()
        {
            base.Awake();
            this.InitView();
        }

        private void InitView()
        {
            TMPro.TextMeshProUGUI txtTitleText = transform.Find("Content/TitleText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TextMeshProUGUI txtNameTipText = transform.Find("Content/NameTipText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            InputField inputNameInputField = transform.Find("Content/NameTipText/NameInputField").gameObject.GetComponent<InputField>();
            TMPro.TextMeshProUGUI txtPasswordTipText = transform.Find("Content/PasswordTipText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TMP_InputField InputFieldProPasswordInputField = transform.Find("Content/PasswordTipText/PasswordInputField").gameObject.GetComponent<TMPro.TMP_InputField>();
            Button btnLoginButton = transform.Find("Content/LoginButton").gameObject.GetComponent<Button>();

            //**
            m_txtTitleText = txtTitleText;
            m_txtNameTipText = txtNameTipText;
            m_inputNameInputField = inputNameInputField;
            m_txtPasswordTipText = txtPasswordTipText;
            m_InputFieldProPasswordInputField = InputFieldProPasswordInputField;
            m_btnLoginButton = btnLoginButton;

        }

        public override void ShowWindow(params object[] parameter)
        {
            base.ShowWindow(parameter);

        }

        #endregion

        #region  创建视图
        private void ShowDefaultView()
        {

        }
        #endregion

    }
}