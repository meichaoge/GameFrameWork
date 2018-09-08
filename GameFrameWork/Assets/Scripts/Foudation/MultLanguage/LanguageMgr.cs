using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 多语言管理器
    /// </summary>
    public class LanguageMgr : Singleton_Static<LanguageMgr>
    {

        private Language m_LanguageType = Language.Chinese;
        /// <summary>
        /// 当前应用使用的语言类型
        /// </summary>
        public Language CurLanguageType { get { return m_LanguageType; } private set { m_LanguageType = value; } }

        public System.Action<Language> OnAppLanguageSwitchAct = null; //切换语言事件


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            CurLanguageType = AppConfigSetting.Instance.LanguageType;
        }


        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language"></param>
        public void SwitchLanguage(Language language)
        {
            if (language == CurLanguageType) return;
            CurLanguageType = language;
            Debug.LogInfor("切换语言：" + language);

            AppConfigSetting.Instance.UpdateAndSaveConfig_Languege();  //更新配置文件

            if (OnAppLanguageSwitchAct != null)
                OnAppLanguageSwitchAct.Invoke(CurLanguageType);
        }

        /// <summary>
        /// 许多配置文件根据多语言按照枚举名分类
        /// </summary>
        /// <returns></returns>
        public string GetCurLanguageStr()
        {
            return CurLanguageType.ToString();
        }
        public string GetLanguageStr(Language language)
        {
            return language.ToString();
        }

       
    }
}