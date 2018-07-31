using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 仅仅加载Resources资源目录下的资源
    /// </summary>
    public class ResourcesLoader : BaseAbstracResourceLoader
    {

        #region 加载资源

        /// <summary>
        /// 加载Resources 资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onCompleteAct"></param>
        ///  <param name="isloadScene"> 如果加载的是场景 则这里必须填true ,否则false</param>
        /// <returns></returns>
        public static ResourcesLoader LoadResourcesAsset(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct,bool isloadScene = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(WWWLoader)));
                return null;
            }
            bool isLoaderExit = false;
            ResourcesLoader resourcesLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ResourcesLoader>(url, ref isLoaderExit);
            resourcesLoader.m_OnCompleteAct.Add(onCompleteAct);

            if (isLoaderExit)
            {
                if (resourcesLoader.IsCompleted)
                    resourcesLoader.OnCompleteLoad(resourcesLoader.IsError, resourcesLoader.Description, resourcesLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return resourcesLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            resourcesLoader. m_LoadAssetCoroutine = ApplicationMgr.Instance.StartCoroutine(resourcesLoader.LoadAssetAsync(url, resourcesLoader));
            return resourcesLoader;
        }

        /// <summary>
        /// 异步加载Resources 资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourcesLoader"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetAsync(string url, ResourcesLoader resourcesLoader)
        {
            m_ResourcesUrl = url;
            ResourceRequest resourcesRequest = Resources.LoadAsync(url);
            while (resourcesRequest.isDone == false)
            {
                Process = resourcesRequest.progress;
                yield return null;
            }

            if (resourcesRequest.asset == null)
                resourcesLoader.OnCompleteLoad(true, string.Format("[ResourcesLoader] Load Resource Asset Fail,Not Exit {0}", url), null, true);
            else
                resourcesLoader.OnCompleteLoad(false, string.Format("[ResourcesLoader]  Load Resource Asset Success: {0}", url), resourcesRequest.asset,  true);

            yield break;
        }

        /// <summary>
        /// 同步加载Resources 资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourcesLoader"></param>
        private void LoadAssetSync(string url, ResourcesLoader resourcesLoader)
        {
            m_ResourcesUrl = url;
            ResultObj = Resources.Load(url);
            if (ResultObj == null)
                resourcesLoader.OnCompleteLoad(true, string.Format("[ResourcesLoader]  Load Resource Asset Fail,Not Exit {0} ", url), null, true);
            else
                resourcesLoader.OnCompleteLoad(false, string.Format("[ResourcesLoader]  Load Resource Asset Success: {0}", url), ResultObj, true);
        }
        #endregion

        #region 资源卸载
        public static void UnLoadAsset(string url, bool isForceDelete=false)
        {
            ResourcesLoader resourcesLoader = ResourcesLoaderMgr.GetExitLoaderInstance<ResourcesLoader>(url);
            if (resourcesLoader == null) return;
            resourcesLoader.ReduceReference(resourcesLoader, isForceDelete);
        }
        #endregion  


        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if (m_LoadAssetCoroutine != null)
                ApplicationMgr.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }


        public override void Dispose()
        {
            base.Dispose();
            Resources.UnloadAsset(ResultObj as UnityEngine.Object);
        }

    }
}