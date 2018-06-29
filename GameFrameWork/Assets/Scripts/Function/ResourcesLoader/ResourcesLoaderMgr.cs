using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{


    /// <summary>
    /// 资源加载器管理器
    /// </summary>
    public class ResourcesLoaderMgr : Singleton_Static<ResourcesLoaderMgr>
    {
        /// <summary>
        /// 保存所有类型的加载器
        /// </summary>
        public static Dictionary<Type, Dictionary<string, BaseAbstracResourceLoader>> S_AllTypeLoader = new Dictionary<Type, Dictionary<string, BaseAbstracResourceLoader>>();

        /// <summary>
        /// 使用完成当到达一个时间点时候回被销毁 （Value中的 Queue 按照时间是先入先出）
        /// </summary>
        public static Dictionary<Type, Queue<BaseAbstracResourceLoader>> S_UnUseLoader = new Dictionary<Type, Queue<BaseAbstracResourceLoader>>();

        /// <summary>
        /// 尝试循环利用待删除的加载器 如果不存在则创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RecycleUnUseLoader<T>() where T : BaseAbstracResourceLoader
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
                result = default(T);
            }
            return result;
        }


        /// <summary>
        /// 创建生成一个加载器并开始加载资源
        /// </summary>
        /// <typeparam name="T">加载器类型</typeparam>
        /// <param name="url">资源唯一标识</param>
        /// <param name="completeCallback">加载完成回调</param>
        /// <param name="forceCreateNew">是否强制重新创建</param>
        /// <param name="loadModel">资源加载模式 默认异步加载</param>
        public static void CreateLoader<T>(string url, System.Action completeCallback, bool forceCreateNew = false, LoadAssetModel loadModel = LoadAssetModel.Async, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath) where T : BaseAbstracResourceLoader
        {
            BaseAbstracResourceLoader m_Loader = null;
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders))
            {
                #region 存在这个类型加载器
                List<System.Action> allCompleteAct = new List<Action>();
                allCompleteAct.Add(completeCallback);
                foreach (var loader in typeOfLoaders)
                {
                    if (loader.Key == url)
                    {
                        m_Loader = loader.Value;
                        break;
                    }
                }

                if (m_Loader != null)
                {
                    if (forceCreateNew)
                    {
                        allCompleteAct.AddRange(m_Loader.OnCompleteLoadAct);
                        typeOfLoaders.Remove(url);
                        DeleteLoader<T>(true, m_Loader, url);
                        m_Loader = null;
                    }//如果已经存在这个加载器且需要强制删除 则记录之前的回调事件
                    else
                    {
                        m_Loader = ResourcesLoaderMgr.RecycleUnUseLoader<T>();
                        m_Loader.OnCompleteLoadAct.AddRange(allCompleteAct);
                        m_Loader.AddReference();//增加引用计数
                        typeOfLoaders.Add(url, m_Loader);
                    }
                }
                else
                {
                    m_Loader = ResourcesLoaderMgr.RecycleUnUseLoader<T>();
                    m_Loader.OnCompleteLoadAct.AddRange(allCompleteAct);
                    m_Loader.AddReference();//增加引用计数
                    typeOfLoaders.Add(url, m_Loader);
                }

                #endregion
            }
            else
            {
                typeOfLoaders = new Dictionary<string, BaseAbstracResourceLoader>();
                m_Loader = ResourcesLoaderMgr.RecycleUnUseLoader<T>();
                m_Loader.OnCompleteLoadAct.Add(completeCallback);
                typeOfLoaders.Add(url, m_Loader);
                ResourcesLoaderMgr.S_AllTypeLoader.Add(typeof(T), typeOfLoaders);
            }//不存在这个类型

            //     LoadAsset<T>(url, loadModel);
        }


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

        public static Dictionary<string, BaseAbstracResourceLoader> CheckIfContainLoaderOfType<T>(out bool isRecorded  )
        {
            Dictionary<string, BaseAbstracResourceLoader> typeOfLoaders = null;
            if (ResourcesLoaderMgr.S_AllTypeLoader.TryGetValue(typeof(T), out typeOfLoaders))
            {
                isRecorded = true;
                return typeOfLoaders;
            }
            isRecorded= false;
            typeOfLoaders = new Dictionary<string, BaseAbstracResourceLoader>();
            S_AllTypeLoader.Add(typeof(T), typeOfLoaders);
            return typeOfLoaders;
        }

    }
}