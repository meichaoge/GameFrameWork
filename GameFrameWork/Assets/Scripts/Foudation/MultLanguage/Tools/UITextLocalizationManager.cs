using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LitJson;
#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
#endif

namespace GameFrameWork
{
    /// <summary>
    /// Text组件本地化脚本管理
    /// </summary>
    public class UITextLocalizationManager : MonoBehaviour
    {
        #region 变量属性
        [Header("当前语言类型")]
        public Language currentLanguageType;
        [Header("所有的Text本地化组件")]
        public List<UITextLocalization> allUITextLocalizations = new List<UITextLocalization>();

        private Language m_lastLanguageType;
        #endregion

        #region 编辑器操作
#if UNITY_EDITOR
        /// <summary>
        /// 移除Text组件本地化脚本
        /// </summary>
        private void RemoveTextLocalization()
        {
            List<UITextLocalization> textLocalizations = new List<UITextLocalization>();
            gameObject.GetComponentsInChildren<UITextLocalization>(true, textLocalizations);
            foreach (UITextLocalization textLocalization in textLocalizations)
            {
                textLocalization.RemoveConfigKey();
                DestroyImmediate(textLocalization);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            currentLanguageType = Language.Chinese;
            m_lastLanguageType = currentLanguageType;
        }

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
            List<UITextLocalization> textLocalizations = new List<UITextLocalization>();
            gameObject.GetComponentsInChildren<UITextLocalization>(true, textLocalizations);
            foreach (UITextLocalization textLocalization in textLocalizations)
            {
                textLocalization.ChangeLanguageType(languageType);
            }
        }

        /// <summary>
        /// 获取所有的Text本地化组件
        /// </summary>
        public void GetAllUITextLocalizations()
        {
            allUITextLocalizations.Clear();
            List<UITextLocalization> textLocalizations = new List<UITextLocalization>();
            gameObject.GetComponentsInChildren<UITextLocalization>(true, textLocalizations);
            foreach (UITextLocalization textLocalization in textLocalizations)
            {
                if (string.IsNullOrEmpty(textLocalization.key))
                    continue;
                allUITextLocalizations.Add(textLocalization);
            }
        }

        [MenuItem("CONTEXT/UITextLocalizationManager/生成Text本地化组件，保存到指定配置文件")]
        private static void GenTextLocalization(UnityEditor.MenuCommand cmd)
        {
            UITextLocalizationManager current = cmd.context as UITextLocalizationManager;
            if (current == null)
            {
                return;
            }

            GameObject goSelected = current.gameObject;
            List<TMPro.TextMeshProUGUI> textMeshes = new List<TMPro.TextMeshProUGUI>();
            goSelected.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true, textMeshes);
            textMeshes = textMeshes.Where((textMesh) =>
            {
                if (textMesh.tag == "TMPro.TextMeshProUGUI" || textMesh.tag == "TextIngnoreLocalization") //动态文本不处理
                    return false;
                if (string.IsNullOrEmpty(textMesh.text)) //空文本不处理
                    return false;
                float result;
                if (float.TryParse(textMesh.text, out result)) //数字不处理
                    return false;
                return true;
            }).ToList();

            if (textMeshes.Count == 0)
            {
                Debug.Log("没有Text组件需要本地化！");
                DestroyImmediate(current);
                return;
            }

            string fileName = goSelected.name;
            string filePath = EditorUtility.SaveFilePanel("生成Text本地化组件", Application.dataPath + "/Resources/Localization/cn/ui_static", fileName + ".txt", "txt");
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.Log("已取消Text组件需要本地化！");
                return;
            }
            fileName = Path.GetFileNameWithoutExtension(filePath);

            foreach (var obj in System.Enum.GetValues(typeof(Language)))
            {
                Language languageType = (Language)obj;
                string languageName = LanguageMgr.Instance.GetLanguageStr(languageType);

                filePath = string.Format("{0}/Resources/Localization/{1}/ui_static/{2}.txt", Application.dataPath, languageName, fileName);
                JsonData jsonData = new JsonData();
                //if (File.Exists(filePath))
                //{
                //    jsonData = JsonMapper.ToObject(ResourceMgr.instance.Load<TextAsset>(
                //        string.Format("Localization/{0}/ui_static/{1}", languageName, fileName)).text);
                //}

                //foreach (TMPro.TextMeshProUGUI textMesh in textMeshes)
                //{
                //    UITextLocalization textLocalization = textMesh.gameObject.GetComponent<UITextLocalization>();
                //    if (textLocalization == null)
                //        textLocalization = textMesh.gameObject.AddComponent<UITextLocalization>();
                //    else
                //        textLocalization.Reset();
                //    textLocalization.fileName = fileName;

                //    if (jsonData.Contains(textLocalization.key))
                //        continue;

                //    string value = textMesh.text;
                //    if (languageType != Language.Chinese)
                //        value += string.Format("({0})", languageName);
                //    jsonData[textLocalization.key] = value;
                //}

                //StringBuilder sb = new StringBuilder();
                //sb.Append("{\n");
                //foreach (string key in jsonData.Keys)
                //{
                //    sb.Append(string.Format("\"{0}\":\"{1}\",\n", key, jsonData[key]));
                //}
                //sb.Append("}");

                //StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
                //sw.Write(sb.ToString());
                //sw.Close();
            }

            current.GetAllUITextLocalizations();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("生成Text本地化组件", "生成Text本地化组件完成！", "OK");
        }

        [MenuItem("CONTEXT/UITextLocalizationManager/移除Text本地化组件")]
        private static void RemoveTextLocalization(UnityEditor.MenuCommand cmd)
        {
            UITextLocalizationManager current = cmd.context as UITextLocalizationManager;
            if (current == null)
            {
                return;
            }

            current.RemoveTextLocalization();
            EditorUtility.DisplayDialog("移除Text本地化组件", "移除Text本地化组件完成！", "OK");
        }

        [MenuItem("CONTEXT/UITextLocalizationManager/统计所有的Text本地化组件")]
        private static void GetAllUITextLocalizations(UnityEditor.MenuCommand cmd)
        {
            UITextLocalizationManager current = cmd.context as UITextLocalizationManager;
            if (current == null)
            {
                Debug.LogError("统计失败！");
                return;
            }

            current.GetAllUITextLocalizations();
            Debug.Log("统计所有的Text本地化组件完成！");
        }
#endif
        #endregion
    }
}