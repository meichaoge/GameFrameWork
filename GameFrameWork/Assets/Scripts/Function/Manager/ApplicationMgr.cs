using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{


    /// <summary>
    /// 整个应用程序的主管理器 负责管理其他模块
    /// </summary>
    public class ApplicationMgr : Singleton_Mono<ApplicationMgr>
    {
        /// <summary>
        /// 资源加载路径优先级设置
        /// </summary>
        [System.Serializable]
        public class LoadAssetPathInfor
        {
            public LoadAssetPathEnum m_AssetPathEnum = LoadAssetPathEnum.PersistentDataPath;
            public int m_Priority = 1;  //优先级
        }


        #region Data 

        /// <summary>
        /// 配置的加载路径优先级
        /// </summary>
        public List<LoadAssetPathInfor> m_LoadAssetPath = new List<LoadAssetPathInfor>();
        public List<LoadAssetPathInfor> m_LoadAssetPathOfPriority = new List<LoadAssetPathInfor>(); //排序后的优先级序列

        /// <summary>
        /// 全局控制资源加载的方式 要么同步，要么异步加载，否则同步异步同时进行有问题
        /// </summary>
        [Header("资源加载同步/异步")]
        public LoadAssetModel m_CurLoadAssetModel = LoadAssetModel.Async;

        #endregion

        #region Frame
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            SetDefaultLoadPathInfor();
        }

        /// <summary>
        /// 设置默认的优先级
        /// </summary>
        private void SetDefaultLoadPathInfor()
        {
            m_LoadAssetPath.Clear();
            var loadAssetPaths = System.Enum.GetValues(typeof(LoadAssetPathEnum));
            int priority = 3;
            foreach (var item in loadAssetPaths)
            {
                LoadAssetPathInfor infor = new LoadAssetPathInfor();
                infor.m_AssetPathEnum = (LoadAssetPathEnum)System.Enum.Parse(typeof(LoadAssetPathEnum), item.ToString());
                infor.m_Priority = priority;
                m_LoadAssetPath.Add(infor);
                --priority;
            }
        }

#endif


        protected override void Awake()
        {
#if UNITY_EDITOR
            Debug.Log(" Application.dataPath=" + Application.dataPath);
            Debug.Log(" Application.persistentDataPath=" + Application.persistentDataPath);
            Debug.Log(" Application.temporaryCachePath=" + Application.temporaryCachePath);
            Debug.Log(" Application.streamingAssetsPath=" + Application.streamingAssetsPath);

#endif

            base.Awake();
            Debug.LogInfor("ApplicationMgr Start...");
            SortLoadAssetPathPriority();
            //TODO 其他一些操作

        }

        void Start()
        {

        }

        void Update()
        {

        }

        private void FixedUpdate()
        {
            ResourcesLoaderMgr.Tick();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (AssetBundleMgr.Instance.m_MainAssetBundle)
                AssetBundleMgr.Instance.m_MainAssetBundle.Unload(true); //卸载所有的 AssetBundle 资源
        }

        #endregion

        #region 配置
        /// <summary>
        /// 按照优先级高到低排序
        /// </summary>
        private void SortLoadAssetPathPriority()
        {
            m_LoadAssetPathOfPriority.Clear();
            m_LoadAssetPathOfPriority.AddRange(m_LoadAssetPath);

            if (m_LoadAssetPathOfPriority.Count == 0)
            {
                Debug.LogError("没有配置资源加载路径 优先级关系");
                return;
            }

            m_LoadAssetPathOfPriority.Sort((lparameter, rparameter) =>
            {
                if (lparameter.m_AssetPathEnum == LoadAssetPathEnum.None)
                    return 1;

                if (lparameter.m_Priority < rparameter.m_Priority)
                    return 1;
                if (lparameter.m_Priority == rparameter.m_Priority)
                    return 0;
                return -1;
            });
        }

        /// <summary>
        /// 根据当前的加载路径获取下一个第一级优先级的路径枚举
        /// </summary>
        /// <param name="curAssetPath">返回是否</param>
        /// <returns></returns>
        public bool GetNextLoadAssetPath(ref LoadAssetPathEnum curAssetPath)
        {
            for (int dex = 0; dex < m_LoadAssetPath.Count; ++dex)
            {
                if (m_LoadAssetPath[dex].m_AssetPathEnum == curAssetPath)
                {
                    curAssetPath = m_LoadAssetPath[dex].m_AssetPathEnum;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取优先级最高的路径
        /// </summary>
        /// <returns></returns>
        public LoadAssetPathEnum GetFirstPriortyAssetPathEnum()
        {
            return m_LoadAssetPathOfPriority[0].m_AssetPathEnum;
        }

        #endregion


    }
}