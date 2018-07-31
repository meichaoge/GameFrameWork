using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 场景加载器(不同于其他的加载器 这里场景中只有几个单独的挂点和一个场景管理脚本)  在场景被卸载时候需要手动卸载引用
    /// </summary>
    public class SceneLoader : ApplicationLoader_NotDestroy
    {
        #region  加载资源
        public static SceneLoader LoadScene(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(FontLoader)));
                return null;
            }

            bool isContainLoaders = false;
            SceneLoader sceneLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<SceneLoader>(url, ref isContainLoaders);
            sceneLoader.m_OnCompleteAct.Add(completeHandler);

            sceneLoader.AddReference(null, url);
            if (isContainLoaders)
            {
                if (sceneLoader.IsCompleted)
                    sceneLoader.OnCompleteLoad(sceneLoader.IsError, sceneLoader.Description, sceneLoader.ResultObj, sceneLoader.IsCompleted);
                return sceneLoader;
            }


            sceneLoader.m_LoadAssetCoroutine = ApplicationMgr.Instance.StartCoroutine(sceneLoader.LoadSceneAsset(url));
            return sceneLoader;
        }

        private IEnumerator LoadSceneAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }

        #endregion

        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            if (result != null)
            {
                ResultObj = null;
            }
            else
            {
                if (typeof(AssetBundle) == result.GetType())
                {
                    ResultObj = result as AssetBundle;
                }
                else
                {
                    Debug.LogInfor("加载场景 未知类型 " + result.GetType());
                    ResultObj = result;
                }
            }

            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);
            if (m_BridgeLoader != null)
                ResourcesLoaderMgr.DeleteExitLoaderInstance(typeof(BridgeLoader), m_ResourcesUrl);

        }

        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if (m_LoadAssetCoroutine != null)
                ApplicationMgr.Instance.StopCoroutine(m_LoadAssetCoroutine);

        }




    }
}