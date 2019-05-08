using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{

    /// <summary>
    /// Shader 加载器 缓存已经加载的Shader (不需要卸载)
    /// </summary>
    public class ShaderLoader : ApplicationLoader_NotDestroy
    {

        #region  加载资源
        public static ShaderLoader LoadAsset(string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAssetSync(url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadAssetAsync(url, completeHandler);
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
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static ShaderLoader LoadAssetAsync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(ShaderLoader)));
                return null;
            }
            bool isContainLoaders = false;
            ShaderLoader shaderLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ShaderLoader>(url, ref isContainLoaders);
            shaderLoader.m_OnCompleteAct.Add(completeHandler);
            shaderLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）

            shaderLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (shaderLoader.IsCompleted)
                    shaderLoader.OnCompleteLoad(shaderLoader.IsError, shaderLoader.Description, shaderLoader.ResultObj, shaderLoader.IsCompleted);
                return shaderLoader;
            }

            shaderLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(shaderLoader.LoadShaderAssetAsync(url));
            return shaderLoader;
        }


        private IEnumerator LoadShaderAssetAsync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Async, null, false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion


        #region 异步加载

        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static ShaderLoader LoadAssetSync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(ShaderLoader)));
                return null;
            }
            bool isContainLoaders = false;
            ShaderLoader shaderLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ShaderLoader>(url, ref isContainLoaders);
            shaderLoader.m_OnCompleteAct.Add(completeHandler);

            shaderLoader.AddReference(null, url);
            if (isContainLoaders && shaderLoader.IsCompleted)
            {
                shaderLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
                shaderLoader.OnCompleteLoad(shaderLoader.IsError, shaderLoader.Description, shaderLoader.ResultObj, shaderLoader.IsCompleted);
                return shaderLoader;
            }

            if (shaderLoader.LoadassetModel == LoadAssetModel.Async)
            {
                shaderLoader.ForceBreakLoaderProcess();
            }
            shaderLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
            shaderLoader.m_LoadAssetCoroutine = null;
            shaderLoader.LoadShaderAssetSync(url);
            return shaderLoader;
        }


        private void LoadShaderAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, false);

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }
        #endregion

        #endregion



        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, iscomplete, process);
            ResultObj = result as Shader;
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