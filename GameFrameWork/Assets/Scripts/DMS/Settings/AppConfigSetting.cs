using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// App 应用配置信息
    /// </summary>
    public class AppConfigSetting : Singleton_Static<AppConfigSetting>, ISetting
    {
        #region  ISetting 接口信息

        private bool m_IsEnable = false;
        /// <summary>
        /// 标识当前Setting  是否正确完成初始化
        /// </summary>
        public bool IsEnable { get { return m_IsEnable; } private set { m_IsEnable = value; } }

        #endregion

        #region 配置文件信息
        /// <summary>
        /// 当前应用使用的语言类型
        /// </summary>
        public Language LanguageType { get; private set; }
        /// <summary>
        /// 日志输出等级
        /// </summary>
        public Debug.LogLevel LogLevelInf { get; private set; }

        public JsonData AppCofigJsonData; //配置文件信息
        #endregion


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialSetting();
        }

        #region 读取和解析配置文件

        public void InitialSetting()
        {
            if (IsEnable) return;  //已经初始化了
            ResourcesMgr.Instance.LoadFile(Define_Config.AppConfigPath, LoadAssetModel.Sync, (dataStr) =>
            {
                if (string.IsNullOrEmpty(dataStr))
                {
                    IsEnable = false;
                    return;
                }
                AnalysisConfig(dataStr);
            });
        }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="jsonStr"></param>
        private void AnalysisConfig(string jsonStr)
        {
            m_IsEnable = true;
            try
            {
                AppCofigJsonData = JsonMapper.ToObject(jsonStr);
                if (AppCofigJsonData.Keys.Contains("LanguageType"))
                    LanguageType = (Language)System.Enum.Parse(typeof(Language), AppCofigJsonData["LanguageType"].ToString());
                else
                    LanguageType = Language.Chinese;

                if (AppCofigJsonData.Keys.Contains("LanguageType"))
                    LogLevelInf = (Debug.LogLevel)System.Enum.Parse(typeof(Language), AppCofigJsonData["LogLevl"].ToString());
                else
                    LogLevelInf = Debug.LogLevel.Log;
                
            }
            catch (System.Exception ex)
            {
                m_IsEnable = false;
                Debug.LogError(string.Format("解析配置文件{0}出错 {1}", Define_Config.AppConfigPath, ex.ToString()));
            }  //放置解析错误
        }

        #endregion

        #region  更新配置文件信息 并保存到本地
        /// <summary>
        /// 更新语言配置
        /// </summary>
        /// <returns></returns>
        public bool UpdateAndSaveConfig_Languege()
        {
            AppCofigJsonData["LanguageType"] = (int)LanguageMgr.Instance.CurLanguageType;
            return IoUtility.Instance.SaveLocalDataOutStore(Define_Config.AppConfigPath, AppCofigJsonData.ToString(), true, false);
        }
        #endregion

    }
}