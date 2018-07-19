using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{

    /// <summary>
    /// Shader 加载器 缓存已经加载的Shader (不需要卸载)
    /// </summary>
    public class ShaderLoader : ApplicationLoader_NotDestroy
    {

        #region  加载资源
        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static ShaderLoader LoadAsset(GameObject requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            bool isContainLoaders = false;
            ShaderLoader shaderLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ShaderLoader>(url, ref isContainLoaders);
            shaderLoader.m_OnCompleteAct.Add(completeHandler);

            shaderLoader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (shaderLoader.IsCompleted)
                    shaderLoader.OnCompleteLoad(shaderLoader.IsError, shaderLoader.Description, shaderLoader.ResultObj, shaderLoader.IsCompleted);
                return shaderLoader;
            }


            ApplicationMgr.Instance.StartCoroutine(shaderLoader.LoadShaderAsset(url));
            return shaderLoader;
        }


        private IEnumerator LoadShaderAsset(string url)
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
        //public static void UnLoadAsset(string url, bool isForceDelete=false)
        //{
        //    ShaderLoader shaderLoader = ResourcesLoaderMgr.GetExitLoaderInstance<ShaderLoader>(url);
        //    if (shaderLoader == null)
        //    {
        //        //Debug.LogError("无法获取指定类型的加载器 " + typeof(WWWLoader));
        //        return;
        //    }
        //    shaderLoader.ReduceReference(isForceDelete);
        //}
        #endregion

        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, iscomplete, process);
            ResultObj = result as Shader;
            if (m_BridgeLoader != null)
                ResourcesLoaderMgr.DeleteExitLoaderInstance(typeof(BridgeLoader),m_ResourcesUrl);
        }


    }
}