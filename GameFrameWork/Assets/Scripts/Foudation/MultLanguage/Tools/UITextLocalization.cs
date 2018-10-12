using System.Linq;
using UnityEngine;
using LitJson;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    /// <summary>
    /// Text组件本地化脚本
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class UITextLocalization : MonoBehaviour
    {
        #region 变量属性
        [Header("文本翻译的关键字")]
        public string key;
        [Header("文本翻译的配置文件名称")]
        public string fileName;
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
            GenConfigKey();
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
        }

        /// <summary>
        /// 切换当前语言类型
        /// </summary>
        /// <param name="languageType"></param>
        public void ChangeLanguageType(Language languageType)
        {
            currentLanguageType = languageType;
            OnValidate();
        }

        /// <summary>
        /// 生成配置文件关键字
        /// </summary>
        private void GenConfigKey()
        {
            UITextLocalizationManager textLocalizationMgr = gameObject.GetComponentInParent<UITextLocalizationManager>();
            Transform tfRoot = textLocalizationMgr == null ? transform.root : textLocalizationMgr.transform;
            fileName = tfRoot.name;

            key = transform.name;
            Transform tfParent = transform.parent;
            while (tfParent != null && tfParent != tfRoot.parent)
            {
                key = tfParent.name + "/" + key;
                tfParent = tfParent.parent;
            }
        }

        /// <summary>
        /// 移除配置文件关键字
        /// </summary>
        public void RemoveConfigKey()
        {
            bool removed = false;

            foreach (var obj in System.Enum.GetValues(typeof(Language)))
            {
                Language languageType = (Language)obj;
                string languageName = LanguageMgr.Instance.GetLanguageStr(languageType);

                string filePath = string.Format("{0}/Resources/Localization/{1}/ui_static/{2}.txt", Application.dataPath, languageName, fileName);
                if (!File.Exists(filePath))
                {
                    continue;
                }

                string staticConfigfilePath = string.Format(Define_Config.StaticTextConfigPath, languageName, fileName);
                ResourcesMgr.Instance.LoadFile(staticConfigfilePath, LoadAssetModel.Sync, (dataStr) => {
                    JsonData jsonData = JsonMapper.ToObject(dataStr);

                    if (!jsonData.ContainsKey(key))
                        return;
                    removed = true;

                    if (jsonData.Keys.Count(x => x != key) == 0)
                    {
                        File.Delete(filePath);
                        return;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("{\n");
                    foreach (string tmpKey in jsonData.Keys)
                    {
                        if (tmpKey == key)
                            continue;
                        sb.Append(string.Format("\"{0}\":\"{1}\",\n", tmpKey, jsonData[tmpKey]));
                    }
                    sb.Append("}");

                    StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
                    sw.Write(sb.ToString());
                    sw.Close();

                });
            }

            if (removed)
            {
                key = string.Empty;
                fileName = string.Empty;

                UITextLocalizationManager mgr = gameObject.GetComponentInParent<UITextLocalizationManager>();
                if (mgr != null)
                    mgr.GetAllUITextLocalizations();

                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 生成配置表关键字
        /// </summary>
        private void GenTextLocalization()
        {
            GenConfigKey();

            string filePath = EditorUtility.SaveFilePanel("生成配置表关键字", Application.dataPath + "/Resources/Localization/cn/ui_static", fileName + ".txt", "txt");
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.Log("已取消生成配置表关键字！");
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

                //string value = GetComponent<TMPro.TextMeshProUGUI>().text;
                //if (languageType != Language.Chinese)
                //    value += string.Format("({0})", languageName);
                //jsonData[key] = value;

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

            UITextLocalizationManager mgr = gameObject.GetComponentInParent<UITextLocalizationManager>();
            if (mgr != null)
                mgr.GetAllUITextLocalizations();
        }

        [MenuItem("CONTEXT/UITextLocalization/移除配置表关键字")]

        private static void RemoveConfigKey(UnityEditor.MenuCommand cmd)
        {
            UITextLocalization current = cmd.context as UITextLocalization;
            if (current == null)
            {
                return;
            }

            current.RemoveConfigKey();
            Debug.Log("移除配置表关键字完成！");
        }

        [MenuItem("CONTEXT/UITextLocalization/生成配置表关键字")]
        private static void GenTextLocalization(UnityEditor.MenuCommand cmd)
        {
            UITextLocalization current = cmd.context as UITextLocalization;
            if (current == null)
            {
                return;
            }

            current.GenTextLocalization();
            AssetDatabase.Refresh();
            Debug.Log("生成配置表关键字完成！");
        }
#endif
        #endregion

        #region 视图操作
        private void Awake()
        {
            InitView(LanguageMgr.Instance.CurLanguageType);
        }

        /// <summary>
        /// 初始化视图
        /// </summary>
        private void InitView(Language languageType)
        {
            currentLanguageType = languageType;

            TMPro.TextMeshProUGUI text = GetComponent<TMPro.TextMeshProUGUI>();
        //    text.text = Localization.instance.GetStaticTextString(fileName, key, languageType);
        }
        #endregion
    }
}