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
        /// <summary>
        /// 加载精灵图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static PrefabLoader LoadAsset(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}" ,typeof(PrefabLoader)));
                if (completeHandler != null)
                    completeHandler(null);
                return null;
            }

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


            prefabloader. m_LoadAssetCoroutine = ApplicationMgr.Instance.StartCoroutine(prefabloader.LoadPrefabAsset(url));
            return prefabloader;
        }


        private IEnumerator LoadPrefabAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null,false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            yield return new WaitForSeconds(1);

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
            if(m_LoadAssetCoroutine!=null)
            ApplicationMgr.Instance.StopCoroutine(m_LoadAssetCoroutine);            //(LoadPrefabAsset(m_ResourcesUrl));
        }

    }
}