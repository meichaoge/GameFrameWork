using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 通过AssetBundleLoader 加载出来的资源信息
    /// </summary>
    public class AssetBundleLoaderResultInfor 
    {

        private bool m_IsInitialed = false;

        /// <summary>
        /// 当前资源的外层资源名(如果是整AssetBundle 则是这个大的资源名)
        /// </summary>
        public string AssetName { get; protected set; }

        /// <summary>
        /// 加载完成的AssetBundle 资源（可能是包含若干字资源的AssetBundle）
        /// </summary>
        public AssetBundle LoadedAssetBundle = null;
        /// <summary>
        /// 对于一个文件夹打包成一个整预制体 存在同时加载这个预制体中N个资源的情况
        /// Key 标识当前资源的url+文件名   value标识当前资源
        /// </summary>
        public Dictionary<string, object> LoadAssetBundleResultInforDic = new Dictionary<string, object>();

        /// <summary>
        /// 标识是单独的AssetBundle 还是整个文件夹打包的大AssetBundle
        /// </summary>
        public AssetBundleExitState AssetBundleExitStateEnum = AssetBundleExitState.None;

        /// <summary>
        /// 当前需要加载的资源 需要在加载完清空 （只是文件名）
        /// </summary>
        public HashSet<string> m_AllNeedLoadAssetRecord = new HashSet<string>();




        public void SetAssetName(string assetName, AssetBundleExitState exitState)
        {
            if (m_IsInitialed)
            {
                Debug.LogError("SetAssetName  Fail,Asset Already Initialed");
                return;
            }
            m_IsInitialed = true;
            AssetName = assetName;
            AssetBundleExitStateEnum = exitState;
        }

        /// <summary>
        /// 通过资源Url 获取资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName">可以是null</param>
        /// <returns></returns>
        public object GetAssetByUrl(string url, string fileName)
        {
            if (System.IO.Path.GetExtension(url) != ConstDefine.AssetBundleExtensionName)
                url += ConstDefine.AssetBundleExtensionName;
            url = url.ToLower();

            object result = null;
            if (LoadAssetBundleResultInforDic.TryGetValue(url, out result) == false)
            {
                Debug.LogError("GetAssetByUrl  Fail,Not Exit  !!!!  url=" + url);
            }
            return result;
        }

        /// <summary>
        /// 检测当前资源是否被加载
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool CheckIfLoadAsset(string fileName)
        {
            if (LoadAssetBundleResultInforDic.ContainsKey(fileName))
                return true;
            return false;
        }


        /// <summary>
        /// 记录下载的资源
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="assetData"></param>
        /// <returns></returns>
        public bool RecordLoadAsset(string assetUrl, object assetData)
        {
            if (AssetBundleExitStateEnum == AssetBundleExitState.SinglePrefab)
                assetUrl = AssetName ;


            if (System.IO.Path.GetExtension(assetUrl) != ConstDefine.AssetBundleExtensionName)
                assetUrl += ConstDefine.AssetBundleExtensionName;

            if (LoadAssetBundleResultInforDic.ContainsKey(assetUrl))
            {
                Debug.LogError("RecordLoadAsset Fail,Asset Exit  " + assetUrl);
                return false;
            }
            Debug.LogEditorInfor("RecordLoadAsset  assetUrl= " + assetUrl);
            LoadAssetBundleResultInforDic.Add(assetUrl, assetData);
            return true;
        }

       
        public void ClearData()
        {
            LoadAssetBundleResultInforDic.Clear();
            m_AllNeedLoadAssetRecord.Clear();
            AssetBundleExitStateEnum = AssetBundleExitState.None;
            LoadedAssetBundle=null;
        }

    }
}