using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    public class LocalizationBase : MonoBehaviour
    {
        #region 变量属性
        [Header("当前语言类型")]
        public Language currentLanguageType;

        private Language m_lastLanguageType;
        #endregion

        #region 编辑器操作
#if UNITY_EDITOR
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            currentLanguageType = Language.Chinese;
            m_lastLanguageType = currentLanguageType;
            ResetConfig();
            SaveView(currentLanguageType);
        }

        /// <summary>
        /// 重置配置
        /// </summary>
        protected virtual void ResetConfig()
        {

        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="languageType"></param>
        public void ChangeLanguageType(Language languageType)
        {
            InitView(currentLanguageType);
        }

        /// <summary>
        /// 保存视图
        /// </summary>
        protected virtual void SaveView(Language languageType)
        {

        }

        [MenuItem("CONTEXT/LocalizationBase/保存当前语言下的配置")]
        private static void SaveCurrentView(UnityEditor.MenuCommand cmd)
        {
            LocalizationBase current = cmd.context as LocalizationBase;
            if (current == null)
            {
                return;
            }

            current.SaveView(current.currentLanguageType);
            EditorUtility.DisplayDialog("", "保存当前语言下的配置完成！", "OK");
        }

        [MenuItem("CONTEXT/LocalizationBase/刷新当前语言下的视图")]
        private static void UpdateCurrentView(UnityEditor.MenuCommand cmd)
        {
            LocalizationBase current = cmd.context as LocalizationBase;
            if (current == null)
            {
                return;
            }

            current.transform.localPosition += Vector3.one;
            current.InitView(current.currentLanguageType);
            EditorUtility.DisplayDialog("", "刷新当前语言下的视图完成！", "OK");
            current.transform.localPosition -= Vector3.one;  //为了强制刷新
        }
#endif
        #endregion

        #region 视图操作
        private void Awake()
        {
            InitView(AppConfigSetting.Instance.LanguageType);
        }

        /// <summary>
        /// 初始化视图
        /// </summary>
        protected virtual void InitView(Language languageType)
        {
            currentLanguageType = languageType;
        }
        #endregion
    }
}
