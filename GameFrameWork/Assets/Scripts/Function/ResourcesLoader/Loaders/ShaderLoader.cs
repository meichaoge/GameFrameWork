using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{

    public delegate void CompleteShaderLoaderHandler(bool isError, Shader data);
    /// <summary>
    /// Shader 加载器 缓存已经加载的Shader
    /// </summary>
    public class ShaderLoader : BaseAbstracResourceLoader
    {

        public Shader ResultShader { get { return ResultObj as Shader; } }

        /// <summary>
        /// 加载完成的回调
        /// </summary>
        public readonly List<CompleteShaderLoaderHandler> m_AllCompleteLoader = new List<CompleteShaderLoaderHandler>();


        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static ShaderLoader LoadAsset(string url, CompleteShaderLoaderHandler completeHandler, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        {
            bool isContainLoaders = false;
            Dictionary<string, BaseAbstracResourceLoader> resultLoaders = ResourcesLoaderMgr.GetLoaderOfType<ByteLoader>(ref isContainLoaders);
            ShaderLoader shaderLoader = null;
            foreach (var item in resultLoaders)
            {
                if (item.Key == url)
                {
                    shaderLoader = item.Value as ShaderLoader;
                    break;
                }
            }

            if (shaderLoader == null)
            {
                shaderLoader = new ShaderLoader();
                shaderLoader.m_ResourcesUrl = url;
                resultLoaders.Add(url, shaderLoader);
                shaderLoader.m_AllCompleteLoader.Add(completeHandler);
                shaderLoader.LoadShader(url, loadAssetPath);
            }
            else
            {
                shaderLoader.AddReference();
                if (completeHandler != null)
                    completeHandler(shaderLoader.IsError, shaderLoader.ResultShader);
            }
            return shaderLoader;

        }


        protected virtual void LoadShader(string url, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        {
            if (ResourcesLoaderMgr.GetAssetPathOfLoadAssetPath(ref url, loadAssetPath, false) == PathResultEnum.Invalid)
            {
                OnCompleteLoad(true, string.Format("Path is Invalidate {0}", url), null);
                return;
            }
            Debug.Log("AA " + System.IO.Path.GetFileName(url));
            ResultObj = Shader.Find(System.IO.Path.GetFileName(url));
            if (ResultObj == null)
            {
                Debug.LogError("加载失败");
                OnCompleteLoad(true, string.Format("Path is Invalidate {0}", url), null);
                return;
            }

            AddReference();
            Debug.LogInfor("LoadShader Success");
            OnCompleteLoad(false, string.Format("CompleteLoad: {0}", url), ResultObj);
        }

        /// <summary>
        ///  卸载资源
        /// </summary>
        /// <param name="url"></param>
        public static void UnLoadAsset(string url)
        {
            bool isContainLoaders = false;
            Dictionary<string, BaseAbstracResourceLoader> resultLoaders = ResourcesLoaderMgr.GetLoaderOfType<ByteLoader>(ref isContainLoaders);
            if (isContainLoaders == false)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }

            ShaderLoader shaderLoader = null;
            ResourcesLoaderMgr.GetLoaderOfTypeAndUrl<ShaderLoader>(ref shaderLoader, url, resultLoaders, null);
            if (shaderLoader == null)
            {
                //Debug.LogError("UnLoadAsset Fail  ,无法找到指定Url 的加载器 : " + url);
                return;
            }
            shaderLoader.ReduceReference();
            if (shaderLoader.ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<ShaderLoader>(url, false);
            }//引用计数为0时候开始回收资源
        }



        public override void Dispose()
        {
            m_AllCompleteLoader.Clear();
        }

        protected override void OnCompleteLoad(bool isError, string description, object result, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, process);
            for (int dex=0;dex< m_AllCompleteLoader.Count;++dex)
            {
                m_AllCompleteLoader[dex](IsError,ResultShader);
            }
        }
    }
}