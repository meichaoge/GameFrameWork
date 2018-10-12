using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.ResourcesLoader;
using GameFrameWork.HotUpdate;

namespace GameFrameWork
{
    /// <summary>
    /// 提供编辑器下配置一些应用的功能
    /// </summary>
    public class ApplicationConfig : Singleton_Mono_NotDestroy<ApplicationConfig>
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

        [CustomerHeaderAttribute("应用当前的语言版本", "#00E7E7FF")]
        [SerializeField]
        private Language m_CurLanguageType = Language.Chinese;
        public Language CurLanguageType
        {
            get { return m_CurLanguageType; }
        }

        [CustomerHeaderAttribute("标识是否是开发模式(true 则优先加载Resources文件)", "#FF0000FF")]
        [Space(20)]
        public bool IsDevelopMode = true;

        #region  资源加载配置

        /// <summary>
        /// 配置的加载路径优先级
        /// </summary>
        [CustomerHeaderAttribute("默认的资源加载路径优先级 可以设置 m_Priority 改变优先级", "#FF0000FF")]
        [SerializeField]
        [Space(20)]
        private List<LoadAssetPathInfor> m_LoadAssetPath = new List<LoadAssetPathInfor>();

        //[CustomerHeaderAttribute("优先级排序规则 ，默认从高到低规则(根据定义的LoadAssetPathEnum 确定如何选择)", "#FF0000FF")]
        //[SerializeField]
        //private bool m_IsPriorityUpToLower = true;

        [CustomerHeaderAttribute("排序后的优先级序列   只读不要修改!!!!!", "gray")]
        [SerializeField]
        private List<LoadAssetPathInfor> m_LoadAssetPathOfPriority = new List<LoadAssetPathInfor>(); //排序后的优先级序列

        /// <summary>
        /// 全局控制资源加载的方式 要么同步，要么异步加载，否则同步异步同时进行有问题
        /// </summary>
        [CustomerHeaderAttribute("资源加载同步/异步")]
        [Space(10)]
        public LoadAssetModel m_CurLoadAssetModel = LoadAssetModel.Async;

        #endregion


        //[Space(10)]
        //[CustomerHeaderAttribute("日志输出级别，默认只输出 Log 级别日志 ", "#00E7E7FF")]
        //public Debug.LogLevel m_LogLevel = Debug.LogLevel.Log;

        [Space(10)]
        [CustomerHeaderAttribute("控制是否统计资源被加载次数,用于系统资源分析", "#00E7E7FF")]
        public bool m_IsEnableResourcesLoadTrace = true;

        [Space(10)]
        [CustomerHeaderAttribute("标识是否是编辑器环境  如果是的话服务器器地址加载的是本地的资源 路径会不同", "#00E7E7FF")]
        public bool m_IsRemoteServer = false;

        [Space(10)]
        [CustomerHeaderAttribute("标识用于资源热更新的服务器地址", "#00E7E7FF")]
        //public string m_AssetServerPath = "file://" + @"E:/My_WorkSpace/AssetBundle_Test/";
        public List<HotAssetServerAddressInfor> m_AllHotAssetServerInfor = new List<HotAssetServerAddressInfor>();


