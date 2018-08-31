using GameFrameWork.HotUpdate;
using GameFrameWork.ResourcesLoader;
using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFrameWork
{

    /// <summary>
    /// 整个应用程序的主管理器 负责管理其他模块
    /// </summary>
    [RequireComponent(typeof(ApplicationConfig))]
    public class ApplicationMgr : Singleton_Mono_NotDestroy<ApplicationMgr>
    {

        private ApplicationConfig m_ApplicationConfig;

        #region Frame

        protected override void Awake()
        {
#if UNITY_EDITOR
            Debug.Log(" Application.dataPath=" + Application.dataPath);
            Debug.Log(" Application.persistentDataPath=" + Application.persistentDataPath);
            Debug.Log(" Application.temporaryCachePath=" + Application.temporaryCachePath);
            Debug.Log(" Application.streamingAssetsPath=" + Application.streamingAssetsPath);

#endif
            m_ApplicationConfig = transform.GetAddComponent<ApplicationConfig>();

            Debug.S_LogLevel = m_ApplicationConfig.m_LogLevel;  //设置日志输出级别

            TimeTickUtility.Instance.StartUpTimer();
            base.Awake();
            Debug.LogInfor("ApplicationMgr Start...");

            //TODO 其他一些操作
            AppSceneManager.Instance.OnApplicationStart();
        }

        private void Start()
        {
            UIManager.Instance.CreateUI<UIAssetUpdateView>(UIResourcesPath.UIAssetUpdateViewPath, UIManager.Instance.PageParentTrans, (obj) => {
                UIManager.Instance.OpenPage(obj);
            }, false, true, "");

        }

        void Update()
        {
            TimeTickUtility.Instance.Tick();  //启动计时器
        }

        private void FixedUpdate()
        {
            ResourcesLoaderMgr.Tick();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
#if UNITY_EDITOR
            if (AssetBundleMgr.Instance.m_MainAssetBundle)
                AssetBundleMgr.Instance.m_MainAssetBundle.Unload(true); //卸载所有的 AssetBundle 资源
#endif
        }

        private void OnApplicationQuit()
        {
#if !UNITY_EDITOR
            if (AssetBundleMgr.Instance.m_MainAssetBundle)
                AssetBundleMgr.Instance.m_MainAssetBundle.Unload(true); //卸载所有的 AssetBundle 资源
#endif
        }

        #endregion

    }
}