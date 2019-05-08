using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{

    /// <summary>
    /// 以字节流方式加载资源
    /// </summary>
    public class ByteLoader : BaseAbstracResourceLoader
    {
        protected byte[] m_Data;
        protected FileStream fileStream = null;  //读取文件的流对象




        #region 资源加载
        /// <summary>
        /// 资源加载统一接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadModel">同步异步加载模式</param>
        /// <param name="onCompleteAct"></param>
        /// <returns></returns>
        public static ByteLoader LoadAsset(string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAssetSync(url, onCompleteAct);
                case LoadAssetModel.Async:
                    return LoadAssetAsync(url, onCompleteAct);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }



        #region  同步加载
        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        protected static ByteLoader LoadAssetSync(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(ByteLoader)));
                return null;
            }
            bool isLoaderExit = false;
            ByteLoader byteLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ByteLoader>(url, ref isLoaderExit);
            byteLoader.m_OnCompleteAct.Add(onCompleteAct);

            if (isLoaderExit && byteLoader.IsCompleted)
            {
                byteLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要
                byteLoader.OnCompleteLoad(byteLoader.IsError, byteLoader.Description, byteLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return byteLoader;
            }//如果当前加载器存在 且已经加载完成已经完成加载 则手动触发事件

            if (byteLoader.LoadassetModel == LoadAssetModel.Async)
            {
                Debug.LogEditorInfor(string.Format("有资源({0})正在异步加载过程中，同时有同步加载请求 ，则之前的加载逻辑作废!! ", url));
                byteLoader.ForceBreakLoaderProcess();
            }//有一个正在异步加载的加载器运行着
            byteLoader.LoadassetModel = LoadAssetModel.Sync;
            byteLoader.LoadByteAssetSync(url);
            return byteLoader;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        protected virtual void LoadByteAssetSync(string path)
        {
            m_ResourcesUrl = path;
            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_Data = new byte[fileStream.Length];
                int realCount = fileStream.Read(m_Data, 0, m_Data.Length);
                if (realCount != m_Data.Length)
                    Debug.LogError("数据长度不统一 " + m_Data.Length + "::" + realCount);
                else
                    Debug.Log("读取完成路径" + m_ResourcesUrl + " 文件大小 " + m_Data.Length);

                OnCompleteLoad((m_Data.Length == 0), string.Format("CompleteLoad: {0}", m_ResourcesUrl), m_Data, true);
            }
            catch (System.Exception ex)
            {

            //    Debug.LogError("LoadByteAssetSync  Fail,error" + ex.Message);
                OnCompleteLoad(IsError, ex.Message, null, true);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

        }
        #endregion

        #region  异步加载
        /// <summary>
        /// (异步)生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        protected static ByteLoader LoadAssetAsync(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(ByteLoader)));
                return null;
            }
            bool isLoaderExit = false;
            ByteLoader byteLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ByteLoader>(url, ref isLoaderExit);
            byteLoader.m_OnCompleteAct.Add(onCompleteAct);
            byteLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）

            if (isLoaderExit)
            {
                if (byteLoader.IsCompleted)
                    byteLoader.OnCompleteLoad(byteLoader.IsError, byteLoader.Description, byteLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return byteLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            byteLoader.LoadByteAssetASync(url);
            return byteLoader;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        protected virtual void LoadByteAssetASync(string path)
        {
            m_ResourcesUrl = path;
            Debug.Log("[ByteLoader] loadByteAssetASync  url=" + path);
            if (System.IO.File.Exists(path) == false)
            {
                //Debug.LogError(string.Format("Load File Not Exit Path:{0}", path));
                OnCompleteLoad(true, string.Format("Load File Not Exit Path:{0}", path), null, true);
                return;
            }

            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_Data = new byte[fileStream.Length];
                fileStream.BeginRead(m_Data, 0, m_Data.Length, CompleteAsyncRead, fileStream);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(" [ByteLoader] LoadByteAssetASync  Fail,error {0}" + ex.Message);
                OnCompleteLoad(true, ex.Message, null, true);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>
        /// FileStream 异步读取回调
        /// </summary>
        /// <param name="asyncResult"></param>
        protected virtual void CompleteAsyncRead(IAsyncResult asyncResult)
        {
            FileStream stream = asyncResult.AsyncState as FileStream;
            int realDataCount = stream.EndRead(asyncResult);
            if (realDataCount != m_Data.Length)
                Debug.LogError("数据长度不统一 :" + m_Data.Length + "::" + realDataCount);
            else
                Debug.Log("读取完成路径:" + m_ResourcesUrl + " 文件大小 " + m_Data.Length);

            stream.Close();
            OnCompleteLoad((m_Data.Length == 0), string.Format("CompleteLoad: {0}", m_ResourcesUrl), m_Data, true);
        }
        #endregion

        #endregion

        #region 资源卸载
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="url"></param>
        public static void UnLoadAsset(string url, bool isForceDelete = false)
        {
            ByteLoader byteLoader = ResourcesLoaderMgr.GetExitLoaderInstance<ByteLoader>(url);
            if (byteLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }
            byteLoader.ReduceReference(byteLoader, isForceDelete);

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
            if (fileStream != null)
                fileStream.Close();
        }

        public override void Dispose()
        {
            m_Data = null;
            base.Dispose();

        }
    }
}