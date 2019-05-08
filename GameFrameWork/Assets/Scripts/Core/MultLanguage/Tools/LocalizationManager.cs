using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    /// <summary>
    /// 本地化组件管理
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        #region 变量属性
        [Header("当前语言类型")]
        public Language currentLanguageType;
        [Header("所有的文本本地化组件")]
        public List<UITextLocalization> allUITextLocalizations = new List<UITextLocalization>();
        [Header("所有的艺术字本地化组件")]
        public List<ImageSwitchTag> allImageSwitchTags = new List<ImageSwitchTag>();
        [Header("所有的RectTransform本地化组件")]
        public List<LocalizationRectTransform> allLocalizationRectTransforms = new List<LocalizationRectTransform>();
        [Header("所有的TextMeshPro本地化组件")]
        public List<LocalizationTextMeshPro> allLocalizationTextMeshPros = new List<LocalizationTextMeshPro>();
        [Header("所有的Image本地化组件")]
        public List<LocalizationImage> allLocalizationImages = new List<LocalizationImage>();

        private Language m_lastLanguageType;
        #endregion

        #region 编辑器操作
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            if (m_lastLanguageType == currentLanguageType)
                return;
            InitView(currentLanguageType);
            m_lastLanguageType = currentLanguageType;
            Debug.Log("切换语言完成！");
        }

        /// <summary>
        /// 初始化视图
        /// </summary>
        private void InitView(Language languageType)
        {
            foreach (UITextLocalization localization in allUITextLocalizations)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }

            foreach (ImageSwitchTag localization in allImageSwitchTags)
            {
                if (localization == null)
                    continue;
                localization.ShowImageViewBaseOnLanguage(languageType);
            }

            foreach (LocalizationRectTransform localization in allLocalizationRectTransforms)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }

            foreach (LocalizationTextMeshPro localization in allLocalizationTextMeshPros)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }

            foreach (LocalizationImage localization in allLocalizationImages)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }
        }

        /// <summary>
        /// 获取所有的本地化组件
        /// </summary>
        public void GetAllLocalizations()
        {
            gameObject.GetComponentsInChildren(true, allUITextLocalizations);
            gameObject.GetComponentsInChildren(true, allImageSwitchTags);
            gameObject.GetComponentsInChildren(true, allLocalizationRectTransforms);
            gameObject.GetComponentsInChildren(true, allLocalizationTextMeshPros);
            gameObject.GetComponentsInChildren(true, allLocalizationImages);
        }

        [MenuItem("CONTEXT/LocalizationManager/统计所有本地化组件")]
        private static void GetAllLocalizations(UnityEditor.MenuCommand cmd)
        {
            LocalizationManager current = cmd.context as LocalizationManager;
            if (current == null)
            {
                Debug.LogError("统计失败！");
                return;
            }

            current.GetAllLocalizations();
            EditorUtility.DisplayDialog("", "统计所有本地化组件完成！", "OK");
        }
#endif
        #endregion
    }
}
