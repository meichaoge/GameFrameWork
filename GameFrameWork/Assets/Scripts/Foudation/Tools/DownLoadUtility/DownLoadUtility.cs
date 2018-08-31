using GameFrameWork.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.HotUpdate
{
    /// <summary>
    /// 使用网络下载的统一接口
    /// </summary>
    public class DownLoadUtility : Singleton_Static<DownLoadUtility>
    {
        /// <summary>
        /// 网路下载资源 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <param name="needCheckUrl">是否需要检测Url 以“http://” 开头</param>
        public void DownLoadAsset(string url, System.Action<WWW, string> callback, bool needCheckUrl)
        {
            DownLoadHelper_WWW.DownLoadWithOutSaveLocal(url, callback, needCheckUrl);
        }

    }
}