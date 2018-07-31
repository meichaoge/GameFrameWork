using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 包含常用的路径定义
    /// </summary>
    public partial class ConstDefine : Singleton_Static<ConstDefine>
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





    }
}