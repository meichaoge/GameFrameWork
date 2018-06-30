using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork
{
    public delegate void CompleteByteLoaderHandler(bool isError, byte[] data);

    /// <summary>
    /// 以字节流方式加载资源
    /// </summary>
    public class ByteLoader : BaseAbstracResourceLoader
    {
        /// <summary>
        /// 最终加载的资源
        /// </summary>
        public byte[] ResultBytes
        {
            get
            {
                if (ResultObj != null)
                    return ResultObj as byte[];
                return new byte[0];
            }
        }

        protected byte[] m_Data;
        protected FileStream fileStream = null;  //读取文件的流对象
        /// <summary>
        /// 加载完成的回调
        /// </summary>
        public readonly List<CompleteByteLoaderHandler> m_AllCompleteLoader = new List<CompleteByteLoaderHandler>();

        /// <summary>
        /// 生成一个指定路径的加载器并加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">资源唯一路径</param>
        /// <param name="completeHandler">加载完成回调</param>
        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
        public static ByteLoader LoadAsset(string url, CompleteByteLoaderHandler completeHandler, LoadAssetModel loadModel = LoadAssetModel.Async, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath) 
        {
            bool isContainLoaders = false;
            Dictionary<string, BaseAbstracResourceLoader> resultLoaders = ResourcesLoaderMgr.GetLoaderOfType<ByteLoader>(ref isContainLoaders);
            ByteLoader byteLoader = null;
            foreach (var item in resultLoaders)
            {
                if (item.Key == url)
                {
                    byteLoader = item.Value as ByteLoader;
                    break;
                }
            }

            if (byteLoader == null)
            {
                byteLoader = new ByteLoader();
                byteLoader.m_ResourcesUrl = url;
                resultLoaders.Add(url, byteLoader);
                byteLoader.m_AllCompleteLoader.Add(completeHandler);
                byteLoader. LoadByteAsset(url, loadModel, loadAssetPath);
            }
            else
            {
                byteLoader.AddReference();
                if (completeHandler != null)
                    completeHandler(byteLoader.IsError, byteLoader.ResultBytes);
            }
            return byteLoader;

        }

        #region 资源加载
        /// <summary>
        /// 根据参数指定的加载方式和优先选择的路径加载资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadModel"></param>
        /// <param name="loadAssetPath"></param>
        protected virtual void LoadByteAsset(string url, LoadAssetModel loadModel = LoadAssetModel.Async, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        {
            if (ResourcesLoaderMgr.GetAssetPathOfLoadAssetPath(ref url, loadAssetPath,true) == PathResultEnum.Invalid)
            {
                OnCompleteLoad(true,string.Format("Path is Invalidate {0}" , url),null);
                return;
            }

            switch (loadModel)
            {
                case LoadAssetModel.Sync:
                    LoadByteAssetSync(url);
                    break;
                case LoadAssetModel.Async:
                    LoadByteAssetASync(url);
                    break;
                default:
                    Debug.LogError("无法识别的加载模式 "+ loadModel);
                    break;
            }
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
                if(realCount!= m_Data.Length)
                    Debug.LogError("数据长度不统一 " + m_Data.Length+"::"+ realCount);
                else
                    Debug.Log("读取完成路径" + m_ResourcesUrl + " 文件大小 " + m_Data.Length);

                OnCompleteLoad((m_Data.Length == 0), string.Format("CompleteLoad: {0}", m_ResourcesUrl), m_Data);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadByteAssetSync  Fail,error" + ex.Message);
                OnCompleteLoad(IsError, ex.Message, null);
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
            Debug.Log(System.IO.File.Exists(path));

            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_Data = new byte[fileStream.Length];
                fileStream.BeginRead(m_Data, 0, m_Data.Length, CompleteAsyncRead, fileStream);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadByteAssetASync  Fail,error" + ex.Message);
                OnCompleteLoad(true,ex.Message,null);
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
            OnCompleteLoad((m_Data.Length == 0),string.Format("CompleteLoad: {0}", m_ResourcesUrl), m_Data);
        }

        #endregion

        #region 资源卸载
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="url"></param>
        public static void UnLoadAsset(string url)
        {
            bool isContainLoaders = false;
            Dictionary<string, BaseAbstracResourceLoader> resultLoaders = ResourcesLoaderMgr.GetLoaderOfType<ByteLoader>(ref isContainLoaders);
            if (isContainLoaders == false)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(ByteLoader));
                return;
            }

            ByteLoader byteLoader = null;
            ResourcesLoaderMgr.GetLoaderOfTypeAndUrl<ByteLoader>(ref byteLoader, url, resultLoaders, null);
            if(byteLoader==null)
            {
                //Debug.LogError("UnLoadAsset Fail  ,无法找到指定Url 的加载器 : " + url);
                return;
            }
            byteLoader.ReduceReference();
            if (byteLoader.ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<ByteLoader>(url, false);
            }//引用计数为0时候开始回收资源

        }
        #endregion



        protected override void OnCompleteLoad(bool isError, string description, object result, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, process);
            for (int dex = 0; dex < m_AllCompleteLoader.Count; ++dex)
            {
                if (m_AllCompleteLoader[dex] != null)
                    m_AllCompleteLoader[dex](this.IsError, this.ResultBytes);
            }
        }


        public override void Dispose()
        {
            m_Data = null;
            m_AllCompleteLoader.Clear();
        }
    }
}