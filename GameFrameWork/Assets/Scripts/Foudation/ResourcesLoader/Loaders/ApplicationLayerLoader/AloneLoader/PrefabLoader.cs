using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 预制体加载器 (加载预制体返回预制体资源GameObject)
    /// </summary>
    public class PrefabLoader : ApplicationLoader_Alone
    {
        #region      加载资源

        #region   加载资源
        public static PrefabLoader LoadAsset(Transform requestTarget, string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAssetSync(requestTarget, url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadAssetAsync(requestTarget, url, completeHandler);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }
        #endregion

        #region 异步加载方式
        /// <summary>
        /// 加载精灵图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static PrefabLoader LoadAssetAsync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(PrefabLoader)));
                if (completeHandler != null)
                    completeHandler(null);
                return null;
            }

            bool isContainLoaders = false;
            PrefabLoader prefabloader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<PrefabLoader>(url, ref isContainLoaders);
            prefabloader.m_OnCompleteAct.Add(completeHandler);
            prefabloader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）


            prefabloader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (prefabloader.IsCompleted)
                    prefabloader.OnCompleteLoad(prefabloader.IsError, prefabloader.Description, prefabloader.ResultObj, prefabloader.IsCompleted);
                return prefabloader;
            }


            prefabloader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(prefabloader.LoadPrefabAssetAsync(url));
            return prefabloader;
        }


        private IEnumerator LoadPrefabAssetAsync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Async, null, false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            //yield return new WaitForSeconds(1);

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion

        #region 同步加载
        /// <summary>
        /// (同步)加载预制体
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static PrefabLoader LoadAssetSync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(PrefabLoader)));
                if (completeHandler != null)
                    completeHandler(null);
                return null;
            }

            bool isContainLoaders = false;
            PrefabLoader prefabloader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<PrefabLoader>(url, ref isContainLoaders);
            prefabloader.m_OnCompleteAct.Add(completeHandler);

            prefabloader.AddReference(requestTarget, url);
            if (isContainLoaders && prefabloader.IsCompleted)
            {
                prefabloader.LoadassetModel = LoadAssetModel.Sync;
                prefabloader.OnCompleteLoad(prefabloader.IsError, prefabloader.Description, prefabloader.ResultObj, prefabloader.IsCompleted);
                return prefabloader;
            }

            if (prefabloader.LoadassetModel == LoadAssetModel.Async)
            {
                prefabloader.ForceBreakLoaderProcess();
            }
            prefabloader.LoadassetModel = LoadAssetModel.Sync;
            prefabloader.m_LoadAssetCoroutine = null;
            prefabloader.LoadPrefabAssetSync(url);
            return prefabloader;
        }


        private void LoadPrefabAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, false);
            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }

        #endregion

        #endregion

        #region 卸载资源
        public static void UnLoadAsset(string url, object requestTarget = null)
        {
            PrefabLoader prefabloader = ResourcesLoaderMgr.GetExitLoaderInstance<PrefabLoader>(url);
            if (prefabloader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(PrefabLoader));
                return;
            }
            if (requestTarget == null)
                requestTarget = prefabloader.m_RequesterTarget;

            prefabloader.ReduceReference(prefabloader, false);
        }
        #endregion


        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            ResultObj = result as GameObject;
            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);
        }


        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if (LoadassetModel != LoadAssetModel.Async)
            {
                Debug.LogError("非异步加载方式不需要强制结束 " + LoadassetModel);
                return;
            }
            if (m_LoadAssetCoroutine != null)
                EventCenter.Instance.StopCoroutine(m_LoadAssetCoroutine);            //(LoadPrefabAsset(m_ResourcesUrl));
        }

    }
}