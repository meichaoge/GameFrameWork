using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 扩展UI常用的组件
    /// </summary>
    public static class UIExpandUtility
    {
        #region 遍历一个节点下所有的子节点

        /// <summary>
        /// 递归遍历获取所有子对象集合
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<GameObject> GetChildCollectionRecursive(this GameObject obj)
        {
            List<Transform> objList = obj.transform.GetChildCollectionRecursive();
            return objList.ConvertAll(T => T.gameObject);
        }

        public static List<Transform> GetChildCollectionRecursive(this Transform obj)
        {
            List<Transform> objList = new List<Transform>();
            obj.GetChildCollectionRecursive(ref objList);
            return objList;
        }

        /// <summary>
        /// 递归遍历获取所有子对象变换集合
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objList"></param>
        public static void GetChildCollectionRecursive(this Transform obj, ref List<Transform> objList)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform childObj = obj.GetChild(i);
                objList.Add(childObj);

                // 如果子对象还有子对象，则再对子对象的子对象进行递归遍历
                if (childObj.childCount > 0)
                {
                    childObj.GetChildCollectionRecursive(ref objList);
                }
            }
        }
        #endregion

        /// <summary>
        /// 格式化字符串，删除非字母下划线数字的字符，首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatName(string str)
        {
            string strRet = "";
            char[] chs = str.ToCharArray();
            Regex regex = new Regex(@"^\w+$");
            bool bFirstChar = true;
            foreach (char s in chs)
            {
                if (regex.IsMatch(s.ToString()))
                {
                    string strTemp = s.ToString();
                    if (bFirstChar)
                    {
                        strTemp = strTemp.ToUpper();
                        bFirstChar = false;
                    }
                    strRet += strTemp;
                }
            }
            return strRet;
        }

        /// <summary>
        /// 保证字符串唯一性
        /// </summary>
        /// <param name="names"></param>
        /// <param name="name"></param>
        /// <param name="sep"></param>
        public static void UniqueName(ref Dictionary<string, int> names, ref string name, string sep = "_")
        {
            if (names == null)
            {
                names = new Dictionary<string, int>();
            }

            if (!names.ContainsKey(name))
            {
                names.Add(name, 1);
                return;
            }

            int value = names[name] + 1;
            names[name] = value;
            name = name + sep + value.ToString();
        }

        /// <summary>
        /// 获取节点到选中节点的路径
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objSelected"></param>
        /// <param name="includeSelected"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public static string GetPath(this Transform obj, Transform objSelected, bool includeSelected = false, string sep = "/")
        {
            string path = obj.name;
            Transform objParent = obj.parent;
            while (objParent != null && (includeSelected ? objParent != objSelected.parent : objParent != objSelected))
            {
                path = objParent.name + "/" + path;
                objParent = objParent.parent;
            }
            return path;
        }



    }
}