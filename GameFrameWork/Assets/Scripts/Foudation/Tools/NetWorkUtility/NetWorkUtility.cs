using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.Network
{

    /// <summary>
    /// 网络流量单位
    /// </summary>
    public enum NetDataEnum
    {
        B,
        KB,
        MB,
        GB,
        TB,
        PB
    }
    /// <summary>
    /// 网络环境状态
    /// </summary>
    public enum NetWorkStateEnum
    {
        Unknow,  //无法预知
        NetNotValible,//网络不可用
        UseWifiNet,  //Wifi 环境
        UseFlowDataNet, //使用流量
    }

    /// <summary>
    /// 提供网络方面公共的接口
    /// </summary>
    public class NetWorkUtility : Singleton_Static<NetWorkUtility>
    {
        private NetWorkStateEnum m_CurNetWorkStateEnum = NetWorkStateEnum.Unknow;
        public NetWorkStateEnum CurNetWorkStateEnum
        {
            get { return m_CurNetWorkStateEnum; }

            private set
            {
                if (value != m_CurNetWorkStateEnum)
                {
                    m_CurNetWorkStateEnum = value;
                    switch (m_CurNetWorkStateEnum)
                    {
                        case NetWorkStateEnum.NetNotValible:
                            if (OnNetWorkNotValibleEvent != null) OnNetWorkNotValibleEvent();
                            break;
                        case NetWorkStateEnum.UseWifiNet:
                            if (OnNetWorkNotValibleEvent != null) OnUseWifiNetEvent();
                            break;
                        case NetWorkStateEnum.UseFlowDataNet:
                            if (OnNetWorkNotValibleEvent != null) OnUseFlowDataNetEvent();
                            break;
                        default:
                            Debug.Log("Not Define " + m_CurNetWorkStateEnum);
                            break;
                    }//网络环境改变
                }
            }
        } //当前的网络环境

        public Action OnNetWorkNotValibleEvent;  //网络数据不可用
        public Action OnUseWifiNetEvent;  //使用Wifi 
        public Action OnUseFlowDataNetEvent;  //使用流量

       


        /// <summary>
        /// 是否在使用Wifi 环境
        /// </summary>
        /// <param name="callback"></param>
        public bool IsUsingWifiNet()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("IsUsingWifiNet False.... NetWork NotConnect");
                return false;
            }

            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                Debug.Log("IsUsingWifiNet false .... Use flow Data ..");
                return true;
            }
            Debug.Log("IsUsingWifiNet true     .... Use Wifi Data ..");
            return false;
        }

        /// <summary>
        /// 是否正在使用流量
        /// </summary>
        /// <returns></returns>
        public bool IsUsingFlowDataNet()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("IsUsingFlowDataNet False.... NetWork NotConnect");
                return false;
            }

            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                Debug.Log("IsUsingFlowDataNet false .... Use Wifi Data ..");
                return false;
            }

            Debug.Log("IsUsingFlowDataNet true .... Use flow Data ..");
            return false;
        }


        #region  流量的显示方式转换

        /// <summary>
        /// 根据输入的数据大小和类型 返回正确的网络流量表示法  (注意这里考虑了参数不是以B为单位的情况，需要自己指定起始的计数流量单位)
        /// </summary>
        /// <param name="byteSize">数据大小</param>
        /// <param name="dataEnum">数据单位</param>
        /// <param name="isUpdaRounding">true是否是向上取整,false 时候直接除以1024 取整 </param>
        public void GetNetDataDesciption(ref int byteSize, ref NetDataEnum dataEnum, bool isUpdaRounding)
        {
            if (byteSize < 1024) return;
            if (isUpdaRounding)
                byteSize = (byteSize + 1023) / 1024;
            else
                byteSize = byteSize / 1024;
            dataEnum = (NetDataEnum)(dataEnum + 1);

            GetNetDataDesciption(ref byteSize, ref dataEnum, isUpdaRounding);
        }

        /// <summary>
        /// 根据输入的数据大小和类型 返回正确的网络流量表示法
        /// </summary>
        /// <param name="byteSize">数据大小</param>
        /// <param name="dataEnum">数据单位</param>
        /// <param name="accuracy">显示的数值精确到小数点后面几位</param>
        public void GetNetDataDesciption(ref float byteSize, ref NetDataEnum dataEnum, uint accuracy = 2)
        {
            if (byteSize < 1024)
            {
                byteSize = (int)(Mathf.Pow(10, accuracy) * byteSize) / Mathf.Pow(10, accuracy);
                return;
            }
            byteSize = byteSize / 1024;
            dataEnum = (NetDataEnum)(dataEnum + 1);

            GetNetDataDesciption(ref byteSize, ref dataEnum, accuracy);
        }
        #endregion

    }
}