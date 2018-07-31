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
        /// <summary>
        /// 加载材质球资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="completeHandler"></param>
        /// <returns></returns>
        public static MaterialLoader LoadAsset(Transform requestTarget, string url, System.Action<BaseAbstracResourceLoader> completeHandler)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(string.Format("Url Can't Be Null , TypeLoader={0}" , typeof(MaterialLoader)));
                return null;
            }

            bool isContainLoaders = false;
            MaterialLoader materialLoader = ResourcesLoaderMgr.GetOrCreateLoaderInstance<MaterialLoader>(url, ref isContainLoaders);
            materialLoader.m_OnCompleteAct.Add(completeHandler);


            materialLoader.AddReference(requestTarget, url);
            if (isContainLoaders)
            {
                if (materialLoader.IsCompleted)
                    materialLoader.OnCompleteLoad(materialLoader.IsError, materialLoader.Description, materialLoader.ResultObj, materialLoader.IsCompleted);
                return materialLoader;
            }


            materialLoader. m_LoadAssetCoroutine = ApplicationMgr.Instance.StartCoroutine(materialLoader.LoadMaterialAsset(url));
            return materialLoader;
        }


        private IEnumerator LoadMaterialAsset(string url)
        {
            m_ResourcesUrl = url;
            m_BridgeLoader = BridgeLoader.LoadAsset(url, null);
            while (m_BridgeLoader.IsCompleted == false)
                yield return null;

            OnCompleteLoad(m_BridgeLoader.IsError, m_BridgeLoader.Description, m_BridgeLoader.ResultObj, m_BridgeLoader.IsCompleted);
            yield break;
        }
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
            if(result.GetType()==typeof(AssetBundle))
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
            if(m_LoadAssetCoroutine!=null )
            ApplicationMgr.Instance.StopCoroutine(m_LoadAssetCoroutine);
        }
    }
}