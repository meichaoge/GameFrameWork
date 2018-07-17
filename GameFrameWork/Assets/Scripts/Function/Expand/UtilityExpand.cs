using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameFrameWork
{
    /// <summary>
    /// 通用的扩展
    /// </summary>
    public static class UtilityExpand
    {

        #region Transform 扩展
        public static T GetAddComponent<T>(this Transform target) where T : Component
        {
            if (target == null)
            {
                Debug.LogError("GetAddComponent  Fail,Target Transform Is Null");
                return null;
            }

            return GetAddComponent<T>(target.gameObject);
        }
        #endregion

        #region  GameObject 扩展
        public static T GetAddComponent<T>(this GameObject target) where T : Component
        {
            if (target == null)
            {
                Debug.LogError("GetAddComponent  Fail,Target GameObject Is Null");
                return null;
            }

            T result = target.GetComponent<T>();
            if (result == null)
                result = target.AddComponent<T>();
            return result;
        }
        #endregion

        #region  String(包括IO/Path ) 扩展
        /// <summary>
        /// 获取参数路径的父级目录名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetPathParentDirectoryName(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "";

            string directoryPath = System.IO.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryPath))
                return "";
            directoryPath = System.IO.Path.GetFileNameWithoutExtension(directoryPath);
            return directoryPath;
        }

        /// <summary>
        /// 获取不带文件扩展名的文件路径(与  System.IO.Path.GetFileNameWithoutExtension() 不同，这里只是过滤了扩展名，并不是截取了文件名)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFilePathWithoutExtension(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            int index = filePath.LastIndexOf('.');
            if (index == -1) return filePath;
            return filePath.Substring(0, index);
        }

        #endregion

        #region  Rect 扩展
        /// <summary>
        ///增加一个Rect区域
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(this Rect target, float x = 0, float y = 0, float width = 0, float height = 0)
        {
            return new Rect(target.x + x, target.y + y, target.width + width, target.height + height);
        }
        /// <summary>
        /// 增加一个Rect区域
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect AddRect(this Rect target, Rect rect)
        {
            return new Rect(target.x + rect.x, target.y + rect.y, target.width + rect.width, target.height + rect.height);
        }


        /// <summary>
        /// 减少一个Rect区域
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect ReduceRect(this Rect target, Rect rect)
        {
            return new Rect(target.x - rect.x, target.y - rect.y, target.width - rect.width, target.height - rect.height);
        }

        /// <summary>
        ///减少一个Rect区域
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect ReduceRect(this Rect target, float x = 0, float y = 0, float width = 0, float height = 0)
        {
            return new Rect(target.x - x, target.y - y, target.width - width, target.height - height);
        }
        #endregion



    }
}