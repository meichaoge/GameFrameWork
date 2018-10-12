using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// Resources相对目录的路径定义
    /// </summary>
    public class Define_ResPath
    {
        #region UGUI 

        public static string UIAssetUpdateViewPath = "Prefabs/UI/AssetUpdate/UIAssetUpdateView"; //热更界面

        #region 登录界面 UILogin

        public static string UILoginPopupViewPath = "Prefabs/UI/Login/UILoginPopupView"; //登录弹窗界面
        #endregion

        #region 公共组件
        public static string UITextTipViewPath = "Prefabs/UI/Common/UITextTipView"; //飘字界面
        public static string UICanvasMaskViewPath = "Prefabs/UI/Common/UICanvasMaskView"; //全屏Mask界面

        #endregion

        #endregion

    }
}