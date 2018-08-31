using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 扩展出常用的UI功能编辑菜单
    /// </summary>
    public class UIEditorEx : Editor
    {
        #region 项目定义的标签信息

        public enum EmObjType
        {
            Transform,
            Gameobject,
            Component,
            RectTransform,
        }

        [System.Serializable]
        public class TagInfor
        {
            public EmObjType objType;
            public string objName;
            public string prefix;  //对应的脚本
            public string tplText; //前缀

            public TagInfor(EmObjType objType, string objName, string prefix)
            {
                this.objType = objType;
                this.objName = objName;
                this.prefix = prefix;
                this.tplText = "";

                if (objType == EmObjType.Transform)
                {
                    this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\");\n";
                }
                else if (objType == EmObjType.Gameobject)
                {
                    this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\").gameObject;\n";
                }
                else if (objType == EmObjType.Component)
                {
                    this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\").gameObject.GetComponent<{0}>();\n";
                }
                else if (objType == EmObjType.RectTransform)
                {
                    this.tplText = "\t\t\t{0} {1} = transform.Find(\"{2}\") as RectTransform;\n";
                }
            }
        }

        /// <summary>
        /// 当前项目定义的Tag 
        /// </summary>
        public static Dictionary<string, TagInfor> TagObjs = new Dictionary<string, TagInfor>()
        {
            {"UI.Text", new TagInfor(EmObjType.Component, "Text", "txt")},
            {"UI.Image", new TagInfor(EmObjType.Component, "Image", "img")},
            {"UI.RawImage", new TagInfor(EmObjType.Component, "RawImage", "raw")},
            {"UI.Button", new TagInfor(EmObjType.Component, "Button", "btn")},
            {"UI.Toggle", new TagInfor(EmObjType.Component, "Toggle", "tgl")},
            //{"UI.Slider", new TagInfor(EmObjType.Component, "Slider", "sld")},
            //{"UI.Scrollbar", new TagInfor(EmObjType.Component, "Scrollbar", "scb")},
            {"UI.Dropdown", new TagInfor(EmObjType.Component, "Dropdown", "drop")},
            {"UI.InputField", new TagInfor(EmObjType.Component, "InputField", "input")},
            {"UI.Canvas", new TagInfor(EmObjType.Component, "Canvas", "cav")},
       //     {"UI.ScrollRect", new TagInfor(EmObjType.Component, "ScrollRect", "scr")},
            {"UnityEngine.GameObject", new TagInfor(EmObjType.Gameobject, "GameObject", "go")},
            {"UnityEngine.Transform", new TagInfor(EmObjType.Transform, "Transform", "tf")},
            {"UnityEngine.RectTransform", new TagInfor(EmObjType.RectTransform, "RectTransform", "rtf")},
            {"UnityEngine.ParticleSystem", new TagInfor(EmObjType.Component, "ParticleSystem", "pts")},
            {"TMPro.TextMeshProUGUI", new TagInfor(EmObjType.Component, "TMPro.TextMeshProUGUI", "txt")},
        //    {"UICacheItemMark", new TagInfor(EmObjType.Component, "UICacheItemMark", "")},
             {"AudioSource", new TagInfor(EmObjType.Component, "AudioSource", "Aud")},
          //  {"ScrollRectEx", new TagInfor(EmObjType.Component, "ScrollRectEx", "scroll")},
               {"TMPro.TMP_Dropdown", new TagInfor(EmObjType.Component, "TMPro.TMP_Dropdown", "DropdownPro")},
        };


        #endregion


        [MenuItem("Assets/UIEdirorEx/创建精灵 SpriteRender 预制体")]
        public static void CreateSpriteRender()
        {
            EditorCreateSpriteRender.CreateSpriteRender();
        }

        [MenuItem("Assets/UIEdirorEx/选中节点，生成View代码")]
        // [MenuItem("CONTEXT/UICacheView/生成View代码")]
        public static void GenUIViewCode()
        {
            GameObject goSelected = Selection.activeGameObject;
            if (goSelected == null)
            {
                Debug.LogError("没有选中Prefab！");
                return;
            }

            if (!File.Exists(EditorDefine.S_UIEdirorViewTempPath))
            {
                Debug.LogError("模板文件不存在！" + EditorDefine.S_UIEdirorViewTempPath);
                return;
            }
            string className = goSelected.name;

            if (System.IO.Directory.Exists(EditorDefine.S_UGUISpritePath) == false)
                System.IO.Directory.CreateDirectory(EditorDefine.S_UGUISpritePath);

            string filePath = EditorUtility.SaveFilePanel("Generate Code", EditorDefine.S_UGUISpritePath, className + ".cs", "cs");
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            className = Path.GetFileNameWithoutExtension(filePath);

            List<GameObject> goChilds = goSelected.GetChildCollectionRecursive();
            goChilds.Insert(0, goSelected);

            StringBuilder uiParameter = new StringBuilder();
            StringBuilder viewTouiParameter = new StringBuilder();
            StringBuilder sbInitView = new StringBuilder();
            Dictionary<string, int> propertyNames = new Dictionary<string, int>();
            foreach (GameObject go in goChilds)
            {
                if (TagObjs.ContainsKey(go.tag))
                {
                    TagInfor tagObj = TagObjs[go.tag];
                    string objName = tagObj.objName;
                    string propertyName = tagObj.prefix + UIExpandUtility.FormatName(go.name);
                    UIExpandUtility.UniqueName(ref propertyNames, ref propertyName);
                    string path = go.transform.GetPath(goSelected.transform);
                    string value = string.Format(tagObj.tplText, objName, propertyName, path);
                    sbInitView.Append(value);
                    uiParameter.Append("private " + objName + " m_" + propertyName + " ;\n");
                    viewTouiParameter.Append("m_" + propertyName + "=" + propertyName + ";\n");
                }
            }

            string strInitView = sbInitView.ToString();
            StreamReader sr = new StreamReader(EditorDefine.S_UIEdirorViewTempPath, Encoding.UTF8);
            string strTpl = sr.ReadToEnd();
            sr.Close();


            strTpl = Regex.Replace(strTpl, " #UIPARAMETER#", uiParameter.ToString());
            strTpl = Regex.Replace(strTpl, " #INITVIEWTOPARAMETER#", viewTouiParameter.ToString());

            strTpl = Regex.Replace(strTpl, "#CLASSNAME#", className);
            //Debug.Log(className);
            strTpl = Regex.Replace(strTpl, "#INITVIEW#", strInitView);
            Debug.Log(strInitView);

            //拷贝到剪贴板
            TextEditor te = new TextEditor();
            te.text = strInitView;
            te.SelectAll();
            te.Copy();

            StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
            sw.Write(strTpl);
            sw.Close();

            AssetDatabase.Refresh();
            Debug.Log("生成View代码完成！");
        }

        [MenuItem("Assets/UIEdirorEx/选中节点，拷贝View代码到剪贴板")]
        //     [MenuItem("CONTEXT/UICacheView/拷贝View代码")]
        public static void CopyViewCodeToClipbord()
        {
            GameObject goSelected = Selection.activeGameObject;
            if (goSelected == null)
            {
                Debug.LogError("没有选中Prefab！");
                return;
            }


            if (!File.Exists(EditorDefine.S_UIEdirorViewTempPath))
            {
                Debug.LogError("模板文件不存在！" + EditorDefine.S_UIEdirorViewTempPath);
                return;
            }

            List<GameObject> goChilds = goSelected.GetChildCollectionRecursive();
            goChilds.Insert(0, goSelected);

            StringBuilder sbInitView = new StringBuilder();
            Dictionary<string, int> propertyNames = new Dictionary<string, int>();
            foreach (GameObject go in goChilds)
            {
                if (TagObjs.ContainsKey(go.tag))
                {
                    TagInfor tagObj = TagObjs[go.tag];
                    string objName = tagObj.objName;
                    string propertyName = tagObj.prefix + UIExpandUtility.FormatName(go.name);
                    UIExpandUtility.UniqueName(ref propertyNames, ref propertyName);
                    string path = go.transform.GetPath(goSelected.transform);
                    string value = string.Format(tagObj.tplText, objName, propertyName, path);
                    sbInitView.Append(value);
                }
            }

            string strInitView = sbInitView.ToString();
            StreamReader sr = new StreamReader(EditorDefine.S_UIEdirorViewTempPath, Encoding.UTF8);
            string strTpl = sr.ReadToEnd();
            sr.Close();

            strTpl = Regex.Replace(strTpl, "#INITVIEW#", strInitView);
            Debug.Log(strInitView);

            //拷贝到剪贴板
            TextEditor te = new TextEditor();
            te.text = strInitView;
            te.SelectAll();
            te.Copy();

            Debug.Log("拷贝View代码到剪贴板完成！");
        }
    }
}