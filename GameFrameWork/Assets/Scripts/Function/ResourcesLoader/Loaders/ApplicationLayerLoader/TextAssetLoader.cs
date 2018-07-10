using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 加载配置文件 以string 形式返回结果
    /// </summary>
    public class TextAssetLoader : ApplicationLayerBaseLoader
    {
        #region      加载资源
        /// <summary>
        /// 加载.text 文件 并以string 形式返回
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static TextAssetLoader LoadAsset(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            bool isContainLoaders = false;
            TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<TextAssetLoader>(url, ref isContainLoaders);
            textAssetLoader.m_OnCompleteAct.Add(completeHandler);

            if (isContainLoaders)
            {
                textAssetLoader.AddReference();
                if (textAssetLoader.IsCompleted)
                    textAssetLoader.OnCompleteLoad(textAssetLoader.IsError, textAssetLoader.Description, textAssetLoader.ResultObj, textAssetLoader.IsCompleted);
                return textAssetLoader;
            }


            ApplicationMgr.Instance.StartCoroutine(textAssetLoader.LoadTextAsset(url));
            return textAssetLoader;
        }


        private IEnumerator LoadTextAsset(string url)
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
        public static void UnLoadAsset(string url)
        {
            TextAssetLoader textAssetLoader = ResourcesLoaderMgr.GetExitLoaderInstance<TextAssetLoader>(url);
            if (textAssetLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(TextAssetLoader));
                return;
            }
            textAssetLoader.ReduceReference();
            if (textAssetLoader.ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<TextAssetLoader>(url, false);
            }//引用计数为0时候开始回收资源
        }
        #endregion


        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            Debug.Log("result.GetType()== " + result.GetType());
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


            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);
        }

    }
}