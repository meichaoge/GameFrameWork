using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 全局的颜色定义
    /// </summary>
    public partial class ColorDefine : Singleton_Static<ColorDefine>
    {
        /// <summary>
        /// 金色
        /// </summary>
        public Color GoldenColor = new Color(248,200,0,255)/255f;
        public string GoldenColor_Tag = "#F8C800FF";

    }
}