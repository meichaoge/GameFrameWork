using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 在编辑器下定义常用的名字
    /// </summary>
    public class EditorDefine : Singleton_Static<EditorDefine>
    {
#if UNITY_EDITOR

        /// <summary>
        /// 编辑器 View 模板文件 (基本继承子UIViewBase)
        /// </summary>
        public static string S_UIEdirorViewTempPath
        {
            get
            {
                return Application.dataPath + "/" + "Editor/Template/UIView.tpl.txt";
            }
        }

        /// <summary>
        /// 编辑器 View 模板文件 (MonoBehaviour)
        /// </summary>
        public static string S_UIEdirorNormalViewTempPath
        {
            get
            {
                return Application.dataPath + "/" + "Editor/Template/UINormalView.tpl.txt";
            }
        }

        /// <summary>
        /// UI脚本存放目录
        /// </summary>
        public static string S_UGUISpritePath
        {
            get
            {
                return Application.dataPath + "/Scripts/UGUI";
            }
        }

        /// <summary>
        /// UI 图片资源相对于Asset 的目录(所有的美术资源UI 放在这个目录下)
        /// </summary>
        public static string S_UITextureTopRelativePath
        {
            get { return "Art/UI/"; }
        }
        /// <summary>
        /// 对于存在多个版本的资源 设置Sprite Tage时候需要过滤掉这个，目录名
        /// </summary>
        public static string S_UILocalizationPathFileName = "LocalizationUI/";

        /// <summary>
        ///场景模型资源相对于Asset 的目录(所有的美术模型 放在这个目录下)
        /// </summary>
        public static string S_ModelTopRelativePath
        {
            get { return "Art/Model/"; }
        }


        /// <summary>
        /// 打包多语言资源配置
        /// </summary>
        public static string S_BuildAppMultLanguageAssetPath {
            get
            {
                return "Assets/Editor/BuildApplication/multiLanguageRecource.asset";
            }
        }

        /// <summary>
        /// 打包多语言资源配置
        /// </summary>
        public static string S_MoveOutMultLanguageAssetPath
        {
            get
            {
                return "Assets/Editor/BuildApplication/moveOutAssetRecord.asset";
            }
        }

        /// <summary>
        /// 打包多语言资源配置 (打包前非当前语言资源临时存储路径)
        /// </summary>
        public static string S_BuildAppMultLanguageTempStorePath
        {
            get
            {
                string assetPath = System.IO.Path.GetDirectoryName(Application.dataPath); //获取Assets 同级目录
                return  string.Format("{0}/MultLanguageTempStore", assetPath);
            }
        }


        /// <summary>
        /// 框架的版本信息
        /// </summary>
        public static string S_FrameWorkVersionLogPath
        {
            get
            {
                return "Configure/FrameWorkVersion_config";
            }
        }


        private static GUISkin _CustomerGUISkin;
        public static GUISkin S_CustomerGUISkin
        {
            get
            {
                if (_CustomerGUISkin == null)
                    _CustomerGUISkin = Resources.Load<GUISkin>("CustomerGUISkin") ;
                return _CustomerGUISkin;
            }
        }



#endif
    }
}