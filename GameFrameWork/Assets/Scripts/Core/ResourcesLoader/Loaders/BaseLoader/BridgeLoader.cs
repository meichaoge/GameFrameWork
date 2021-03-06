﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 加载资源的桥连接器 会根据配置路径加载优先级加载资源,加载完成自动卸载当前加载器 (优先级在ApplicationMgr 脚本界面配置)
    /// </summary>
    public class BridgeLoader : BaseAbstracResourceLoader
    {
        protected BaseAbstracResourceLoader m_ConnectLoader = null;  //当前资源加载器使用的实际加载器
        protected bool m_IsLoadSceneAsset = false;  //标识是否是加载场景资源

        #region  加载资源
        public static BridgeLoader LoadAsset(string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> onCompleteAct, bool isloadSceneAsset)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAssetSync(url, onCompleteAct, isloadSceneAsset);
                case LoadAssetModel.Async:
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                    {
                        Debug.LogError("编辑器下非运行模式不要使用异步模式");
                        return null;
                    }
#endif
                    return LoadAssetAsync(url, onCompleteAct, isloadSceneAsset);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }


        #region 异步加载

        /// <summary>
        /// 加载资源的桥连接器 会根据配置路径加载优先级加载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onCompleteAct"></param>
        /// <param name="isloadSceneAsset"> 如果加载的是场景资源 则这里必须为true  否则为false</param>
        /// <returns></returns>
        private static BridgeLoader LoadAssetAsync(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct, bool isloadSceneAsset)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(BridgeLoader)));
                return null;
            }

            bool isLoaderExit = false;
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<BridgeLoader>(url, ref isLoaderExit);
            bridgeLoader.m_OnCompleteAct.Add(onCompleteAct);
            bridgeLoader.m_IsLoadSceneAsset = isloadSceneAsset;
            bridgeLoader.LoadassetModel = LoadAssetModel.Async;  //这里获得的加载器不需要特殊处理

            if (isLoaderExit)
            {
                if (bridgeLoader.IsCompleted)
                    bridgeLoader.OnCompleteLoad(bridgeLoader.IsError, bridgeLoader.Description, bridgeLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return bridgeLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            EventCenter.Instance.StartCoroutine(bridgeLoader.LoadAssetByPriorityAsync(url, bridgeLoader, isloadSceneAsset));
            EventCenter.Instance.StartCoroutine(bridgeLoader.LoadAssetAsync(url, bridgeLoader));
            return bridgeLoader;
        }

        /// <summary>
        /// 根据配置的路径加载优先级 选择合适的加载器加载资源  (可能存在加载不到资源的情况,目前只处理LoadAssetPathEnum.PersistentDataPath和PersistentDataPath.ResourcesPath)
        /// </summary>
        /// <param name="bridgeLoader"></param>
        /// <param name="url"></param>
        /// <param name="isloadScene"> 如果加载的是场景 则这里必须填true ,否则false</param>
        /// <returns></returns>
        private IEnumerator LoadAssetByPriorityAsync(string url, BridgeLoader bridgeLoader, bool isloadSceneAsset)
        {
            LoadAssetPathEnum curLoadAssetPathEnum = ApplicationConfig.Instance.GetFirstPriortyAssetPathEnum();  //加载的优先级
            string fileName = System.IO.Path.GetFileNameWithoutExtension(url);
            do
            {
                if (curLoadAssetPathEnum == LoadAssetPathEnum.PersistentDataPath)
                {

                    AssetBundleExitState assetBundleExitState;
                    string newUrl = HotUpdate.AssetBundleMgr.Instance.CheckIfAssetBundleAsset(url.ToLower(), out assetBundleExitState);

                    if (assetBundleExitState != AssetBundleExitState.None)
                    {
                        Debug.Log("[AssetBundler ]加载外部资源，且以AssetBundle 加载");
                        bridgeLoader.m_ConnectLoader = NewAssetBundleLoader3.LoadAssetBundleAsset(newUrl, fileName, LoadAssetModel.Async, assetBundleExitState, isloadSceneAsset);   //整体打包的资源
                    }
                    else
                    {
                        if (isloadSceneAsset == false)
                        {
                            Debug.Log("[byteLoader ]优先加载外部资源,但是不是AssetBundle 资源，则以Byte[] 尝试 加载");
                            string path = url;
                            if (url.StartsWith(ConstDefine.S_AssetBundleTopPath) == false)
                                path = ConstDefine.S_AssetBundleTopPath + url;
                            bridgeLoader.m_ConnectLoader = ByteLoader.LoadAsset(path, LoadAssetModel.Async, null);
                        }
                        else
                        {
                            //***场景资源不通过这种方式
                        }
                    }
                }
                else if (curLoadAssetPathEnum == LoadAssetPathEnum.ResourcesPath)
                {
                    Debug.Log("[RecourcsLoader ]  加载Resources 资源 " + url);
                    bridgeLoader.m_ConnectLoader = ResourcesLoader.LoadResourcesAsset(url, LoadAssetModel.Async, null, isloadSceneAsset);
                }

                while (bridgeLoader.m_ConnectLoader.IsCompleted == false)
                    yield return null;

                if (curLoadAssetPathEnum == LoadAssetPathEnum.PersistentDataPath && (bridgeLoader.m_ConnectLoader.GetType() == typeof(NewAssetBundleLoader3)))
                {
                    NewAssetBundleLoader3 assetBundleLoader = bridgeLoader.m_ConnectLoader as NewAssetBundleLoader3;
                    if (assetBundleLoader.AssetLoaderResultInfor.GetAssetByUrl(assetBundleLoader.m_ResourcesUrl, fileName) != null)
                        yield break;
                }
                else
                {
                    if (bridgeLoader.m_ConnectLoader.ResultObj != null)
                    {
                        yield break;
                    }
                }
                //    bridgeLoader.m_ConnectLoader.ReduceReference(bridgeLoader.m_ConnectLoader, false);  //卸载这个加载器
                ApplicationConfig.Instance.GetNextLoadAssetPath(ref curLoadAssetPathEnum);
                continue;  //如果加载得到则返回否则继续尝试其他的加载方式
            } while (curLoadAssetPathEnum != LoadAssetPathEnum.None);
            Debug.LogError("如果加载成功不会执行到这里" + url);
            bridgeLoader.m_ConnectLoader = null;  //如果加载成功不会执行到这里
        }

        /// <summary>
        /// 返回链接的底层加载器的状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="birdgeLoader"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetAsync(string url, BridgeLoader bridgeLoader)
        {
            bridgeLoader.m_ResourcesUrl = url;
            if (bridgeLoader.m_ConnectLoader == null)
            {
                OnLoadAssetFail();
                yield break;
            }

            while (bridgeLoader.m_ConnectLoader.IsCompleted == false)
                yield return null;

            if (bridgeLoader.m_ConnectLoader.GetType() == typeof(NewAssetBundleLoader3))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(url);
                NewAssetBundleLoader3 assetBundleloader = bridgeLoader.m_ConnectLoader as NewAssetBundleLoader3;
                object asset = assetBundleloader.AssetLoaderResultInfor.GetAssetByUrl(assetBundleloader.m_ResourcesUrl, fileName);
                OnCompleteLoad(bridgeLoader.m_ConnectLoader.IsError, bridgeLoader.m_ConnectLoader.Description, asset, bridgeLoader.m_ConnectLoader.IsCompleted);
            }
            else
            {
                OnCompleteLoad(bridgeLoader.m_ConnectLoader.IsError, bridgeLoader.m_ConnectLoader.Description, bridgeLoader.m_ConnectLoader.ResultObj, bridgeLoader.m_ConnectLoader.IsCompleted);
            }
        }
        #endregion


        #region 同步加载
        /// <summary>
        /// 同步加载资源 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onCompleteAct"></param>
        /// <param name="isloadSceneAsset"></param>
        /// <returns></returns>
        private static BridgeLoader LoadAssetSync(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct, bool isloadSceneAsset)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(BridgeLoader)));
                return null;
            }
            bool isLoaderExit = false;
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<BridgeLoader>(url, ref isLoaderExit);
            bridgeLoader.m_OnCompleteAct.Add(onCompleteAct);
            bridgeLoader.m_IsLoadSceneAsset = isloadSceneAsset;

            if (isLoaderExit && bridgeLoader.IsCompleted)
            {
                bridgeLoader.LoadassetModel = LoadAssetModel.Sync;
                bridgeLoader.OnCompleteLoad(bridgeLoader.IsError, bridgeLoader.Description, bridgeLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return bridgeLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            if (bridgeLoader.LoadassetModel == LoadAssetModel.Async)
            {
                bridgeLoader.ForceBreakLoaderProcess();
            }
            bridgeLoader.LoadassetModel = LoadAssetModel.Sync;
#if UNITY_EDITOR
            if(Application.isPlaying==false)
            {
                bridgeLoader.LoadAssetByPriority_Editor(url, bridgeLoader, isloadSceneAsset);
                bridgeLoader.LoadAssetSync(url, bridgeLoader);
                return bridgeLoader;
            }//编辑器下非运行状态特殊处理
#endif

            bridgeLoader.LoadAssetByPrioritySync(url, bridgeLoader, isloadSceneAsset);
            bridgeLoader.LoadAssetSync(url, bridgeLoader);
            return bridgeLoader;
        }


        /// <summary>
        /// (同步加载)根据配置的路径加载优先级 选择合适的加载器加载资源  (可能存在加载不到资源的情况,目前只处理LoadAssetPathEnum.PersistentDataPath和PersistentDataPath.ResourcesPath)
        /// </summary>
        /// <param name="bridgeLoader"></param>
        /// <param name="url"></param>
        /// <param name="isloadScene"> 如果加载的是场景 则这里必须填true ,否则false</param>
        /// <returns></returns>
        private void LoadAssetByPrioritySync(string url, BridgeLoader bridgeLoader, bool isloadSceneAsset)
        {
            LoadAssetPathEnum curLoadAssetPathEnum = ApplicationConfig.Instance.GetFirstPriortyAssetPathEnum();  //加载的优先级
            string fileName = System.IO.Path.GetFileNameWithoutExtension(url);
            do
            {
                if (curLoadAssetPathEnum == LoadAssetPathEnum.PersistentDataPath)
                {
                    AssetBundleExitState assetBundleExitState;
                    string newUrl = HotUpdate.AssetBundleMgr.Instance.CheckIfAssetBundleAsset(url.ToLower(), out assetBundleExitState);

                    if (assetBundleExitState != AssetBundleExitState.None)
                    {
                        Debug.Log("加载外部资源，且以AssetBundle 加载");
                        bridgeLoader.m_ConnectLoader = NewAssetBundleLoader3.LoadAssetBundleAsset(newUrl, fileName, LoadAssetModel.Sync, assetBundleExitState, isloadSceneAsset);
                    }
                    else
                    {
                        if (isloadSceneAsset == false)
                        {
                            Debug.Log("[ByteLoader] 优先加载外部资源,但是不是AssetBundle 资源，则以Byte[] 尝试 加载 url=" + url);
                            string path = url;
                            if (url.StartsWith(ConstDefine.S_AssetBundleTopPath) == false)
                                path = ConstDefine.S_AssetBundleTopPath + url;
                            bridgeLoader.m_ConnectLoader = ByteLoader.LoadAsset(path, LoadAssetModel.Sync, null);
                        }
                        else
                        {
                            //***场景资源不通过这种方式
                        }
                    }
                }
                else if (curLoadAssetPathEnum == LoadAssetPathEnum.ResourcesPath)
                {
                    bridgeLoader.m_ConnectLoader = ResourcesLoader.LoadResourcesAsset(url, LoadAssetModel.Sync, null, isloadSceneAsset);
                }

                //********处理不同的加载器回调
                if (bridgeLoader.m_ConnectLoader.GetType() == typeof(NewAssetBundleLoader3))
                {
                    NewAssetBundleLoader3 assetBundleloader = bridgeLoader.m_ConnectLoader as NewAssetBundleLoader3;
                    if (assetBundleloader.AssetLoaderResultInfor.GetAssetByUrl(assetBundleloader.m_ResourcesUrl, fileName) != null)
                        return;
                }// AssetBundler 类型
                else
                {
                    if (bridgeLoader.m_ConnectLoader.ResultObj != null)
                        return;
                }
                //   bridgeLoader.m_ConnectLoader.ReduceReference(bridgeLoader.m_ConnectLoader, false);  //卸载这个加载器
                ApplicationConfig.Instance.GetNextLoadAssetPath(ref curLoadAssetPathEnum);
                continue;  //如果加载得到则返回否则继续尝试其他的加载方式
            } while (curLoadAssetPathEnum != LoadAssetPathEnum.None);
            Debug.LogInfor("如果加载成功不会执行到这里");
            bridgeLoader.m_ConnectLoader = null;  //如果加载成功不会执行到这里
        }

        /// <summary>
        /// (同步加载)返回链接的底层加载器的状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="birdgeLoader"></param>
        /// <returns></returns>
        private void LoadAssetSync(string url, BridgeLoader bridgeLoader)
        {
            bridgeLoader.m_ResourcesUrl = url;
            if (bridgeLoader.m_ConnectLoader == null)
            {
                OnLoadAssetFail();
                return;
            }

            if (bridgeLoader.m_ConnectLoader.GetType() == typeof(NewAssetBundleLoader3))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(url);
                NewAssetBundleLoader3 assetBundleloader = bridgeLoader.m_ConnectLoader as NewAssetBundleLoader3;
                object asset = assetBundleloader.AssetLoaderResultInfor.GetAssetByUrl(assetBundleloader.m_ResourcesUrl, fileName);
                OnCompleteLoad(bridgeLoader.m_ConnectLoader.IsError, bridgeLoader.m_ConnectLoader.Description, asset, bridgeLoader.m_ConnectLoader.IsCompleted);
            }
            else
            {
                OnCompleteLoad(bridgeLoader.m_ConnectLoader.IsError, bridgeLoader.m_ConnectLoader.Description, bridgeLoader.m_ConnectLoader.ResultObj, bridgeLoader.m_ConnectLoader.IsCompleted);
            }
        }


        #endregion



