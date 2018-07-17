using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 挂载在每一个动态加载资源的物体上 用来标记当前加载的资源方便卸载资源
    /// </summary>
    public class LoaderResourceHandlerTag : MonoBehaviour
    {
        [System.Serializable]
        public class LoaderHanderInfor
        {
            public string m_Url; //标识资源
            public ApplicationLayerBaseLoader m_Loader;

            public LoaderHanderInfor(string url, ApplicationLayerBaseLoader loader)
            {
                m_Url = url;
                m_Loader = loader;
            }
        }

        [SerializeField]
        private List<LoaderHanderInfor> m_LoadedResourceInfor = new List<LoaderHanderInfor>();


        /// <summary>
        /// 在加载资源前判断下是否已经使用相同类型加载器(避免一个物体同时使用多个加载器的Bug )加载一个资源，
        /// 如果新资源的URL 与以前的不同则以前的资源要减少引用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loader"></param>
        public void TryLoadAsset(string url, ApplicationLayerBaseLoader loader)
        {
            int index = -1;
            bool isExitTypeOfLoader = false;
            for (int dex = 0; dex < m_LoadedResourceInfor.Count; ++dex)
            {
                if (m_LoadedResourceInfor[dex].m_Loader.GetType() == loader.GetType())
                {
                    isExitTypeOfLoader = true;
#if UNITY_EDITOR
                    if (index != -1)
                    {
                        Debug.LogError(string.Format("TryLoadAsset  {0}存在多个同类型的加载器{1} url= {2}", gameObject.name, loader.GetType(), loader.m_ResourcesUrl));
                        continue;
                    }
#endif
                    if (m_LoadedResourceInfor[dex].m_Url != url)
                    {
                        Debug.LogInfor("尝试使用同种加载器，加载不同的资源，卸载旧的资源");
                        m_LoadedResourceInfor[dex].m_Loader.ReduceReference(m_LoadedResourceInfor[dex].m_Loader, false);
                        index = dex;
#if ! UNITY_EDITOR
                        break;
#endif
                    }
                }
            }

            if (index != -1)
                m_LoadedResourceInfor.RemoveAt(index);

            if (isExitTypeOfLoader == false || index != -1)
                m_LoadedResourceInfor.Add(new LoaderHanderInfor(url, loader));

        }

        private void OnDestroy()
        {
            for (int dex = 0; dex < m_LoadedResourceInfor.Count; ++dex)
            {
                if (m_LoadedResourceInfor[dex].m_Loader != null)
                    m_LoadedResourceInfor[dex].m_Loader.ReduceReference(m_LoadedResourceInfor[dex].m_Loader, false);
            }
        }

    }
}