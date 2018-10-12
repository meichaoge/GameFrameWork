using GameFrameWork.HotUpdate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 指定Url 的AssetBundle 存在方式 (注意 
    /// </summary>
    public enum AssetBundleExitState
    {
        SinglePrefab,  //单独的预制体
        FolderPrefab, //按照文件夹目录名打包成一个整体
        None, //不存在这个AssetBundle 资源
    }

    public class NewAssetBundleLoader3 : BaseAbstracResourceLoader
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


        /// <summary>
        /// 当前AssetBundleLoader 加载的所有的资源的结果
        /// </summary>
        public AssetBundleLoaderResultInfor AssetLoaderResultInfor { get; private set; }

        public override void InitialLoader()
        {
            base.InitialLoader();
            AssetLoaderResultInfor = new AssetBundleLoaderResultInfor();
        }



        #region     加载AssetBundle 资源
        /// <summary>
        ///  AssetBundle 加载资源
        /// </summary>
        /// <param name="url">标识当前需要加载的AssetBundel 资源路径</param>
        /// <param name="assetFileName">在这个AssetBundel 中的资源名称</param>
        /// <param name="loadModel"></param>
        /// <param name="assetBundleState">标识是单独的AssetBundle 还是文件夹组成的AssetBundle</param>
        /// <param name="isloadSceneAsset">标识是否是场景资源加载</param>
        /// <returns></returns>
        public static NewAssetBundleLoader3 LoadAssetBundleAsset(string url, string assetFileName, LoadAssetModel loadModel, AssetBundleExitState assetBundleState, bool isloadSceneAsset)
        {
            if (assetBundleState == AssetBundleExitState.None)
            {
                Debug.LogError("LoadAssetBundleAsset Fail,Not AssetBundle Asset url=" + url);
                return null;
            }
            url = url.ToLower();
            assetFileName = assetFileName.ToLower();

          
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAssetBundleAssetSync(url, assetFileName, assetBundleState, isloadSceneAsset);
                case LoadAssetModel.Async:
                    return LoadAssetBundleAssetAsync(url, assetFileName, assetBundleState, isloadSceneAsset);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }


        #region 同步加载

        /// <summary>
        /// （同步加载）加载AssetBundle 资源
        /// </summary>
        /// <param name="url">相对于AseetBundle 资源存放路径的路径 (如果是打包成整个预制体则是整个预制体的路径)</param>
        /// <param name="assetFileName">实际加载AssetBundle 时候的文件名称(考虑到加载整个AssetBundle 中一个资源的情况)</param>
        /// <param name="onCompleteAct">加载完成回调</param>
        ///  <param name="isloadSceneAsset"> 如果加载的是场景 则这里必须填true ,否则false</param>
        /// <returns></returns>
        private static NewAssetBundleLoader3 LoadAssetBundleAssetSync(string url, string assetFileName, AssetBundleExitState assetBundleState, bool isloadSceneAsset)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(NewAssetBundleLoader3)));
                return null;
            }

            if (assetBundleState == AssetBundleExitState.None)
            {
                Debug.LogError("LoadAssetBundleAssetSync Fail,Not AssetBundle Asset" + url + "    assetFileName=" + assetFileName);
                return null;
            }

            Debug.LogInfor("LoadAssetBundleAsset  url=" + url + "    assetFileName=" + assetFileName);
            bool isLoaderExit = false;
            NewAssetBundleLoader3 assetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<NewAssetBundleLoader3>(url, ref isLoaderExit);

            if (isLoaderExit && assetBundleLoader.IsCompleted)
            {
                assetBundleLoader.LoadassetModel = LoadAssetModel.Sync;

                //*** 判断当前资源是否加载了
                if (string.IsNullOrEmpty(assetFileName) == false && assetBundleLoader.AssetLoaderResultInfor.m_AllNeedLoadAssetRecord.Contains(assetFileName) == false)
                    assetBundleLoader.AssetLoaderResultInfor.m_AllNeedLoadAssetRecord.Add(assetFileName);
                assetBundleLoader.LoadAndRecordAsset(assetBundleLoader.AssetLoaderResultInfor, assetBundleState, isloadSceneAsset);  //加载已经加载的AssetBundle 中的资源

                assetBundleLoader.OnCompleteLoad(assetBundleLoader.IsError, assetBundleLoader.Description, null, true);  //如果当前加载器已经完成加载 则手动触发事件
                return assetBundleLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
            if (assetBundleLoader.LoadassetModel == LoadAssetModel.Async)
            {
                assetBundleLoader.ForceBreakLoaderProcess();
            } //结束之前的加载进程

            assetBundleLoader.LoadassetModel = LoadAssetModel.Sync;
            assetBundleLoader.AssetLoaderResultInfor.SetAssetName(url, assetBundleState);
            assetBundleLoader.m_LoadAssetCoroutine = null;
            if (string.IsNullOrEmpty(assetFileName) == false)
                assetBundleLoader.AssetLoaderResultInfor.m_AllNeedLoadAssetRecord.Add(assetFileName);
            assetBundleLoader.LoadAssetBundleSync(url, assetFileName, assetBundleState, isloadSceneAsset);
            return assetBundleLoader;
        }



        /// <summary>
        /// 同步加载资源 (这里处理的是最外层的AssetBundle ,当前资源依赖的AssetBundle 处理在LoadDepdenceAssetBundleAsync)
        /// </summary>
        /// <param name="url"></param>
        protected void LoadAssetBundleSync(string url, string assetFileName, AssetBundleExitState assetBundleState, bool isloadScene)
        {
            m_ResourcesUrl = url;
            if (System.IO.Path.GetExtension(m_ResourcesUrl) != ConstDefine.AssetBundleExtensionName)
                m_ResourcesUrl += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            #region  加载依赖的资源
            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(m_ResourcesUrl);  //获取所有的依赖文件
            List<NewAssetBundleLoader3> allDependenceAssetLoader = new List<NewAssetBundleLoader3>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                NewAssetBundleLoader3 subAssetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<NewAssetBundleLoader3>(item, ref isLoaderExit);
                if (isLoaderExit && subAssetBundleLoader.IsCompleted)
                {
                    Debug.LogEditorInfor("LoadAssetBundleASync Exit ," + item);
                    continue;  //已经存在了 则继续
                }
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                LoadDepdenceAssetBundleSync(item, subAssetBundleLoader, assetBundleState);
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("[AssetBundleLoader] LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoad(false, string.Format("[AssetBundleLoader] LoadAssetBundle Fail,AssetBundle Path= {0}", allDependenceAssetLoader[dex].m_ResourcesUrl), null, true);
                    break;
                }
            } //判断依赖项是否加载完成
            #endregion


            #region   AssetBundle  同步加载 
            AssetLoaderResultInfor.LoadedAssetBundle = AssetBundle.LoadFromFile(S_AssetBundleTopPath + m_ResourcesUrl);
            if (AssetLoaderResultInfor.LoadedAssetBundle == null)
            {
                ResultObj = null;
                OnCompleteLoad(true, string.Format("同步加载本地AssetBundle 失败{0}", url), null, true, 1);
                return;
            }
            LoadAndRecordAsset(AssetLoaderResultInfor, assetBundleState, isloadScene);

            OnCompleteLoad(AssetLoaderResultInfor.LoadedAssetBundle == null, string.Format("[AssetBundleLoader] LoadAssetBundleSuccess  {0}", m_ResourcesUrl), null, true);
            #endregion

            Debug.LogInfor("加载AssetBundle  成功");
        }


        /// <summary>
        /// (同步)加载当前AssetBundle 依赖的资源(注意 ： 这里加载成功之后不能直接生成)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="assetBundleLoade"></param>
        /// <returns></returns>
        protected virtual void LoadDepdenceAssetBundleSync(string url, NewAssetBundleLoader3 depdebceAssetBundleLoader, AssetBundleExitState assetBundleState)
        {
            depdebceAssetBundleLoader.m_ResourcesUrl = url;
            if (System.IO.Path.GetExtension(depdebceAssetBundleLoader.m_ResourcesUrl) != ConstDefine.AssetBundleExtensionName)
                depdebceAssetBundleLoader.m_ResourcesUrl += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(depdebceAssetBundleLoader.m_ResourcesUrl);  //获取所有的依赖文件
            List<NewAssetBundleLoader3> allDependenceAssetLoader = new List<NewAssetBundleLoader3>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                NewAssetBundleLoader3 subAssetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<NewAssetBundleLoader3>(item, ref isLoaderExit);

                if (isLoaderExit && subAssetBundleLoader.IsCompleted)
                {
                    Debug.LogEditorInfor("LoadDepdenceAssetBundleAsync Exit ," + item);
                    continue;
                }
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                LoadDepdenceAssetBundleSync(item, subAssetBundleLoader, assetBundleState);
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("[AssetBundleLoader] LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoadDepdenceAsset(allDependenceAssetLoader[dex], false, string.Format("[AssetBundleLoader] LoadAssetBundle Fail,AssetBundle Path={0}" + allDependenceAssetLoader[dex].m_ResourcesUrl), null, true);
                    break;
                }
            } //判断依赖项是否加载完成

            #region  AssetBundle 本地同步加载
            depdebceAssetBundleLoader.AssetLoaderResultInfor.LoadedAssetBundle = AssetBundle.LoadFromFile(S_AssetBundleTopPath + depdebceAssetBundleLoader.m_ResourcesUrl);
            if (depdebceAssetBundleLoader.AssetLoaderResultInfor.LoadedAssetBundle == null)
            {
                depdebceAssetBundleLoader.ResultObj = null;
                OnCompleteLoad(true, string.Format("同步加载本地AssetBundle 失败{0}", url), null, true, 1);
                return;
            }
            depdebceAssetBundleLoader.OnCompleteLoad(false, string.Format("[AssetBundleLoader] Load DepdenceAssetBundel Success {0}", depdebceAssetBundleLoader.m_ResourcesUrl), null, true);

            #endregion
        }

        #endregion

        #region 异步加载

        /// <summary>
        /// 加载AssetBundle 资源
        /// </summary>
        /// <param name="url">相对于AseetBundle 资源存放路径的路径 (如果是打包成整个预制体则是整个预制体的路径)</param>
        /// <param name="assetFileName">实际加载AssetBundle 时候的文件名称(考虑到加载整个AssetBundle 中一个资源的情况)</param>
        /// <param name="onCompleteAct">加载完成回调</param>
        ///  <param name="isloadSceneAsset"> 如果加载的是场景 则这里必须填true ,否则false</param>
        /// <returns></returns>
        private static NewAssetBundleLoader3 LoadAssetBundleAssetAsync(string url, string assetFileName, AssetBundleExitState assetBundleState, bool isloadSceneAsset)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(assetFileName))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(NewAssetBundleLoader3)));
                return null;
            }

            if (assetBundleState == AssetBundleExitState.None)
            {
                Debug.LogError("LoadAssetBundleAssetSync Fail,Not AssetBundle Asset" + url + "    assetFileName=" + assetFileName);
                return null;
            }

            Debug.LogInfor("LoadAssetBundleAsset  url=" + url + "    assetFileName=" + assetFileName);
            bool isLoaderExit = false;
            NewAssetBundleLoader3 assetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<NewAssetBundleLoader3>(url, ref isLoaderExit);
            assetBundleLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）

            if (isLoaderExit)
            {
                if (assetBundleLoader.IsCompleted)
                {
                    assetBundleLoader.LoadAndRecordAsset(assetBundleLoader.AssetLoaderResultInfor, assetBundleState, isloadSceneAsset);
                    assetBundleLoader.OnCompleteLoad(assetBundleLoader.IsError, assetBundleLoader.Description, null, true);  //如果当前加载器已经完成加载 则手动触发事件
                }
                else
                {
                    assetBundleLoader.AssetLoaderResultInfor.m_AllNeedLoadAssetRecord.Add(assetFileName);  //记录需要加载这个指定的资源
                }
                return assetBundleLoader;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }
           assetBundleLoader.AssetLoaderResultInfor.SetAssetName(url, assetBundleState);
            assetBundleLoader.AssetLoaderResultInfor.m_AllNeedLoadAssetRecord.Add(assetFileName);  //记录需要加载这个指定的资源
            assetBundleLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(assetBundleLoader.LoadAssetBundleASync(url, assetFileName, assetBundleState, isloadSceneAsset));
            return assetBundleLoader;
        }

        /// <summary>
        /// 异步加载资源 (这里处理的是最外层的AssetBundle ,当前资源依赖的AssetBundle 处理在LoadDepdenceAssetBundleAsync)
        /// </summary>
        /// <param name="url"></param>
        protected IEnumerator LoadAssetBundleASync(string url, string assetFileName, AssetBundleExitState assetBundleState, bool isloadScene)
        {
            m_ResourcesUrl = url;
            if (System.IO.Path.GetExtension(m_ResourcesUrl) != ConstDefine.AssetBundleExtensionName)
                m_ResourcesUrl += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            #region  加载依赖的资源
            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(m_ResourcesUrl);  //获取所有的依赖文件
            List<NewAssetBundleLoader3> allDependenceAssetLoader = new List<NewAssetBundleLoader3>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                NewAssetBundleLoader3 subAssetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<NewAssetBundleLoader3>(item, ref isLoaderExit);
                if (isLoaderExit && subAssetBundleLoader.IsCompleted)
                {
                    Debug.LogEditorInfor("LoadAssetBundleASync Exit ," + item);
                    continue;  //已经存在了 则继续
                }
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                EventCenter.Instance.StartCoroutine(LoadDepdenceAssetBundleAsync(item, subAssetBundleLoader));
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                while (allDependenceAssetLoader[dex].IsCompleted == false)
                    yield return null;

                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("[AssetBundleLoader] LoadAssetBundleSync  Fail, 依赖的 AssetBundle 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoad(false, string.Format("[AssetBundleLoader] LoadAssetBundle Fail DepdenceLoader Fail ,AssetBundle Path= {0} DepdenceLoader Path={1}",
                        m_ResourcesUrl, allDependenceAssetLoader[dex].m_ResourcesUrl), null, true);
                    yield break;
                }
            } //判断依赖项是否加载完成
            #endregion

            #region AssetBundleCreateRequest 加载
            string fileAbsolutelyPath = string.Format("{0}{1}", S_AssetBundleTopPath, m_ResourcesUrl);  //当先需要下载的资源的绝对路径
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(fileAbsolutelyPath);
            yield return request;

            if (request.isDone)
            {
                Debug.Log("LoadDepdenceAssetBundleAsync Success  depdebceAssetBundleLoader.m_ResourcesUrl=" + m_ResourcesUrl);
                AssetLoaderResultInfor.LoadedAssetBundle = request.assetBundle;
                ResultObj = null;
                LoadAndRecordAsset(AssetLoaderResultInfor, assetBundleState, isloadScene);
            }
            else
            {
                AssetLoaderResultInfor.LoadedAssetBundle = null;
                Debug.LogError("LoadDepdenceAssetBundleAsync  FailAA==  " + url);
                Debug.LogError("LoadDepdenceAssetBundleAsync  Failbb==  " + fileAbsolutelyPath);
            }


            OnCompleteLoad(AssetLoaderResultInfor.LoadedAssetBundle == null, string.Format("[AssetBundleLoader] Load DepdenceAssetBundel Success {0}", m_ResourcesUrl), null, true);
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
        protected virtual IEnumerator LoadDepdenceAssetBundleAsync(string url, NewAssetBundleLoader3 depdebceAssetBundleLoader)
        {
            Debug.Log("LoadDepdenceAssetBundleAsync  url=" + url);
            depdebceAssetBundleLoader.m_ResourcesUrl = url;
            if (System.IO.Path.GetExtension(depdebceAssetBundleLoader.m_ResourcesUrl) != ConstDefine.AssetBundleExtensionName)
                depdebceAssetBundleLoader.m_ResourcesUrl += ConstDefine.AssetBundleExtensionName;  //组合上扩展名

            string[] dependenceAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(depdebceAssetBundleLoader.m_ResourcesUrl);  //获取所有的依赖文件
            List<NewAssetBundleLoader3> allDependenceAssetLoader = new List<NewAssetBundleLoader3>();  //所有依赖文件的加载器
            foreach (var item in dependenceAssets)
            {
                bool isLoaderExit = false;
                NewAssetBundleLoader3 subAssetBundleLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<NewAssetBundleLoader3>(item, ref isLoaderExit);

                if (isLoaderExit && subAssetBundleLoader.IsCompleted)
                {
                    Debug.LogEditorInfor("LoadDepdenceAssetBundleAsync Exit ," + item);
                    continue;
                }
                allDependenceAssetLoader.Add(subAssetBundleLoader);
                Debug.Log("LoadDepdenceAssetBundleAsync  allDependenceAssetLoader   item=" + item);
                EventCenter.Instance.StartCoroutine(LoadDepdenceAssetBundleAsync(item, subAssetBundleLoader));
            } //递归加载资源的依赖项


            for (int dex = 0; dex < allDependenceAssetLoader.Count; ++dex)
            {
                while (allDependenceAssetLoader[dex].IsCompleted == false)
                    yield return null;

                if (allDependenceAssetLoader[dex].IsError)
                {
                    Debug.LogError("[AssetBundleLoader] LoadDepdenceAssetBundleAsync  Fail, 依赖的 AssetBundle " + url + " 资源不存在 " + allDependenceAssetLoader[dex].m_ResourcesUrl);
                    OnCompleteLoadDepdenceAsset(allDependenceAssetLoader[dex], false,
                        string.Format("[AssetBundleLoader] LoadAssetBundle Fail,AssetBundle Path={0}" + allDependenceAssetLoader[dex].m_ResourcesUrl), null, true);
                    yield break;
                }
            } //判断依赖项是否加载完成


            #region  AssetBundleCreateRequest 加载
            string fileAbsolutelyPath = string.Format("{0}{1}", S_AssetBundleTopPath, url);  //当先需要下载的资源的绝对路径

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(fileAbsolutelyPath);
            yield return request;
            if (request.isDone)
            {
                Debug.Log("LoadDepdenceAssetBundleAsync Success  depdebceAssetBundleLoader.m_ResourcesUrl=" + depdebceAssetBundleLoader.m_ResourcesUrl);
                depdebceAssetBundleLoader.AssetLoaderResultInfor.LoadedAssetBundle = request.assetBundle;
            }
            else
            {
                depdebceAssetBundleLoader.AssetLoaderResultInfor.LoadedAssetBundle = null;
                Debug.LogError("LoadDepdenceAssetBundleAsync  FailAA==  " + url);
                Debug.LogError("LoadDepdenceAssetBundleAsync  Failbb==  " + fileAbsolutelyPath);
            }
            depdebceAssetBundleLoader.OnCompleteLoad(depdebceAssetBundleLoader.AssetLoaderResultInfor.LoadedAssetBundle == null, string.Format("[AssetBundleLoader] Load DepdenceAssetBundel Success {0}", depdebceAssetBundleLoader.m_ResourcesUrl), null, true);

            #endregion

            //Debug.LogInfor("加载AssetBundle  成功");
            yield break;
        }
        #endregion



        /// <summary>
        /// AssetBundle 依赖的AssetBundle 完成加载
        /// (注意必须给出一个指向当前子AssetBundleLoader 的参数 ，否则操作的是外层AssetBundleLoader 的参数而导致状态异常)
        /// </summary>
        /// <param name="AssetLoader"></param>
        /// <param name="isError"></param>
        /// <param name="description"></param>
        /// <param name="result"></param>
        /// <param name="iscomplete"></param>
        /// <param name="process"></param>
        protected void OnCompleteLoadDepdenceAsset(NewAssetBundleLoader3 AssetLoader, bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            if (AssetLoader == null)
                return;

            AssetLoader.IsCompleted = iscomplete;
            AssetLoader.IsError = isError;
            AssetLoader.Description = description;
            AssetLoader.ResultObj = result;
            AssetLoader.Process = process;

            AssetLoader.AssetLoaderResultInfor.m_AllNeedLoadAssetRecord.Clear();
        }

        /// <summary>
        /// 从加载完成的AssetBundel 中加载资源
        /// </summary>
        /// <param name="loadRecordInfor"></param>
        /// <param name="isloadSceneAsset"></param>

        private void LoadAndRecordAsset(AssetBundleLoaderResultInfor loadRecordInfor, AssetBundleExitState assetBundleState, bool isloadSceneAsset)
        {
            if (isloadSceneAsset)
            {
                var asset = loadRecordInfor.LoadedAssetBundle;  //场景资源返回AssetBundle 使用不同的API加载
                foreach (var item in loadRecordInfor.m_AllNeedLoadAssetRecord)
                {
                    if (loadRecordInfor.CheckIfLoadAsset(item)) continue;
                    AssetLoaderResultInfor.RecordLoadAsset(item, asset);
                }
            }
            else
            {
                if (assetBundleState == AssetBundleExitState.FolderPrefab)
                {
                    if (AssetBundleMgr.Instance.CheckIfNeedRecord(m_ResourcesUrl))
                    {
                        var content = loadRecordInfor.LoadedAssetBundle.GetAllAssetNames();
                        AssetBundleMgr.Instance.RecordFolderAssetBundleContainAsset(m_ResourcesUrl, content);
                    }
                } //记录

                foreach (var item in loadRecordInfor.m_AllNeedLoadAssetRecord)
                {
                    object asset = null;
                    if (assetBundleState == AssetBundleExitState.FolderPrefab)
                    {
                        if (loadRecordInfor.CheckIfLoadAsset(item)) continue;
                        if (AssetBundleMgr.Instance.CheckIfContainAsset(m_ResourcesUrl, item))
                        {
                            asset = loadRecordInfor.LoadedAssetBundle.LoadAsset(item);  //
                        }
                    }
                    else
                    {
                        asset = loadRecordInfor.LoadedAssetBundle.LoadAsset(item);  //
                    }

                    AssetLoaderResultInfor.RecordLoadAsset(item, asset);
                }
            }//非场景资源需要加载出来 
        }

        #endregion







        public override void ReleaseLoader()
        {
            base.ReleaseLoader();
            AssetLoaderResultInfor.ClearData();
        }


        protected override void ForceBreakLoaderProcess() { }




    }
}