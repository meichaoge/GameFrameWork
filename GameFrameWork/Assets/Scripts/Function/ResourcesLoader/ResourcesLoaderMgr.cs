using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    public enum PathResultEnum
    {
        Valid,
        Invalid,
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
                    return 0.1f;//编辑器下0.1秒

                if (UnityEngine.Debug.isDebugBuild)
                    return 0.3f;  //打包时候开启 Develop Builder 
                return 1;
            }
        }

        private static float lastCheckTime = 0;
   


        /// <summary>
        /// 保存所有类型的加载器
        /// </summary>
        public static Dictionary<Type, Dictionary<string, BaseAbstracResourceLoader>> S_AllTypeLoader = new Dictionary<Type, Dictionary<string, BaseAbstracResourceLoader>>();
        /// <summary>
        /// 使用完成当到达一个时间点时候回被销毁 （Value中的 Queue 按照时间是先入先出）
        /// </summary>
        public static Dictionary<Type, Queue<BaseAbstracResourceLoader>> S_UnUseLoader = new Dictionary<Type, Queue<BaseAbstracResourceLoader>>();

        #region  创建初始化
        public static void InitialResourcesMgr()
        {
           
        }
        #endregion


        #region 创建加载器

        /// <summary>
        /// 创建生成一个加载器并开始加载资源
        /// </summary>
        /// <typeparam name="T">加载器类型</typeparam>
        /// <param name="url">资源唯一标识</param>
        ///// <param name="completeCallback">加载完成回调</param>
        ///// <param name="forceCreateNew">是否强制重新创建</param>
        ///// <param name="loadModel">资源加载模式 默认异步加载</param>
        public static void CreateLoader<T>(System.Action completeCallback) where T : BaseAbstracResourceLoader, new()
        {
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders)==false)
            {
                typeOfLoaders = new Dictionary<string, BaseAbstracResourceLoader>();
                ResourcesLoaderMgr.S_AllTypeLoader.Add(typeof(T), typeOfLoaders);
            }//不存在这个类型


            if (completeCallback != null)
                completeCallback();
        
        }
        #endregion

        #region 删除加载器  添加到待删除队列中

        /// <summary>
        ///  将Loader 删除放到不使用的字典中等待删除(需要判断引用计数是否小于1)
        /// </summary>
        /// <param name="loader">Loader.</param>
        public static void DeleteLoader<T>(bool isForceDelete, BaseAbstracResourceLoader loader, string url)
        {
            Queue<BaseAbstracResourceLoader> typeOfLoaders = null;
            if (S_UnUseLoader.TryGetValue(typeof(T), out typeOfLoaders) == false)
            {
                typeOfLoaders = new Queue<BaseAbstracResourceLoader>();
                S_UnUseLoader.Add(typeof(T), typeOfLoaders);
            }
            if (isForceDelete)
            {
                loader.ReleaseLoader();
                typeOfLoaders.Enqueue(loader);  //释放资源加载器资源并加入队列中
                return;
            }

            if (loader.ReferCount > 0)
            {
                Debug.LogInfor("DeleteLoader Fail,加载器的引用不为0,如果需要强制删除 请设置参数isForceDelete=true    ");
                return;
            }
            loader.ReleaseLoader();
            typeOfLoaders.Enqueue(loader);  //释放资源加载器资源并加入队列中
            Debug.Log("回收加载器  " + typeof(T) + "::" + url);
        }
        #endregion

        #region 获取一个指定类型的加载器 避免创建资源

        /// <summary>
        /// 尝试循环利用待删除的加载器 如果不存在则创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RecycleUnUseLoader<T>() where T : BaseAbstracResourceLoader,new()
        {
            Queue<BaseAbstracResourceLoader> typeOfLoaders = null;
            T result = null;
            if (S_UnUseLoader.TryGetValue(typeof(T), out typeOfLoaders))
            {
                if (typeOfLoaders.Count > 0)
                {
                    result = (T)typeOfLoaders.Dequeue();
                    result.ResetLoader();
                }
            }

            if (result == null)
            {
                result =new T() ;
            }
            return result;
        }
        #endregion

        #region 检测是否需要释放加载器资源
        public static void Tick()
        {
          if(Time.realtimeSinceStartup- lastCheckTime>= S_CheckGCTime)
            {
                CheckLoaderStateForGC();
                lastCheckTime = Time.realtimeSinceStartup;
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
        /// 检测并获取指定类型的加载器是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRecorded"></param>
        /// <returns></returns>
        public static bool CheckIfContainLoaderOfType<T>()  where T: BaseAbstracResourceLoader, new()
        {
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检获取指定类型的加载器 如果不存在则创建一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isRecorded"></param>
        /// <returns></returns>
        public static Dictionary<string, BaseAbstracResourceLoader> GetLoaderOfType<T>(ref bool isRecorded)
        {
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders))
            {
                isRecorded = true;
                return typeOfLoaders;
            }
            isRecorded = false;
            typeOfLoaders = new Dictionary<string, BaseAbstracResourceLoader>();
            S_AllTypeLoader.Add(typeof(T), typeOfLoaders);
            return typeOfLoaders;
        }

        /// <summary>
        /// 在指定类型的加载器集合中查找一个加载器 并返回这个加载器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader">查找返回的加载器(为null标识没有找打)</param>
        /// <param name="url">加载器对应的路径</param>
        /// <param name="searchArrange">查找的范围</param>
        /// <param name="OnFindLoaderAct">r如果找到这个加载器执行的操作</param>
        public static void GetLoaderOfTypeAndUrl<T>(ref T loader, string url, Dictionary<string, BaseAbstracResourceLoader> searchArrange, System.Action OnFindLoaderAct) where T : BaseAbstracResourceLoader, new()
        {
            loader = null;
            if (searchArrange == null)
                return;

            foreach (var item in searchArrange)
            {
                if (item.Key == url)
                {
                    loader = (T)item.Value;
                    if (OnFindLoaderAct != null)
                        OnFindLoaderAct();
                    break;
                }
            }
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
        public static PathResultEnum GetAssetPathOfLoadAssetPath(ref string url, LoadAssetPathEnum loadPathEnum,bool isFileAbsolutelyPath)
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
                    if (isFileAbsolutelyPath)
                        url = ConstDefine.S_ResourcesPath + url;
                    return PathResultEnum.Valid;
                case LoadAssetPathEnum.StreamingAssetsPath:
                    return PathResultEnum.Valid;
                case LoadAssetPathEnum.EditorAssetDataPath:
                    return PathResultEnum.Valid;
                default:
                    break;
            }

            Debug.LogError("没有配置的路径");
            return PathResultEnum.Invalid;
        }

        #endregion

    }
}