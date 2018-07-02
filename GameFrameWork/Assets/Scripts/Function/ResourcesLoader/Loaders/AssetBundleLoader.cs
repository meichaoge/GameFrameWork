using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 加载AssetBundle 资源
    /// </summary>
    public class AssetBundleLoader : BaseAbstracResourceLoader
    {
        //public AssetBundle ResultAssetBundle
        //{
        //    get
        //    {
        //        return ResultObj as AssetBundle;
        //    }
        //}
        public HashSet<System.Action<object>> m_AllCompleteAct = new HashSet<System.Action<object>>();  //加载完成回调
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

        public static void LoadAssetBundleAsset(string url, System.Action<object> onCompleteAct)
        {
            bool isLoaderExit = false;
            AssetBundleLoader assetBundleLoader = ResourcesLoaderMgr.GetLoaderInstance<AssetBundleLoader>(url, ref isLoaderExit);
            assetBundleLoader.m_AllCompleteAct.Add(onCompleteAct);

            if (isLoaderExit)
            {
                if (assetBundleLoader.IsCompleted)
                    assetBundleLoader.OnCompleteLoad(assetBundleLoader.IsError, assetBundleLoader.Description, assetBundleLoader.ResultObj);  //如果当前加载器已经完成加载 则手动触发事件
                return;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            ApplicationMgr.Instance.StartCoroutine(assetBundleLoader.LoadAssetBundleASync(url, assetBundleLoader));
        }


        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="url"></param>
        public void LoadAssetBundleSync(string url, AssetBundleLoader assetBundleLoader)
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
        public IEnumerator LoadAssetBundleASync(string url, AssetBundleLoader assetBundleLoade )
        {
            string assetName = url;
            if (System.IO.Path.GetExtension(assetName) != ConstDefine.AssetBundleExtensionName)
                assetName += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(assetName);  //获取所有的依赖文件
            List<AssetBundleLoader> allDependenceAssetLoader = new List<AssetBundleLoader>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                AssetBundleLoader subAssetBundleLoader = ResourcesLoaderMgr.GetLoaderInstance<AssetBundleLoader>(item, ref isLoaderExit);
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                ApplicationMgr.Instance.StartCoroutine(LoadDepdenceAssetBundleAsync(item, null));
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                if (allDependenceAssetLoader[dex].IsCompleted == false)
                    yield return null;

                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoad(false, string.Format("LoadAssetBundle Fail,AssetBundle Path=" + allDependenceAssetLoader[dex].m_ResourcesUrl), null);
                    yield break;
                }
            } //判断依赖项是否加载完成

            ByteLoader byteLoader = ByteLoader.LoadAsset(S_AssetBundleTopPath + assetName, null, LoadAssetModel.Async);  //加载资源
            if (byteLoader.IsError)
            {
                OnCompleteLoad(false, string.Format("LoadAssetBundle Fail, AssetBundle Path=" + (S_AssetBundleTopPath + assetName)), null);
                yield break;
            }

            AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromMemoryAsync(byteLoader.ResultBytes, 0);
            if (assetBundleRequest.isDone == false)
                yield return null;
            Debug.Log("System.IO.Path.GetFileName(assetName)" + System.IO.Path.GetFileNameWithoutExtension(assetName));
            ResultObj = assetBundleRequest.assetBundle.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(assetName));
            OnCompleteLoad(ResultObj == null, string.Format("LoadAssetBundleSuccess " + assetName), ResultObj);
            Debug.LogInfor("加载AssetBundle  成功");
            yield break;
        }

        /// <summary>
        /// 加载当前AssetBundle 依赖的资源(注意 ： 这里加载成功之后不能直接生成)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="assetBundleLoade"></param>
        /// <returns></returns>
        protected virtual IEnumerator LoadDepdenceAssetBundleAsync(string url, AssetBundleLoader assetBundleLoade)
        {
            string assetName = url;
            if (System.IO.Path.GetExtension(assetName) != ConstDefine.AssetBundleExtensionName)
                assetName += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(assetName);  //获取所有的依赖文件
            List<AssetBundleLoader> allDependenceAssetLoader = new List<AssetBundleLoader>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                AssetBundleLoader subAssetBundleLoader = ResourcesLoaderMgr.GetLoaderInstance<AssetBundleLoader>(item, ref isLoaderExit);
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                ApplicationMgr.Instance.StartCoroutine(LoadDepdenceAssetBundleAsync(item, null));
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                if (allDependenceAssetLoader[dex].IsCompleted == false)
                    yield return null;

                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoadDepdenceAsset(false, string.Format("LoadAssetBundle Fail,AssetBundle Path=" + allDependenceAssetLoader[dex].m_ResourcesUrl), null);
                    yield break;
                }
            } //判断依赖项是否加载完成

            ByteLoader byteLoader = ByteLoader.LoadAsset(S_AssetBundleTopPath + assetName, null, LoadAssetModel.Async);  //加载资源
            if (byteLoader.IsError)
            {
                OnCompleteLoadDepdenceAsset(false, string.Format("LoadAssetBundle Fail, AssetBundle Path=" + (S_AssetBundleTopPath + assetName)), null);
                yield break;
            }

            AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromMemoryAsync(byteLoader.ResultBytes, 0); //异步加载当前AssetBundel 资源
            if (assetBundleRequest.isDone == false)
                yield return null;
            OnCompleteLoadDepdenceAsset(ResultObj == null, string.Format("LoadAssetBundleSuccess " + assetName), ResultObj);
            //Debug.LogInfor("加载AssetBundle  成功");
            yield break;
        }


        protected void OnCompleteLoadDepdenceAsset(bool isError, string description, object result, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, process);
        }

        protected override void OnCompleteLoad(bool isError, string description, object result, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, process);
            foreach (var item in m_AllCompleteAct)
            {
                if (item != null)
                    item(ResultObj);
            }
            m_AllCompleteAct.Clear();
        }

        public override void Dispose()
        {

        }

    }
}
