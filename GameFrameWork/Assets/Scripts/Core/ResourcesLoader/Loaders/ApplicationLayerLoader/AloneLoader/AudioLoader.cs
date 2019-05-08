using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 加载音效资源的加载器
    /// </summary>
    public class AudioLoader : ApplicationLoader_Alone
    {

        #region 加载资源
        public static AudioLoader LoadAudioClip(Transform requestTarget, string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAudioClipSync(requestTarget, url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadAudioClipAsync(requestTarget, url, completeHandler);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }



        #region 异步加载
        /// <summary>
        /// （异步）加载声音资源
        /// </summary>
        /// <param name="requestTarget"></param>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static AudioLoader LoadAudioClipAsync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(AudioLoader)));
                return null;
            }
            bool isContainLoaders = false;
            AudioLoader audiolLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<AudioLoader>(url, ref isContainLoaders);
            audiolLoader.m_OnCompleteAct.Add(completeHandler);
            audiolLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）


            audiolLoader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (audiolLoader.IsCompleted)
                    audiolLoader.OnCompleteLoad(audiolLoader.IsError, audiolLoader.Description, audiolLoader.ResultObj, audiolLoader.IsCompleted);
                return audiolLoader;
            }

            audiolLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(audiolLoader.LoadAudioClicpAssetAsync(url));
            return audiolLoader;
        }
        private IEnumerator LoadAudioClicpAssetAsync(string url)
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
        /// （同步）加载声音资源
        /// </summary>
        /// <param name="requestTarget"></param>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static AudioLoader LoadAudioClipSync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(AudioLoader)));
                return null;
            }
            bool isContainLoaders = false;
            AudioLoader audiolLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<AudioLoader>(url, ref isContainLoaders);
            audiolLoader.m_OnCompleteAct.Add(completeHandler);

            audiolLoader.AddReference(requestTarget, url);
            if (isContainLoaders && audiolLoader.IsCompleted)
            {
                audiolLoader.LoadassetModel = LoadAssetModel.Sync;
                audiolLoader.OnCompleteLoad(audiolLoader.IsError, audiolLoader.Description, audiolLoader.ResultObj, audiolLoader.IsCompleted);
                return audiolLoader;
            }
            if (audiolLoader.LoadassetModel == LoadAssetModel.Sync)
            {
                audiolLoader.ForceBreakLoaderProcess();
            }
            audiolLoader.LoadassetModel = LoadAssetModel.Sync;
            audiolLoader.m_LoadAssetCoroutine = null;
            audiolLoader.LoadAudioClicpAssetSync(url);
            return audiolLoader;
        }

        private void LoadAudioClicpAssetSync(string url)
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
            AudioLoader audiolLoader = ResourcesLoaderMgr.GetExitLoaderInstance<AudioLoader>(url);
            if (audiolLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(materialLoader));
                return;
            }
            if (requestTarget == null)
                requestTarget = audiolLoader.m_RequesterTarget;

            audiolLoader.ReduceReference(audiolLoader, false);
        }
        #endregion

        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            if (result.GetType() == typeof(AssetBundle))
            {
                ResultObj = (result as AssetBundle).LoadAsset<AudioClip>(ResourseFileName.ToLower());
            }
            else
            {
                Debug.LogInfor("音效加载器 资源返回类型 " + result.GetType());
                ResultObj = result;
            }


            base.OnCompleteLoad(isError, description, result, iscomplete, process);
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