#if UNITY_EDITOR
        /// <summary>
        ///只在编辑器下可用
        /// </summary>
        /// <param name="bridgeLoader"></param>
        /// <param name="url"></param>
        /// <param name="isloadScene"> 如果加载的是场景 则这里必须填true ,否则false</param>
        /// <returns></returns>
        private void LoadAssetByPriority_Editor(string url, BridgeLoader bridgeLoader, bool isloadSceneAsset)
        {
            LoadAssetPathEnum curLoadAssetPathEnum = ApplicationConfig.GetDefaultEditorLoadAssetPath();
            string fileName = System.IO.Path.GetFileNameWithoutExtension(url);
            if (curLoadAssetPathEnum == LoadAssetPathEnum.ResourcesPath)
            {
                bridgeLoader.m_ConnectLoader = ResourcesLoader.LoadResourcesAsset(url, LoadAssetModel.Sync, null, isloadSceneAsset);
                if (bridgeLoader.m_ConnectLoader.ResultObj != null)
                    return;
                else
                {
                    Debug.LogError("加载资源失败  url" + url);
                }
            }

            Debug.LogError("LoadAssetByPriority_Editor  Fail,  无法识别的模式 " + curLoadAssetPathEnum);
        }

#endif

        #endregion


        #region 资源卸载
        public static void UnLoadAsset(string url, bool isForceDelete = false)
        {
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetExitLoaderInstance<BridgeLoader>(url);
            if (bridgeLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }
            bridgeLoader.ReduceReference(bridgeLoader, isForceDelete);
        }
        #endregion



        //  public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        //  {
        //      base.OnCompleteLoad(isError, description, result, iscomplete, process);
        //      if (m_ConnectLoader != null)
        //          m_ConnectLoader.ReduceReference(m_ConnectLoader, false);
        //      ReduceReference(this, false);
        //  }


        protected override void ForceBreakLoaderProcess()
        {
            if (m_ConnectLoader.IsCompleted) return;
            if (LoadassetModel != LoadAssetModel.Async)
            {
                Debug.LogError("非异步加载方式不需要强制结束 " + LoadassetModel);
                return;
            }
            EventCenter.Instance.StopCoroutine(LoadAssetByPriorityAsync(m_ResourcesUrl, this, m_IsLoadSceneAsset));
            EventCenter.Instance.StopCoroutine(LoadAssetAsync(m_ResourcesUrl, this));
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