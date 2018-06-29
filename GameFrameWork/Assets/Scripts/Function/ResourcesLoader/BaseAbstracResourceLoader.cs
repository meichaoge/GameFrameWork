using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace GameFrameWork
{
    /// <summary>
    /// 资源加载模式(同步/异步)
    /// </summary>
    public enum LoadAssetModel
    {
        Sync, //同步加载 当前帧返回
        Async, //协程异步加载
    }

    /// <summary>
    /// 资源加载时候选择的路径
    /// </summary>
    public enum LoadAssetPathEnum
    {
        PersistentDataPath,  //外部的资源目录
        ResourcesPath, //Resources路径
        StreamingAssetsPath, //
        EditorAssetDataPath,  //编辑器下路径
    }


    /// <summary>
    /// 所有其他资源加载器的父类
    /// </summary>
    public abstract class BaseAbstracResourceLoader : IAsyncOperate, IDisposable
    {
        public bool IsCompleted { get; protected set; }
        public bool IsError { get; protected set; }
        public float Process { get; protected set; }
        public string Description { get; protected set; }
        public object ResultObj { get; protected set; }

        /// <summary>
        /// GC回收时间间隔
        /// </summary>
        protected virtual float m_GCInterval
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                    return 1f;//编辑器下1秒

                if (UnityEngine.Debug.isDebugBuild)
                    return 5f;  //打包时候开启 Develop Builder 
                return 10f;
            }
        }

        protected string m_ResourcesUrl; //资源的唯一标识
        public readonly List<System.Action> OnCompleteLoadAct = new List<Action>(); //加载完成事件
        protected float m_DisposeDelayTime = 0f; //(加载完成后引用计数为0时候)延迟销毁的时间
        public float UnUseTime { get; protected set; } //加载器失效时候的时间
        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferCount { get; protected set; }

        protected BaseAbstracResourceLoader()
        {
            ResetLoader(); //创建 /重置 时候引用计数为1
        }


        //protected virtual void LoadAsset<T>(string url, LoadAssetModel loadModel = LoadAssetModel.Async, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        //{
        //    switch (loadModel)
        //    {
        //        case LoadAssetModel.Sync:
        //            SyncLoadAsset(url);
        //            break;
        //        case LoadAssetModel.Async:
        //            ASyncLoadAsset(url);
        //            break;
        //        default:
        //            Debug.LogError("LoadAsset Fail,未定义的加载类型 " + loadModel);
        //            break;
        //    }

        //}
        ///// <summary>
        ///// 同步加载资源
        ///// </summary>
        ///// <param name="url"></param>
        //protected virtual void SyncLoadAsset(string url, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        //{
        //    switch (loadAssetPath)
        //    {
        //        case LoadAssetPathEnum.PersistentDataPath:
        //            SyncLoadAssetOfPersistentData(url);
        //            break;
        //        case LoadAssetPathEnum.ResourcesPath:
        //            SyncLoadAssetOfResources(url);
        //            break;
        //        case LoadAssetPathEnum.StreamingAssetsPath:
        //            break;
        //        case LoadAssetPathEnum.EditorAssetDataPath:
        //            break;
        //        default:
        //            break;
        //    }
        //}

        ///// <summary>
        ///// 异步加载资源
        ///// </summary>
        ///// <param name="url"></param>
        //protected virtual void ASyncLoadAsset(string url, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        //{

        //}

        //#region 同步加载资源
        ///// <summary>
        ///// 同步加载Resources 资源
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name=""></param>
        //protected virtual void SyncLoadAssetOfResources(string url )
        //{
        //    ResultObj= Resources.Load(url);
        //    Process = 1;
        //    IsCompleted = true;
        //    IsError = false;
        //    OnCompleteLoad();
        //}
        ///// <summary>
        ///// 同步加载 PersistentDataPath 资源
        ///// </summary>
        ///// <param name="url"></param>
        //protected virtual void SyncLoadAssetOfPersistentData(string url)
        //{
        //    using (FileStream stream = new FileStream(ConstDefine.S_PersistentDataPath+url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    {


        //    }
        //}
        //#endregion



        protected virtual void OnCompleteLoad()
        {
            for (int index = 0; index < OnCompleteLoadAct.Count; index++)
            {
                if (OnCompleteLoadAct[index] != null)
                    OnCompleteLoadAct[index]();
            }
        }



        /// <summary>
        ///  将Loader重新激活
        /// </summary>
        public virtual void ResetLoader(int refCount=1)
        {
            ReferCount = refCount;
            IsError = IsCompleted = false;
            Process = 0;
            Description = null;
            ResultObj = null;
            OnCompleteLoadAct.Clear();
        }
        /// <summary>
        /// 将资源加载器放入待删除队列时候 释放必要的资源
        /// </summary>
        public virtual void ReleaseLoader()
        {
            ReferCount = 0;
            OnCompleteLoadAct.Clear();
            UnUseTime = Time.realtimeSinceStartup;
        }

        public virtual void AddReference()
        {
            ++ReferCount;
        }

        public abstract void Dispose();
    }
}