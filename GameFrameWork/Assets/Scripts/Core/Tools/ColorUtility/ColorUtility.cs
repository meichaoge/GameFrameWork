using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 颜色转换常用接口
    /// </summary>
    public class ColorUtility : Singleton_Static<ColorUtility>
    {
        /// <summary>
        /// html 颜色转成Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public  Color htmlToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.black;

            // 转换颜色
            hex = hex.ToLower();
            if (hex.IndexOf("#") == 0 && (hex.Length == 7|| hex.Length == 9))
            {
                int r = Convert.ToInt32(hex.Substring(1, 2), 16);
                int g = Convert.ToInt32(hex.Substring(3, 2), 16);
                int b = Convert.ToInt32(hex.Substring(5, 2), 16);
                int a = 255;
                if (hex.Length == 9)
                     a = Convert.ToInt32(hex.Substring(7, 2), 16);
                return new Color(r / 255f, g / 255f, b / 255f,a/255);
            }
            else if (hex == "red")
            {
                return Color.red;
            }
            else if (hex == "green")
            {
                return Color.green;
            }
            else if (hex == "blue")
            {
                return Color.blue;
            }
            else if (hex == "yellow")
            {
                return Color.yellow;
            }
            else if (hex == "black")
            {
                return Color.black;
            }
            else if (hex == "white")
            {
                return Color.white;
            }
            else if (hex == "cyan")
            {
                return Color.cyan;
            }
            else if (hex == "gray")
            {
                return Color.gray;
            }
            else if (hex == "grey")
            {
                return Color.grey;
            }
            else if (hex == "magenta")
            {
                return Color.magenta;
            }
            else
            {
                return Color.black;
            }
        }

    }
}