using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 加载配置文件 以string 形式返回结果 (加载完成不会主动卸载)
    /// </summary>
    public class TextAssetLoader : ApplicationLoader_NotDestroy
    {
        #region      加载资源
        /// <summary>
        ///  加载.text 文件 并以string 形式返回
        /// </summary>
        /// <param name="requestTarget">请求加载资源的对象</param>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static TextAssetLoader LoadAsset(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}",typeof(TextAsset)));
                return null;
            }
            bool isContainLoaders = false;
            TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<TextAssetLoader>(url, ref isContainLoaders);
            textAssetLoader.m_OnCompleteAct.Add(completeHandler);


            textAssetLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (textAssetLoader.IsCompleted)
                    textAssetLoader.OnCompleteLoad(textAssetLoader.IsError, textAssetLoader.Description, textAssetLoader.ResultObj, textAssetLoader.IsCompleted);
                return textAssetLoader;
            }
            else
            {
                textAssetLoader. m_LoadAssetCoroutine= ApplicationMgr.Instance.StartCoroutine(textAssetLoader.LoadTextAsset(url));
                return textAssetLoader;
            }
        }


        private IEnumerator LoadTextAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion

        #region 卸载资源
        ///// <summary>
        ///// 卸载指定的资源  
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="requestTarget">为null 则卸载加载时候请求的对象</param>
        //public static void UnLoadAsset( string url, bool isForceDelete=false,object requestTarget=null)
        //{
        //    TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetExitLoaderInstance<TextAssetLoader>(url);
        //    if (textAssetLoader == null)
        //    {
        //        //Debug.LogError("无法获取指定类型的加载器 " + typeof(TextAssetLoader));
        //        return;
        //    }
        //    if (requestTarget == null)
        //        requestTarget = textAssetLoader. m_RequesterTarget;

        //    if (textAssetLoader.TryDeleteRecord(requestTarget))
        //        textAssetLoader.ReduceReference(isForceDelete);
        //}
        #endregion



        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if(m_LoadAssetCoroutine!=null)
            ApplicationMgr.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }

        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {

            if (result.GetType() == typeof(byte[]))
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
        }

    }
}