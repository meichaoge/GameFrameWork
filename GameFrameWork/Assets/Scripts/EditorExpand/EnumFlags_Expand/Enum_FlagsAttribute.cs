using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    ///枚举属性显示成LayerMask 类似形式
    /// </summary>
    public class Enum_FlagsAttribute : PropertyAttribute
    {
        private string headTitle;

        /// <summary>
        ///使用[EnumFlagsAttribute("......")] 情况
        /// </summary>
        /// <param name="title"></param>
        public Enum_FlagsAttribute(string title)
        {
            headTitle = title;
        }


        public Enum_FlagsAttribute()
        {

        }

    }
}
