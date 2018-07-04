using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 加载资源的桥连接器 会根据配置路径加载优先级加载资源,加载完成自动卸载当前加载器 (优先级在ApplicationMgr 脚本界面配置)
    /// </summary>
    public class BridgeLoader : BaseAbstracResourceLoader
    {
        protected BaseAbstracResourceLoader m_ConnectLoader = null;  //当前资源加载器使用的实际加载器

        #region  加载资源
        /// <summary>
        /// 加载资源的桥连接器 会根据配置路径加载优先级加载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onCompleteAct"></param>
        /// <returns></returns>
        public static BridgeLoader LoadAsset(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            bool isLoaderExit = false;
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<BridgeLoader>(url, ref isLoaderExit);
            bridgeLoader.m_OnCompleteAct.Add(onCompleteAct);


            if (isLoaderExit)
            {
                if (bridgeLoader.IsCompleted)
                    bridgeLoader.OnCompleteLoad(bridgeLoader.IsError, bridgeLoader.Description, bridgeLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return bridgeLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            #region  根据配置的路径加载优先级 选择合适的加载器加载资源  (可能存在加载不到资源的情况,目前只处理LoadAssetPathEnum.PersistentDataPath和PersistentDataPath.ResourcesPath)
            LoadAssetPathEnum curLoadAssetPathEnum = ApplicationMgr.Instance.GetFirstPriortyAssetPathEnum();  //加载的优先级
            do
            {
                if (curLoadAssetPathEnum == LoadAssetPathEnum.PersistentDataPath)
                {
                    if (AssetBundleLoader.CheckIsAssetBundleExit(url))
                    {
                        Debug.Log("加载外部资源，且以AssetBundle 加载");
                        bridgeLoader.m_ConnectLoader = AssetBundleLoader.LoadAssetBundleAsset(url, null);
                    }
                    else
                    {
                        Debug.Log("加载外部资源，且以Byte[]  加载");
                        bridgeLoader.m_ConnectLoader = ByteLoader.LoadAsset(url, null);
                    }
                    break;
                }

                if (curLoadAssetPathEnum == LoadAssetPathEnum.ResourcesPath)
                {
                    bridgeLoader.m_ConnectLoader = ResourcesLoader.LoadResourcesAsset(url, null);
                    break;
                }

                ApplicationMgr.Instance.GetNextLoadAssetPath(ref curLoadAssetPathEnum);
            } while (curLoadAssetPathEnum != LoadAssetPathEnum.None);
            #endregion

            ApplicationMgr.Instance.StartCoroutine(bridgeLoader.LoadAsset(url, bridgeLoader));
            return bridgeLoader;
        }
        #endregion


        #region 资源卸载
        public static void UnLoadAsset(string url)
        {
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetExitLoaderInstance<BridgeLoader>(url);
            if (bridgeLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }
            bridgeLoader.ReduceReference();
        }
        #endregion  

        private IEnumerator LoadAsset(string url, BridgeLoader birdgeLoader)
        {
            birdgeLoader. m_ResourcesUrl = url;
            if (birdgeLoader.m_ConnectLoader == null)
            {
                OnLoadAssetFail();
                yield break;
            }
            if (birdgeLoader.m_ConnectLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_ConnectLoader.IsError, m_ConnectLoader.Description, m_ConnectLoader.ResultObj, m_ConnectLoader.IsCompleted);
            yield break;
        }



        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, iscomplete, process);
            if (m_ConnectLoader != null)
                m_ConnectLoader.ReduceReference();
            ReduceReference();
        }

        public override void ReduceReference()
        {
            base.ReduceReference();
            if (ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<BridgeLoader>(m_ResourcesUrl, false);
            }//引用计数为0时候开始回收资源
        }


        private void OnLoadAssetFail()
        {
            IsCompleted = true;
            IsError = true;
            Process = 1;
            m_Description = "无法识别的资源类型";
            foreach (var item in m_OnCompleteAct)
            {
                if (item != null)
                    item(this);
            }
            m_OnCompleteAct.Clear();
            UnLoadAsset(m_ResourcesUrl);
        }

    }
}