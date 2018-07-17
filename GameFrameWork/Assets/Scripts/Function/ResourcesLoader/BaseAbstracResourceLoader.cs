using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace GameFrameWork
{
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
        [SerializeField]
        protected object m_RequesterTarget;  //请求加载资源的对象

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


        public float UnUseTime { get; protected set; } //加载器失效时候的时间


        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferCount { get; protected set; }

        protected readonly HashSet<System.Action<BaseAbstracResourceLoader>> m_OnCompleteAct = new HashSet<System.Action<BaseAbstracResourceLoader>>(); //加载完成回调



        protected BaseAbstracResourceLoader()
        {
            ResetLoader(0); //创建  时候引用计数为0
        }

        /// <summary>
        /// 创建时候初始化
        /// </summary>
        public virtual void InitialLoader()
        {
            IsCompleted = false;
            IsError = false;
            Process = 0;
            m_Description = "";
            ResultObj = null;
        }


        #region 加载/卸载资源

        /// <summary>
        ///  将Loader重新激活
        /// </summary>
        public virtual void ResetLoader(int refCount = 1)
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
        protected virtual void AddReference(GameObject requestTarget, string url)
        {
            ++ReferCount;
        }

        ///// <summary>
        ///// 减少引用计数
        ///// </summary>
        //protected  void ReduceReference<T>(bool isForcesDelete) where T:BaseAbstracResourceLoader
        //{
        //    ReduceReference(typeof(T), isForcesDelete);
        //}
        protected  void ReduceReference( bool isForcesDelete)
        {
            ReduceReference(this.GetType(), isForcesDelete);
        }

        public void ReduceReference(BaseAbstracResourceLoader loader,bool isForcesDelete)
        {
            ReduceReference(loader.GetType(), isForcesDelete);
        }

        protected virtual void ReduceReference(Type loaderType,bool isForcesDelete) 
        {
            --ReferCount;
            if (ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader(loaderType, m_ResourcesUrl, false);
            }//引用计数为0时候开始回收资源
        }


        #endregion


        #region  状态更新接口
        //public void UpdateLoadingState(AsyncOperation asyncOperate)
        //{
        //    IsCompleted = asyncOperate.isDone;
        //    Process = asyncOperate.progress;
        //    IsError = false;
        //}
        #endregion

        /// <summary>
        /// 完成下载
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        /// <param name="process"></param>
        protected virtual void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            IsCompleted = iscomplete;
            IsError = isError;
            Description = description;
            ResultObj = result;
            Process = process;

#if UNITY_EDITOR
            if (IsError)
                Debug.LogError(Description);
            else
                Debug.LogInfor(Description);
#endif


            foreach (var item in m_OnCompleteAct)
            {
                if (item != null)
                    item(this);
            }
            m_OnCompleteAct.Clear();

        }



        public virtual void Dispose()
        {
            m_OnCompleteAct.Clear();
            ResultObj = null;
        }
    }
}