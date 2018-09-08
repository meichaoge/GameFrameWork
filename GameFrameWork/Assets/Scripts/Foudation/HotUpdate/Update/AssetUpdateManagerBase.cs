using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFrameWork.HotUpdate
{

    [System.Serializable]
    public enum HotAssetEnum
    {
        AssetBundleAsset,
        LuaAsset,  //Lua 脚本
    }

    [System.Serializable]
    public class HotAssetServerAddressInfor
    {
#if UNITY_EDITOR
        //  [FormerlySerializedAs("m_AssetEum")]   //FormerlySerializedAs 将一个字段序列化为已有的字段，多用于字段的重命名
        [SerializeField]
        private string m_EditorAssetEnum;

        public void EditorUpdateView()
        {
            m_EditorAssetEnum = m_AssetEum.ToString();
        }

#endif
        public HotAssetEnum m_AssetEum = HotAssetEnum.AssetBundleAsset;
        public string m_ServerAssetPath = "";  //服务器的配置文件地址 外层目录地址，其他所有的资源包括配置配件都是相对于这个目录下载
        public string m_ServerAssetFileName = ""; //文件名

        /// <summary>
        /// 获取资源完整的路径
        /// </summary>
        /// <returns></returns>
        public string GetConfigTotalPath()
        {
            return m_ServerAssetPath + string.Format("{0}_{1}", AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(), m_ServerAssetFileName);
        }

    }


    /// <summary>
    /// 资源更新的基类 处理如何更新资源
    /// 必须重写的接口 GetLocalAssetRecordInfor()/  GetServerAssetRecordInfor()/GetAssetDownLoadPath()/GetAssetRelativePathByUrl()
    /// </summary>
    public abstract class AssetUpdateManagerBase
    {
        #region  Data 
        protected HotAssetBaseRecordInfor m_LocalAssetRecord = null; //本地配置的 资源 信息
        protected HotAssetBaseRecordInfor m_ServerAssetRecord = null;//服务器配置的 资源 信息

        protected string m_ServerConfigureStr = ""; //服务器的资源配置表 会写到本地去

        protected Dictionary<string, int> m_AllNeedUpdateAssetPath = new Dictionary<string, int>(); //所有需要下载的 资源 包名  也包含下载失败时候 缓存的需要重新下载的资源列表
        protected Dictionary<string, int> m_AllDownLoadFailAssetRecord = new Dictionary<string, int>();    //记录所有下载失败的案例    //Valua 记录下载的次数
        protected Dictionary<string, bool> m_AllCompleteDownLoadAssetDic = new Dictionary<string, bool>(); //所有已经正确下载保存到本地的子  资源 。value标识是否已经被更新到本地的配置文件中


        protected int m_AllNeedDownLoadAssetCount = 0; //所有需要下载的资源总数
        protected int m_CurrentDownLoadCount = 0;//当前下载的AB资源数量
        protected int m_TotalDownLoadAssetCount = 0;   //下载成功的资源数量
  //      protected static int S_DownLoadCoutToRecord = 5;  //当下载的资源数量是5的整数倍则开始保存记录

        protected int m_MaxTryDownLoadTime = 3; //下载失败最大尝试次数

        #region  更新本地配置文件设置
        protected bool m_IsContainNewDownloadAsset = false; //标识是否在m_UpdateLocalRecordSpace 时间内有新的下载资源
        protected float m_UpdateLocalRecordSpace = 2f; //每次检测是否需要更新本地配置的时间间隔

        protected int m_UpdateTimerHashCode = 0; //更新定时器
        #endregion

        #endregion

        #region 状态

        public bool m_IsBegingAssetUpdate { protected set; get; }  //标识是否已经开始更新当前种类的资源
        private bool _IsInitialed = false;
        /// <summary>
        /// 标识是否调用了InitialedUpdateMgr(） 进行初始化
        /// </summary>
        public bool m_IsInitialed
        {
            set
            {
                _IsInitialed = value;
            }
            get
            {
                return _IsInitialed;
            }
        }

        public bool m_IsUpdateRecorded { protected set; get; }    //标识是否已经更新过记录文件
        public bool m_IsCompleteUpdate { protected set; get; }   //只有当所有的资源下载完成时候才是True
        public bool m_IsCompleteMD5 { protected set; get; }   //只有当所有的资源md5 码比对完成
        public bool m_IsDownLoadServerConfigure { protected set; get; }   //是否成功下载了服务器的配置文件 

        /// <summary>
        /// 需要下载的资源大小 单位B
        /// </summary>
        public int TotalNeedDownLoadCount { protected set; get; }

        #endregion

        #region   更新加载回调
        public Action<string> m_OnUpdateFailAct = null; //资源热更新失败回调
        public Action m_OnDownLoadServerConfigureFailAct = null;  //当下载服务器的配置文件失败时候
        public Action<int> m_CompleteCheckAssetStateAct = null;  //比对完服务器和本地  资源的版本
        public Action<List<string>> m_OnDownLoadAssetFailAct = null;  //当下载资源失败时候调用
        public Action<bool> m_OnAssetUpdateDone = null; //当资源已经下载完成时  参数标识是否完成整个更新
        public Action<string, int> m_OnDownLoadAssetSuccessAct = null; //下载保存资源成功  主要用于返回下载速度
        #endregion

        /// <summary>
        /// 热更新资源管理器类型
        /// </summary>
        public HotAssetEnum HotAssetManagerEnum { get; protected set; }
        protected string m_LocalAssetConfigurePath; //本地资源配置表路径
        protected string m_LocalAssetConfigureFileName; //本地资源配置文件名
        protected HotAssetServerAddressInfor m_ServerAssetConfInfor;  //服务器资源的配置文件
#if UNITY_EDITOR
        private Dictionary<string, int> m_TestDownloadRecordDic = new Dictionary<string, int>();  //测试记录下载的资源路径和大小 
#endif
        /// <summary>
        /// 加载前调用 初始化
        /// </summary>
        protected virtual void InitialedUpdateMgr()
        {
            m_IsUpdateRecorded = false;
            m_IsBegingAssetUpdate = false;
            m_IsCompleteMD5 = false;
            m_IsDownLoadServerConfigure = false;
            m_IsCompleteUpdate = false;
            TotalNeedDownLoadCount = 0;
            InitialState();
        }


        /// <summary>
        /// 更新完要主动调用一下 清理资源
        /// </summary>
        public virtual void OnDestroyMgr()
        {
            if (m_IsUpdateRecorded == false)
            {
                if (m_UpdateTimerHashCode != 0)
                    TimeTickUtility.Instance.UnRegisterTimer(m_UpdateTimerHashCode);
                UpdateLocalRecordConfigureText(true);  //避免某些情况下 没有刷新本地的配置文件

            }

            m_OnDownLoadServerConfigureFailAct = null;
            m_CompleteCheckAssetStateAct = null;
            m_OnDownLoadAssetFailAct = null;
            m_OnAssetUpdateDone = null;

            InitialState();
        }

        #region 资源更新 外部接口
        /// <summary>
        /// 开始资源比对/下载更新流程
        /// </summary>
        public void BeginAssetUpdateProcess(HotAssetServerAddressInfor serverInfor)
        {
            m_IsBegingAssetUpdate = true;
            m_ServerAssetConfInfor = serverInfor;
            CheckLoadAssetUpdateState();
        }
        #endregion

        /// <summary>
        ///  检测并开始加载流程
        /// </summary>
        /// <param name="localAsssetConfigPath">本地资源版本信息配置文件路径</param>
        /// <param name="localAssetConfigFileName">本地资源版本配置文件名</param>
        protected void CheckLoadAssetUpdateState()
        {
            if (m_IsInitialed == false)
            {
                InitialedUpdateMgr();
                // Debug.LogEditorInfor("调用前 请先调用 InitialedUpdateMgr() 初始化 !!,");
            }
            if (m_IsBegingAssetUpdate)
            {
                Debug.LogError("已经在更新过程中");
                if (m_OnUpdateFailAct != null)
                    m_OnUpdateFailAct.Invoke("已经在更新过程中");
                return;
            }
            if (m_ServerAssetConfInfor == null)
            {
                Debug.LogEditorInfor("没有配置正确的资源下载服务器信息 ,请在子类中重写InitialedUpdateMgr()  并初始化 m_ServerAssetConfInfor");
                return;
            }


            m_IsBegingAssetUpdate = true;
            GetLocalAseetConfigureRecordText(m_LocalAssetConfigurePath, m_LocalAssetConfigureFileName);
        }


        /// <summary>
        /// 重置状态
        /// </summary>
        protected virtual void InitialState()
        {
            if (m_IsInitialed) return;
            m_IsInitialed = true;
            m_TotalDownLoadAssetCount = 0;
            m_AllNeedDownLoadAssetCount = m_CurrentDownLoadCount = 0;

            m_AllDownLoadFailAssetRecord.Clear();
            m_AllNeedUpdateAssetPath.Clear();
            m_AllCompleteDownLoadAssetDic.Clear();

            m_LocalAssetRecord = m_ServerAssetRecord = null;

            m_IsCompleteMD5 = m_IsDownLoadServerConfigure = false;
            m_IsCompleteUpdate = false;
            m_IsUpdateRecorded = false;
        }

        #region  读取本地配置文件

        /// <summary>
        /// 加载本地的AB资源配置文件
        /// </summary>
        protected void GetLocalAseetConfigureRecordText(string localAsssetConfigPath, string localAssetConfigFileName)
        {
            Debug.Log("GetLocalAseetConfigureRecordText>>> " + localAsssetConfigPath + localAssetConfigFileName);
            ResourcesMgr.Instance.LoadFile(localAsssetConfigPath + localAssetConfigFileName, LoadAssetModel.Async, OnLocalConfigureRecorded);
        }

        /// <summary>
        /// 资源加载完成后的操作(转成对应的对象)
        /// </summary>
        /// <param name="assetText"></param>
        protected abstract void GetLocalAssetRecordInfor(string assetText);
        // {
        //需要根据不同的配置文件转成不同的对象
        //    m_LocalAssetRecord = JsonMapper.ToObject<HotAssetBaseRecordInfor>(assetText);
        //  }

        /// <summary>
        /// 本地配置文件加载解析后处理逻辑(转化配置/  下载服务器配置)
        /// </summary>
        /// <param name="assetText"></param>
        private void OnLocalConfigureRecorded(string assetText)
        {
            //****
            Debug.LogEditorInfor("这里需要优化 ，可以同步加载本地和处理服务器请求  TODO");
            GetLocalAssetRecordInfor(assetText);
            GetServerAssetConfigureRecordText(CheckLocalAbundleNeedUpdateRecord);
        }

        #endregion


        #region  下载并解析服务器的配置文件
        /// <summary>
        /// 服务器的Asset  配置
        /// </summary>
        protected virtual void GetServerAssetConfigureRecordText(System.Action callback)
        {
            Debug.LogInfor("GetServerAssetRecordText");
            DownLoadUtility.Instance.DownLoadAsset(m_ServerAssetConfInfor.GetConfigTotalPath(), (www, assetUrl) =>
            {
                if (string.IsNullOrEmpty(www.error) == false)
                {
                    Debug.LogError("GetServerABundleRecordText Fail  Error: " + www.error);
                    OnDownLoadServerConfigFail();
                    return;
                }
                m_ServerConfigureStr = www.text;  //保存配置文件
                GetServerAssetRecordInfor(www.text);
                if (m_ServerAssetRecord == null)
                {
                    Debug.LogError("Server ABInfor Can't Identify");
                    OnDownLoadServerConfigFail();
                    if (m_OnUpdateFailAct != null)
                        m_OnUpdateFailAct.Invoke("Server ABInfor Can't Identify");
                    return;
                }
                if (callback != null) callback();
            }, true);

        }

        /// <summary>
        /// 转化服务器给的配置信息 (需要根据不同的配置文件转成不同的对象)
        /// </summary>
        /// <param name="assetText"></param>
        protected abstract void GetServerAssetRecordInfor(string assetText);
        //  {
        //需要根据不同的配置文件转成不同的对象
        //    m_ServerAssetRecord = JsonMapper.ToObject<HotAssetRecordInfor>(assetText);
        //   }

        /// <summary>
        /// 当下载服务器的配置文件失败时候
        /// </summary>
        /// <param name="ww"></param>
        protected virtual void OnDownLoadServerConfigFail()
        {
            Debug.LogError("OnDownLoadServerConfigFail......");
            m_IsDownLoadServerConfigure = false;
            if (m_OnDownLoadServerConfigureFailAct != null)
                m_OnDownLoadServerConfigureFailAct();
        }

        #endregion


        #region 检查  Asset 是否需要更新
        /// <summary>
        /// 对比MD5 获取那些文件需要更新下载
        /// </summary>
        protected virtual void CheckLocalAbundleNeedUpdateRecord()
        {
            Debug.LogInfor("GetNeedUpdateRecourRecord");
            m_IsDownLoadServerConfigure = true;

            if (m_LocalAssetRecord == null || m_LocalAssetRecord.AllAssetRecordsDic.Count == 0)
            {
                foreach (var item in m_ServerAssetRecord.AllAssetRecordsDic)
                {
                    RecordNeedUpdateAssetState(GetAssetDownLoadPath(item.Key), item.Value.m_ByteSize, true);
                    TotalNeedDownLoadCount += item.Value.m_ByteSize;
                }
            } //第一次则全量更新
            else
            {
                HotAssetBaseInfor checkABInfor = null;
                foreach (var item in m_ServerAssetRecord.AllAssetRecordsDic)
                {
                    if (m_LocalAssetRecord.AllAssetRecordsDic.TryGetValue(item.Key, out checkABInfor) && (item.Value.m_MD5Code == checkABInfor.m_MD5Code))
                    {
                        continue;
                    }  //资源的MD5 信息一致说明不需要更新
                    else
                    {
                        RecordNeedUpdateAssetState(GetAssetDownLoadPath(item.Key), item.Value.m_ByteSize, true);
                        TotalNeedDownLoadCount += item.Value.m_ByteSize;
                    }//记录需要下载更新的资源
                }
            } //逐条比对 获取需要更新的资源列表
            m_IsCompleteMD5 = true;
            Debug.LogInfor("TotalNeedDownLoadCount=" + TotalNeedDownLoadCount);
            if (m_CompleteCheckAssetStateAct != null)
                m_CompleteCheckAssetStateAct(TotalNeedDownLoadCount);
        }


        /// <summary>
        /// 根据 Asset 名获取下载时候的路径
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        protected abstract string GetAssetDownLoadPath(string assetName);
        //{
        //    return "";
        //    //  return m_ServerABundlePath + ConstDefine.ABundleTopFileNameOfPlatformat + "/" + assetBundleName;
        //}
        #endregion


        #region 开始更新下载资源

        /// <summary>
        /// 开始更新下载资源
        /// </summary>
        /// <param name="donwnloadCallback">每次更新资源下载成功回调</param>
        /// <param name="assetUpdateDoneAct">完成资源更新回调</param>
        public virtual void BeginUpdateAsset(Action<string, int> donwnloadCallback, System.Action<bool> assetUpdateDoneAct)
        {
            if (m_IsCompleteMD5 == false || m_IsDownLoadServerConfigure == false)
            {
                Debug.LogError("BeginUpdateAsset Fail " + (m_IsDownLoadServerConfigure == false));
                return;
            }
            m_OnAssetUpdateDone = assetUpdateDoneAct;
            if (m_AllNeedUpdateAssetPath.Count == 0)
            {
                OnFinishDownLoadAsset();
                return;
            } //所有的资源已经是最新的

            m_UpdateTimerHashCode = TimeTickUtility.Instance.RegisterTimer(m_UpdateLocalRecordSpace, OnUpdateCallback);
            m_OnDownLoadAssetSuccessAct = donwnloadCallback;
            OnTryDownLoadAsset(OnDownloadAssetCallBack);
        }

        /// <summary>
        /// 下载 Asset 资源回调
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="url"></param>
        protected virtual void OnDownloadAssetCallBack(WWW ww, string url)
        {
            string relativetePath = GetAssetRelativePathByUrl(url);
            OnTrySaveDownloadAsset(ww, url, relativetePath);
            //Debug.LogEditorInfor("这里需要在考虑下是否需要这个检查TODO  " + url);
            int dex = url.IndexOf("file:///");
            string relativeUrl = url;
            if (dex != -1)
                relativeUrl = relativeUrl.Substring(dex + "file:///".Length);
            if (ww != null)
            {
                m_AllCompleteDownLoadAssetDic.Add(relativetePath, false);  //记录当前 AssetBundle 已经下载完成
                if (m_AllNeedUpdateAssetPath.ContainsKey(relativeUrl))
                {
                    if (m_AllNeedUpdateAssetPath[relativeUrl] != ww.bytes.Length)
                    {
                        Debug.LogEditorInfor(string.Format("下载资源大小不一致：{0}  服务器{1}  实际下载{2}", relativeUrl, m_AllNeedUpdateAssetPath[relativeUrl], ww.bytes.Length));
                    }
                    m_AllNeedUpdateAssetPath.Remove(relativeUrl);
                }
                else
                {
                    Debug.LogError("OnDownloadAssetCallBack Fail ::" + relativeUrl);
                }
            }
            CheckWhetherNeedReLoad();
        }

        /// <summary>
        /// 从下载完成回调中获取当前  的相对路径名以便于记录
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected abstract string GetAssetRelativePathByUrl(string url);


        /// <summary>
        /// 检测是否需要重新下载部分资源
        /// </summary>
        protected virtual void CheckWhetherNeedReLoad()
        {
            if (m_CurrentDownLoadCount != m_AllNeedDownLoadAssetCount) return;  //等待本次下载任务完成

            Debug.LogInfor("CheckWhetherNeedReLoad...." + m_AllDownLoadFailAssetRecord.Count);
            if (m_AllDownLoadFailAssetRecord.Count == 0)
            {
                OnFinishDownLoadAsset();
                Debug.Log("CheckWhetherNeedReLoad  Complete");
                return;
            }

            List<string> allFailDownloadAsset = new List<string>();
            foreach (var item in m_AllDownLoadFailAssetRecord)
            {
                if (item.Value >= m_MaxTryDownLoadTime)
                {
                    allFailDownloadAsset.Add(item.Key);
                }
                else
                {
                    Debug.LogInfor(">>下载子ABundle 资源失败 ，正在重新下载 " + item.Key);
                    RecordNeedUpdateAssetState(GetAssetDownLoadPath(item.Key), item.Value, true);
                }
            }

            if (allFailDownloadAsset.Count != 0)
            {
                Debug.LogError("OnDownLoadMainABundleCallBack  有资源下载多次失败 本次下载失败");
                UpdateLocalRecordConfigureText(true);  //这里强制写入一次配置文件 避免由于部分资源下载失败而没有记录的问题
                if (m_OnDownLoadAssetFailAct != null)
                    m_OnDownLoadAssetFailAct(allFailDownloadAsset);

                return;
            }

            if (m_AllNeedUpdateAssetPath.Count > 0)
                OnTryDownLoadAsset(OnDownloadAssetCallBack);
        }

        /// <summary>
        /// 固定时间回调 处理刷新本地数据
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hashCode"></param>
        protected void OnUpdateCallback(float time, int hashCode)
        {
            if (m_UpdateTimerHashCode != hashCode) return;
            if (m_IsContainNewDownloadAsset == false) return;
            m_IsContainNewDownloadAsset = false;
            Debug.LogInfor("aaaaaaaaaaaa " + time);

            UpdateLocalRecordConfigureText(false);
        }

        #endregion



        #region 下载 Asset 以及回调处理  保存下载的资源
        /// <summary>
        /// 下载新的资源
        /// </summary>
        protected void OnTryDownLoadAsset(Action<WWW, string> downLoadCallback)
        {
            Debug.LogInfor("OnTryDownLoadAsset Count=" + m_AllNeedUpdateAssetPath.Count);
            m_AllNeedDownLoadAssetCount = m_AllNeedUpdateAssetPath.Count;
            m_CurrentDownLoadCount = 0; //重置标识位

            foreach (var item in m_AllNeedUpdateAssetPath)
            {
                DownLoadUtility.Instance.DownLoadAsset(item.Key, downLoadCallback, false);
            }
            //    m_AllNeedUpdateAssetPath.Clear();
        }

        /// <summary>
        /// 处理下载回调  保存下载的资源
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="url"></param>
        protected void OnTrySaveDownloadAsset(WWW ww, string url, string relativePath)
        {
            ++m_CurrentDownLoadCount;
            if (ww == null || string.IsNullOrEmpty(ww.error) == false)
            {
                //string path = GetAssetRelativePathByUrl(url);
                Debug.LogError(string.Format("下载失败URL {0}", relativePath));
                RecordNeedUpdateAssetState(relativePath, 0, false);
                return;
            }
            int index = url.IndexOf(m_ServerAssetConfInfor.m_ServerAssetPath);
            if (index == -1)
            {
                Debug.LogError("OnTrySaveDownloadAsset  Fail,无法解析的地址 " + url);
                return;
            }
            m_IsContainNewDownloadAsset = true;  //标识有新的资源文件被下载
            try
            {
                ++m_TotalDownLoadAssetCount;
                string Relativepath = url.Substring(index + m_ServerAssetConfInfor.m_ServerAssetPath.Length);  //获取资源的相对路径
                string path = ConstDefine.S_AssetBundleTopPath + Relativepath;

                #region  保存下载的资源 (删除旧的资源)
                string DictionaryPath = Path.GetDirectoryName(path);
                if (Directory.Exists(DictionaryPath) == false)
                    Directory.CreateDirectory(DictionaryPath);  //创建路径

                //Debug.LogInfor(Relativepath + "    本地保存的Asset  path=" + path);
                if (File.Exists(path))
                    File.Delete(path);   //删除旧的文件
                FileStream fileStream = File.Create(path);
                fileStream.Write(ww.bytes, 0, ww.bytes.Length);

                fileStream.Flush();
                fileStream.Close();

                OnDownLoadSuccess(relativePath, ww.bytes.Length); //下载成功   去除下载失败的记录
                #endregion

                m_IsContainNewDownloadAsset = true;

#if UNITY_EDITOR
                m_TestDownloadRecordDic.Add(relativePath, ww.bytes.Length);
#endif

            }
            catch (System.Exception ex)
            {
                Debug.LogError("OnDownLoadCallBack Exception:  " + ex.ToString());
            }
        }
        #endregion

        #region 更新本地配置文件
        /// <summary>
        /// 更新本地的配置文件  防止由于意外下载完成一半而导致部分 Asset 已经更新但是配置文件没有更新的问题
        /// </summary>
        /// <param name="forceRecord">是否忽略下载的文件个数限制</param>
        protected virtual void UpdateLocalRecordConfigureText(bool forceRecord)
        {
            if (m_IsCompleteUpdate || m_AllCompleteDownLoadAssetDic.Count == 0)
            {
                //Debug.LogInfor("UpdateLocalRecordConfigureText   No Need");
                return;
            }//已经完成正常的更新流程

            if (m_ServerAssetRecord == null)
            {
                Debug.LogError("UpdateLocalRecordConfigureText Fail ,Server Configure is Null");
                return;
            }//服务器的配置文件不存在

            m_IsUpdateRecorded = true;

            if (m_LocalAssetRecord == null)
            {
                m_LocalAssetRecord = new HotAssetBaseRecordInfor();
            }


            #region  对比更新本地配置文件数据
            HotAssetBaseInfor bundleInfor = null;
            List<string> allNewRecordKeys = new List<string>();
            foreach (var abundleRecord in m_AllCompleteDownLoadAssetDic)
            {
                if (abundleRecord.Value)
                    continue;  //已经被记录到本地了

                if (m_LocalAssetRecord.AllAssetRecordsDic.TryGetValue(abundleRecord.Key, out bundleInfor))
                {
                    if (bundleInfor.m_MD5Code != m_ServerAssetRecord.AllAssetRecordsDic[abundleRecord.Key].m_MD5Code)
                    {
                        //   Debug.LogInfor("Need Update AssetBundle :" + abundleRecord.Key);
                        m_LocalAssetRecord.AllAssetRecordsDic[abundleRecord.Key] = m_ServerAssetRecord.AllAssetRecordsDic[abundleRecord.Key];
                        allNewRecordKeys.Add(abundleRecord.Key);
                        continue;
                    }
                }
                else
                {
                    //Debug.LogInfor("New Add AssetBundle  :: " + abundleRecord.Key);
                    if (m_ServerAssetRecord.AllAssetRecordsDic.ContainsKey(abundleRecord.Key))
                    {
                        m_LocalAssetRecord.AllAssetRecordsDic.Add(abundleRecord.Key, m_ServerAssetRecord.AllAssetRecordsDic[abundleRecord.Key]);
                        allNewRecordKeys.Add(abundleRecord.Key);
                    }
                    continue;
                }
            }

            for (int dex = 0; dex < allNewRecordKeys.Count; ++dex)
            {
                m_AllCompleteDownLoadAssetDic[allNewRecordKeys[dex]] = true;
            }
            allNewRecordKeys.Clear();

            #endregion


            string localConfigureFilePath = m_LocalAssetConfigurePath + m_LocalAssetConfigureFileName;
            Debug.LogEditorInfor("localConfigureFilePath=" + localConfigureFilePath);
            UpdateLocalRecordFile(localConfigureFilePath, JsonMapper.ToJson(m_LocalAssetRecord));

        }
        #endregion

        /// <summary>
        /// 更新本地配置文件
        /// </summary>
        /// <param name="localConfigureFilePath"></param>
        /// <param name="jsonData"></param>
        private void UpdateLocalRecordFile(string localConfigureFilePath, string jsonData)
        {
            FileStream fileStream = null;
            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            try
            {
                //***********更新本地配置文件
                if (File.Exists(localConfigureFilePath))
                {
                    fileStream = File.Open(localConfigureFilePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
                else
                {
                    fileStream = File.Create(localConfigureFilePath);
                }

                if (fileStream != null)
                    fileStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Debug.LogError("ex=" + ex);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }


        #region 更新完成

        /// <summary>
        /// 所有的资源下载完成 
        /// </summary>
        protected virtual void OnFinishDownLoadAsset()
        {
            Debug.LogInfor("OnFinishDownLoadAB");
            m_IsCompleteUpdate = true;
            string filePath = m_LocalAssetConfigurePath + m_LocalAssetConfigureFileName;

            UpdateLocalRecordFile(filePath, m_ServerConfigureStr); //更新配置文件
            m_AllCompleteDownLoadAssetDic.Clear();
            if (m_OnAssetUpdateDone != null)
                m_OnAssetUpdateDone(m_IsCompleteUpdate);
        }
        #endregion

        #region 记录下载的文件的状态

        /// <summary>
        /// 检测当前资源是否能够加入到下载队列
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected void RecordNeedUpdateAssetState(string path, int size, bool isBeginDownOrDownLoadFail)
        {
            //Debug.Log("RecordNeedUpdateAssetState  " + path);
            if (isBeginDownOrDownLoadFail == false)
            {
                if (m_AllDownLoadFailAssetRecord.ContainsKey(path))
                    ++m_AllDownLoadFailAssetRecord[path];
                else
                    m_AllDownLoadFailAssetRecord.Add(path, 1);

                return;
            } //下载失败的时候记录状态

            if (m_AllDownLoadFailAssetRecord.ContainsKey(path))
            {
                if (m_AllDownLoadFailAssetRecord[path] < m_MaxTryDownLoadTime)
                {
                    Debug.Log("m_AllNeedUpdateAssetPath Add " + path);
                    m_AllNeedUpdateAssetPath.Add(path, size);
                    return;
                }
                else
                {
                    Debug.Log(string.Format("RecordNeedUpdateAssetState  资源 {0},已经下载失败{1}次", path, m_MaxTryDownLoadTime));
                    return;
                }
            } //当前资源已经下载失败过
            //Debug.Log("m_AllNeedUpdateAssetPath Add " + path);
            m_AllNeedUpdateAssetPath.Add(path, size);
        }
        protected void OnDownLoadSuccess(string path, int assetSize)
        {
            //Debug.Log("AAAAAAA  path" + path + "          assetSize= " + assetSize);
            if (m_OnDownLoadAssetSuccessAct != null)
                m_OnDownLoadAssetSuccessAct(path, assetSize);

            if (m_AllDownLoadFailAssetRecord.ContainsKey(path))
            {
                m_AllDownLoadFailAssetRecord.Remove(path);
                Debug.Log("有一些资源重新下载成功 " + path);
            }
        }
        #endregion


    }
}