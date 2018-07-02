﻿using System.Collections;
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
    ///    Sync, //同步加载 当前帧返回
        Async, //协程异步加载
    }

    /// <summary>
    /// 资源加载时候选择的路径
    /// </summary>
    [System.Serializable]
    public enum LoadAssetPathEnum
    {
        PersistentDataPath,  //外部的资源目录
        ResourcesPath, //Resources路径
        StreamingAssetsPath, //
      //  EditorAssetDataPath,  //编辑器下路径
    }

    /// <summary>
    /// 资源的类型  会根据资源类型处理不同的
    /// </summary>
    public enum AssetTypeTag
    {
        None,  //  不做处理
        ShaderAsset,  //Shader资源
        Material,  //材质球
    }

    /// <summary>
    /// 资源类型和扩展名对应关系
    /// </summary>
    public class AssetTypeAndExtension
    {
        public AssetTypeTag m_AssetTypeTag;  //资源类型
        public string m_ExtensionName; //扩展名
    }


    /// <summary>
    /// 所有其他资源加载器的父类
    /// </summary>
    public abstract class BaseAbstracResourceLoader : IAsyncOperate, IDisposable
    {
        public bool IsCompleted { get; protected set; }
        public bool IsError { get; protected set; }
        public float Process { get; protected set; }

        protected string m_Description = "";
        public string Description
        {
            get
            {
                return m_Description;
            }
            protected set
            {
                m_Description = value;
                if (OnUpdateDescriptionAct != null)
                    OnUpdateDescriptionAct(m_Description);
            }
        }//描述更新 触发事件
        public System.Action<string> OnUpdateDescriptionAct = null; //更新描述信息

        public object ResultObj { get; protected set; }

        /// <summary>
        /// GC回收时间间隔
        /// </summary>
        public virtual float m_GCInterval
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                    return 3f;//编辑器下1秒

                if (UnityEngine.Debug.isDebugBuild)
                    return 5f;  //打包时候开启 Develop Builder 
                return 10f;
            }
        }

        /// <summary>
        /// 资源的唯一标识
        /// </summary>
        public string m_ResourcesUrl { get; protected set; }


        public AssetTypeTag m_AssetTypeTag = AssetTypeTag.None; //资源类型
        public float UnUseTime { get; protected set; } //加载器失效时候的时间


        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferCount { get; protected set; }

        protected BaseAbstracResourceLoader()
        {
            ResetLoader(); //创建 /重置 时候引用计数为1
        }

        /// <summary>
        /// 创建时候初始化
        /// </summary>
        public virtual void InitialLoader()
        {

        }


        #region 加载/卸载资源


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
        }
        /// <summary>
        /// 将资源加载器放入待删除队列时候 释放必要的资源
        /// </summary>
        public virtual void ReleaseLoader()
        {
            ReferCount = 0;
            UnUseTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 增加引用计数
        /// </summary>
        public virtual void AddReference()
        {
            ++ReferCount;
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        public virtual void ReduceReference()
        {
            --ReferCount;
        }

        #endregion

        #region 类型扩展名处理  TODO 

        #endregion

        /// <summary>
        /// 完成下载
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        /// <param name="process"></param>
        protected virtual void OnCompleteLoad(bool isError, string description, object result, float process = 1)
        {
            IsCompleted = true;
            IsError = isError;
            Description = description;
            ResultObj = result;
            Process = process;
        }



        public abstract void Dispose();
    }
}