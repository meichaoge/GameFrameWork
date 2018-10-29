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
        private Toggle m_tglShowPasswordToggle;
        private Button m_btnLoginButton;
        private Button m_btnCloseButton;
        #endregion

        #region Data 
        private LocalAccountInfor m_LoginAccount = null;
        private bool m_IsShowPassworld = false; //是否显示密码
        private string m_RealPassworld = "";


        private const uint S_MinPlayerNameCharLimt = 4;  //用户名和密码输入字符限制
        private const uint S_MaxPlayerNameCharLimt = 14;
        private const uint S_MinPlayerPasswordCharLimt = 8;
        private const uint S_MaxPlayerPasswordCharLimt = 18;

        private string m_PreviousInputPlayerName;
        private string m_PreviousInputPlayerPassword;

        #endregion

        #region  多语言
        protected const string S_LocalViewUIConfigureFile = "UILoginConfig"; //当前视图关联的动态文本


        private string m_PlayerNameCharLimitStr;  //用户名至少包含{0}字符至多{1}个字符
        private string m_PlayerPasswordCharLimitStr; //用户密码至少包含{0}字符至多{1}个字符

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
            Toggle tglShowPasswordToggle = transform.Find("Content/PasswordTipText/ShowPasswordToggle").gameObject.GetComponent<Toggle>();
            Button btnLoginButton = transform.Find("Content/LoginButton").gameObject.GetComponent<Button>();
            Button btnCloseButton = transform.Find("Content/CloseButton").gameObject.GetComponent<Button>();

            //**
            m_txtTitleText = txtTitleText;
            m_txtNameTipText = txtNameTipText;
            m_inputNameInputField = inputNameInputField;
            m_txtPasswordTipText = txtPasswordTipText;
            m_InputFieldProPasswordInputField = InputFieldProPasswordInputField;
            m_tglShowPasswordToggle = tglShowPasswordToggle;
            m_btnLoginButton = btnLoginButton;
            m_btnCloseButton = btnCloseButton;
            //**
            m_tglShowPasswordToggle.onValueChanged.AddListener(OnShowPassworldToggleVlueChange);
            m_btnLoginButton.onClick.AddListener(OnLoginBtnClick);
            m_btnCloseButton.onClick.AddListener(CloseBtnClick);
            m_inputNameInputField.onValueChanged.AddListener(OnPlayerNameInputFieldValueChange);
            m_InputFieldProPasswordInputField.onValueChanged.AddListener(OnPlayerPasswordInputFieldValueChange);

        }

        protected override void LoadUIConfigString()
        {
            base.LoadUIConfigString();
            m_PlayerNameCharLimitStr = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "PlayerNameCharLimit");
            m_PlayerPasswordCharLimitStr = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "PlayerPasswordCharLimit");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_IsShowPassworld = false;
            m_tglShowPasswordToggle.isOn = m_IsShowPassworld;
        }

        public override void ShowWindow(UIParameterArgs parameter)
        {
            base.ShowWindow(parameter);
            ShowDefaultView();

            OnCompleteShowWindow();
        }


        public override void HideWindow(UIParameterArgs parameter)
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
        /// <summary>
        /// 玩家名称输入
        /// </summary>
        /// <param name="InputplayerName"></param>
        private void OnPlayerNameInputFieldValueChange(string InputplayerName)
        {
            int charCount = System.Text.Encoding.Default.GetBytes(InputplayerName).Length;

            if (charCount > S_MaxPlayerNameCharLimt)
            {
                m_inputNameInputField.text = m_PreviousInputPlayerName;
            }
            else
            {
                m_inputNameInputField.text = m_inputNameInputField.text.Trim();
                m_PreviousInputPlayerName = m_inputNameInputField.text;
            }

            if (string.IsNullOrEmpty(m_inputNameInputField.text) || string.IsNullOrEmpty(m_InputFieldProPasswordInputField.text))
            {
                m_btnLoginButton.interactable = false;
            }
            else
            {
                m_btnLoginButton.interactable = true;
            }
        }

        /// <summary>
        /// 玩家密码输入
        /// </summary>
        /// <param name="InputplayerName"></param>
        private void OnPlayerPasswordInputFieldValueChange(string InputplayerPassword)
        {
            int charCount = System.Text.Encoding.Default.GetBytes(InputplayerPassword).Length;

            if (charCount > S_MaxPlayerPasswordCharLimt)
            {
                m_InputFieldProPasswordInputField.text = m_PreviousInputPlayerPassword;
            }
            else
            {
                m_InputFieldProPasswordInputField.text = m_InputFieldProPasswordInputField.text.Trim();
                m_PreviousInputPlayerPassword = m_InputFieldProPasswordInputField.text;
            }

            if (string.IsNullOrEmpty(m_inputNameInputField.text) || string.IsNullOrEmpty(m_InputFieldProPasswordInputField.text))
            {
                m_btnLoginButton.interactable = false;
            }
            else
            {
                m_btnLoginButton.interactable = true;
            }
        }

        /// <summary>
        /// 密码输入框状态改变
        /// </summary>
        /// <param name="isOn"></param>
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
                Debug.LogEditorInfor("OnLoginBtnClick  >密码>" + m_InputFieldProPasswordInputField.text);
            }

            int playerNameCharCount = System.Text.Encoding.Default.GetBytes(m_inputNameInputField.text).Length;
            int playerPasswordCharCount = System.Text.Encoding.Default.GetBytes(m_InputFieldProPasswordInputField.text).Length;

            if (playerNameCharCount < S_MinPlayerNameCharLimt || playerNameCharCount > S_MaxPlayerNameCharLimt)
            {
                Debug.LogError("用户名不合法");
                UIHelper.Instance.ShowTipsViewSync(string.Format(m_PlayerNameCharLimitStr, S_MinPlayerNameCharLimt, S_MaxPlayerNameCharLimt), 2);
                return;
            }
            if (playerPasswordCharCount < S_MinPlayerPasswordCharLimt || playerPasswordCharCount > S_MaxPlayerPasswordCharLimt)
            {
                Debug.LogError("用户密码不合法");
                UIHelper.Instance.ShowTipsViewSync(string.Format(m_PlayerPasswordCharLimitStr, S_MinPlayerPasswordCharLimt, S_MaxPlayerPasswordCharLimt), 2);
                return;
            }

            string inputPassworld = m_InputFieldProPasswordInputField.text;
            inputPassworld = inputPassworld.Trim();
            if (inputPassworld != m_RealPassworld)
            {
                if (m_LoginAccount == null)
                    m_LoginAccount = new LocalAccountInfor(m_PreviousInputPlayerName, m_PreviousInputPlayerPassword);

                Debug.Log("保存账户信息");
                LocalAccountMgr.Instance.UpdateLocalAccount(m_LoginAccount);
            } //更新本地账户列表



            if (UIViewReference.Instance.UiAssetUpdateView != null && UIViewReference.Instance.UiAssetUpdateView.IsActivate)
                UIViewReference.Instance.UiAssetUpdateView.ClosePage(UIParameterArgs.Create(), UIParameterArgs.Create());



            UIViewReference.Instance.UiCanvasMaskView.OpenWidget(UIManagerHelper.Instance.WidgetParentTrans, 0, true, UIParameterArgs.Create());

            AppSceneManager.Instance.LoadSceneAsync(SceneNameEnum.StartUp, LoadSceneModeEnum.KeepPrevious, (isComplete) =>
            {
                UIViewReference.Instance.UiCanvasMaskView.CloseWidget(false, UIParameterArgs.Create(false, true));
                if (isComplete)
                {
                    Debug.LogInfor("应用已经起来了..Go!!!");
                }
            }, () => { Debug.LogInfor("卸载其他场景完成"); }
            );

        }


        private void CloseBtnClick()
        {
            HideWindow(UIParameterArgs.Create());
        }


        #endregion

    }
}