        #region  Frame 
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            SetDefaultLoadPathInfor();
            SetDefaultAssetUpdatePathInfor();
        }

        /// <summary>
        /// 组件序列化变化
        /// </summary>
        private void OnValidate()
        {
            for (int dex = 0; dex < m_AllHotAssetServerInfor.Count; ++dex)
            {
                m_AllHotAssetServerInfor[dex].EditorUpdateView();
            }
        }

        /// <summary>
        /// 设置默认的优先级
        /// </summary>
        private void SetDefaultLoadPathInfor()
        {
            m_LoadAssetPath.Clear();
            var loadAssetPaths = System.Enum.GetValues(typeof(LoadAssetPathEnum));
            int priority = 1;
            foreach (var item in loadAssetPaths)
            {
                LoadAssetPathInfor infor = new LoadAssetPathInfor();
                infor.m_AssetPathEnum = (LoadAssetPathEnum)System.Enum.Parse(typeof(LoadAssetPathEnum), item.ToString());
                infor.m_Priority = priority;
                if (infor.m_AssetPathEnum == LoadAssetPathEnum.None) continue;
                m_LoadAssetPath.Add(infor);
                ++priority;
            }
        }

        /// <summary>
        /// 设置默认的热更新资源服务器地址
        /// </summary>
        private void SetDefaultAssetUpdatePathInfor()
        {
            m_AllHotAssetServerInfor.Clear();
            var loadAssetEnum = System.Enum.GetValues(typeof(HotAssetEnum));
            foreach (var item in loadAssetEnum)
            {
                HotAssetServerAddressInfor infor = new HotAssetServerAddressInfor();
                infor.m_AssetEum = (HotAssetEnum)System.Enum.Parse(typeof(HotAssetEnum), item.ToString());
                infor.m_ServerAssetPath = "";
                m_AllHotAssetServerInfor.Add(infor);
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
            });  //默认优先级是从高到低

#if UNITY_EDITOR
            if (IsDevelopMode)
            {
                int index = -1;
                for (int dex = 0; dex < m_LoadAssetPathOfPriority.Count; ++dex)
                {
                    if (m_LoadAssetPathOfPriority[dex].m_AssetPathEnum == LoadAssetPathEnum.ResourcesPath)
                    {
                        index = dex;
                        break;
                    }
                }

                if (index == -1)
                {
                    Debug.LogError("SortLoadAssetPathPriority Fail,Not Define ResourcesLoad");
                    return;
                }
                LoadAssetPathInfor resourceLoadInfor = m_LoadAssetPathOfPriority[index];
                m_LoadAssetPathOfPriority.RemoveAt(index);
                resourceLoadInfor.m_Priority = int.MaxValue;
                m_LoadAssetPathOfPriority.Insert(0, resourceLoadInfor);
            }//编辑器开发者模式下优先加载Resources 目录资源

            for (int dex = 0; dex < m_LoadAssetPathOfPriority.Count; ++dex)
            {
                Debug.LogEditorInfor(string.Format("dex={0} value={1}", dex, m_LoadAssetPathOfPriority[dex].m_AssetPathEnum));
            }
#endif

        }

        /// <summary>Oa
        /// 根据当前的加载路径获取下一个第一级优先级的路径枚举
        /// </summary>
        /// <param name="curAssetPath">返回是否</param>
        /// <returns></returns>
        public void GetNextLoadAssetPath(ref LoadAssetPathEnum curAssetPath)
        {
            for (int dex = 0; dex < m_LoadAssetPathOfPriority.Count - 1; ++dex)
            {
                if (m_LoadAssetPathOfPriority[dex].m_AssetPathEnum == curAssetPath)
                {
                    curAssetPath = m_LoadAssetPathOfPriority[dex + 1].m_AssetPathEnum;
                    return;
                }
            }
            curAssetPath = LoadAssetPathEnum.None;
        }

        /// <summary>
        /// 获取优先级最高的路径
        /// </summary>
        /// <returns></returns>
        public LoadAssetPathEnum GetFirstPriortyAssetPathEnum()
        {
            return m_LoadAssetPathOfPriority[0].m_AssetPathEnum;
        }

        /// <summary>
        /// 根据资源类型获取服务器地址
        /// </summary>
        /// <param name="assetEum"></param>
        /// <returns></returns>
        public HotAssetServerAddressInfor GetHotAssetServerAddressInforByType(HotAssetEnum assetEum)
        {
            foreach (var item in m_AllHotAssetServerInfor)
            {
                if (assetEum == item.m_AssetEum)
                    return item;
            }
            Debug.LogError("没有定义热更新资源服务器地址：" + assetEum);
            return null;
        }

        #endregion



    }
}