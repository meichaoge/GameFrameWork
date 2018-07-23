using GameFrameWork.ResourcesLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 编辑器下显示资源加载器的详情
    /// </summary>
    public class DebugShowLoaderInfor : Singleton_Mono<DebugShowLoaderInfor>
    {
#if UNITY_EDITOR

        /// <summary>
        /// 每一个加载器的信息
        /// </summary>
        [System.Serializable]
        public class EditorLoaderInfor
        {
            public string m_Url;
            public int m_RefenceCount;
            public bool m_IsCompleted;
            public bool m_IsError;
            public float m_Process;
        }

        /// <summary>
        /// 每一种类型加载器信息
        /// </summary
        [System.Serializable]
        public class EditorResourceLoaderTypeInfor
        {
            public string m_LoderTypeName;
            public List<EditorLoaderInfor> m_AllLoaderInfor = new List<EditorLoaderInfor>();
        }


        public List<EditorResourceLoaderTypeInfor> m_AllTypesOfLoaders = new List<EditorResourceLoaderTypeInfor>();
        public List<EditorResourceLoaderTypeInfor> m_AllUnUseTypesOfLoaders = new List<EditorResourceLoaderTypeInfor>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ShowAllLoaderInfor();
            ShowAllUnUseLoaderInfor();
        }

        /// <summary>
        /// 显示正在使用的Loader
        /// </summary>
        private void ShowAllLoaderInfor()
        {
            m_AllTypesOfLoaders.Clear();
            foreach (var item in ResourcesLoaderMgr.S_AllTypeLoader)
            {
                EditorResourceLoaderTypeInfor typeLoadersInfor = new EditorResourceLoaderTypeInfor();
                typeLoadersInfor.m_LoderTypeName = item.Key.Name.ToString();
                foreach (var loader in item .Value)
                {
                    EditorLoaderInfor loaderInfor = new EditorLoaderInfor();
                    loaderInfor.m_Url = loader.Value.m_ResourcesUrl;
                    loaderInfor. m_RefenceCount = loader.Value.ReferCount;
                    loaderInfor.m_IsCompleted = loader.Value.IsCompleted;
                    loaderInfor.m_IsError = loader.Value.IsError;
                    loaderInfor.m_Process = loader.Value.Process;


                    typeLoadersInfor.m_AllLoaderInfor.Add(loaderInfor);
                }

                m_AllTypesOfLoaders.Add(typeLoadersInfor);
            }
        }

        /// <summary>
        /// 显示不再使用的Loader
        /// </summary>
        private void ShowAllUnUseLoaderInfor()
        {
            m_AllUnUseTypesOfLoaders.Clear();
            foreach (var item in ResourcesLoaderMgr.S_UnUseLoader)
            {
                EditorResourceLoaderTypeInfor typeLoadersInfor = new EditorResourceLoaderTypeInfor();
                typeLoadersInfor.m_LoderTypeName = item.Key.Name.ToString();
                List<BaseAbstracResourceLoader> data = new List<BaseAbstracResourceLoader>();
                data.AddRange(item.Value);

                foreach (var loader in data)
                {
                    EditorLoaderInfor loaderInfor = new EditorLoaderInfor();
                    loaderInfor.m_Url = loader.m_ResourcesUrl;
                    loaderInfor.m_RefenceCount = loader.ReferCount;
                    loaderInfor.m_IsCompleted = loader.IsCompleted;
                    loaderInfor.m_IsError = loader.IsError;
                    loaderInfor.m_Process = loader.Process;

                    typeLoadersInfor.m_AllLoaderInfor.Add(loaderInfor);
                }

                m_AllUnUseTypesOfLoaders.Add(typeLoadersInfor);
            }
        }


#endif
    }
}