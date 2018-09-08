using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 材质球加载器
    /// </summary>
    public class MaterialLoader : ApplicationLoader_Alone
    {

        #region      加载资源
        public static MaterialLoader LoadAsset(Transform requestTarget, string url, LoadAssetModel loadModel, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            switch (loadModel)
            {
                case LoadAssetModel.None:
                    Debug.LogError("异常的加载默认  LoadAssetModel.None 是默认的值 ，使用前请正确赋值");
                    return null;
                case LoadAssetModel.Sync:
                    return LoadAssetSync(requestTarget, url, completeHandler);
                case LoadAssetModel.Async:
                    return LoadAssetAsync(requestTarget, url, completeHandler);
                default:
                    Debug.LogError("没有定义的加载类型 " + loadModel);
                    return null;
            }
        }

        #region 异步加载
        /// <summary>
        /// 加载材质球资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static MaterialLoader LoadAssetAsync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(MaterialLoader)));
                return null;
            }

            bool isContainLoaders = false;
            MaterialLoader materialLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<MaterialLoader>(url, ref isContainLoaders);
            materialLoader.m_OnCompleteAct.Add(completeHandler);
            materialLoader.LoadassetModel = LoadAssetModel.Async; //这里貌似没必要（由于异步加载时候同步加载必定完成了）


            materialLoader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (materialLoader.IsCompleted)
                    materialLoader.OnCompleteLoad(materialLoader.IsError, materialLoader.Description, materialLoader.ResultObj, materialLoader.IsCompleted);
                return materialLoader;
            }


            materialLoader.m_LoadAssetCoroutine = EventCenter.Instance.StartCoroutine(materialLoader.LoadMaterialAssetAsync(url));
            return materialLoader;
        }

        private IEnumerator LoadMaterialAssetAsync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Async, null, false);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
        #endregion

        #region 同步加载
        /// <summary>
        /// (同步)加载材质球资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        protected static MaterialLoader LoadAssetSync(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}", typeof(MaterialLoader)));
                return null;
            }

            bool isContainLoaders = false;
            MaterialLoader materialLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<MaterialLoader>(url, ref isContainLoaders);
            materialLoader.m_OnCompleteAct.Add(completeHandler);


            materialLoader.AddReference(requestTarget, url);
            if (isContainLoaders && materialLoader.IsCompleted)
            {
                materialLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要（由于异步加载时候同步加载必定完成了）
                materialLoader.OnCompleteLoad(materialLoader.IsError, materialLoader.Description, materialLoader.ResultObj, materialLoader.IsCompleted);
                return materialLoader;
            }
            if (materialLoader.LoadassetModel == LoadAssetModel.Async)
            {
                materialLoader.ForceBreakLoaderProcess();
            }
            materialLoader.m_LoadAssetCoroutine = null;
            materialLoader.LoadassetModel = LoadAssetModel.Sync; //这里貌似没必要
            materialLoader.LoadMaterialAssetSync(url);
            return materialLoader;
        }

        private void LoadMaterialAssetSync(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, LoadAssetModel.Sync, null, false);
            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
        }
        #endregion

        #endregion

        #region 卸载资源
        public static void UnLoadAsset(string url, object requestTarget = null)
        {
            MaterialLoader materialLoader = ResourcesLoaderMgr.GetExitLoaderInstance<MaterialLoader>(url);
            if (materialLoader == null)
            {
                //Debug.LogError("无法获取指定类型的加载器 " + typeof(materialLoader));
                return;
            }
            if (requestTarget == null)
                requestTarget = materialLoader.m_RequesterTarget;

            materialLoader.ReduceReference(materialLoader, false);
        }
        #endregion


        public override void OnCompleteLoad(bool isError, string description, object result, bool iscomplete, float process = 1)
        {
            if (result.GetType() == typeof(AssetBundle))
            {
                ResultObj = (result as AssetBundle).LoadAsset<Material>(ResourseFileName.ToLower());
            }
            else
            {
                Debug.LogInfor("材质球加载器 资源返回类型 " + result.GetType());
                ResultObj = result;
            }

            base.OnCompleteLoad(isError, description, ResultObj, iscomplete, process);
        }


        protected override void ForceBreakLoaderProcess()
        {
            if (IsCompleted) return;
            if (LoadassetModel != LoadAssetModel.Async)
            {
                Debug.LogError("非异步加载方式不需要强制结束 " + LoadassetModel);
                return;
            }
            if (m_LoadAssetCoroutine != null)
                EventCenter.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }
    }
}