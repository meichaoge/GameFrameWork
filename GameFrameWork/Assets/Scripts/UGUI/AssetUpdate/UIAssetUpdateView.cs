using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.Network;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 首页 资源热更新页面
    /// </summary>
    public class UIAssetUpdateView : UIBasePageView
    {
        #region UI
        private Image m_imgProcessBar;
        private TMPro.TextMeshProUGUI m_txtUpdateStateText;
        private TMPro.TextMeshProUGUI m_txtUpdateProcessText;
        private TMPro.TextMeshProUGUI m_txtUpdateSpeedText;
        private TMPro.TextMeshProUGUI m_txtUpdateSizeText;
        private TMPro.TextMeshProUGUI m_txtDownLoadFailText;
        private Button m_btnStartUpButton;
        #endregion

        #region State/Data
        public int TotalSize { get; private set; }  //总共需要下载的数量 单位B
        private int m_TotalSize = 0;  //总共需要下载的数量   单位 m_TotalSizeEnum
        private NetDataEnum m_TotalSizeEnum = NetDataEnum.B;

        public bool m_IsBegingDownLoad = false;

        private int m_DownLoadSizeRecordThisSecond = 0;
        private int m_TotalDownloadedSize = 0;

        #endregion

        #region 多语言配置
        protected const string S_LocalViewUIConfigureFile = "UIConfig_AssetUpdate"; //当前视图关联的动态文本

        private string m_StrCheckingLocalAssetState;  //正在检测本地资源....
        private string m_StrCheckLocalAssetVersion; //正在获取本地资源版本....
        private string m_StrUpdateAssteComplete; //下载完成....
        private string m_StrAssetUpdateSuccess; //资源更新完成....
        private string m_StrBegineDownLoadAsset; //准备更新资源....
        private string m_StrLocalAssetIsTheLastVersion; //资源是最新版本....

        #endregion

        #region Frame
        protected override void Awake()
        {
            base.Awake();
            this.InitView();
        }

        private void InitView()
        {
            Image imgProcessBar = transform.Find("processBgImage/ProcessBar").gameObject.GetComponent<Image>();
            TMPro.TextMeshProUGUI txtUpdateStateText = transform.Find("processBgImage/UpdateStateText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TextMeshProUGUI txtUpdateProcessText = transform.Find("processBgImage/UpdateProcessText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TextMeshProUGUI txtUpdateSpeedText = transform.Find("processBgImage/UpdateSpeedText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TextMeshProUGUI txtUpdateSizeText = transform.Find("processBgImage/UpdateSizeText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            TMPro.TextMeshProUGUI txtDownLoadFailText = transform.Find("processBgImage/DownLoadFailText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            Button btnStartUpButton = transform.Find("StartUpButton").gameObject.GetComponent<Button>();

            //**
            m_imgProcessBar = imgProcessBar;
            m_txtUpdateStateText = txtUpdateStateText;
            m_txtUpdateProcessText = txtUpdateProcessText;
            m_txtUpdateSpeedText = txtUpdateSpeedText;
            m_txtUpdateSizeText = txtUpdateSizeText;
            m_txtDownLoadFailText = txtDownLoadFailText;
            m_btnStartUpButton = btnStartUpButton;

            //**
            m_btnStartUpButton.onClick.AddListener(OnStartUpBtnClick);
        }

        protected override void LoadUIConfigString()
        {
            base.LoadUIConfigString();
            m_StrCheckingLocalAssetState = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "CheckingLocalAssetState");
            m_StrCheckLocalAssetVersion = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "CheckLocalAssetVersion");
            m_StrUpdateAssteComplete = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "UpdateAssteComplete");
            m_StrAssetUpdateSuccess = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "AssetUpdateSuccess");
            m_StrBegineDownLoadAsset = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "BegineDownLoadAsset");
            m_StrLocalAssetIsTheLastVersion = UILanguageMgr.Instance.GetUIDynamicStrConfig(S_LocalViewUIConfigureFile, "LocalAssetIsTheLastVersion");

        }


        public override void ShowWindow(params object[] parameter)
        {
            base.ShowWindow(parameter);
            StartCoroutine(OnEnumerateShowWindow());
        }


        protected override IEnumerator OnEnumerateShowWindow()
        {
            // return base.OnEnumerateShowWindow();
            InitialedView();
            m_txtUpdateStateText.text = m_StrCheckingLocalAssetState;
            yield return new WaitForSeconds(0.3f);
            if (AssetUpdateMgr.S_IsCompleteUpdateAsset == false)
                AssetUpdateMgr.Instance.BeginAssetUpdate(OnBeginUpdateAsset, OnCompleteUpdateAsset, OnUpdateFailAsset, OnCompleteCheckAssetState, OnUpdateProcessBreak);
            else
                OnAssetUpdateComplete(true);

            OnCompleteShowWindow();
        }

        public override void HideWindow(params object[] parameter)
        {
            base.HideWindow(parameter);

            OnCompleteHideWindow();
        }

        #endregion


        #region  创建视图


        /// <summary>
        /// 初始化视图 和状态
        /// </summary>
        private void InitialedView()
        {
            m_txtUpdateStateText.text = "";
            m_txtUpdateProcessText.text = "";
            m_txtUpdateSpeedText.text = "";
            m_txtUpdateSizeText.text = "";
            m_txtDownLoadFailText.text = "";
            m_imgProcessBar.fillAmount = 0;
            m_IsBegingDownLoad = false;
            m_TotalDownloadedSize = 0;
            m_btnStartUpButton.interactable = false;

        }

        /// <summary>
        /// 更新下载进度 （InvokeRepeating调用）
        /// </summary>
        private void UpdateDownLoadProcess()
        {
            //    Debug.Log("UpdateDownLoadProcess  m_DownLoadSizeRecordThisSecond=" + m_DownLoadSizeRecordThisSecond);
            if (CheckIsCompleteDownLoad()) return;
            Debug.LogEditorInfor("UpdateDownLoadProcess m_TotalDownloadedSize= " + m_TotalDownloadedSize + "  TotalSize=" + TotalSize);
            UpdateDownloadProcessView();
        }

        /// <summary>
        /// 更新下载进度视图
        /// </summary>
        private void UpdateDownloadProcessView()
        {
            m_txtUpdateProcessText.text = (int)(m_TotalDownloadedSize * 100f / TotalSize) + "%"; //下载百分比

            NetDataEnum dataEnum = NetDataEnum.B;
            NetWorkUtility.Instance.GetNetDataDesciption(ref m_DownLoadSizeRecordThisSecond, ref dataEnum, true);
            m_txtUpdateSpeedText.text = string.Format("{0} {1}/秒", m_DownLoadSizeRecordThisSecond, dataEnum.ToString());  //下载速度


            dataEnum = NetDataEnum.B;
            int totalDownLoadSize = m_TotalDownloadedSize;
            NetWorkUtility.Instance.GetNetDataDesciption(ref totalDownLoadSize, ref dataEnum, true);
            m_txtUpdateSizeText.text = string.Format("{0} {1}/{2}{3}", totalDownLoadSize, dataEnum, m_TotalSize, m_TotalSizeEnum);  //下载的大小
            Debug.LogEditorInfor("UpdateDownLoadProcess  " + dataEnum + "  Size=" + m_DownLoadSizeRecordThisSecond);

            m_DownLoadSizeRecordThisSecond = 0;
        }

        /// <summary>
        /// 检测资源是否下载完成
        /// </summary>
        /// <returns></returns>
        private bool CheckIsCompleteDownLoad()
        {
            if (m_TotalDownloadedSize == TotalSize)
            {
                m_txtUpdateStateText.text = m_StrUpdateAssteComplete;
                Debug.LogInfor("CheckIsCompleteDownLoad>>>>>>>>>>>> 下载完成 ");
                EventCenter.Instance.DelayDoEnumerator(0.2f, StopUpdateDownloadProcess);
                return true;
            }
            return false;
        }

        private void StopUpdateDownloadProcess()
        {
            CancelInvoke("UpdateDownLoadProcess");
            UpdateDownloadProcessView();
        }



        /// <summary>
        /// 直接显示更新完成的视图
        /// </summary>
        private void DirectShowCompleteUpdateView()
        {
            m_imgProcessBar.fillAmount = 1;
            m_txtUpdateStateText.text = m_StrLocalAssetIsTheLastVersion;
            m_txtUpdateSpeedText.text = "";
            m_txtUpdateSizeText.text = "";
            m_txtUpdateProcessText.text = "100%";
            m_txtDownLoadFailText.text = "";
        }

        #endregion

        #region 回调处理
        private void OnBeginUpdateAsset()
        {
            m_txtUpdateStateText.text = m_StrCheckLocalAssetVersion;
        }

        private void OnCompleteUpdateAsset()
        {

        }

        private void OnUpdateFailAsset(string tipView)
        {
            Debug.LogError("资源更新失败" + tipView);
            m_txtDownLoadFailText.text = tipView;
        }

        /// <summary>
        /// 完成资源状态检测 计算有多少资源需要下载
        /// </summary>
        /// <param name="size"></param>
        private void OnCompleteCheckAssetState(int size)
        {
            Debug.LogInfor("OnCompleteCheckAssetState >>>>>> size=" + size);
            if (size > 0)
            {
                m_txtUpdateStateText.text = m_StrBegineDownLoadAsset;
                m_TotalSize = TotalSize = size;
                NetWorkUtility.Instance.GetNetDataDesciption(ref m_TotalSize, ref m_TotalSizeEnum, true);  //获取总共需要下载的数据总量
                InvokeRepeating("UpdateDownLoadProcess", 0, 0.3f);

                AssetUpdateMgr.Instance.BeginDownloadAsset(0.2f, OnDownLoadAssetSuccess);
            }
            else
            {
                //m_txtUpdateStateText.text = m_StrLocalAssetIsTheLastVersion;
                Debug.LogInfor("资源不需要更新 可以开始游戏!!!");
                OnAssetUpdateComplete(true);
            }
        }

        /// <summary>
        /// 准备下载资源
        /// </summary>
        /// <param name="dataSize"></param>
        private void OnDownLoadAssetSuccess(string path, int dataSize)
        {
            m_txtUpdateStateText.text = path;
            m_DownLoadSizeRecordThisSecond += dataSize;
            m_TotalDownloadedSize += dataSize;

            //  Debug.Log("OnDownLoadAssetSuccess XXXXXXXXXXXXXX" + path + "  dataSize=" + dataSize + "   m_TotalDownloadedSize=" + m_TotalDownloadedSize + "   m_DownLoadSizeRecordThisSecond=" + m_DownLoadSizeRecordThisSecond);

            if (m_TotalDownloadedSize == TotalSize)
            {
                Debug.LogInfor("资源更新完毕 可以开始游戏!!!");
                OnAssetUpdateComplete(false);
            }

        }

        /// <summary>
        /// 资源更新完成
        /// </summary>
        /// <param name="isDirectComplete"></param>
        private void OnAssetUpdateComplete(bool isDirectComplete)
        {
            if (isDirectComplete)
                DirectShowCompleteUpdateView();

            m_btnStartUpButton.interactable = true;
        }


        /// <summary>
        /// 致命错误导致更新失败
        /// </summary>
        /// <param name="errorRecord"></param>
        private void OnUpdateProcessBreak(HotUpdate.AssetUpdateErrorRecordInfor errorRecord)
        {
            CancelInvoke("UpdateDownLoadProcess");
            m_txtUpdateStateText.text = errorRecord.ErrorDescription;
        }
        #endregion

        #region  界面操作
        private void OnStartUpBtnClick()
        {
            if (AssetUpdateMgr.Instance.IsUpdateError)
            {
                Debug.LogError("无法启动游戏  资源更新失败");
                return;
            }


            UIManager.Instance.ForceGetUIAsync<UILoginPopupView>(Define_ResPath.UILoginPopupViewPath, UIManagerHelper.Instance.PopupParentTrans, (loginPopView) =>
            {
                UIManager.Instance.OpenPopUp(loginPopView, PopupOpenOperateEnum.KeepPreviousAvailable,this, true, null);
            }, true, true);

            //UIManager.Instance.ForceGetUISync<UILoginPopupView>(Define_ResPath.UILoginPopupViewPath, UIManagerHelper.Instance.PopupParentTrans, (loginPopView) =>
            //{
            //    UIManager.Instance.OpenPopUp(loginPopView, PopupOpenOperateEnum.KeepPreviousAvailable, true, null);
            //}, false, true);

        }
        #endregion



    }


}