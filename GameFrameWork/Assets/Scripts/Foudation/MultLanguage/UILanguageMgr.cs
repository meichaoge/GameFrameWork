using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// UI动态/静态文本配置信息
    /// </summary>
    public class UILanguageMgr : Singleton_Static<UILanguageMgr>
    {
        /// <summary>
        /// 缓存下来避免每次都去转换和加载
        /// </summary>
        private Dictionary<string, JsonData> m_AllUIConfigFileInfor = new Dictionary<string, JsonData>(); //保存不同的配置文件中的配置


        #region  动态文本配置
        /// <summary>
        ///获得默认语言下的动态文本配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetUIDynamicStrConfig(string fileName, string key)
        {
            return GetUIDynamicStrConfig(fileName, key, LanguageMgr.Instance.CurLanguageType);
        }

        /// <summary>
        /// 获得指定语言下的动态文本配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetUIDynamicStrConfig(string fileName, string key, Language language)
        {
            return GetUIConfigStr(fileName, key, true, language);
        }
        #endregion


        #region  静态文本配置
        /// <summary>
        /// 获得默认语言下的静态文本配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetUIStaticStrConfig(string fileName, string key)
        {
            return GetUIStaticStrConfig(fileName, key, LanguageMgr.Instance.CurLanguageType);
        }
        /// <summary>
        /// 获得指定语言下的静态文本配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetUIStaticStrConfig(string fileName, string key, Language language)
        {
            return GetUIConfigStr(fileName, key, false, language);
        }
        #endregion

        /// <summary>
        /// 获得指定语言下指定文件中的配置信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        /// <param name="isDynamic"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private string GetUIConfigStr(string fileName, string key, bool isDynamic, Language language)
        {
            string filePath = null;
            if (isDynamic)
                filePath = string.Format("{0}/{1}/{2}/{3}", ConstDefine.S_MultLanguageConfigPathName, LanguageMgr.Instance.GetLanguageStr(language), ConstDefine.S_UIDynamincConfigPathName, fileName);
            else
                filePath = string.Format("{0}/{1}/{2}/{3}", ConstDefine.S_MultLanguageConfigPathName, LanguageMgr.Instance.GetLanguageStr(language), ConstDefine.S_UIStaticConfigPathName, fileName);

            string result = "";
            JsonData data;
            if (m_AllUIConfigFileInfor.TryGetValue(filePath, out data))
            {
                try
                {
                    result = data[key].ToString();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("GetUIConfigStr  Fail1 ,Not Exit!! Error= " + ex);
                }
            } //已经加载过的配置文件
            else
            {
                ResourcesMgr.Instance.LoadFile(filePath, LoadAssetModel.Sync, (configure) =>
                {
                    if (string.IsNullOrEmpty(configure)) return;
                    try
                    {
                        data = JsonMapper.ToObject(configure);
                        result = data[key].ToString();
                        m_AllUIConfigFileInfor.Add(filePath, data);  //保存数据
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("GetUIConfigStr  Fail2 ,Not Exit!! Error= " + ex);
                    }
                });
            }//第一次加载配置i文件则同步加载 缓存下来
            return result;
        }

    }
}