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
        /// <summary>
        /// 外部资源存储的主目录
        /// </summary>
        public static string S_PersistentDataPath {
            get
            {
                return Application.persistentDataPath + "/GameFrame/";
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
                    resourcesPath = Application.dataPath + "/Resources/";
                return resourcesPath;
            }
        }



    }
}