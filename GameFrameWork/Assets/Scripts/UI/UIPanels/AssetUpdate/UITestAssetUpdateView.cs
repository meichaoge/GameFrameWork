using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameFrameWork.Network;

namespace GameFrameWork.UGUI
{

    /// <summary>
    /// 测试显示资源更新下载的进度
    /// </summary>
    public class UITestAssetUpdateView : MonoBehaviour
    {
        public TextMeshProUGUI m_UpdateStateText;
        public TextMeshProUGUI m_UpdateProcessText;
        public TextMeshProUGUI m_UpdateSpeedText;
        public TextMeshProUGUI m_UpdateSizeText;
        public Image m_ProcessImg;

        public int TotalSize = 0;  //总共需要下载的数量

        public bool m_IsBegingDownLoad = false;
        private float m_LastRecordTime;
        private int m_DownLoadSizeRecordThisSecond = 0;
        private int m_TotalDownloadedSize = 0;
        private float m_UpdateDetail = 0.1f; //状态更新时间间隔

        //用于描述总共需要下载的数据总量
        private float m_TotalSize = 0;
        private NetDataEnum m_TotalSizeEnum = NetDataEnum.B;

        private void OnEnable()
        {
            m_UpdateStateText.text = "";
            m_UpdateProcessText.text = "";
            m_UpdateSpeedText.text = "";
            m_UpdateSizeText.text = "";
            m_ProcessImg.fillAmount = 0;
            m_IsBegingDownLoad = false;

            m_TotalDownloadedSize = 0;
        }


        /// <summary>
        /// 设置显示的状态 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="isBegingDownLoad"></param>
        public void ShowAssetStateInfor(string state, int totalSize = -1)
        {
            m_UpdateStateText.text = state;
            if (totalSize != -1)
            {
                m_TotalSize = TotalSize = totalSize;
                NetWorkUtility.Instance.GetNetDataDesciption(ref m_TotalSize, ref m_TotalSizeEnum, 2);  //获取总共需要下载的数据总量
            }
        }

        public void BeginUpdateProcess()
        {
            m_IsBegingDownLoad = true;
            if (TotalSize == 0)
            {
                Debug.LogError("资源不需要更新");
                m_IsBegingDownLoad = false;
                return;
            }
            m_DownLoadSizeRecordThisSecond = 0;
            m_LastRecordTime = Time.time;
        }

        public void StopUpdateProcess(bool isSuccess)
        {
            if (m_IsBegingDownLoad)
            {
                m_IsBegingDownLoad = false;
                if (isSuccess)
                    ShowDownloadProcess(true, true); //这里需要调用一次 否则最后 m_UpdateDetail 间隔内的下载数据没有显示正确
                else
                    ShowDownloadProcess(false, false); //这里需要调用一次 否则最后 m_UpdateDetail 间隔内的下载数据没有显示正确
            }
        }

        /// <summary>
        /// 下载成功一个资源的回调
        /// </summary>
        /// <param name="assetSize"></param>
        public void OnDownLoadAssetCallBack(int assetSize)
        {
            m_DownLoadSizeRecordThisSecond += assetSize;
            m_TotalDownloadedSize += assetSize;

            Debug.LogInfor(string.Format("OnDownLoadAssetCallBack {0}:{1}", m_DownLoadSizeRecordThisSecond, m_TotalDownloadedSize));
        }



        private void Update()
        {
            if (m_IsBegingDownLoad == false) return;
            if (Time.time - m_LastRecordTime >= m_UpdateDetail)
            {
                ShowDownloadProcess(true, true);
                //准备下一次
                m_LastRecordTime = Time.time;
                m_DownLoadSizeRecordThisSecond = 0;
            }
        }

        private void ShowDownloadProcess(bool isShowDownloadSpeed, bool isShowDownloadCount)
        {
            float process = m_TotalDownloadedSize / (TotalSize * 1f);
            m_UpdateProcessText.text = (int)(process * 100) + "%";
            if (isShowDownloadSpeed)
            {
                float downloadSize = m_DownLoadSizeRecordThisSecond / m_UpdateDetail;
                NetDataEnum dataEnum = NetDataEnum.B;
                NetWorkUtility.Instance.GetNetDataDesciption(ref downloadSize, ref dataEnum, 2);
                if (m_UpdateSpeedText.gameObject.activeSelf == false)
                    m_UpdateSpeedText.gameObject.SetActive(true);
                m_UpdateSpeedText.text = string.Format("当前网速{0}{1}/S", downloadSize, dataEnum.ToString());
                // m_UpdateSpeedText.text = "当前网速" + m_DownLoadSizeRecordThisSecond/ m_UpdateDetail + "B/S";
            }
            else
            {
                m_UpdateSpeedText.gameObject.SetActive(false);
            }

            if (isShowDownloadCount)
            {
                float TotalDownloadedSize = m_TotalDownloadedSize;
                NetDataEnum dataEnum2 = NetDataEnum.B;
                NetWorkUtility.Instance.GetNetDataDesciption(ref TotalDownloadedSize, ref dataEnum2, 2);
                if (m_UpdateSizeText.gameObject.activeSelf == false)
                    m_UpdateSizeText.gameObject.SetActive(true);
                m_UpdateSizeText.text = string.Format("更新进度{0}{1}/{2}{3}", TotalDownloadedSize, dataEnum2.ToString(), m_TotalSize, m_TotalSizeEnum.ToString());
                //  m_UpdateSizeText.text = string.Format("更新进度{0}B/{1}B", m_TotalDownloadedSize, TotalSize);
            }
            else
            {
                m_UpdateSizeText.gameObject.SetActive(false);
            }
            m_ProcessImg.fillAmount = process;
        }


    }
}