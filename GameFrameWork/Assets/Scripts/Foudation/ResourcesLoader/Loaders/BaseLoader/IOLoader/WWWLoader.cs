using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{

    /// <summary>
    /// 以WWW方式加载资源(只支持异步加载)
    /// </summary>
    public class WWWLoader : BaseAbstracResourceLoader
    {
        /// <summary>
        /// 标识加载的路径是否存在
        /// </summary>
        public bool IsPathExit { get; private set; }

        protected string m_TopPath;  //url 目录前面的绝对目录


        public override void InitialLoader()
        {
            base.InitialLoader();
            IsPathExit = true;
            LoadassetModel = LoadAssetModel.Async;
        }

        #region  加载资源
        /// <summary>
        /// WWW 方式异步加载资源
        /// </summary>
        /// <param name="topPath">资源相对目录前的绝对目录</param>
        /// <param name="url">资源相对目录</param>
        /// <param name="onCompleteAct"></param>
        /// <returns></returns>
        public static WWWLoader WWWLoadAsset(string topPath, LoadAssetModel loadModel, string url, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            if(loadModel!= LoadAssetModel.Async)
            {
                Debug.LogError("WWWLoader  只支持异步加载资源");
                return null;
            }
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(WWWLoader)));
                return null;
            }
            bool isLoaderExit = false;
            Debug.LogInfor("WWWLoadAsset  url=" + url);


            WWWLoader wwwLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<WWWLoader>(url, ref isLoaderExit);
            wwwLoader.m_OnCompleteAct.Add(onCompleteAct);

            if (isLoaderExit)
            {
                if (wwwLoader.IsCompleted)
                    wwwLoader.OnCompleteLoad(wwwLoader.IsError, wwwLoader.Description, wwwLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return wwwLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            wwwLoader. m_TopPath = topPath;
            wwwLoader. m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(wwwLoader.LoadAssetAsync(topPath, url, wwwLoader));
            return wwwLoader;
        }


        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="topPath"></param>
        /// <param name="url"></param>
        /// <param name="wwwLoader"></param>
        /// <returns></returns>
        protected IEnumerator LoadAssetAsync(string topPath, string url, WWWLoader wwwLoader)
        {
            string fileAbsolutelyPath = string.Format("{0}{1}", topPath, url);  //当先需要下载的资源的绝对路径
            if (System.IO.File.Exists(fileAbsolutelyPath) == false)
            {
                IsPathExit = false;
                base.OnCompleteLoad(true, string.Format("File: {0}  Not Exit", fileAbsolutelyPath), null, true, 0);
                Debug.LogInfor("文件路径不存在  " + fileAbsolutelyPath);
                yield break;
            }
            m_ResourcesUrl = url;
            IsPathExit = true;
            WWW www = new WWW(string.Format(@"file:///{0}", fileAbsolutelyPath)); //WWW 下载路径必须加上 file:///  
            while (www.isDone == false)
            {
                Process = www.progress;
                yield return null;
            }
            if (string.IsNullOrEmpty(www.error))
                OnCompleteLoad(false, string.Format("Complete WWW Load {0}", fileAbsolutelyPath),www, true);
            else
                OnCompleteLoad(true, www.error, null, true);
        }
        #endregion

        #region  卸载资源

        /// <summary>
        /// 卸载指定类型的加载器
        /// </summary>
        /// <param name="url"></param>
        public static void UnLoadAsset(string url,bool isForceDelete=false)
        {
            WWWLoader wwwLoader = ResourcesLoaderMgr.GetExitLoaderInstance<WWWLoader>(url);
            if (wwwLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(WWWLoader));
                return;
            }
            wwwLoader.ReduceReference(wwwLoader, isForceDelete);
        }
        #endregion

        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if (LoadassetModel != LoadAssetModel.Async)
            {
                Debug.LogError("非异步加载方式不需要强制结束 " + LoadassetModel);
                return;
            }
            if (m_LoadAssetCoroutine!=null)
            EventCenter.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }

        public override void Dispose()
        {
            if (ResultObj != null&& ResultObj is WWW)
                (ResultObj as WWW).Dispose();
            ResultObj = null;
            base.Dispose();

        }




    }
}