using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// Font 字体加载器
    /// </summary>
    public class FontLoader : ApplicationLoader_NotDestroy
    {

        #region  加载资源

        public static FontLoader LoadFontAsset(string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadFontAssetSync(url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadFontAssetAsync(url, completeHandler);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }

        #region 异步加载

        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static FontLoader LoadFontAssetAsync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(FontLoader)));
                return null;
            }
            bool isContainLoaders = false;
            FontLoader fontLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<FontLoader>(url, ref isContainLoaders);
            fontLoader.m_OnCompleteAct.Add(completeHandler);
            fontLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）

            fontLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (fontLoader.IsCompleted)
                    fontLoader.OnCompleteLoad(fontLoader.IsError, fontLoader.Description, fontLoader.ResultObj, fontLoader.IsCompleted);
                return fontLoader;
            }


            fontLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(fontLoader.LoadFontAssetAsync(url));
            return fontLoader;
        }


        private IEnumerator LoadFontAssetAsync(string url)
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
        /// (同步)生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static FontLoader LoadFontAssetSync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(FontLoader)));
                return null;
            }
            bool isContainLoaders = false;
            FontLoader fontLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<FontLoader>(url, ref isContainLoaders);
            fontLoader.m_OnCompleteAct.Add(completeHandler);

            fontLoader.AddReference(null, url);
            if (isContainLoaders && fontLoader.IsCompleted)
            {
                fontLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
                fontLoader.OnCompleteLoad(fontLoader.IsError, fontLoader.Description, fontLoader.ResultObj, fontLoader.IsCompleted);
                return fontLoader;
            }
            if (fontLoader.LoadassetModel == LoadAssetModel.Async)
            {
                fontLoader.ForceBreakLoaderProcess();
            }
            fontLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
            fontLoader.m_LoadAssetCoroutine = null;
            fontLoader.LoadFontAssetSync(url);
            return fontLoader;
        }


        private void LoadFontAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, false);

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }
        #endregion

        #endregion

        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            ResultObj = result as Font;
            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);

            if (m_BridgeLoader != null)
                ResourcesLoaderMgr.DeleteLoader(typeof(BridgeLoader), m_ResourcesUrl, false);
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