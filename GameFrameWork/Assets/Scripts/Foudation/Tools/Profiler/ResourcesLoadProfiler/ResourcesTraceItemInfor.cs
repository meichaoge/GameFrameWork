using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 被追踪的资源信息
    /// </summary>
    [System.Serializable]
    public class ResourcesTraceItemInfor
    {
        public string m_ResourcesUrl;
        public int m_LoadCount { get; private set; }  //加载次数
        public Type m_ResourcesType;  //资源类型

        public ResourcesTraceItemInfor(string url, Type type)
        {
            m_ResourcesUrl = url;
            m_ResourcesType = type;
            m_LoadCount = 1;
        }

        public void AddRecordCount()
        {
            ++m_LoadCount;
        }

    }
}