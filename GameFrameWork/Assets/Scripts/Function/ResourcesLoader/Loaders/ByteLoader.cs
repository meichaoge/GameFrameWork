using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    public delegate void CompleteByteLoaderHandler(bool isError, byte[] data);

    /// <summary>
    /// 以字节流方式加载资源
    /// </summary>
    public class ByteLoader : BaseAbstracResourceLoader
    {
        public byte[] ResultBytes { get { return ResultObj as byte[]; } }

        public readonly List<CompleteByteLoaderHandler> m_AllCompleteLoader = new List<CompleteByteLoaderHandler>(); 


        public static void Load<T>(string url, CompleteByteLoaderHandler completeHandler,bool isForceCreateNew, LoadAssetModel loadModel = LoadAssetModel.Async, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
        {
            bool isContainLoaders = false;
            Dictionary<string, BaseAbstracResourceLoader> resultLoaders = ResourcesLoaderMgr.CheckIfContainLoaderOfType<T>(out isContainLoaders);
            ByteLoader byteLoader = null;
            foreach (var item in resultLoaders)
            {
                if(item.Key==url)
                {
                    byteLoader = item.Value as ByteLoader;
                    break;
                }
            }


            if (byteLoader!=null)
            {
                if(isForceCreateNew)
                {
                    byteLoader.ResetLoader(0);
                    byteLoader.m_AllCompleteLoader.Clear();
                }
            }//已经存在

            if (byteLoader == null)
            {
                byteLoader = new ByteLoader();
                resultLoaders.Add(url,byteLoader);
            }


        }


        public override void Dispose()
        {
           
        }
    }
}