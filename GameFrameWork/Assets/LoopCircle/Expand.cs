//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;


//public static class Expand
//{

//    public static T GetAddComponent<T>(this GameObject obj) where T : Component
//    {
//        T result = obj.GetComponent<T>();
//        if (result == null)
//            result = obj.AddComponent<T>();

//        return result;
//    }

//    public static T GetAddComponent<T>(this Transform trans) where T : Component
//    {
//        T result = trans.GetComponent<T>();
//        if (result == null)
//            result = trans.gameObject.AddComponent<T>();

//        return result;
//    }

//    /// <summary>
//    ///Rectransform 扩展方法 ，返回RectTransform 子节点
//    /// </summary>
//    /// <param name="target"></param>
//    /// <param name="index"></param>
//    /// <returns></returns>
//    public static RectTransform GetChildEX(this RectTransform target, int index)
//    {
//        if (target == null)
//        {
//            Debug.LogError("target is Null");
//            return null;
//        }
//        return target.transform.GetChild(index) as RectTransform;
//    }

//    /// <summary>
//    /// 根据给定的路径返回不带扩展名路径  (例如 AA/BB/CC.text==>AA/BB/CC)
//    /// </summary>
//    /// <param name="path"></param>
//    /// <returns></returns>
//    public static string GetPathWithoutExtension(this string path)
//    {
//        if (string.IsNullOrEmpty(path))
//            return path;
//        int dex = path.IndexOf(".");
//        if (dex != -1)
//            return path.Substring(0, dex);
//        return path;
//    }



//    /// <summary>
//    /// 水平方向上 target 是否在trans 里面
//    /// </summary>
//    /// <param name="trans"></param>
//    /// <param name="target"></param>
//    /// <returns></returns>
//    public static bool IsInsideRect_Horizontial(this RectTransform trans, RectTransform target)
//    {
//        Vector2 relativePos = trans.InverseTransformPoint(target.position);  //target相对于trans坐标
//        if (relativePos.x == 0)
//            return true;

//        bool IsoutsideLeft = false;
//        bool IsoutsideRight = false;

//        if (relativePos.x > 0)
//        {
//            IsoutsideRight = relativePos.x - target.rect.width / 2f >= trans.rect.width / 2f;  //target左边界是否超出trans右边界
//        }
//        else
//        {
//            IsoutsideRight = relativePos.x + target.rect.width/ 2f <= -1 * trans.rect.width / 2f;  //target右边界是否超出trans左边界
//        }

//        return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
//    }

//    /// <summary>
//    /// 垂直方向上 target 是否在trans 里面
//    /// </summary>
//    /// <param name="trans"></param>
//    /// <param name="target"></param>
//    /// <returns></returns>
//    public static bool IsInsideRect_Vertical(this RectTransform trans, RectTransform target)
//    {
//        Vector2 relativePos = trans.InverseTransformPoint(target.position);  //target相对于trans坐标
//        if (relativePos.y == 0)
//            return true;

//        bool IsoutsideTop = false;
//        bool IsoutsideBottom = false;


//        if (relativePos.y > 0)
//        {
//            IsoutsideTop = relativePos.y - target.rect.height / 2f >= trans.rect.height / 2f;   //target下边界是否超出trans上边界
//        }
//        if (relativePos.y < 0)
//        {
//            IsoutsideBottom = relativePos.y + target.rect.height / 2f <= -1 * trans.rect.height / 2f;  //target上边界是否超出trans下边界
//        }

//        return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
//    }




//    /// <summary>
//    /// 垂直方向上 世界左边点 是否在trans 里面
//    /// </summary>
//    /// <param name="trans"></param>
//    /// <param name="target"></param>
//    /// <returns></returns>
//    public static bool IsInsideRect_Vertical(this RectTransform trans, Vector3 wordPosition)
//    {
//        Vector2 relativePos = trans.InverseTransformPoint(wordPosition);  //target相对于trans坐标
//        if (relativePos.y == 0)
//            return true;

//        bool IsoutsideTop = false;
//        bool IsoutsideBottom = false;


//        if (relativePos.y > 0)
//        {
//            IsoutsideTop = relativePos.y >= trans.rect.height / 2f;   //target下边界是否超出trans上边界
//        }
//        if (relativePos.y < 0)
//        {
//            IsoutsideBottom = relativePos.y <= -1 * trans.rect.height / 2f;  //target上边界是否超出trans下边界
//        }

//        return !(IsoutsideTop || IsoutsideBottom); //注意取反操作
//    }

//    /// <summary>
//    /// 水平方向上 世界坐标点 wordPosition 是否在trans 里面
//    /// </summary>
//    /// <param name="trans"></param>
//    /// <param name="target"></param>
//    /// <returns></returns>
//    public static bool IsInsideRect_Horizontial(this RectTransform trans, Vector3 wordPosition)
//    {
//        Vector2 relativePos = trans.InverseTransformPoint(wordPosition);  //target相对于trans坐标
//        if (relativePos.x == 0)
//            return true;

//        bool IsoutsideLeft = false;
//        bool IsoutsideRight = false;

//        if (relativePos.x > 0)
//        {
//            IsoutsideRight = relativePos.x >= trans.rect.width / 2f;  //target左边界是否超出trans右边界
//        }
//        else
//        {
//            IsoutsideLeft = relativePos.x <= -1 * trans.rect.width / 2f;  //target右边界是否超出trans左边界
//        }

//        return !(IsoutsideLeft || IsoutsideRight); //注意取反操作
//    }


//}
