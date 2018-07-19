using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 自定义带颜色标识的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class CustomerHeaderAttribute : PropertyAttribute
    {
        /// <summary>
        /// 显示的标题名
        /// </summary>
        public string m_Ttitle;
        /// <summary>
        /// 标题颜色 使用常用的颜色字符串(如"yellow"/"gray")或者16进制颜色"#FFFFFF"
        /// </summary>
        public string m_Color;

        
        public CustomerHeaderAttribute(string title, string color)
        {
            m_Ttitle = title;
            m_Color = color;
        }


        public CustomerHeaderAttribute(string title)
        {
            m_Ttitle = title;
            m_Color = "gray";  //默认灰色
        }
    }




}