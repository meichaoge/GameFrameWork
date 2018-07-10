using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 指定Url 的AssetBundle 存在方式
    /// </summary>
    public enum AssetBundleExitState
    {
        SinglePrefab,  //单独的预制体
        FolderPrefab, //按照文件夹目录名打包成一个整体
        None, //不存在这个AssetBundle 资源
    }

    /// <summary>
    /// 加载AssetBundle 资源  (需要区分是加载单个预制体还是打包到一起的资源)
    /// </summary>
    public class AssetBundleLoader : BaseAbstracResourceLoader
    {

        private static string _AssetBundleTopPath;
        /// <summary>
        /// AssetBundle i资源的绝对路径部分
        /// </summary>
        public static string S_AssetBundleTopPath
        {
            get
            {
                if (string.IsNullOrEmpty(_AssetBundleTopPath))
                    _AssetBundleTopPath = ConstDefine.S_AssetBundleTopPath + AssetBundleMgr.Instance.GetAssetBundlePlatformName() + "/";
                return _AssetBundleTopPath;
            }
        }


        public override void InitialLoader()
        {
            base.InitialLoader();
            if (AssetBundleMgr.Instance.S_AssetBundleManifest == null)
            {
                Debug.LogInfor("加载  AssetBundleManifest...");
            }
        }

        /// <summary>
        /// 检测这个url 对应的资源是否存在 (会检测资源路径扩展名是否是。unity3d,)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="newUrl">如果这个url 对应的AssetBundle 资源是以整个文件夹名的预制体，则返回这个整预制体的url(只再这个情况下有效)</param>
        /// <returns>这个资源是否存在 已经存在的方式(单独预制体/整个文件夹名的预制体/不存在)</returns>
        public static AssetBundleExitState CheckIsAssetBundleExit(string url, ref string newUrl)
        {
            string platformtName = AssetBundleMgr.Instance.GetAssetBundlePlatformName();
            string templePath = "";
            if (System.IO.Path.GetExtension(url) != ConstDefine.AssetBundleExtensionName)
                templePath = string.Format("{0}{1}/{2}{3}", ConstDefine.S_AssetBundleTopPath, platformtName, url, ConstDefine.AssetBundleExtensionName);
            else
                templePath = string.Format("{0}{1}/{2}", ConstDefine.S_AssetBundleTopPath, platformtName, url);

            //     Debug.Log("AA assetPersistentPath=" + assetPersistentPath);
            if (System.IO.File.Exists(templePath))
                return AssetBundleExitState.SinglePrefab;

            string shortDirectoryName = templePath.GetPathParentDirectoryName().ToLower(); //当前文件路径父级目录名
            templePath = string.Format("{0}/{1}", System.IO.Path.GetDirectoryName(templePath), shortDirectoryName);
            //         Debug.Log("BB assetPersistentPath=" + assetPersistentPath);
            if (System.IO.Path.GetExtension(templePath) != ConstDefine.AssetBundleExtensionName)
                templePath += ConstDefine.AssetBundleExtensionName;
            if (System.IO.File.Exists(templePath))
            {
                string parentFolderPath = System.IO.Path.GetDirectoryName(url); //当前路径对应的父路径
                string parentFolderPathwithoutExtension = System.IO.Path.GetFileNameWithoutExtension(parentFolderPath);
                newUrl = string.Format("{0}/{1}{2}", parentFolderPathwithoutExtension, shortDirectoryName, ConstDefine.AssetBundleExtensionName);  //返回文件
                Debug.LogInfor("当前AssetBundle 资源是被打成一个统一的AssetBundle ::" + newUrl);

                return AssetBundleExitState.FolderPrefab;
            }
            return AssetBundleExitState.None;
        }

        #region 加载AssetBundle 资源



        /// <summary>
        /// 加载AssetBundle 资源
        /// </summary>
        /// <param name="url">相对于AseetBundle 资源存放路径的路径 (如果是打包成整个预制体则是整个预制体的路径)</param>
        /// <param name="assetFileName">实际加载AssetBundle 时候的文件名称(考虑到加载整个AssetBundle 中一个资源的情况)</param>
        /// <param name="onCompleteAct"></param>
        /// <returns></returns>
        public static AssetBundleLoader LoadAssetBundleAsset(string url,string assetFileName, System.Action<BaseAbstracResourceLoader> onCompleteAct)
        {
            bool isLoaderExit = false;
            AssetBundleLoader assetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<AssetBundleLoader>(url, ref isLoaderExit);
            assetBundleLoader.m_OnCompleteAct.Add(onCompleteAct);

            if (isLoaderExit)
            {
                if (assetBundleLoader.IsCompleted)
                    assetBundleLoader.OnCompleteLoad(assetBundleLoader.IsError, assetBundleLoader.Description, assetBundleLoader.ResultObj, true);  //如果当前加载器已经完成加载 则手动触发事件
                return assetBundleLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            ApplicationMgr.Instance.StartCoroutine(assetBundleLoader.LoadAssetBundleASync(url,  assetFileName, assetBundleLoader));
            return assetBundleLoader;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="url"></param>
        protected void LoadAssetBundleSync(string url, AssetBundleLoader assetBundleLoader)
        {
            //string assetName = url;
            //if (System.IO.Path.GetExtension(assetName) != ConstDefine.AssetBundleExtensionName)
            //    assetName += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            //string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(url);  //获取所有的依赖文件
            //List<AssetBundleLoader> allDependenceAssetLoader = new List<AssetBundleLoader>();  //所有依赖文件的加载器
            //foreach (var item in dependenceAssets)
            //{

            //    bool isLoaderExit = false;
            //    AssetBundleLoader subAssetBundleLoader = ResourcesLoaderMgr.GetLoaderInstance<AssetBundleLoader>(item, ref isLoaderExit);

            //    if (isLoaderExit)
            //    {
            //        if (assetBundleLoader.IsCompleted)
            //            assetBundleLoader.OnCompleteLoad(assetBundleLoader.IsError, assetBundleLoader.Description, assetBundleLoader.ResultObj);  //如果当前加载器已经完成加载 则手动触发事件
            //        else
            //        {
            //            Debug.LogError("LoadAssetBundleSync  有相同URL 的资源正在异步加载。同步加载资源失败  TODO ");
            //        }
            //        return ;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            //    }

            //    allDependenceAssetLoader.Add(assetBundleLoader));
            //} //递归加载资源的依赖项

            //for (int dex=0;dex< allDependenceAssetLoader.Count;++dex)
            //{
            //    if(allDependenceAssetLoader[dex].IsError)
            //    {
            //        Debug.LogError("LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
            //        OnCompleteLoad(false, string.Format("LoadAssetBundle Fail,AssetBundle Path=" + assetBundleLoader.m_ResourcesUrl), null);
            //        return assetBundleLoader;
            //    }
            //} //判断依赖项是否加载完成

            //ByteLoader byteLoader = ByteLoader.LoadAsset(S_AssetBundleTopPath + url, null, LoadAssetModel.Sync);  //加载资源
            //if(byteLoader.IsError)
            //{
            //    OnCompleteLoad(false, string.Format("LoadAssetBundle Fail, AssetBundle Path=" + (S_AssetBundleTopPath + url)), null);
            //    return null;
            //}

            //ResultObj = AssetBundle.LoadFromMemory(byteLoader.ResultBytes, 0); 
            //OnCompleteLoad(ResultObj==null,string.Format("LoadAssetBundleSuccess "+assetName), ResultObj);
            //Debug.LogInfor("加载AssetBundle  成功");
            //return assetBundleLoader;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url"></param>
        protected IEnumerator LoadAssetBundleASync(string url, string assetFileName, AssetBundleLoader assetBundleLoade)
        {
            m_ResourcesUrl = url;
            if (System.IO.Path.GetExtension(m_ResourcesUrl) != ConstDefine.AssetBundleExtensionName)
                m_ResourcesUrl += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(m_ResourcesUrl);  //获取所有的依赖文件
            List<AssetBundleLoader> allDependenceAssetLoader = new List<AssetBundleLoader>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                AssetBundleLoader subAssetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<AssetBundleLoader>(item, ref isLoaderExit);
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                ApplicationMgr.Instance.StartCoroutine(LoadDepdenceAssetBundleAsync(item, subAssetBundleLoader));
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                if (allDependenceAssetLoader[dex].IsCompleted == false)
                    yield return null;

                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoad(false, string.Format("LoadAssetBundle Fail,AssetBundle Path= {0}", allDependenceAssetLoader[dex].m_ResourcesUrl), null, true);
                    yield break;
                }
            } //判断依赖项是否加载完成

            #region WWWLoader 加载
            WWWLoader wwwLoader = WWWLoader.WWWLoadAsset(S_AssetBundleTopPath, m_ResourcesUrl, null);
            if (wwwLoader.IsCompleted == false)
                yield return null;

            if (wwwLoader.IsError)
            {
                OnCompleteLoad(false, string.Format("LoadAssetBundle Fail, AssetBundle Path= {0}", (S_AssetBundleTopPath + m_ResourcesUrl)), null, true);
                WWWLoader.UnLoadAsset(wwwLoader.m_ResourcesUrl);
                yield break;
            }

            ResultObj = (wwwLoader.ResultObj as WWW).assetBundle.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(assetFileName));
            OnCompleteLoad(ResultObj == null, string.Format("LoadAssetBundleSuccess  {0}", m_ResourcesUrl), ResultObj, true);
            WWWLoader.UnLoadAsset(wwwLoader.m_ResourcesUrl);
            #endregion

            #region  ByteLoader 加载
            //ByteLoader byteLoader = ByteLoader.LoadAsset(S_AssetBundleTopPath + assetName, null, LoadAssetModel.Async);  //加载资源
            //if (byteLoader.IsError)
            //{
            //    OnCompleteLoad(false, string.Format("LoadAssetBundle Fail, AssetBundle Path=" + (S_AssetBundleTopPath + assetName)), null, true);
            //    yield break;
            //}
            //AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromMemoryAsync(byteLoader.ResultBytes, 0);
            //if (assetBundleRequest.isDone == false)
            //    yield return null;
            //Debug.Log("System.IO.Path.GetFileName(assetName)" + System.IO.Path.GetFileNameWithoutExtension(assetName));
            //ResultObj = assetBundleRequest.assetBundle.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(assetName));
            //OnCompleteLoad(ResultObj == null, string.Format("LoadAssetBundleSuccess " + assetName), ResultObj, true);
            #endregion

            Debug.LogInfor("加载AssetBundle  成功");
            yield break;
        }

        /// <summary>
        /// 加载当前AssetBundle 依赖的资源(注意 ： 这里加载成功之后不能直接生成)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="assetBundleLoade"></param>
        /// <returns></returns>
        protected virtual IEnumerator LoadDepdenceAssetBundleAsync(string url, AssetBundleLoader depdebceAssetBundleLoader)
        {
            depdebceAssetBundleLoader.m_ResourcesUrl = url;
            if (System.IO.Path.GetExtension(depdebceAssetBundleLoader.m_ResourcesUrl) != ConstDefine.AssetBundleExtensionName)
                depdebceAssetBundleLoader.m_ResourcesUrl += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(depdebceAssetBundleLoader.m_ResourcesUrl);  //获取所有的依赖文件
            List<AssetBundleLoader> allDependenceAssetLoader = new List<AssetBundleLoader>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                AssetBundleLoader subAssetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<AssetBundleLoader>(item, ref isLoaderExit);
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                ApplicationMgr.Instance.StartCoroutine(LoadDepdenceAssetBundleAsync(item, subAssetBundleLoader));
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                if (allDependenceAssetLoader[dex].IsCompleted == false)
                    yield return null;

                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoadDepdenceAsset(false, string.Format("LoadAssetBundle Fail,AssetBundle Path={0}" + allDependenceAssetLoader[dex].m_ResourcesUrl), null, true);
                    yield break;
                }
            } //判断依赖项是否加载完成

            #region WWWLoader 
            WWWLoader wwwLoader = WWWLoader.WWWLoadAsset(S_AssetBundleTopPath, depdebceAssetBundleLoader.m_ResourcesUrl, null);
            if (wwwLoader.IsCompleted == false)
                yield return null;

            if (wwwLoader.IsError)
            {
                OnCompleteLoadDepdenceAsset(false, string.Format("LoadAssetBundle Fail, AssetBundle Path={0}" + (S_AssetBundleTopPath + depdebceAssetBundleLoader.m_ResourcesUrl)), null, true);
                WWWLoader.UnLoadAsset(wwwLoader.m_ResourcesUrl);
                yield break;
            }

            ResultObj = (wwwLoader.ResultObj as WWW).assetBundle;//.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(assetName));
            OnCompleteLoadDepdenceAsset(ResultObj == null, string.Format("LoadAssetBundleSuccess " + depdebceAssetBundleLoader.m_ResourcesUrl), ResultObj, true);
            WWWLoader.UnLoadAsset(wwwLoader.m_ResourcesUrl);
            #endregion

            #region ByteLoader 加载

            //ByteLoader byteLoader = ByteLoader.LoadAsset(S_AssetBundleTopPath + assetName, null, LoadAssetModel.Async);  //加载资源
            //if (byteLoader.IsError)
            //{
            //    OnCompleteLoadDepdenceAsset(false, string.Format("LoadAssetBundle Fail, AssetBundle Path=" + (S_AssetBundleTopPath + assetName)), null, true);
            //    yield break;
            //}

            //AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromMemoryAsync(byteLoader.ResultBytes, 0); //异步加载当前AssetBundel 资源
            //if (assetBundleRequest.isDone == false)
            //    yield return null;

            //ResultObj = assetBundleRequest.assetBundle;
            //if (ResultObj == null)
            //    OnCompleteLoadDepdenceAsset(false, string.Format("LoadAssetBundleFail " + assetName), ResultObj, true);
            //else
            //    OnCompleteLoadDepdenceAsset(true, string.Format("LoadAssetBundleSuccess " + assetName), ResultObj, true);

            #endregion

            depdebceAssetBundleLoader.OnCompleteLoad(false, string.Format("Load DepdenceAssetBundel Success {0}", depdebceAssetBundleLoader.m_ResourcesUrl), depdebceAssetBundleLoader.ResultObj, true);
            //Debug.LogInfor("加载AssetBundle  成功");
            yield break;
        }

        #endregion

        #region 卸载资源
        public static void UnLoadAsset(string url)
        {
            AssetBundleLoader assetBundleLoader = ResourcesLoaderMgr.GetExitLoaderInstance<AssetBundleLoader>(url);
            if (assetBundleLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(WWWLoader));
                return;
            }
            assetBundleLoader.ReduceReference();
        }
        #endregion

        public override void ReduceReference()
        {
            base.ReduceReference();
            if (ReferCount <= 0)
            {
                ResourcesLoaderMgr.DeleteLoader<AssetBundleLoader>(m_ResourcesUrl, false);
            }//引用计数为0时候开始回收资源
        }


        protected void OnCompleteLoadDepdenceAsset(bool isError, string description, object result, bool iscomplete, float process = 1)
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
        }




    }
}
