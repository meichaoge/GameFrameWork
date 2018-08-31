//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//namespace GameFrameWork.Network
//{

//    /// <summary>
//    /// DownLoad CallHandler
//    /// </summary>
//    /// <param name="isSuccess">is WWW Down Success</param>
//    /// <param name="url"></param>
//    /// <param name="filePath">file Store Path</param>
//    /// <param name="data"></param>
//    /// <param name="downLoadAct"></param>
//    public delegate void DownLoadTetureHandler(bool isSuccess, string url, string filePath, byte[] data, Action<string, string> downLoadAct);
//    public class TextureDownLoadHepler : Singleton_Static<TextureDownLoadHepler>
//    {
//        [System.Serializable]
//        /// <summary>
//        /// 缓存数据
//        /// </summary>
//        public class TextureDownLoadHeplerCacheData
//        {
//            public string Url;
//            public Action<string, string> DownLoadAct;

//            public TextureDownLoadHeplerCacheData(string url, Action<string, string> action)
//            {
//                Url = url;
//                DownLoadAct = action;
//            }
//        }

//        protected override void InitialSingleton() { }

//        private int CurrentTaskCount = 0; //当前的下载任务数量
//        private int MaxTaskCount = 5;  //最大下载任务数量

//        /// <summary>
//        /// 已经缓存的数据和索引
//        /// </summary>
//        private Dictionary<string, string> m_AllLocalCacheTexturesDic = new Dictionary<string, string>(); //Cache Dic
//        //如果任务过多则缓存任务    所有等待下载的任务
//        private Queue<TextureDownLoadHeplerCacheData> m_WaitingList = new Queue<TextureDownLoadHeplerCacheData>();



//        /// <summary>
//        /// 下载图片接口
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="downLoadAct">下载成功或者失败后的回调</param>
//        public void DownLoadTexture(string url, Action<string, string> downLoadAct)
//        {
//            #region Url 检测

//            if (string.IsNullOrEmpty(url))
//            {
//                if (downLoadAct != null) downLoadAct("", url);
//                Debug.LogError("Url Should Not Be Null or Empty ");
//                return;
//            }

//            if (url.StartsWith("http") == false) url = "http://" + url;  //手机端必须以 http:// 开始否则无法识别地址
//            #endregion

//            if (m_AllLocalCacheTexturesDic.ContainsKey(url))
//            {
////#if UNITY_EDITOR
////                DataShow_TextureDownLoad.Insance.FlushView();
////#endif
//                Debug.Log("The File Is Exit ,Load Cache Image >>>  " + url);
//                if (downLoadAct != null) downLoadAct(m_AllLocalCacheTexturesDic[url], url);  //本地已经缓存了
//                return;
//            }

//            if (CurrentTaskCount >= MaxTaskCount)
//            {
//                //#if UNITY_EDITOR
//                //                DataShow_TextureDownLoad.Insance.FlushView();
//                //#endif
//                Debug.Log("Delay DownLoad .... DelayList Count=" + m_WaitingList.Count);
//                m_WaitingList.Enqueue(new TextureDownLoadHeplerCacheData(url, downLoadAct));
//                return;
//            } //当下载任务过多时候 进行限制

//            //DownLoad Texture
//            string fileFullPath = FileDirHelper.GetUrlFileDir(url);
//            fileFullPath = GetLocalCachePath(fileFullPath);
//            Debug.Log("Start DownLoad " + url + "     fileFullPath=" + fileFullPath);

//            ++CurrentTaskCount;
//            EventCenter.GetInstance().StartCoroutine(EventCenter.GetInstance().DownLoadAction(url, fileFullPath, downLoadAct, DownLoad));

//        }

//        private void DownLoad(bool isSuccess, string url, string filePath, byte[] data, Action<string, string> downLoadAct)
//        {
////#if UNITY_EDITOR
////            DataShow_TextureDownLoad.Insance.FlushView();
////#endif
//            --CurrentTaskCount;
//            try
//            {
//                if (false == isSuccess)
//                {
//                    if (downLoadAct != null)
//                        downLoadAct("", url);
//                    return;
//                }

//                if (m_AllLocalCacheTexturesDic.ContainsKey(url) == false)
//                {//Save To Dictionary
//                    Debug.Log("Save To Local Cache " + url);
//                    m_AllLocalCacheTexturesDic.Add(url, filePath);
//                    if (System.IO.File.Exists(filePath))
//                        System.IO.File.Delete(filePath);
//                    System.IO.File.WriteAllBytes(filePath, data);
//                }

//                if (downLoadAct != null) downLoadAct(filePath, url);
//            }
//            catch (Exception e) { Debug.LogError("DownLoad Fail ,Exception: " + e.Message); }
//            finally
//            {
//                Debug.Log("final .... downPreviousData Count=" + m_WaitingList.Count);
//                if (m_WaitingList.Count > 0)
//                {
//                    if (CurrentTaskCount < MaxTaskCount)
//                    {
//                        TextureDownLoadHeplerCacheData down = m_WaitingList.Dequeue();
//                        DownLoadTexture(down.Url, down.DownLoadAct);  //下载缓存的任务
//                    }//if
//                }//if
//            }//finally
//        }


//        //Save The File To Cache File
//        string GetLocalCachePath(string filePath)
//        {
//            string storePath = ConstDefine.TextureCachePath + System.IO.Path.GetDirectoryName(filePath);
//            Debug.Log("filePath=" + filePath + "    storePath=" + storePath);
//            if (System.IO.Directory.Exists(storePath) == false)
//                System.IO.Directory.CreateDirectory(storePath);

//            return ConstDefine.TextureCachePath + filePath;
//        }


//        #region Unity Editor Test

//#if UNITY_EDITOR
//        public  List<TextureDownLoadHeplerCacheData> GetAllWaitingList()
//        {
//            List<TextureDownLoadHeplerCacheData> result = new List<TextureDownLoadHeplerCacheData>();
//            foreach (var item in m_WaitingList)
//                result.Add(item);
//            return result;
//        }

//        public  List<string> GetAllDownLoadedList()
//        {
//            List<string> result = new List<string>();
//            foreach (var item in m_AllLocalCacheTexturesDic)
//                result.Add(item.Key);

//            Debug.Log("GetAllDownLoadedList..... " + result.Count);
//            return result;
//        }

//#endif

//        #endregion



//    }
//}


