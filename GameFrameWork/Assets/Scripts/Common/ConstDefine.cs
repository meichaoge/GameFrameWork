using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 包含常用的路径定义
    /// </summary>
    public class ConstDefine : Singleton_Static<ConstDefine>
    {

        #region Application. PersistentData/ Application. Resources  路径配置

        private static string _PersistentDataPath;
        /// <summary>
        /// 外部资源存储的主目录
        /// </summary>
        public static string S_PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_PersistentDataPath))
                    _PersistentDataPath = Application.persistentDataPath + "/GameFrame/";
                return _PersistentDataPath;
            }
        }

        private static string resourcesPath;
        /// <summary>
        /// Resources 目录的文件路径
        /// </summary>
        public static string S_ResourcesPath
        {
            get
            {
                if (string.IsNullOrEmpty(resourcesPath))
                    resourcesPath = string.Format("{0}/{1}/", Application.dataPath, S_ResourcesName);
                return resourcesPath;
            }
        }

        #endregion


        #region  AssetBundle 相关

        private static string _AssetBundleTopPath;
        /// <summary>
        /// 打包后加载AssetBundle 资源的路径
        /// </summary>
        public static string S_AssetBundleTopPath
        {
            get
            {
                if (string.IsNullOrEmpty(_AssetBundleTopPath))
                    _AssetBundleTopPath = S_PersistentDataPath + "AssetBundleResources/";
                return _AssetBundleTopPath;
            }
        }

        private static string StreamingAssetPath = "";
        /// <summary>
        /// StreamingAssetpath
        /// </summary>
        public static string S_StreamingAssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(StreamingAssetPath))
                    StreamingAssetPath = Application.streamingAssetsPath;
                return StreamingAssetPath;
            }
        }


        /// <summary>
        /// AssetBundle 资源的扩展名(在生成资源的时候加的)
        /// </summary>
        public const string AssetBundleExtensionName = ".unity3d";

        /// <summary>
        /// AssetBundleManifest 的名字 
        /// </summary>
        public static string S_AssetBundleManifest { get { return "AssetBundleManifest"; } }

        public static string S_AssetBundleBuildRecordConfigureName { get { return "_AssetBundleInfor.txt"; } }
        #endregion

        #region 各个平台名称
        public const string AndroidPlatform = "Android";
        public const string WindowsPlatform = "Window";
        public const string IphonePlatform = "Iphone";
        public const string OSXPlatform = "OS";
        #endregion

        #region  应用版本信息

        /// <summary>
        /// 框架的版本信息
        /// </summary>
        public static string S_ApplicationVersionLogPath
        {
            get
            {
                return "Configure/ApplicationVersion_config";
            }
        }
        #endregion

        #region  网络下载的资源
        private static string _DownLoadAssetTopPath;
        /// <summary>
        /// 使用下载器下载的 资源的默认路径
        /// </summary>
        public static string S_DownLoadAssetTopPath
        {
            get
            {
                if (string.IsNullOrEmpty(_DownLoadAssetTopPath))
                    _DownLoadAssetTopPath = S_PersistentDataPath + "DownloadAsset/";
                return _DownLoadAssetTopPath;
            }
        }
        #endregion

        /// <summary>
        /// Asset的名字
        /// </summary>
        public static string S_AssetName { get { return "Assets"; } }
        /// <summary>
        /// Asset的名字
        /// </summary>
        public static string S_ResourcesName { get { return "Resources"; } }

        /// <summary>
        ///默认的材质球名称
        /// </summary>
        public const string S_DefaultMaterialName = "Default-Material (Instance)";

        /// <summary>
        /// 所有的配置文件存放的目录名(相对于Resources)
        /// </summary>
        public static string S_ConfigurePathName { get { return "Configure"; } }


        /// <summary>
        /// 多语言图片资源在Assets顶层目录
        /// </summary>
        public static string S_MultLanguageSpriteTopPath = "Assets/Art/UI/LocalizationUI/";
      
        /// <summary>
        /// 对于存在多个版本的资源 设置Sprite Tage时候需要过滤掉这个，目录名
        /// 获取多语言资源时候也需要
        /// </summary>
        public static string S_UILocalizationPathFileName = "LocalizationUI/";
        /// <summary>
        /// UI多语言配置信息
        /// </summary>
        public static string S_MultLanguageConfigPathName { get { return "Configure/Localization"; } }
        /// <summary>
        /// UI多语言动态配置
        /// </summary>
        public static string S_UIDynamincConfigPathName { get { return "UI_Dynamic"; } }
        /// <summary>
        /// UI多语言静态配置
        /// </summary>
        public static string S_UIStaticConfigPathName { get { return "UI_Static"; } }

    }
}