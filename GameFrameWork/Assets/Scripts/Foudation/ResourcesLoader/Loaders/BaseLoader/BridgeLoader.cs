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

            ApplicationMgr.Instance.StartCoroutine(bridgeLoader.LoadAssetByPriority(url, bridgeLoader));
            ApplicationMgr.Instance.StartCoroutine(bridgeLoader.LoadAsset(url, bridgeLoader));
            return bridgeLoader;
        }

        /// <summary>
        /// 根据配置的路径加载优先级 选择合适的加载器加载资源  (可能存在加载不到资源的情况,目前只处理LoadAssetPathEnum.PersistentDataPath和PersistentDataPath.ResourcesPath)
        /// </summary>
        /// <param name="bridgeLoader"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetByPriority(string url, BridgeLoader bridgeLoader)
        {
            LoadAssetPathEnum curLoadAssetPathEnum = ApplicationMgr.Instance.GetFirstPriortyAssetPathEnum();  //加载的优先级
            do
            {
                if (curLoadAssetPathEnum == LoadAssetPathEnum.PersistentDataPath)
                {
                    string newUrl = "";
                    AssetBundleExitState assetBundleExitState = AssetBundleLoader.CheckIsAssetBundleExit(url.ToLower(), ref newUrl);

                    if (assetBundleExitState != AssetBundleExitState.None)
                    {
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(url);

                        Debug.Log("加载外部资源，且以AssetBundle 加载");
                        if (assetBundleExitState == AssetBundleExitState.SinglePrefab)
                            bridgeLoader.m_ConnectLoader = AssetBundleLoader.LoadAssetBundleAsset(url.ToLower(), fileName, null);  //单独预制体
                        else
                            bridgeLoader.m_ConnectLoader = AssetBundleLoader.LoadAssetBundleAsset(newUrl, fileName, null);   //整体打包的资源
                    }
                    else
                    {
                        Debug.Log("优先加载外部资源,但是不是AssetBundle 资源，则以Byte[] 尝试 加载");
                        bridgeLoader.m_ConnectLoader = ByteLoader.LoadAsset(url, null);
                    }
                }
                else if (curLoadAssetPathEnum == LoadAssetPathEnum.ResourcesPath)
                {
                    bridgeLoader.m_ConnectLoader = ResourcesLoader.LoadResourcesAsset(url, null);
                }

                if (bridgeLoader.m_ConnectLoader.IsCompleted == false) yield return null;
                if (bridgeLoader.m_ConnectLoader.ResultObj != null)
                {
                    yield break;
                }
                else
                {
                    bridgeLoader.m_ConnectLoader.ReduceReference(bridgeLoader.m_ConnectLoader, false);  //卸载这个加载器
                    ApplicationMgr.Instance.GetNextLoadAssetPath(ref curLoadAssetPathEnum);
                    continue;  //如果加载得到则返回否则继续尝试其他的加载方式
                }

            } while (curLoadAssetPathEnum != LoadAssetPathEnum.None);
            Debug.LogInfor("如果加载成功不会执行到这里");
            bridgeLoader.m_ConnectLoader = null;  //如果加载成功不会执行到这里
        }



        /// <summary>
        /// 返回链接的底层加载器的状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="birdgeLoader"></param>
        /// <returns></returns>
        private IEnumerator LoadAsset(string url, BridgeLoader birdgeLoader)
        {
            birdgeLoader.m_ResourcesUrl = url;
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

        #endregion




        #region 资源卸载
        public static void UnLoadAsset(string url, bool isForceDelete=false)
        {
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetExitLoaderInstance<BridgeLoader>(url);
            if (bridgeLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }
            bridgeLoader.ReduceReference(isForceDelete);
        }
        #endregion  



        protected override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, iscomplete, process);
            if (m_ConnectLoader != null)
                m_ConnectLoader.ReduceReference(m_ConnectLoader, false);
            ReduceReference(false);
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