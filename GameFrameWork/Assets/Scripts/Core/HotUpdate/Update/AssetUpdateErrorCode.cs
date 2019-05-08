using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GameFrameWork.HotUpdate
{
    /// <summary>
    /// 资源更新过程中的错误描述
    /// </summary>
    public enum AssetUpdateErrorCode
    {
        None=0,
        //***致命错误
        ServerConfigNotAvalible=1,   //服务器配置文件不可用
        SaveDateFail,  //保存配置或者更新资源失败  

        
        //***非致命错误
        LocalConfigureNotAvalible=100,  //本地配置不可用
        AssetDownLoadFail,  //资源下载失败


    }
}