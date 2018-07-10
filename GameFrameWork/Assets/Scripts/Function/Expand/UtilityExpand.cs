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
        public static string GetPathParentDirectoryName (this string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "";

            string directoryPath = System.IO.Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryPath))
                return "";
            directoryPath = System.IO.Path.GetFileNameWithoutExtension(directoryPath);
            return directoryPath;
        }
        #endregion


    }
}