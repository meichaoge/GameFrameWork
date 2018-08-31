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
        #endregion

        #region State/Data

        public int TotalSize = 0;  //总共需要下载的数量
        public bool m_IsBegingDownLoad = false;
        private float m_LastRecordTime;
        private int m_DownLoadSizeRecordThisSecond = 0;
        private int m_TotalDownloadedSize = 0;
        private float m_UpdateDetail = 0.1f; //状态更新时间间隔

        //用于描述总共需要下载的数据总量
        private float m_TotalSize = 0;
        private NetDataEnum m_TotalSizeEnum = NetDataEnum.B;

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

            //**
            m_imgProcessBar = imgProcessBar;
            m_txtUpdateStateText = txtUpdateStateText;
            m_txtUpdateProcessText = txtUpdateProcessText;
            m_txtUpdateSpeedText = txtUpdateSpeedText;
            m_txtUpdateSizeText = txtUpdateSizeText;
            m_txtDownLoadFailText = txtDownLoadFailText;
        }


        public override void ShowWindow(params object[] parameter)
        {
            base.ShowWindow(parameter);
            OnShowWindow();
        }


        protected override void OnShowWindow()
        {
            base.OnShowWindow();
            InitialedView();
            AssetUpdateMgr.Instance.BeginAssetUpdate(OnBeginUpdateAsset, OnCompleteUpdateAsset, OnUpdateFailAsset, OnCompleteCheckAssetState);
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
        }


        #endregion

        #region 回调处理
        private void OnBeginUpdateAsset()
        {
            m_txtUpdateStateText.text = "正在获取本地资源版本";
        }

        private void OnCompleteUpdateAsset()
        {
            AppSceneManager.Instance.LoadScene(SceneNameEnum.StartUp, LoadSceneModeEnum.KeepPrevious, (isComplete) =>
            {
                if (isComplete)
                {
                    Debug.LogInfor("应用已经起来了..Go!!!");
                }
            }, () => { Debug.LogInfor("卸载其他场景完成"); }
        );
        }

        private void OnUpdateFailAsset(string tipView)
        {
            Debug.LogError("资源更新失败"+ tipView);
            m_txtDownLoadFailText.text = tipView;
        }


        private void OnCompleteCheckAssetState(int size)
        {
            Debug.LogInfor("OnCompleteCheckAssetState >>>>>> size=" + size);
            if (size > 0)
                m_txtUpdateStateText.text = "准备更新资源";
            else
                m_txtUpdateStateText.text = "资源是最新版本";
            if (size > 0)
            {
                m_TotalSize = TotalSize = size;
                NetWorkUtility.Instance.GetNetDataDesciption(ref m_TotalSize, ref m_TotalSizeEnum, 2);  //获取总共需要下载的数据总量
            }
        }

        private void OnDownLoadAssetSuccess()
        {

        }

        #endregion





    }
}