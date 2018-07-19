using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 预制体加载器 (加载预制体返回预制体资源GameObject)
    /// </summary>
    public class PrefabLoader : ApplicationLoader_Alone
    {
        #region      加载资源
        /// <summary>
        /// 加载精灵图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static PrefabLoader LoadAsset(GameObject requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            bool isContainLoaders = false;
            PrefabLoader prefabloader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<PrefabLoader>(url, ref isContainLoaders);
            prefabloader.m_OnCompleteAct.Add(completeHandler);


            prefabloader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (prefabloader.IsCompleted)
                    prefabloader.OnCompleteLoad(prefabloader.IsError, prefabloader.Description, prefabloader.ResultObj, prefabloader.IsCompleted);
                return prefabloader;
            }


            ApplicationMgr.Instance.StartCoroutine(prefabloader.LoadPrefabAsset(url));
            return prefabloader;
        }


        private IEnumerator LoadPrefabAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null);
            if (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
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

            prefabloader.ReduceReference(false);
        }
        #endregion


        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            ResultObj = result as GameObject;
            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);

        }


    }
}