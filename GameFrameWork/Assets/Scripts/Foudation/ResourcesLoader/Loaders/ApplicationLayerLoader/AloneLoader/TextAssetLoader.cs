using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 加载配置文件 以string 形式返回结果 (加载完成会主动卸载)
    /// </summary>
    public class TextAssetLoader : ApplicationLoader_AutoUnLoad
    {
        #region      加载资源

        public static TextAssetLoader LoadAsset(string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
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
        ///  加载.text 文件 并以string 形式返回
        /// </summary>
        /// <param name="requestTarget">请求加载资源的对象</param>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static TextAssetLoader LoadAssetAsync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(TextAsset)));
                return null;
            }
            bool isContainLoaders = false;
            TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<TextAssetLoader>(url, ref isContainLoaders);
            textAssetLoader.m_OnCompleteAct.Add(completeHandler);
            textAssetLoader.LoadassetModel = LoadAssetModel.Async;

            textAssetLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (textAssetLoader.IsCompleted)
                    textAssetLoader.OnCompleteLoad(textAssetLoader.IsError, textAssetLoader.Description, textAssetLoader.ResultObj, textAssetLoader.IsCompleted);
                return textAssetLoader;
            }
            else
            {
                textAssetLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(textAssetLoader.LoadTextAsset(url));
                return textAssetLoader;
            }
        }

        private IEnumerator LoadTextAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Async, null, false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion


        #region  同步加载

        /// <summary>
        ///  (同步)加载.text 文件 并以string 形式返回
        /// </summary>
        /// <param name="requestTarget">请求加载资源的对象</param>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static TextAssetLoader LoadAssetSync(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(TextAsset)));
                return null;
            }
            bool isContainLoaders = false;
            TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<TextAssetLoader>(url, ref isContainLoaders);
            textAssetLoader.m_OnCompleteAct.Add(completeHandler);


            textAssetLoader.AddReference(null, url);
            if (isContainLoaders && textAssetLoader.IsCompleted)
            {
                textAssetLoader.LoadassetModel = LoadAssetModel.Sync;
                textAssetLoader.OnCompleteLoad(textAssetLoader.IsError, textAssetLoader.Description, textAssetLoader.ResultObj, textAssetLoader.IsCompleted);
                return textAssetLoader;
            }

            if (textAssetLoader.LoadassetModel == LoadAssetModel.Async)
            {
                textAssetLoader.ForceBreakLoaderProcess();
            }
            textAssetLoader.LoadassetModel = LoadAssetModel.Sync;
            textAssetLoader.m_LoadAssetCoroutine = null;
            textAssetLoader.LoadTextAssetSync(url);
            return textAssetLoader;
        }

        private void LoadTextAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, false);
            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }
        #endregion

        #endregion

        #region 卸载资源
        /// <summary>
        /// 卸载指定的资源  
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestTarget">为null 则卸载加载时候请求的对象</param>
        public static void UnLoadAsset(string url, bool isForceDelete = false)
        {
            TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetExitLoaderInstance<TextAssetLoader>(url);
            if (textAssetLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(TextAssetLoader));
                return;
            }
            textAssetLoader.ReduceReference(textAssetLoader, isForceDelete);
        }
        #endregion



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

        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            if (result == null)
            {
                ResultObj = "";
            }
            else if (result.GetType() == typeof(byte[]))
            {
                ResultObj = Encoding.UTF8.GetString(result as byte[]);
            }
            else if (result.GetType() == typeof(GameObject))
            {
                Debug.LogInfor("没有处理的类型" + typeof(GameObject));
            }
            else if (result.GetType() == typeof(UnityEngine.Object))
            {
                Debug.LogInfor("没有处理的类型" + typeof(UnityEngine.Object));
            }
            else if (result.GetType() == typeof(UnityEngine.WWW))
            {
                ResultObj = (result as WWW).text;
            }
            else if (result.GetType() == typeof(UnityEngine.TextAsset))
            {
                ResultObj = (result as UnityEngine.TextAsset).text;
            }
            else if (result.GetType() == typeof(string))
            {

            }
            else
            {
                Debug.Log("没有定义的类型  result.GetType()== " + result.GetType());
            }

            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);

            UnLoadAsset(m_ResourcesUrl, false); //使用完成卸载资源
        }

    }
}