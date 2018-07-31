using GameFrameWork.ResourcesLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{

    /// <summary>
    /// 资源加载追踪,用于后期输出方便分析
    /// </summary>
    public class ResourcesLoadTraceMgr : Singleton_Static<ResourcesLoadTraceMgr>
    {
        /// <summary>
        /// 所有被加载的资源记录
        /// </summary>
        private Dictionary<string, ResourcesTraceItemInfor> m_AllTraceRecordItems = new Dictionary<string, ResourcesTraceItemInfor>();
        /// <summary>
        /// 资源为空的物体
        /// </summary>
        private Dictionary<string, int> m_AllNullResourcesRecordItems = new Dictionary<string, int>();


        public void RecordTraceResourceInfor(BaseAbstracResourceLoader loader)
        {
            if (ApplicationConfig.Instance.m_IsEnableResourcesLoadTrace == false) return;

            #region  记录加载为Null 的资源信息
            if (loader == null || loader.ResultObj == null)
            {
                if (loader != null)
                {
                    if (m_AllNullResourcesRecordItems.ContainsKey(loader.m_ResourcesUrl))
                        m_AllNullResourcesRecordItems[loader.m_ResourcesUrl]++;
                    else
                    {
                        m_AllNullResourcesRecordItems.Add(loader.m_ResourcesUrl, 1);
                    }
                }
                return;
            }
            #endregion

            if (m_AllTraceRecordItems.ContainsKey(loader.m_ResourcesUrl))
            {
                m_AllTraceRecordItems[loader.m_ResourcesUrl].AddRecordCount();
            }
            else
            {
                ResourcesTraceItemInfor itemInfor = new ResourcesTraceItemInfor(loader.m_ResourcesUrl, loader.ResultObj.GetType());
                m_AllTraceRecordItems.Add(loader.m_ResourcesUrl, itemInfor);
            }
        }

        /// <summary>
        /// 根据次数排序结果
        /// </summary>
        public void SortRecordInforByCount()
        {
            if (ApplicationConfig.Instance.m_IsEnableResourcesLoadTrace == false) return;
            Dictionary<string, ResourcesTraceItemInfor> m_SortRecordInfor = new Dictionary<string, ResourcesTraceItemInfor>(m_AllTraceRecordItems.Count);
            foreach (var record in m_AllTraceRecordItems)
            {
                m_SortRecordInfor.Add(record.Key, record.Value);
            }

            ///m_SortRecordInfor.Sort();
        }

    }
}