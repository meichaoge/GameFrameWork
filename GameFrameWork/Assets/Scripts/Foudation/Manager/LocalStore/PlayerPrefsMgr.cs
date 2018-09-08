using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace GameFrameWork
{
    /// <summary>
    /// 对Unity  内置的PlayerPrefs的封装，用来本地化持续存储数据和分析数据
    /// </summary>
    public class PlayerPrefsMgr : Singleton_Static<PlayerPrefsMgr>
    {
        private Dictionary<string, LocalStoreInfor> m_AllLocalDataRecord = new Dictionary<string, LocalStoreInfor>(); //Value 为序列化的Json 数据
        private const string S_LocalKeyRecord = "GameFrameWork_PlayerPrefsMgr";  //这个字段中包含所有的本地化存储的数据的Key
        private HashSet<string> m_AllLocalStoredKey = new HashSet<string>(); //所有当前框架存储到本地的Key 


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
                    m_AllLocalStoredKey = JsonMapper.ToObject<HashSet<string>>(recordStr);
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
            foreach (var storekey in m_AllLocalStoredKey)
            {
                LocalStoreInfor infor = JsonMapper.ToObject<LocalStoreInfor>(PlayerPrefs.GetString(storekey));
                m_AllLocalDataRecord.Add(storekey, infor);
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
                if (m_AllLocalStoredKey.Contains(key) == false)
                    m_AllLocalStoredKey.Add(key);

                LocalStoreInfor storeInfor = null;
                if (m_AllLocalDataRecord.TryGetValue(key, out storeInfor) == false)
                {
                    storeInfor = new LocalStoreInfor(key, dataValue, StoreInforStateEnum.Add);
                    m_AllLocalDataRecord.Add(key, storeInfor);
                    m_AllLocalStoredKey.Add(key);
                    PlayerPrefs.SetString(S_LocalKeyRecord, JsonMapper.ToJson(m_AllLocalStoredKey));
                }
                else
                {
                    storeInfor.UpdateRecord(dataValue);
                }
                string newData = JsonMapper.ToJson(storeInfor);
                PlayerPrefs.SetString(key, newData);
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
        public int GetInt(string key)
        {
            LocalStoreInfor record = null;
            if (m_AllLocalDataRecord.TryGetValue(key, out record))
            {
                return int.Parse(record.DataValue.ToString());
            }
            Debug.LogError("GetInt  Fail,Not Exit " + key);
            return 1;
        }

        public float GetFloat(string key)
        {
            LocalStoreInfor record = null;
            if (m_AllLocalDataRecord.TryGetValue(key, out record))
            {
                return float.Parse(record.DataValue.ToString());
            }
            Debug.LogError("GetInt  Fail,Not Exit " + key);
            return 1f;
        }

        public string GetString(string key)
        {
            LocalStoreInfor record = null;
            if (m_AllLocalDataRecord.TryGetValue(key, out record))
            {
                return record.DataValue.ToString();
            }
            Debug.LogError("GetInt  Fail,Not Exit " + key);
            return "";
        }
        #endregion

    }
}