using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{

    /// <summary>
    /// AssetBundle 资源加载管理器
    /// </summary>
    public class AssetBundleMgr : Singleton_Static<AssetBundleMgr>
    {
        private AssetBundleManifest _AssetBundleManifest = null;
        /// <summary>
        /// AssetBundle 主资源
        /// </summary>
        public AssetBundleManifest S_AssetBundleManifest
        {
            get
            {
                if (_AssetBundleManifest == null)
                    LoadAssetBundleManifest();
                return _AssetBundleManifest;
            }
        }


        #region  加载 AssetBundleManifest
        /// <summary>
        /// 加载 AssetBundleManifest 资源
        /// </summary>
        private void LoadAssetBundleManifest()
        {
            string platformName = GetAssetBundlePlatformName();
            string manifestPath = string.Format("{0}{1}/{2}", ConstDefine.S_AssetBundleTopPath, platformName, platformName);
            AssetBundle mainAssetBundle = AssetBundle.LoadFromFile(manifestPath);
            if (mainAssetBundle == null)
            {
                Debug.LogError("mainAssetBundle is Null At Path"+ manifestPath);
                return;
            }
            _AssetBundleManifest = mainAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        #endregion



        #region  获取运行时的平台AssetBundle 路径
        /// <summary>
        /// 根据运行的平台获取对应的路径
        /// </summary>
        /// <returns></returns>
        public string GetAssetBundlePlatformName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return ConstDefine.AndroidPlatform;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return ConstDefine.WindowsPlatform;
                case RuntimePlatform.IPhonePlayer:
                    return ConstDefine.IphonePlatform;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return ConstDefine.OSXPlatform;
                default:
                    Debug.LogError("无法处理的平台类型");
                    return null;
            }
        }
        #endregion



    }
}