using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    ///  Sprite2D加载器，加载已经制作成Prefab 的Sprite
    /// </summary>
    public class SpriteLoader : ApplicationLoader_Alone
    {

        #region      加载资源
        /// <summary>
        /// 加载精灵图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static SpriteLoader LoadAsset(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(SpriteLoader)));
                return null;
            }

            bool isContainLoaders = false;
            SpriteLoader spriteLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<SpriteLoader>(url, ref isContainLoaders);
            spriteLoader.m_OnCompleteAct.Add(completeHandler);


            spriteLoader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (spriteLoader.IsCompleted)
                    spriteLoader.OnCompleteLoad(spriteLoader.IsError, spriteLoader.Description, spriteLoader.ResultObj, spriteLoader.IsCompleted);
                return spriteLoader;
            }


            spriteLoader.m_LoadAssetCoroutine = ApplicationMgr.Instance.StartCoroutine(spriteLoader.LoadSpriteAsset(url));
            return spriteLoader;
        }


        private IEnumerator LoadSpriteAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null,false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion

        #region 卸载资源
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestTarget"></param>
        public static void UnLoadAsset(string url, object requestTarget = null)
        {
            SpriteLoader spriteLoader = ResourcesLoaderMgr.GetExitLoaderInstance<SpriteLoader>(url);
            if (spriteLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(SpriteLoader));
                return;
            }
            if (requestTarget == null)
                requestTarget = spriteLoader.m_RequesterTarget;

            spriteLoader.ReduceReference(spriteLoader, false);
        }
        #endregion


        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            //  Debug.LogInfor("AAAAAAA>>>> " + (result as GameObject).GetComponent<SpriteRenderer>());
            Type resultType = result.GetType();
            if (resultType == typeof(UnityEngine.GameObject))
                ResultObj = (result as GameObject).GetComponent<SpriteRenderer>().sprite;
            else if (resultType == typeof(UnityEngine.Sprite))
                ResultObj = result as Sprite;
            else
                Debug.LogError("没有定义的类型" + resultType);

            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);

        }

        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if (m_LoadAssetCoroutine != null)
                ApplicationMgr.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }


    }
}