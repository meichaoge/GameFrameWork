using GameFrameWork.HotUpdate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 资源更新管理基类
    /// </summary>
    public class AssetUpdateMgr : Singleton_Static<AssetUpdateMgr>
    {
        //管理不同种类资源更新
        private Dictionary<HotAssetEnum, AssetUpdateManagerBase> m_AllAssetUpdateManagers = new Dictionary<HotAssetEnum, AssetUpdateManagerBase>();

        #region 回调事件
        private System.Action m_OnBeginUpdateAssetAct = null;
        private System.Action m_OnCompleteUpdateAssetAct = null;
        private System.Action<string> m_OnUpdateErrorAssetAct = null;
        private System.Action<int> m_CompleteCheckAssetState = null;  //资源状态监测完毕
        #endregion

        #region  状态
        public AssetUpdateManagerBase CurAssetUpdateManager = null; //当前正在更新的资源

        /// <summary>
        /// 标识是否正在更新中
        /// </summary>
        public bool IsUpdatingAsset { get; private set; }

        /// <summary>
        /// 标识是否完成加载
        /// </summary>
        public bool IsCompleteUpdateAsset { get; private set; }

        /// <summary>
        /// 标识是否完成资源MD5 检测
        /// </summary>
        public bool IsCompleteCheckAsset { get; private set; }

        /// <summary>
        /// 标识更新时候是否出错了
        /// </summary>
        public bool IsUpdateError { get; private set; }

        /// <summary>
        /// 总共需要下载的数据量单位比特
        /// </summary>
        public int TotalNeedDownloadAsset { get; private set; }
        #endregion

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            IsUpdatingAsset= IsCompleteUpdateAsset = IsUpdateError = false;

            var assetEnum = System.Enum.GetValues(typeof(HotAssetEnum));
           foreach (var item in assetEnum)
           {
                HotAssetEnum hotAsset = (HotAssetEnum)System.Enum.Parse(typeof(HotAssetEnum), item.ToString());
                m_AllAssetUpdateManagers.Add(hotAsset, AssetUpdateManagerFactory.CreateUpdateMgr(hotAsset));
           }
        }


        #region 更新接口
        /// <summary>
        /// 开始资源更新 (参数是更新资源时候的回调)
        /// </summary>
        /// <param name="BeginUpdateAssetcallback"></param>
        /// <param name="CompleteUpdateAssetcallback"></param>
        /// <param name="UpdateErrorAssetcallback"></param>
        public void BeginAssetUpdate(System.Action BeginUpdateAssetcallback, System.Action CompleteUpdateAssetcallback, 
            System.Action<string> UpdateErrorAssetcallback,System.Action<int> CompleteCheckAssetState)
        {
            m_OnBeginUpdateAssetAct = BeginUpdateAssetcallback;
            m_OnCompleteUpdateAssetAct = CompleteUpdateAssetcallback;
            m_OnUpdateErrorAssetAct = UpdateErrorAssetcallback;
            m_CompleteCheckAssetState = CompleteCheckAssetState;
            if (m_OnBeginUpdateAssetAct != null)
                m_OnBeginUpdateAssetAct.Invoke();
            ApplicationMgr.Instance.StartCoroutine(CheckAssetProcess());

        }


        private IEnumerator CheckAssetProcess()
        {
         
            foreach (var assetUpdateManager in m_AllAssetUpdateManagers)
            {
                CurAssetUpdateManager = assetUpdateManager.Value;

                if (CurAssetUpdateManager == null)
                {
                    continue;
                }
                HotAssetServerAddressInfor serverAddressInfor = ApplicationConfig.Instance.GetHotAssetServerAddressInforByType(assetUpdateManager.Key);
                CurAssetUpdateManager.m_OnUpdateFailAct = m_OnUpdateErrorAssetAct;
                CurAssetUpdateManager.BeginAssetUpdateProcess(serverAddressInfor);

           
                while (CurAssetUpdateManager.m_IsCompleteMD5 == false)
                    yield return null;
                TotalNeedDownloadAsset += CurAssetUpdateManager.TotalNeedDownLoadCount;
            }

            IsCompleteCheckAsset = true;
            if (m_CompleteCheckAssetState != null)
                m_CompleteCheckAssetState.Invoke(TotalNeedDownloadAsset);

            ApplicationMgr.Instance.StartCoroutine(UpdateAssetProcess());
        }



        private IEnumerator UpdateAssetProcess()
        {
            if (TotalNeedDownloadAsset != 0)
                yield return new WaitForSeconds(0.2f);
            foreach (var assetUpdateManager in m_AllAssetUpdateManagers)
           {
                CurAssetUpdateManager = assetUpdateManager.Value;

                if (CurAssetUpdateManager == null)
                {
               //     m_AlreadyCompleteUpdateMager++;
                    continue;
                }
                HotAssetServerAddressInfor serverAddressInfor = ApplicationConfig.Instance.GetHotAssetServerAddressInforByType(assetUpdateManager.Key);
                CurAssetUpdateManager.m_OnUpdateFailAct = m_OnUpdateErrorAssetAct;
                CurAssetUpdateManager.BeginUpdateAsset();

                while (CurAssetUpdateManager.m_IsCompleteUpdate == false)
                {
                    yield return new WaitForSeconds(1f);
                }
                while (CurAssetUpdateManager.m_IsUpdateRecorded == false)
                    yield return null;

            }

            IsCompleteUpdateAsset = true;
            if (m_OnCompleteUpdateAssetAct != null)
                m_OnCompleteUpdateAssetAct.Invoke();
        }



        #endregion


    }
}