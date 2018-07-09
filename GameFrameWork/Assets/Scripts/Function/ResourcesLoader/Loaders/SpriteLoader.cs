﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    ///  Sprite2D 加载器 缓存已经加载的Sprite
    /// </summary>
    public class SpriteLoader : BaseAbstracResourceLoader
    {
        private BridgeLoader m_BridgeLoader;  //加载不同路径的资源桥接器

        #region      加载资源
        /// <summary>
        /// 加载精灵图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static SpriteLoader LoadAsset(string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            bool isContainLoaders = false;
            SpriteLoader spriteLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<SpriteLoader>(url, ref isContainLoaders);
            spriteLoader.m_OnCompleteAct.Add(completeHandler);

            if (isContainLoaders)
            {
                spriteLoader.AddReference();
                if (spriteLoader.IsCompleted)
                    spriteLoader.OnCompleteLoad(spriteLoader.IsError, spriteLoader.Description, spriteLoader.ResultObj, spriteLoader.IsCompleted);
                return spriteLoader;
            }


            ApplicationMgr.Instance.StartCoroutine(spriteLoader.LoadSpriteAsset(url));
            return spriteLoader;
        }


        private IEnumerator LoadSpriteAsset(string url)
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
            SpriteLoader spriteLoader = ResourcesLoaderMgr.GetExitLoaderInstance<SpriteLoader>(url);
            if (spriteLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(WWWLoader));
                return;
            }
            spriteLoader.ReduceReference();
            if (spriteLoader.ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<SpriteLoader>(url, false);
            }//引用计数为0时候开始回收资源
        }
        #endregion


        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, iscomplete, process);
            ResultObj = (result as GameObject).GetComponent<SpriteRenderer>().sprite;
        }

    }
}