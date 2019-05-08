using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 对Unity  内置的PlayerPrefs的封装，用来本地化持续存储数据和分析数据
    /// </summary>
    public class PlayerPrefsMgr : Singleton_Static<PlayerPrefsMgr>
    {
        private Dictionary<string, string> m_AllLocalDataRecord = new Dictionary<string, string>(); //Value
        private const string S_LocalKeyRecord = "GameFrameWork_PlayerPrefsMgr";  //这个字段中包含所有的本地化存储的数据的Key

#if UNITY_EDITOR
        public static string LocalKeyRecord { get { return S_LocalKeyRecord; } }
#endif

        private Dictionary<string, string> m_AllLocalStoredKey = new Dictionary<string, string>(); //所有当前框架存储到本地的Key  value值上一次修改的时间(HashSet无法序列化)


        #region 初始化读取数据

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            GetLocalStoreKeys();
        }

        /// <summary>
        /// 获得本地的数据
        /// </summary>
        private void GetLocalStoreKeys()
        {
            string recordStr = PlayerPrefs.GetString(S_LocalKeyRecord, "");
            if (string.IsNullOrEmpty(recordStr))
            {
                m_AllLocalStoredKey.Clear();
            }
            else
            {
                try
                {
                    m_AllLocalStoredKey = JsonMapper.ToObject<Dictionary<string, string>>(recordStr);
                    GetLocalStoreInfor();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("GetLocalStoreKeys Ex=" + ex);
                }
            }
        }

        /// <summary>
        /// 获取本地保存的所有数据
        /// </summary>
        private void GetLocalStoreInfor()
        {
            foreach (var storekey in m_AllLocalStoredKey.Keys)
            {
                var getValue = PlayerPrefs.GetString(storekey);
                m_AllLocalDataRecord.Add(storekey, getValue);
            }
        }

        #endregion

        #region 存数据
        /// <summary>
        /// 保存Float 类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetFloat(string key, float value)
        {
            return SaveDataToLocalPlatfor(key, value.ToString());
        }


        /// <summary>
        /// 保存int   类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetInt(string key, int value)
        {
            return SaveDataToLocalPlatfor(key, value.ToString());
        }


        /// <summary>
        /// 保存string 类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetString(string key, string value)
        {
            return SaveDataToLocalPlatfor(key, value);
        }

        /// <summary>
        /// 保存数据到本地平台
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        private bool SaveDataToLocalPlatfor(string key, string dataValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("SetFloat  Fail,Key Is Not Avaliable");
                return false;
            }

            try
            {
                if (m_AllLocalStoredKey.ContainsKey(key) == false)
                    m_AllLocalStoredKey.Add(key, System.DateTime.Now.ToString("F"));

                string storeInfor = null;
                if (m_AllLocalDataRecord.TryGetValue(key, out storeInfor) == false)
                {
                    m_AllLocalDataRecord.Add(key, storeInfor);
                    string valueStr = JsonMapper.ToJson(m_AllLocalStoredKey);
                    PlayerPrefs.SetString(S_LocalKeyRecord, valueStr);
                }
                //    string newData = JsonMapper.ToJson(storeInfor);  
                PlayerPrefs.SetString(key, dataValue);  //这里不能直接存储 dataValue 否则取出数据的时候不知道是什么类型
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SaveDataToLocalPlatfor  Fail key={0 } ex={1}"), key, ex);
                return false;
            }
            return true;
        }


        #endregion

        #region  取数据
        public int GetInt(string key, int defaultValue = 0)
        {
            string record = null;
            if (m_AllLocalDataRecord.TryGetValue(key, out record))
            {
                return int.Parse(record);
            }
            Debug.LogError("GetInt  Fail,Not Exit " + key);
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            string record = null;
            if (m_AllLocalDataRecord.TryGetValue(key, out record))
            {
                return float.Parse(record);
            }
            Debug.LogError("GetInt  Fail,Not Exit " + key);
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = "")
        {
            string record = null;
            if (m_AllLocalDataRecord.TryGetValue(key, out record))
            {
                return record;
            }
            Debug.LogInfor("GetInt  Fail,Not Exit " + key);
            return defaultValue;
        }
        #endregion

    }
}