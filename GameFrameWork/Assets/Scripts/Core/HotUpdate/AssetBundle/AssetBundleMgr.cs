using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GameFrameWork.HotUpdate
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

        /// <summary>
        /// 所有本地的AssetBudle 资源 ,Key为当前资源的相对路径 value为当前资源的依赖资源
        /// </summary>
        private Dictionary<string, AssetBundleAssetInfor> m_AllAssetBundleDataRecordDic = new Dictionary<string, AssetBundleAssetInfor>();


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
        /// 根据指定挡圈生成资源
        /// </summary>
        /// <returns></returns>
        public string GetHotAssetBuildPlatformName()
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


        #region  获取本地所有的AssetBundle 资源 /检测指定路径的AssetBundle 是否存在

        /// <summary>
        /// 在进行本地资源更新时候记录本地所有的AssetBundle 资源，方便加载的时候区分是否是AssetBundle 资源
        /// </summary>
        /// <param name="assetRecordInfor">更新获取的服务器资源版本信息</param>
        public void  RecordAllLocalAssetBundleAsset(HotAssetBaseRecordInfor assetRecordInfor)
        {
            foreach (var item in assetRecordInfor.AllAssetRecordsDic)
            {
                m_AllAssetBundleDataRecordDic.Add(item.Key, new AssetBundleAssetInfor());
            }
        }

        /// <summary>
        /// 记录当前文件件包含的AssetBundle 
        /// </summary>
        /// <param name="allContainAsset"></param>
        public void RecordFolderAssetBundleContainAsset(string folderAssetBundleKey,IEnumerable<string> allContainAsset)
        {
            if (m_AllAssetBundleDataRecordDic.ContainsKey(folderAssetBundleKey) == false)
            {
                Debug.LogError("RecordFolderAssetBundleContainAsset  Fail,Not Exit " + folderAssetBundleKey);
                return;
            }
            if (m_AllAssetBundleDataRecordDic[folderAssetBundleKey].IsInitialed)
                return;
            m_AllAssetBundleDataRecordDic[folderAssetBundleKey].IsInitialed = true;
            foreach (var item in allContainAsset)
            {
                m_AllAssetBundleDataRecordDic[folderAssetBundleKey] = new AssetBundleAssetInfor();
            }
        }

        /// <summary>
        /// 检车给定的Key AssetBundle 资源是否被记录了包含的信息
        /// </summary>
        /// <param name="folderAssetBundleKey"></param>
        /// <returns></returns>
        public bool CheckIfNeedRecord(string folderAssetBundleKey)
        {
            if (m_AllAssetBundleDataRecordDic.ContainsKey(folderAssetBundleKey) == false)
                return false;

            if (m_AllAssetBundleDataRecordDic[folderAssetBundleKey].IsInitialed)
                return false;
            return true;
        }

        /// <summary>
        /// 检测当前AssetBundle 资源中是否包含指定的资源
        /// </summary>
        /// <param name="folderAssetBundleKey"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool CheckIfContainAsset(string folderAssetBundleKey,string fileName)
        {
            AssetBundleAssetInfor assetBundleInfor = null;
            if(m_AllAssetBundleDataRecordDic.TryGetValue(folderAssetBundleKey,out assetBundleInfor))
            {
                if (assetBundleInfor.ContainAsset.Contains(fileName))
                    return true;
                return false;
            }
            return false;
        }


        /// <summary>
        /// 检测当前给定的参数路径是否是AssetBundle 资源，并返回这个AssetBundle 资源的路径
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="exitState"></param>
        /// <returns></returns>
        public string  CheckIfAssetBundleAsset(string assetRelativePath,out ResourcesLoader.AssetBundleExitState exitState)
        {
            if(System.IO.Path.GetExtension(assetRelativePath)!= ConstDefine.AssetBundleExtensionName)
            {
                assetRelativePath = assetRelativePath + ConstDefine.AssetBundleExtensionName;
            }//组合上扩展名

            if(m_AllAssetBundleDataRecordDic.ContainsKey(assetRelativePath))
            {
                exitState = ResourcesLoader.AssetBundleExitState.SinglePrefab;
                return assetRelativePath;
            }

            string shortDirectoryName = assetRelativePath.GetPathParentDirectoryName().ToLower(); //当前文件路径父级目录名
            string parentAssetPath = string.Format("{0}/{1}", System.IO.Path.GetDirectoryName(assetRelativePath), shortDirectoryName);
            if (m_AllAssetBundleDataRecordDic.ContainsKey(parentAssetPath))
            {
                if(m_AllAssetBundleDataRecordDic[parentAssetPath].IsInitialed)
                {
                    if(m_AllAssetBundleDataRecordDic[parentAssetPath].ContainAsset.Contains(System.IO.Path.GetFileNameWithoutExtension(assetRelativePath))==false)
                    {
                        exitState = ResourcesLoader.AssetBundleExitState.None;
                        return "";
                    } //说明当前AssetBundle 中不包含这个资源
                }//当前资源已经初始化了
                exitState = ResourcesLoader.AssetBundleExitState.FolderPrefab;
                return parentAssetPath;
            }//按照一个文件夹打包资源的

            exitState = ResourcesLoader.AssetBundleExitState.None;
            return "";
        }

        #endregion

    }
}