using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// Texture2D 加载器，与Sprite2D不同,主要用在加载材质球的贴图等
    /// </summary>
    public class Teture2DLoader : ApplicationLoader_Alone
    {


        #region      加载资源

        public static Teture2DLoader LoadTexture2DAsset(Transform requestTarget, string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadTexture2DAssetSync(requestTarget, url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadTexture2DAssetAsync(requestTarget, url, completeHandler);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }


        #region 异步加载

        /// <summary>
        /// 加载Texture2D 图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static Teture2DLoader LoadTexture2DAssetAsync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            Debug.LogError("TODO  Teture2DLoader 还不完善 需要继续测试 ");
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(Teture2DLoader)));
                return null;
            }

            bool isContainLoaders = false;
            Teture2DLoader texture2DLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<Teture2DLoader>(url, ref isContainLoaders);
            texture2DLoader.m_OnCompleteAct.Add(completeHandler);
            texture2DLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）


            texture2DLoader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (texture2DLoader.IsCompleted)
                    texture2DLoader.OnCompleteLoad(texture2DLoader.IsError, texture2DLoader.Description, texture2DLoader.ResultObj, texture2DLoader.IsCompleted);
                return texture2DLoader;
            }


            texture2DLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(texture2DLoader.LoadTexture2DAssetAsync(url));
            return texture2DLoader;
        }


        private IEnumerator LoadTexture2DAssetAsync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Async, null, false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion


        #region 同步加载

        /// <summary>
        /// 加载Texture2D 图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static Teture2DLoader LoadTexture2DAssetSync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            Debug.LogError("TODO  Teture2DLoader 还不完善 需要继续测试 ");
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(Teture2DLoader)));
                return null;
            }

            bool isContainLoaders = false;
            Teture2DLoader texture2DLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<Teture2DLoader>(url, ref isContainLoaders);
            texture2DLoader.m_OnCompleteAct.Add(completeHandler);

            texture2DLoader.AddReference(requestTarget, url);
            if (isContainLoaders && texture2DLoader.IsCompleted)
            {
                texture2DLoader.LoadassetModel = LoadAssetModel.Sync;
                texture2DLoader.OnCompleteLoad(texture2DLoader.IsError, texture2DLoader.Description, texture2DLoader.ResultObj, texture2DLoader.IsCompleted);
                return texture2DLoader;
            }

            if (texture2DLoader.LoadassetModel == LoadAssetModel.Async)
            {
                texture2DLoader.ForceBreakLoaderProcess();
            }
            texture2DLoader.LoadassetModel = LoadAssetModel.Sync;
            texture2DLoader.m_LoadAssetCoroutine = null;
            texture2DLoader.LoadTexture2DAssetSync(url);
            return texture2DLoader;
        }

        private void LoadTexture2DAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, false);
            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }
        #endregion

        #endregion

        #region 卸载资源
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestTarget"></param>
        public static void UnLoadAsset(string url, object requestTarget = null)
        {
            Teture2DLoader texture2DLoader = ResourcesLoaderMgr.GetExitLoaderInstance<Teture2DLoader>(url);
            if (texture2DLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(Teture2DLoader));
                return;
            }
            if (requestTarget == null)
                requestTarget = texture2DLoader.m_RequesterTarget;

            texture2DLoader.ReduceReference(texture2DLoader, false);
        }
        #endregion


        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            //  Debug.LogInfor("AAAAAAA>>>> " + (result as GameObject).GetComponent<SpriteRenderer>());
            Type resultType = result.GetType();
            //if (resultType == typeof(UnityEngine.GameObject))
            //    ResultObj = (result as GameObject).GetComponent<SpriteRenderer>().sprite;
            //else if (resultType == typeof(UnityEngine.Sprite))
            //    ResultObj = result as Sprite;
            //else
            Debug.LogError("没有定义的类型" + resultType);

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
                EventCenter.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }



    }
}