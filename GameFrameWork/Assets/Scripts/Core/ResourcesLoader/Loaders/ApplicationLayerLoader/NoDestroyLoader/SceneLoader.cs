using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 场景加载器(不同于其他的加载器 这里场景中只有几个单独的挂点和一个场景管理脚本)  在场景被卸载时候需要手动卸载引用
    /// </summary>
    public class SceneLoader : ApplicationLoader_NotDestroy
    {

        #region  加载资源
        public static SceneLoader LoadScene(string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadSceneSync(url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadSceneAsync( url, completeHandler);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }

        #region 异步加载
        /// <summary>
        /// 记载场景资源 
        /// </summary>
        /// <param name="url">场景资源的目录+场景资源名</param>
        /// <param name="sceneDirectory"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static SceneLoader LoadSceneAsync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(FontLoader)));
                return null;
            }

            bool isContainLoaders = false;
            SceneLoader sceneLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<SceneLoader>(url, ref isContainLoaders);
            sceneLoader.m_OnCompleteAct.Add(completeHandler);
            sceneLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）

            sceneLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (sceneLoader.IsCompleted)
                    sceneLoader.OnCompleteLoad(sceneLoader.IsError, sceneLoader.Description, sceneLoader.ResultObj, sceneLoader.IsCompleted);
                return sceneLoader;
            }

            sceneLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(sceneLoader.LoadSceneAssetAsync(url));
            return sceneLoader;
        }

        private IEnumerator LoadSceneAssetAsync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Async, null, true);  //这里的第三个参数必须是true  标识是加载场景 否则会报错
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion

        #region 同步加载
        /// <summary>
        /// 记载场景资源 
        /// </summary>
        /// <param name="url">场景资源的目录+场景资源名</param>
        /// <param name="sceneDirectory"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static SceneLoader LoadSceneSync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(FontLoader)));
                return null;
            }

            bool isContainLoaders = false;
            SceneLoader sceneLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<SceneLoader>(url, ref isContainLoaders);
            sceneLoader.m_OnCompleteAct.Add(completeHandler);

            sceneLoader.AddReference(null, url);
            if (isContainLoaders&& sceneLoader.IsCompleted)
            {
                sceneLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
                sceneLoader.OnCompleteLoad(sceneLoader.IsError, sceneLoader.Description, sceneLoader.ResultObj, sceneLoader.IsCompleted);
                return sceneLoader;
            }
            if (sceneLoader.LoadassetModel == LoadAssetModel.Async)
            {
                sceneLoader.ForceBreakLoaderProcess();
            }
            sceneLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
            sceneLoader.m_LoadAssetCoroutine = null;
            sceneLoader.LoadSceneAssetSync(url);
            return sceneLoader;
        }

        private void LoadSceneAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, true);  //这里的第三个参数必须是true  标识是加载场景 否则会报错
            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }
        #endregion

        #endregion

        #region  卸载场景
        public static void UnLoadScene(string url)
        {
            ResourcesLoaderMgr.DeleteLoader(typeof(SceneLoader), url, true);
        }
        #endregion


        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            if (result != null)
            {
                ResultObj = null;
            }
            else
            {
                if (typeof(AssetBundle) == result.GetType())
                {
                    ResultObj = result as AssetBundle;
                }
                else
                {
                    Debug.LogInfor("加载场景 未知类型 " + result.GetType());
                    ResultObj = result;
                }
            }

            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);
            if (m_BridgeLoader != null)
                ResourcesLoaderMgr.DeleteLoader(typeof(BridgeLoader), m_ResourcesUrl, false);

        }


        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            Debug.LogError("强行结束场景加载 会导致异常");
            if (LoadassetModel != LoadAssetModel.Async)
            {
                Debug.LogError("非异步加载方式不需要强制结束 " + LoadassetModel);
                return;
            }
            if (m_LoadAssetCoroutine != null)
                EventCenter.Instance.StopCoroutine(m_LoadAssetCoroutine);

        }




    }
}