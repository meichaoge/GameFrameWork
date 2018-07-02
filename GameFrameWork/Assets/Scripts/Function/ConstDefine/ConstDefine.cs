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

        private static string _PersistentDataPath;
        /// <summary>
        /// 外部资源存储的主目录
        /// </summary>
        public static string S_PersistentDataPath {
            get
            {
                if(string.IsNullOrEmpty(_PersistentDataPath))
                    _PersistentDataPath= Application.persistentDataPath + "/GameFrame/";
                return _PersistentDataPath;
            }
        }


        private static string _AssetBundleTopPath;
        /// <summary>
        /// 打包后加载AssetBundle 资源的路径
        /// </summary>
        public static string S_AssetBundleTopPath
        {
            get
            {
                if (string.IsNullOrEmpty(_AssetBundleTopPath))
                    _AssetBundleTopPath = S_PersistentDataPath+"AssetBundleResources/";
                return _AssetBundleTopPath;
            }
        }

        /// <summary>
        /// AssetBundle 资源的扩展名(在生成资源的时候加的)
        /// </summary>
        public const string AssetBundleExtensionName = ".unity3d";
        #region 各个平台名称
        public const string AndroidPlatform = "Android";
        public const string WindowsPlatform = "Window";
        public const string IphonePlatform = "Iphone";
        public const string OSXPlatform = "OS";
        #endregion


        private static string resourcesPath;
        /// <summary>
        /// Resources 目录的文件路径
        /// </summary>
        public static string S_ResourcesPath
        {
            get
            {
                if (string.IsNullOrEmpty(resourcesPath))
                    resourcesPath = Application.dataPath + "/Resources/";
                return resourcesPath;
            }
        }



    }
}