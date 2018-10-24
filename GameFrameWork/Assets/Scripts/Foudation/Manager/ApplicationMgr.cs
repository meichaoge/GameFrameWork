using GameFrameWork.HotUpdate;
using GameFrameWork.ResourcesLoader;
using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using GameFrameWork.EditorExpand;
#endif


namespace GameFrameWork
{

    /// <summary>
    /// 整个应用程序的主管理器 负责管理其他模块
    /// </summary>
    [RequireComponent(typeof(ApplicationConfig))]
    public class ApplicationMgr : Singleton_Mono_NotDestroy<ApplicationMgr>
    {

        private ApplicationConfig m_ApplicationConfig;

        private bool m_IsUnloadedAllResources = false;

        #region Frame

        protected override void Awake()
        {
#if UNITY_EDITOR
            Debug.Log(" Application.dataPath=" + Application.dataPath);
            Debug.Log(" Application.persistentDataPath=" + Application.persistentDataPath);
            Debug.Log(" Application.temporaryCachePath=" + Application.temporaryCachePath);
            Debug.Log(" Application.streamingAssetsPath=" + Application.streamingAssetsPath);
            GameObject goDebug = new GameObject("Editor_ShowLoaderInfor");
            goDebug.GetAddComponent<DebugShowLoaderInfor>();

            Resources.UnloadUnusedAssets();
#endif
            m_ApplicationConfig = transform.GetAddComponent<ApplicationConfig>();

            TimeTickUtility.Instance.StartUpTimer();
            PlayerPrefsMgr.GetInstance();

            base.Awake();
      
        }



        private void Start()
        {
            StartCoroutine(LoadAppConfig(() =>
            {
                UIManager.Instance.OpenPage(UIViewReference.Instance.UiAssetUpdateView, UIParameterArgs.Create());
            }));
        }

        private void OnDisable()
        {
            if (m_IsUnloadedAllResources) return;
            m_IsUnloadedAllResources = true;
            if (AssetBundleMgr.Instance.m_MainAssetBundle)
                AssetBundleMgr.Instance.m_MainAssetBundle.Unload(true); //卸载所有的 AssetBundle 资源
            Resources.UnloadUnusedAssets();
        }

        private void Update()
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
            if (m_IsUnloadedAllResources) return;
            m_IsUnloadedAllResources = true;
#if !UNITY_EDITOR
            if (AssetBundleMgr.Instance.m_MainAssetBundle)
                AssetBundleMgr.Instance.m_MainAssetBundle.Unload(true); //卸载所有的 AssetBundle 资源
#endif
        }

        #endregion

        /// <summary>
        /// 加载应用配置信息
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadAppConfig(System.Action onCompleteAct)
        {
            if (Time.realtimeSinceStartup - TimeTickUtility.S_TimeStartUp > 2f)
            {
                Debug.LogError("LoadAppConfig Fail,Load Configure Not Ready");
                yield break;
            }
            if (AppConfigSetting.Instance.IsEnable == false)
                yield return null;

            Debug.LogInfor("ApplicationMgr Start...");
            Debug.S_LogLevel = AppConfigSetting.Instance.LogLevelInf;  //设置日志输出级别

            //TODO 其他一些操作
            AppSceneManager.Instance.OnApplicationStart();

            if (onCompleteAct != null)
                onCompleteAct.Invoke();
        }



    }
}