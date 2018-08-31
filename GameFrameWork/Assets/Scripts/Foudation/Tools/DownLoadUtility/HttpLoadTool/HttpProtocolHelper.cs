using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;

namespace GameFrameWork.Network
{

    public delegate void HttpWebResponseHandle(bool isOk, string _FileName, HttpWebResponse response);  //Http DownLoad CallBack

    //Http协议帮助类
    public class HttpProtocolHelper
    {
        public HttpWebResponseHandle m_HttpFinishDownHandle;
        public string m_Url;
        public string m_Method = "HEAD"; //Http Method
        public string m_FilePath;  //File store Path
        private HttpWebRequest resquest;  //the resquest
        public int m_TryTime = 3; //Try times
 

        public void GetHttpResponse()
        {
            if (m_Url.StartsWith("http") == false)
                m_Url = "http://" + m_Url;

            resquest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(m_Url));
            resquest.Method = m_Method;
            resquest.Timeout = 10000;
            resquest.BeginGetResponse((ar) =>
            {  //Async CallBack
                try
                {
                    #region Get Async Response 
                    //Debug.Log("DownLoad CallBack");
                    HttpWebResponse response = (HttpWebResponse)resquest.EndGetResponse(ar);
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {//Notify Out
                        --HttpDownLoadHelper.currentTaskCount;
                        if (m_HttpFinishDownHandle != null)
                            m_HttpFinishDownHandle(true, m_Url, response);
                    }
                    if (response != null)
                        response.Close();
                    #endregion
                }
                catch (Exception e)
                {
                    #region Try Again
                    Debug.LogError(e.Source + "Try Time " + m_TryTime + "  Http Error  " + e.ToString());
                    if (m_TryTime != 0)
                    {//失败重试3次
                        m_TryTime--;
                        GetHttpResponse();
                    }
                    else
                    {
                        --HttpDownLoadHelper.currentTaskCount;
                        if (m_HttpFinishDownHandle != null)
                            m_HttpFinishDownHandle(false, m_Url, null);
                    }//else
                    #endregion
                }
                finally
                {
                    if (resquest != null && resquest.GetResponse() != null)
                        resquest.GetResponse().Close();
                }
            }, null); //Begine Async Request DownLoad
        }



        public void GetHttpFileAndWriteLocalReturnFilePath()
        {
            if (m_Url.StartsWith("http") == false)
                m_Url = "http://" + m_Url;

            resquest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(m_Url));
            resquest.Method = "GET";
            resquest.Timeout = 20000;
            resquest.BeginGetResponse((ar) =>
            {
                try
                {
                    #region Get Response And Write Byte
                    HttpWebResponse response = (HttpWebResponse)resquest.EndGetResponse(ar);
                    Stream responseSrewam = response.GetResponseStream(); //获得数据流

                    string _direPath = System.IO.Path.GetDirectoryName(m_FilePath);

                    if (File.Exists(m_FilePath))  //Delete previous File
                        File.Delete(m_FilePath);

                    //     Debug.Log("路径AS " + _direPath);
                    if (Directory.Exists(_direPath) == false)
                        Directory.CreateDirectory(_direPath);

                    FileStream fileStream = new FileStream(m_FilePath, FileMode.Create, FileAccess.Write);
                    byte[] bytes = new byte[1024];
                    int readSize = 0;
                    while ((readSize = responseSrewam.Read(bytes, 0, 1024)) > 0)
                    { //循环度流和写文件
                        fileStream.Write(bytes, 0, readSize);
                    }

                    fileStream.Flush();
                    fileStream.Close();

                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        //if (CacheHelper.GetInstance().CacheImageDic2.ContainsKey(m_Url) == false)
                        //    CacheHelper.GetInstance().CacheImageDic2.Add(m_Url, m_FilePath); //Save Record

                        --HttpDownLoadHelper.currentTaskCount;
                        if (m_HttpFinishDownHandle != null)
                            m_HttpFinishDownHandle(true, m_FilePath, response);
                    }
                    if (responseSrewam != null)
                        responseSrewam.Close();
                    #endregion
                }
                catch (Exception e)
                {
                    #region Try Again
                    Debug.Log(m_Url + "  DownLoad Fail Times  " + m_TryTime + "   " + e.ToString());
                    if (m_TryTime != 0)
                    {//Try 3 times
                        m_TryTime--;
                        GetHttpFileAndWriteLocalReturnFilePath();
                    }
                    else
                    {
                        --HttpDownLoadHelper.currentTaskCount;
                        if (m_HttpFinishDownHandle != null)
                            m_HttpFinishDownHandle(false, m_FilePath, null);
                    }//esle
                    #endregion
                }
                finally
                {
                    if (resquest != null && resquest.GetResponse() != null)
                        resquest.GetResponse().Close();
                }
            }, null); //开始异步请求

        }



    }
}
