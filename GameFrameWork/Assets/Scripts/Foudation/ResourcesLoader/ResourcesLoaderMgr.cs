using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{

    /// <summary>
    /// 标识返回结果是否可行
    /// </summary>
    public enum PathResultEnum
    {
        Valid,  //有效路径
        Invalid, //无效路径
    }

    /// <summary>
    /// 资源加载器管理器
    /// </summary>
    public class ResourcesLoaderMgr
    {
        /// <summary>
        /// 资源加载器检测是否需要GC 回收的时间间隔
        /// </summary>
        private static float S_CheckGCTime
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                    return 1f;//编辑器下0.1秒

                if (UnityEngine.Debug.isDebugBuild)
                    return 2f;  //打包时候开启 Develop Builder 
                return 3;
            }
        }

        private static float lastCheckTime = 0; //上一次GC检测时间



        /// <summary>
        /// 保存所有类型的加载器
        /// </summary>
        public static Dictionary<Type, Dictionary<string, BaseAbstracResourceLoader>> S_AllTypeLoader = new Dictionary<Type, Dictionary<string, BaseAbstracResourceLoader>>();
        /// <summary>
        /// 使用完成当到达一个时间点时候回被销毁 （Value中的 Queue 按照时间是先入先出）
        /// </summary>
        public static Dictionary<Type, Queue<BaseAbstracResourceLoader>> S_UnUseLoader = new Dictionary<Type, Queue<BaseAbstracResourceLoader>>();


        #region 删除加载器  添加到待删除队列中

        /// <summary>
        ///  将Loader 删除放到不使用的字典中等待删除(需要判断引用计数是否小于1)
        /// </summary>
        /// <param name="loader">Loader.</param>
        public static void DeleteLoader(Type loaderType, string url, bool isForceDelete) //where T : BaseAbstracResourceLoader
        {

            BaseAbstracResourceLoader loader = DeleteExitLoaderInstance(loaderType, url);
            if (loader == null)
                return;

            ReleaseLoader(loader, isForceDelete);
        }
        public static void DeleteLoader<T>(string url, bool isForceDelete) where T : BaseAbstracResourceLoader
        {
            DeleteLoader(typeof(T), url, isForceDelete);
        }


        /// <summary>
        /// 释放加载器资源(如果强制删除或者计数为0则加入到待删除队列中)
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="isForceDelete"></param>
        private static void ReleaseLoader(BaseAbstracResourceLoader loader, bool isForceDelete)
        {
            Queue<BaseAbstracResourceLoader> allUnUseLoadersOfType = null;
            if (S_UnUseLoader.TryGetValue(loader.GetType(), out allUnUseLoadersOfType) == false)
            {
                allUnUseLoadersOfType = new Queue<BaseAbstracResourceLoader>();
                S_UnUseLoader.Add(loader.GetType(), allUnUseLoadersOfType);
            }
            if (isForceDelete)
            {
                loader.ReleaseLoader();
                allUnUseLoadersOfType.Enqueue(loader);  //释放资源加载器资源并加入队列中
                return;
            }

            if (loader.ReferCount > 0)
            {
                Debug.LogInfor("DeleteLoader Fail,加载器的引用不为0,如果需要强制删除 请设置参数isForceDelete=true    ");
                return;
            }
            loader.ReleaseLoader();
            allUnUseLoadersOfType.Enqueue(loader);  //释放资源加载器资源并加入队列中
                                                    //  Debug.Log("回收加载器  " + typeof(T) + "::" + url + "  count=" + allUnUseLoadersOfType.Count);
        }



        #endregion


        #region 检测是否需要释放加载器资源
        public static void Tick()
        {
            if (Time.realtimeSinceStartup - lastCheckTime >= S_CheckGCTime)
            {
                CheckUnRefenceLoader();
                CheckLoaderStateForGC();
                lastCheckTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 检测哪些资源不再被引用
        /// </summary>
        private static void CheckUnRefenceLoader()
        {
            foreach (var item in S_AllTypeLoader.Values)
            {
                foreach (var loader in item.Values)
                {
                    if (loader is ApplicationLoader_Alone)
                        (loader as ApplicationLoader_Alone).CheckLoaderIsReference();
                }
            }
        }

        /// <summary>
        /// 检测哪些资源需要被GC回收
        /// </summary>
        private static void CheckLoaderStateForGC()
        {
            BaseAbstracResourceLoader checkLoader = null;
            bool isNeedGC = false;  //标识是否需要回收一次资源
            foreach (var loaders in S_UnUseLoader.Values)
            {
                while (true)
                {
                    if (loaders.Count == 0)
                        break;  //已经检测完这个类型的加载器
                    checkLoader = loaders.Peek();
                    if (checkLoader == null)
                    {
                        checkLoader.Dispose();  //加载器释放最后的资源
                        loaders.Dequeue();  //删除对象
                        isNeedGC = true;
                        continue;
                    }
                    if (Time.realtimeSinceStartup - checkLoader.UnUseTime < checkLoader.m_GCInterval)
                        break;  //由于加载器是一个队列 因此最前面的加载肯定先到达最长生命周期

                    Debug.LogInfor("[GC Loaders ] Type=" + checkLoader.GetType().Name + "  Url" + checkLoader.m_ResourcesUrl);

                    //当前加载器回收后超过了最大生命周期时间需要被删除回收资源
                    checkLoader.Dispose();  //加载器释放最后的资源
                    loaders.Dequeue();  //删除对象
                    isNeedGC = true;
                    continue;
                }//while
            }//forech

            if (isNeedGC)
            {
                GC.Collect();  //回收资源
            }
        }

        #endregion


        #region 加载器检测/获取接口

        /// <summary>
        /// /获取指定类型的加载器  如果不存在则创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="isLoaderExit">标识这个加载器是否存在(false 标识是刚创建的)</param>
        /// <returns></returns>
        public static T GetOrCreateLoaderInstance<T>(string url, ref bool isLoaderExit) where T : BaseAbstracResourceLoader, new()
        {
            //Debug.LogEditorInfor("GetOrCreateLoaderInstance  url=" + url+"  type="+typeof(T));
            //       url = string.Format(@"{0}", url);
            T resultLoader = null;
            isLoaderExit = false;
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders) == false)
            {
                typeOfLoaders = new Dictionary<string, BaseAbstracResourceLoader>();
                S_AllTypeLoader.Add(typeof(T), typeOfLoaders);
            }

            foreach (var item in typeOfLoaders)
            {
                if (item.Key == url)
                {
                    isLoaderExit = true;
                    resultLoader = (T)item.Value;
                    break;
                }
            }

            if (resultLoader == null)
            {
                resultLoader = new T();
                typeOfLoaders.Add(url, resultLoader);
                resultLoader.InitialLoader();
            }

            return resultLoader;
        }

        /// <summary>
        /// 获取一个指定类型的加载器是否存在 如果存在则返回这个加载器 否则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetExitLoaderInstance<T>(string url) where T : BaseAbstracResourceLoader
        {
            url = string.Format(@"{0}", url);
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders) == false)
                return null;

            foreach (var item in typeOfLoaders)
            {
                if (item.Key == url)
                {
                    return (T)item.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 如果存在 则删除指定类型的加载器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        public static BaseAbstracResourceLoader DeleteExitLoaderInstance(Type loaderType, string url)
        {
            BaseAbstracResourceLoader resultLoader = null;
            url = string.Format(@"{0}", url);
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (S_AllTypeLoader.TryGetValue(loaderType, out typeOfLoaders) == false)
                return null;

            if (typeOfLoaders.ContainsKey(url) == false)
                return null;

            resultLoader = typeOfLoaders[url];
            typeOfLoaders.Remove(url);
            return resultLoader;
        }

        #endregion


        #region 根据不同加载模式合成对应的加载路径

        /// <summary>
        /// 根据不同的加载路径生成不同的资源路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadPathEnum"></param>
        /// <param name="isFileAbsolutelyPath">是否是绝对路径  (使用IO.File 加载时候必须)</param>
        /// <returns></returns>
        public static PathResultEnum GetAssetPathOfLoadAssetPath(ref string url, LoadAssetPathEnum loadPathEnum, bool isFileAbsolutelyPath, AssetTypeTag assetType = AssetTypeTag.None)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("无法识别的路径 " + url);
                return PathResultEnum.Invalid;
            }

            switch (loadPathEnum)
            {
                case LoadAssetPathEnum.PersistentDataPath:
                    url = ConstDefine.S_PersistentDataPath + url;
                    return PathResultEnum.Valid;
                case LoadAssetPathEnum.ResourcesPath:
                    //if (assetType == AssetTypeTag.ShaderAsset)
                    //    return PathResultEnum.Valid;  //Shade 路径不处理
                    if (isFileAbsolutelyPath)
                        url = ConstDefine.S_ResourcesPath + url;
                    return PathResultEnum.Valid;
                //case LoadAssetPathEnum.StreamingAssetsPath:
                //    return PathResultEnum.Valid;
                //case LoadAssetPathEnum.EditorAssetDataPath:
                //    return PathResultEnum.Valid;
                default:
                    break;
            }

            Debug.LogError("没有配置的路径");
            return PathResultEnum.Invalid;
        }

        #endregion

    }
}