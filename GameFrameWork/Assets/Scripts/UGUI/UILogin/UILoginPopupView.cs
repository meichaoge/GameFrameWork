using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 登录弹窗界面
    /// </summary>
    public class UILoginPopupView : UIBasePopupView
    {
        #region UI
        private TMPro.TextMeshProUGUI m_txtTitleText;
        private TMPro.TextMeshProUGUI m_txtNameTipText;
        private InputField m_inputNameInputField;
        private TMPro.TextMeshProUGUI m_txtPasswordTipText;
        private TMPro.TMP_InputField m_InputFieldProPasswordInputField;
        private Toggle m_tglShowPassworldToggle;
        private Button m_btnLoginButton;
        private Button m_btnCloseButton;
        #endregion

        #region Data 
        private LocalAccountInfor m_LoginAccount = null;
        private bool m_IsShowPassworld = false; //是否显示密码
        private string m_RealPassworld = "";
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
            Toggle tglShowPassworldToggle = transform.Find("Content/PasswordTipText/ShowPassworldToggle").gameObject.GetComponent<Toggle>();
            Button btnLoginButton = transform.Find("Content/LoginButton").gameObject.GetComponent<Button>();
            Button btnCloseButton = transform.Find("Content/CloseButton").gameObject.GetComponent<Button>();

            //**
            m_txtTitleText = txtTitleText;
            m_txtNameTipText = txtNameTipText;
            m_inputNameInputField = inputNameInputField;
            m_txtPasswordTipText = txtPasswordTipText;
            m_InputFieldProPasswordInputField = InputFieldProPasswordInputField;
            m_tglShowPassworldToggle = tglShowPassworldToggle;
            m_btnLoginButton = btnLoginButton;
            m_btnCloseButton = btnCloseButton;
            //**
            m_tglShowPassworldToggle.onValueChanged.AddListener(OnShowPassworldToggleVlueChange);
            m_btnLoginButton.onClick.AddListener(OnLoginBtnClick);
            m_btnCloseButton.onClick.AddListener(CloseBtnClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_IsShowPassworld = false;
            m_tglShowPassworldToggle.isOn = m_IsShowPassworld;
        }

        public override void ShowWindow(params object[] parameter)
        {
            base.ShowWindow(parameter);
            ShowDefaultView();
        }


        public override void HideWindow(params object[] parameter)
        {
            base.HideWindow(parameter);


            OnCompleteHideWindow();
        }
        #endregion

        #region  创建视图
        private void ShowDefaultView()
        {
      
            m_LoginAccount = LocalAccountMgr.Instance.GetLastLoginInAccount();
            if (m_LoginAccount == null)
            {
                m_inputNameInputField.text = "";
                m_InputFieldProPasswordInputField.text = "";
                m_btnLoginButton.interactable = false;
            }
            else
            {
                m_inputNameInputField.text = m_LoginAccount.AccountName;
                m_RealPassworld = LocalAccountMgr.Instance.GetAccountPassworld(m_LoginAccount.AccountPassworld);
                m_InputFieldProPasswordInputField.text = m_RealPassworld;
                m_btnLoginButton.interactable = true;
            }
            ShowPassworld();
            OnCompleteShowWindow();
        }


        private void ShowPassworld()
        {
            if (m_IsShowPassworld)
            {
                m_InputFieldProPasswordInputField.contentType = TMPro.TMP_InputField.ContentType.Standard;
            }
            else
            {
                m_InputFieldProPasswordInputField.contentType = TMPro.TMP_InputField.ContentType.Password;
            }
            m_InputFieldProPasswordInputField.ForceLabelUpdate();  //这里必须手动刷新下
        }


        #endregion

        #region  视图操作



        private void OnShowPassworldToggleVlueChange(bool isOn)
        {
            if (m_IsShowPassworld == isOn) return;
            m_IsShowPassworld = isOn;
            ShowPassworld();
        }

        private void OnLoginBtnClick()
        {
            if (m_IsShowPassworld)
            {
                Debug.Log("OnLoginBtnClick  >密码>" + m_InputFieldProPasswordInputField.text);
            }

            string inputPassworld = m_InputFieldProPasswordInputField.text;
            inputPassworld = inputPassworld.Trim();
            if (inputPassworld != m_RealPassworld)
            {
                LocalAccountMgr.Instance.UpdateLocalAccount(m_LoginAccount.AccountName, m_LoginAccount.AccountPassworld);
            }


        }


        private void CloseBtnClick()
        {
            HideWindow();
        }


        #endregion

    }
}