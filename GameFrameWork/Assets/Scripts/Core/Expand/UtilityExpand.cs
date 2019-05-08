using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

//namespace GameFrameWork
//{
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

    /// <summary>
    /// 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    public static void ResetTransProperty(this Transform target)
    {
        target.ResetTransProperty(Vector3.zero, Vector3.one, Quaternion.identity);
    }
    /// <summary>
    /// / 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="transpos"></param>
    public static void ResetTransProperty(this Transform target, Vector3 transpos)
    {
        target.ResetTransProperty(transpos, Vector3.one, Quaternion.identity);
    }
    /// <summary>
    /// / 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="transpos"></param>
    /// <param name="localScale"></param>
    public static void ResetTransProperty(this Transform target, Vector3 transpos, Vector3 localScale)
    {
        target.ResetTransProperty(transpos, localScale, Quaternion.identity);
    }
    /// <summary>
    /// / 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="transpos"></param>
    /// <param name="localScale"></param>
    /// <param name="rataion"></param>
    public static void ResetTransProperty(this Transform target, Vector3 transpos, Vector3 localScale, Quaternion rataion)
    {
        target.localPosition = transpos;
        target.localScale = localScale;
        target.rotation = rataion;
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

    #region Rectransform 扩展
    public static T GetAddComponent<T>(this RectTransform target) where T : Component
    {
        if (target == null)
        {
            Debug.LogError("GetAddComponent  Fail,Target RectTransform Is Null");
            return null;
        }

        return GetAddComponent<T>(target.gameObject);
    }

    /// <summary>
    /// 重置 RectTransform 位置属性
    /// </summary>
    /// <param name="target"></param>
    public static void ResetRectTransProperty(this RectTransform target)
    {
        target.ResetRectTransProperty(Vector2.zero, target.sizeDelta);
    }
    /// <summary>
    ///  重置 RectTransform 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="anchorPos"></param>
    public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos)
    {
        target.ResetRectTransProperty(anchorPos, target.sizeDelta);
    }
    //public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos, Vector2 size)
    //{
    //    target.ResetRectTransProperty(anchorPos, size);
    //}
    /// <summary>
    ///  重置 RectTransform 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="anchorPos"></param>
    /// <param name="size"></param>
    public static void ResetRectTransProperty(this RectTransform target, Vector2 anchorPos, Vector2 size)
    {
        target.anchoredPosition = anchorPos;
        target.sizeDelta = size;
    }

    /// <summary>
    ///Rectransform 扩展方法 ，返回RectTransform 子节点
    /// </summary>
    /// <param name="target"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static RectTransform GetChildEX(this RectTransform target, int index)
    {
        if (target == null)
        {
            Debug.LogError("target is Null");
            return null;
        }
        return target.transform.GetChild(index) as RectTransform;
    }


    /// <summary>
    /// 水平方向上 target 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Horizontial(this RectTransform trans, RectTransform target)
    {
        Vector2 relativePos = trans.InverseTransformPoint(target.position);  //target相对于trans坐标
        if (relativePos.x == 0)
            return true;

        bool IsoutsideLeft = false;
        bool IsoutsideRight = false;

        if (relativePos.x > 0)
        {
            IsoutsideRight = relativePos.x - target.rect.width / 2f >= trans.rect.width / 2f;  //target左边界是否超出trans右边界
        }
        else
        {
            IsoutsideRight = relativePos.x + target.rect.width / 2f <= -1 * trans.rect.width / 2f;  //target右边界是否超出trans左边界
        }

        return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
    }

    /// <summary>
    /// 垂直方向上 target 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Vertical(this RectTransform trans, RectTransform target)
    {
        Vector2 relativePos = trans.InverseTransformPoint(target.position);  //target相对于trans坐标
        if (relativePos.y == 0)
            return true;

        bool IsoutsideTop = false;
        bool IsoutsideBottom = false;


        if (relativePos.y > 0)
        {
            IsoutsideTop = relativePos.y - target.rect.height / 2f >= trans.rect.height / 2f;   //target下边界是否超出trans上边界
        }
        if (relativePos.y < 0)
        {
            IsoutsideBottom = relativePos.y + target.rect.height / 2f <= -1 * trans.rect.height / 2f;  //target上边界是否超出trans下边界
        }

        return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
    }




    /// <summary>
    /// 垂直方向上 世界左边点 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Vertical(this RectTransform trans, Vector3 wordPosition)
    {
        Vector2 relativePos = trans.InverseTransformPoint(wordPosition);  //target相对于trans坐标
        if (relativePos.y == 0)
            return true;

        bool IsoutsideTop = false;
        bool IsoutsideBottom = false;


        if (relativePos.y > 0)
        {
            IsoutsideTop = relativePos.y >= trans.rect.height / 2f;   //target下边界是否超出trans上边界
        }
        if (relativePos.y < 0)
        {
            IsoutsideBottom = relativePos.y <= -1 * trans.rect.height / 2f;  //target上边界是否超出trans下边界
        }

        return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
    }

    /// <summary>
    /// 水平方向上 世界坐标点 wordPosition 是否在trans 里面
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool IsInsideRect_Horizontial(this RectTransform trans, Vector3 wordPosition)
    {
        Vector2 relativePos = trans.InverseTransformPoint(wordPosition);  //target相对于trans坐标
        if (relativePos.x == 0)
            return true;

        bool IsoutsideLeft = false;
        bool IsoutsideRight = false;

        if (relativePos.x > 0)
        {
            IsoutsideRight = relativePos.x >= trans.rect.width / 2f;  //target左边界是否超出trans右边界
        }
        else
        {
            IsoutsideLeft = relativePos.x <= -1 * trans.rect.width / 2f;  //target右边界是否超出trans左边界
        }

        return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
    }

    #endregion

    #region  String(包括IO/Path ) 扩展
    /// <summary>
    /// 获取参数路径的父级目录名
    /// 示例(.../aa/bb/cc.text)=>bb
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetPathParentDirectoryName(this string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return "";

        string directoryPath = System.IO.Path.GetDirectoryName(filePath); //(.../aa/bb/cc.text)=>.../aa/bb
        if (string.IsNullOrEmpty(directoryPath))
            return "";
        directoryPath = System.IO.Path.GetFileNameWithoutExtension(directoryPath);
        return directoryPath;
    }

    /// <summary>
    /// 获取不带文件扩展名的文件路径(与  System.IO.Path.GetFileNameWithoutExtension() 不同，这里只是过滤了扩展名，并不是截取了文件名)
    ///  示例(.../aa/bb/cc.text)=>.../aa/bb/cc
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

    /// <summary>
    /// 获取指定文件路径上只包含当前文件父目录和文件名的路径
    /// 示例 (..../aa/bb.txt)=>(aa/bb.txt)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string GetFilePathWithOneDirectory(this string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return "";
        string _fileName;  //the realname
        string[] ur = filePath.Split('/');
        if (ur.Length > 0)
        {
            if (ur.Length > 1)
                _fileName = ur[ur.Length - 2] + "/" + ur[ur.Length - 1];
            else
                _fileName = ur[ur.Length - 1];
        }
        else
            _fileName = filePath;
        return _fileName;
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

    #region Stack 扩展
    /// <summary>
    /// 从Stack  中批量删除满足条件的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stackSources"></param>
    /// <param name="condition">要删除的元素满足的条件</param>
    /// <returns></returns>
    public static Stack<T> DeleteElements<T>(this Stack<T> stackSources, Func<T, bool> condition)
    {
        var allItems = stackSources.Where<T>(condition);  //找出所有满足条件的项
        var allRemainItems = stackSources.Except<T>(allItems);   //从源数据中去除查找出来的项
        allRemainItems = allRemainItems.Reverse<T>(); //翻转结果 
        Stack<T> result = new Stack<T>(allRemainItems);  //获取堆 (这里相当于foreach 循环压栈操作)
        return result;
    }
    #endregion

}
//}