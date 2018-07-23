﻿using System;
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
        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static FontLoader LoadAsset( string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            bool isContainLoaders = false;
            FontLoader fontLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<FontLoader>(url, ref isContainLoaders);
            fontLoader.m_OnCompleteAct.Add(completeHandler);

            fontLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (fontLoader.IsCompleted)
                    fontLoader.OnCompleteLoad(fontLoader.IsError, fontLoader.Description, fontLoader.ResultObj, fontLoader.IsCompleted);
                return fontLoader;
            }


            ApplicationMgr.Instance.StartCoroutine(fontLoader.LoadFontAsset(url));
            return fontLoader;
        }


        private IEnumerator LoadFontAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null);
            if (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion


        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, iscomplete, process);
            ResultObj = result as Font;
            if (m_BridgeLoader != null)
                ResourcesLoaderMgr.DeleteExitLoaderInstance(typeof(BridgeLoader), m_ResourcesUrl);
        }





        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            ApplicationMgr.Instance.StopCoroutine(LoadFontAsset(m_ResourcesUrl));

        }


    }
}