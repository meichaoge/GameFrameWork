using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.ResourcesLoader;

namespace GameFrameWork
{
    /// <summary>
    /// 提供编辑器下配置一些应用的功能
    /// </summary>
    public class ApplicationConfig : Singleton_Mono<ApplicationConfig>
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

        /// <summary>
        /// 配置的加载路径优先级
        /// </summary>
        [CustomerHeaderAttribute("默认的资源加载路径优先级 可以设置 m_Priority 改变优先级", "#FF0000FF")]
        [SerializeField]
        private List<LoadAssetPathInfor> m_LoadAssetPath = new List<LoadAssetPathInfor>();

        [CustomerHeaderAttribute("排序后的优先级序列   只读不要修改!!!!!", "gray")]
        [SerializeField]
        private List<LoadAssetPathInfor> m_LoadAssetPathOfPriority = new List<LoadAssetPathInfor>(); //排序后的优先级序列

        /// <summary>
        /// 全局控制资源加载的方式 要么同步，要么异步加载，否则同步异步同时进行有问题
        /// </summary>
        [CustomerHeaderAttribute("资源加载同步/异步")]
        [Space(10)]
        public LoadAssetModel m_CurLoadAssetModel = LoadAssetModel.Async;

        [Space(10)]
        [CustomerHeaderAttribute("日志输出级别，默认只输出 Log 级别日志 ", "#00E7E7FF")]
        public Debug.LogLevel m_LogLevel = Debug.LogLevel.Log;


        [Space(10)]
        [CustomerHeaderAttribute("控制是否统计资源被加载次数,用于系统资源分析", "#00E7E7FF")]
        public bool m_IsEnableResourcesLoadTrace = true;

        [Space(10)]
        [CustomerHeaderAttribute("标识用于资源热更新的服务器地址", "#00E7E7FF")]
        public string m_AssetServerPath = "file://" + @"E:/My_WorkSpace/AssetBundle_Test/";


        #region  Frame 
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
            base.Awake();
            SortLoadAssetPathPriority();
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

        /// <summary>Oa
        /// 根据当前的加载路径获取下一个第一级优先级的路径枚举
        /// </summary>
        /// <param name="curAssetPath">返回是否</param>
        /// <returns></returns>
        public bool GetNextLoadAssetPath(ref LoadAssetPathEnum curAssetPath)
        {
            for (int dex = 0; dex < m_LoadAssetPath.Count - 1; ++dex)
            {
                if (m_LoadAssetPath[dex].m_AssetPathEnum == curAssetPath)
                {
                    curAssetPath = m_LoadAssetPath[dex + 1].m_AssetPathEnum;
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