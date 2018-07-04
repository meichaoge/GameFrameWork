using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork
{

    /// <summary>
    /// 以字节流方式加载资源
    /// </summary>
    public class ByteLoader : BaseAbstracResourceLoader
    {
        protected byte[] m_Data;
        protected FileStream fileStream = null;  //读取文件的流对象

        public override void InitialLoader()
        {
            base.InitialLoader();

        }

        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static ByteLoader LoadAsset(string url, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            bool isLoaderExit = false;
            ByteLoader byteLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<ByteLoader>(url, ref isLoaderExit);
            byteLoader.m_OnCompleteAct.Add(onCompleteAct);

            if (isLoaderExit)
            {
                if (byteLoader.IsCompleted)
                    byteLoader.OnCompleteLoad(byteLoader.IsError, byteLoader.Description, byteLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return byteLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            byteLoader.LoadByteAssetASync(url);
            return byteLoader;
        }

        #region 资源加载
        /// <summary>
        /// 根据参数指定的加载方式和优先选择的路径加载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadModel"></param>
        /// <param name="loadAssetPath"></param>
        protected virtual void LoadByteAsset(string url, LoadAssetModel loadModel = LoadAssetModel.Async)
        {
            //if (ResourcesLoaderMgr.GetAssetPathOfLoadAssetPath(ref url, loadAssetPath,true ,m_AssetTypeTag) == PathResultEnum.Invalid)
            //{
            //    OnCompleteLoad(true,string.Format("Path is Invalidate {0}" , url),null);
            //    return;
            //}

            //switch (loadModel)
            //{
            //    case LoadAssetModel.Sync:
            //        LoadByteAssetSync(url);
            //        break;
            //    case LoadAssetModel.Async:
            //        LoadByteAssetASync(url);
            //        break;
            //    default:
            //        Debug.LogError("无法识别的加载模式 "+ loadModel);
            //        break;
            //}

            LoadByteAssetASync(url);
        }


        /// <summary>
        /// 同步加载资源
        /// </summary>
        protected virtual void LoadByteAssetSync(string path)
        {
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
                Debug.LogError("LoadByteAssetSync  Fail,error" + ex.Message);
                OnCompleteLoad(IsError, ex.Message, null, true);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

        }

        /// <summary>
        /// 异步加载
        /// </summary>
        protected virtual void LoadByteAssetASync(string path)
        {
            Debug.Log("LoadByteAssetASync  url=" + path);
            if (System.IO.File.Exists(path) == false)
            {
                Debug.LogError(string.Format("Load File Not Exit Path:{0}", path));
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
                Debug.LogError("LoadByteAssetASync  Fail,error {0}" + ex.Message);
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

        #region 资源卸载
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="url"></param>
        public static void UnLoadAsset(string url)
        {
            ByteLoader byteLoader = ResourcesLoaderMgr.GetExitLoaderInstance<ByteLoader>(url);
            if (byteLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }
            byteLoader.ReduceReference();
         
        }
        #endregion

        public override void ReduceReference()
        {
            base.ReduceReference();
            if (ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<ByteLoader>(m_ResourcesUrl, false);
            }//引用计数为0时候开始回收资源
        }


        public override void Dispose()
        {
            m_Data = null;
            base.Dispose();

        }
    }
}