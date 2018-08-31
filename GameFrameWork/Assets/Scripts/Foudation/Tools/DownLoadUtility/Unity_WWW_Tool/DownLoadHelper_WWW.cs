using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Network
{
    /// <summary>
    /// 使用Unity 内置的WWW下载 并且控制了最大的下载任务数量
    /// </summary>
    public class DownLoadHelper_WWW
    {
        /// <summary>
        /// 缓存数据
        /// </summary>
        private class DownLoadHelper_WWWCacheData
        {
            public string Url;
            public bool IsNeedCheckUrl = false;
            public Action<WWW, string> downLoadAct;

            public DownLoadHelper_WWWCacheData(string url, Action<WWW, string> action, bool isNeedCheck)
            {
                Url = url;
                downLoadAct = action;
                IsNeedCheckUrl = isNeedCheck;
            }
        }

        //如果任务过多则缓存任务    所有等待下载的任务
        private static Queue<DownLoadHelper_WWWCacheData> m_WaitingList = new Queue<DownLoadHelper_WWWCacheData>();


        public static int CurrentTaskCount = 0;
        private static int MaxTaskCount = 20;   //同一时刻最大的下载任务数量

        /// <summary>
        /// 下载资源 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <param name="needCheckUrl">是否需要检测Url 以“http://” 开头</param>
        public static void DownLoadWithOutSaveLocal(string url, Action<WWW, string> callback, bool needCheckUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null) callback(null, url);

                Debug.LogError("Url Should Not Be Null ");
                return;
            }

        //    if (needCheckUrl)
            {
                if (ApplicationConfig.Instance.m_IsRemoteServer)
                {
                    if (url.StartsWith("http") == false)
                        url = "http://" + url;
                }
                else
                {
                    if (url.StartsWith(@"file:///") == false)
                        url = string.Format(@"file:///{0}", url);
                }
            }

            if (CurrentTaskCount >= MaxTaskCount)
            {
                //Debug.Log("Delay DownLoad .... DelayList Count=" + m_WaitingList.Count);
                m_WaitingList.Enqueue(new DownLoadHelper_WWWCacheData(url, callback, needCheckUrl));
                return;
            }

            EventCenter.Instance.StartCoroutine(DownLoad(url, callback));
        }


        private static IEnumerator DownLoad(string url, Action<WWW, string> callback)
        {
            ++CurrentTaskCount;
            //Debug.Log("DownLoad url=  " + url);
            WWW ww = new WWW(url);
            yield return ww;
            if (string.IsNullOrEmpty(ww.error) == false)
            {
                Debug.LogError("DownLoad Fail " + ww.error + "    url=" + url);
                if (callback != null) callback(null, url);
                yield return null;
                ww.Dispose();
                --CurrentTaskCount;
                DownLoadCacheTask();
                yield break;
            }

            //Debug.Log("DownLoad success  " + ww.url);
            if (callback != null) callback(ww, url);
            yield return null;
            ww.Dispose();

            --CurrentTaskCount;
            DownLoadCacheTask();
            yield break;

        }


        private static void DownLoadCacheTask()
        {
            if (m_WaitingList.Count != 0)
            {
                DownLoadHelper_WWWCacheData down = m_WaitingList.Dequeue();
                //Debug.Log("Down Cache Data " + m_WaitingList.Count + "    down.Url=" + down.Url);
                DownLoadWithOutSaveLocal(down.Url, down.downLoadAct, down.IsNeedCheckUrl);  //下载缓存的任务
            }
        }


    }
}
