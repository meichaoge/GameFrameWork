using UnityEngine;
using System.Net;
using System.IO;
using System;

namespace GameFrameWork.Network
{
    /// <summary>
    /// Http 下载工具类
    /// </summary>
    public class HttpDownLoadHelper
    {
        public static int currentTaskCount = 0;
        static int MaxTaskCount = 3;

        /// <summary>
        /// Http DowmLoad Fuc 
        /// </summary>
        /// <param name="httpUrl">Url </param>
        /// <param name="callback">DoewnLoad Complete CallBack   string Is FilePath(maybe null Or "") ,Byte[] Is RealData(maybe null) ,Noty Get byte[] data first</param>
        /// <param name="sender">sender </param>
        /// <param name="assetSavePath">下载后资源保存在哪个目录</param>
        public static void HttpDownLoad(string httpUrl, Action<string> callback, GameObject sender,string assetSavePath)
        {
            //Debug.Log("当前任务数量 " + currentTaskCount);
            if (sender == null || sender.activeSelf == false) return;

            if (string.IsNullOrEmpty(httpUrl)) return;
            if (httpUrl.StartsWith("http") == false)
                httpUrl = "http://" + httpUrl;

            if (currentTaskCount >= MaxTaskCount)
            {//Delay Dowload request 
                EventCenter.Instance.DelayDoEnumerator(0.1f, () =>
                {
                    HttpDownLoad(httpUrl, callback, sender, assetSavePath);
                });
                return;
            }

            #region File Path
            string _fileRelativeNamePath =httpUrl.GetFilePathWithOneDirectory(); //getfile path with one dicrionary path and fileName .
            string localPath = assetSavePath + _fileRelativeNamePath;  //保存到本地的目录以及名称
            string directoryPath = System.IO.Path.GetDirectoryName(localPath);
            if (Directory.Exists(directoryPath) == false)
                Directory.CreateDirectory(directoryPath);  //create     dictionary     beanvr

            #endregion


            //Debug.Log("xxxxxxxxxxxx " + localPath);
            if (File.Exists(localPath))
                HttpDown_FileExit(httpUrl, callback, sender, localPath);
            else
                HttpDown_FileNotExit(httpUrl, callback, sender, localPath);
        }


        private static void HttpDown_FileExit(string httpUrl, Action<string> callback, GameObject sender, string localPath)
        {
            string _fileRelativeNamePath = httpUrl.GetFilePathWithOneDirectory(); //getfile path with one dicrionary path and fileName .
            #region 同名文件存在
            //Debug.Log("File Exit ::" + localPath);
            HttpProtocolHelper httpHead = new HttpProtocolHelper();
            httpHead.m_Url = httpUrl;
            httpHead.m_Method = "HEAD";
            httpHead.m_HttpFinishDownHandle = (bool isHttpDownLoadOk, string _FileName, HttpWebResponse response) =>
            {
                Loom.QueueOnMainThread(() =>
                { //Go to Main Thread 
                    CheckWhetherNeedToDown(localPath, response, _fileRelativeNamePath, httpUrl, callback);
                });//main thread
            }; //handle
            currentTaskCount++;
            ////下载头信息
            httpHead.GetHttpResponse();
            #endregion
        }

        private static void CheckWhetherNeedToDown(string localPath, HttpWebResponse response, string _fileRelativeNamePath, string httpUrl, Action<string> callback)
        {
            #region   DownLoadHeadComplete            
            FileInfo oldFileInfor = new FileInfo(localPath);//Get the file information ,which contain the length and create time atc.
                                                            //Debug.Log(response.ContentLength + "   Old File Length::" + oldFileInfor.Length);
            if (oldFileInfor.Length != response.ContentLength)
            {  //文件长度不一致就下载新的
                #region Local File Is Need Update
                Debug.Log("File Need Update  ::" + _fileRelativeNamePath);
                HttpProtocolHelper httpFile = new HttpProtocolHelper();
                httpFile.m_Url = httpUrl;
                httpFile.m_FilePath = localPath;
                httpFile.m_HttpFinishDownHandle = (bool isOk, string _flle, HttpWebResponse res) =>
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        HttpDownLoadCallback(isOk, callback, localPath, httpUrl, res);
                    });
                };

                httpFile.GetHttpFileAndWriteLocalReturnFilePath();  //Begin Get File ,return file path

                #endregion
            }//if
            else
            {//读取本地文件
                #region GetLocalFile
                //Debug.Log("  The local File is the newest , Get Local File ");

                //if (CacheHelper.GetInstance().CacheImageDic2.ContainsKey(httpUrl) == false)
                //    CacheHelper.GetInstance().CacheImageDic2.Add(httpUrl, localPath); //Save Record

                if (callback != null)
                    callback(localPath);  //return local filepath

                #endregion
            }
            #endregion
        }

        private static void HttpDown_FileNotExit(string httpUrl, Action<string> callback, GameObject sender, string localPath)
        {
            #region 本地文件不存在 开始下载
            //    Debug.Log("Local File Not Exit ");
            HttpProtocolHelper httpLoad = new HttpProtocolHelper();
            httpLoad.m_Url = httpUrl;
            httpLoad.m_FilePath = localPath;
            httpLoad.m_HttpFinishDownHandle = (bool isOK, string _FileName, HttpWebResponse resp) =>
            {
                Loom.QueueOnMainThread(() =>
                {
                    HttpDownLoadCallback(isOK, callback, localPath, httpUrl, resp);
                });
            };
            currentTaskCount++;
            httpLoad.GetHttpFileAndWriteLocalReturnFilePath();  //Begin Get File ,return file path

            #endregion
        }

        private static void HttpDownLoadCallback(bool isOK, Action<string> callback, string localPath, string fileIdentifyUrl, HttpWebResponse resp)
        {
            #region DownLoad Complete  And Write LocalFile 
            if (isOK)
            {
                // Debug.Log("File DownLoad Complete");
                if (callback != null)
                    callback(localPath);
            }
            else
            {
                Debug.LogError("New  File DownLoad Fail " + localPath);
                if (callback != null) callback(null);  //Fail DownLoad
            }//else
            #endregion
        }


        public static void HttpDownLoadHeadOnly(string httpUrl, Action<string> callback, GameObject sender)
        {
            //   Debug.Log("AA " + currentTaskCount);
            if (currentTaskCount >= MaxTaskCount)
            {
                EventCenter.Instance.DelayDoEnumerator(0.1f, () =>
                {
                    HttpDownLoadHeadOnly(httpUrl, callback, sender);
                });
                return;
            }

            HttpProtocolHelper httpHead = new HttpProtocolHelper();
            httpHead.m_Url = httpUrl;
            httpHead.m_Method = "HEAD";
            httpHead.m_HttpFinishDownHandle = (bool isHttpDownLoadOk, string _FileName, HttpWebResponse response) =>
            {
                Loom.QueueOnMainThread(() =>
                { //Go to Main Thread 
                    if (isHttpDownLoadOk)
                    { //Complete DowLoad
                        if (callback != null) callback(response.ContentLength.ToString());  //DownLoad Faile
                    }
                    else
                    {
                        if (callback != null) callback(null);  //DownLoad Faile
                    }
                });
            };
            currentTaskCount++;
            ////下载头信息
            httpHead.GetHttpResponse();
        }



    }
}
