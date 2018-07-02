//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//namespace GameFrameWork
//{
//    public delegate void CompleteMaterialLoaderHandler(bool isError, Material data);
//    /// <summary>
//    /// 加载材质球
//    /// </summary>
//    public class MaterialLoader : BaseAbstracResourceLoader
//    {
//        public MaterialLoader():base()
//        {
//            m_AssetTypeTag = AssetTypeTag.Material;
//        }

//        /// <summary>
//        /// 最终加载的材质球资源
//        /// </summary>
//        public Material ResultMayerial
//        {
//            get
//            {
//                if (ResultObj == null)
//                    return null;
//                return ResultObj as Material;
//            }
//        }

//        /// <summary>
//        /// 加载完成的回调
//        /// </summary>
//        public readonly List<CompleteMaterialLoaderHandler> m_AllCompleteLoader = new List<CompleteMaterialLoaderHandler>();



//        /// <summary>
//        /// 生成一个指定路径的加载器并加载资源
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="url">资源唯一路径</param>
//        /// <param name="completeHandler">加载完成回调</param>
//        /// <param name="loadModel">加载模式(同步/异步) default=异步</param>
//        /// <param name="loadAssetPath">加载资源路径模式(外部/Resources/StreamAsset ) default=ResourcesPath</param>
//        public static MaterialLoader LoadAsset(string url, CompleteMaterialLoaderHandler completeHandler, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
//        {
//            bool isContainLoaders = false;
//            Dictionary<string, BaseAbstracResourceLoader> resultLoaders = ResourcesLoaderMgr.GetLoaderOfType<ByteLoader>(ref isContainLoaders);
//            MaterialLoader materialShader = null;
//            foreach (var item in resultLoaders)
//            {
//                if (item.Key == url)
//                {
//                    materialShader = item.Value as MaterialLoader;
//                    break;
//                }
//            }

//            if (materialShader != null)
//            {
//                materialShader.AddReference();
//                if (completeHandler != null)
//                    completeHandler(materialShader.IsError, materialShader.ResultMayerial);
//            }
//            else
//            {
//                materialShader = new MaterialLoader();
//                materialShader.m_ResourcesUrl = url;
//                resultLoaders.Add(url, materialShader);
//                materialShader.m_AllCompleteLoader.Add(completeHandler);
//                materialShader.LoadMaterial(url, loadAssetPath);

//            }
//            return materialShader;
//        }


//        /// <summary>
//        /// 加载材质球  (需要先加载Shader )
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="loadAssetPath"></param>
//        protected virtual void LoadMaterial(string url, LoadAssetModel loadModel = LoadAssetModel.Async, LoadAssetPathEnum loadAssetPath = LoadAssetPathEnum.ResourcesPath)
//        {
//            if (ResourcesLoaderMgr.GetAssetPathOfLoadAssetPath(ref url, loadAssetPath, false, m_AssetTypeTag) == PathResultEnum.Invalid)
//            {
//                OnCompleteLoad(true, string.Format("Path is Invalidate {0}", url), null);
//                return;
//            }
//            switch (loadModel)
//            {
//                case LoadAssetModel.Sync:
//                    LoadMaterialAssetSync(url);
//                    break;
//                case LoadAssetModel.Async:
//                    LoadMaterialAssetASync(url);
//                    break;
//                default:
//                    Debug.LogError("无法识别的加载模式 " + loadModel);
//                    break;
//            }

//            //ResultObj = Shader.Find(url);
//            //if (ResultObj == null)
//            //{
//            //    Debug.LogError("加载失败");
//            //    OnCompleteLoad(true, string.Format("Path is Invalidate {0}", url), null);
//            //    return;
//            //}

//            //AddReference();
//            //Debug.LogInfor("LoadShader Success");
//            //OnCompleteLoad(false, string.Format("CompleteLoad: {0}", url), ResultObj);
//        }

//        protected virtual void LoadMaterialAssetSync(string path)
//        {
//        }

//        protected virtual void LoadMaterialAssetASync(string path)
//        {

//        }

//        public override void Dispose()
//        {

//        }
//    }
//}