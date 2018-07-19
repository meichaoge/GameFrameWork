using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 热更新资源记录基本信息
    /// </summary>
    [System.Serializable]
    public class HotAssetBaseRecordInfor
    {
        public string m_Version = "1.0.0.0";  //版本号
                                              //所有资源 AssetInfor Key 为当前 资源 相对路径
        public Dictionary<string, HotAssetBaseInfor> m_AllAssetRecordsDic = new Dictionary<string, HotAssetBaseInfor>(); 


    }
}