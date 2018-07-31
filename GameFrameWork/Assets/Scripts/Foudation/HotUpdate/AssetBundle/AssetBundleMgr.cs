using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

        private AssetBundle _mainAssetBundle = null;
        public AssetBundle m_MainAssetBundle
        {
            get { return _mainAssetBundle; }
        }//主AssetBundle 资源


        #region  加载 AssetBundleManifest
        /// <summary>
        /// 加载 AssetBundleManifest 资源
        /// </summary>
        private void LoadAssetBundleManifest()
        {
            string platformName = GetAssetBundlePlatformName();
            string manifestPath = string.Format("{0}{1}/{2}", ConstDefine.S_AssetBundleTopPath, platformName, platformName);
            _mainAssetBundle = AssetBundle.LoadFromFile(manifestPath);
            if (_mainAssetBundle == null)
            {
                Debug.LogError("mainAssetBundle is Null At Path" + manifestPath);
                return;
            }
            _AssetBundleManifest = _mainAssetBundle.LoadAsset<AssetBundleManifest>(ConstDefine.S_AssetBundleManifest);
        }
        #endregion


        #region  获取运行时的平台AssetBundle 路径
        /// <summary>
        /// 根据运行的平台获取对应的路径
        /// </summary>
        /// <returns></returns>
        public string GetAssetBundlePlatformName()
        {
            return GetHotAssetBuildPlatformName(Application.platform);
        }

        /// <summary>
        /// 根据指定平台生成资源
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <returns></returns>
        public string GetHotAssetBuildPlatformName(RuntimePlatform runtimePlatform)
        {
            switch (runtimePlatform)
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
                    Debug.LogError("无法处理的平台类型" + runtimePlatform);
                    return null;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 根据指定平台生成资源
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <returns></returns>
        public string GetHotAssetBuildPlatformName(BuildTarget buildPlatform)
        {
            return GetHotAssetBuildPlatformName(BuildTarget2RuntimePlatform(buildPlatform));
        }

        /// <summary>
        /// 打包平台到运行时平台的转换
        /// </summary>
        /// <param name="buildPlatform"></param>
        /// <returns></returns>
        private RuntimePlatform BuildTarget2RuntimePlatform(BuildTarget buildPlatform)
        {
            switch (buildPlatform)
            {
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                    return RuntimePlatform.OSXPlayer;

                case BuildTarget.iOS:
                    return RuntimePlatform.IPhonePlayer;

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return RuntimePlatform.WindowsPlayer;

                case BuildTarget.Android:
                    return RuntimePlatform.Android;
                default:
                    Debug.LogError("没有指定配置的类型 " + buildPlatform);
                    return RuntimePlatform.Android;
            }
        }



#endif

        #endregion


        #region  检测本地AssetBundle 资源版本并更新资源接口

        #endregion


    }
}