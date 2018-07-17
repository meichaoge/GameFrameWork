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
        /// 编辑器 View 模板文件
        /// </summary>
        public static string S_UIEdirorViewTempPath
        {
            get
            {
                return Application.dataPath + "/" + "Editor/Template/UIView.tpl.txt";
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
        ///场景模型资源相对于Asset 的目录(所有的美术模型 放在这个目录下)
        /// </summary>
        public static string S_ModelTopRelativePath
        {
            get { return "Art/Model/"; }
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