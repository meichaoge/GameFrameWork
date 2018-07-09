using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 编辑器下的帮助类
    /// </summary>
    public class EditorHelper :Singleton_Static<EditorHelper>
    {
        /// <summary>
        /// 获取UI Texture 资源的Packing 名称（规则，按照文件目录层级相对于 EditorDefine.S_UITextureTopRelativePath 以splitStr链接目录层级）
        /// </summary>
        /// <param name="texturePath"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        public string GetUITexturePackingName(string texturePath,string splitStr="_")
        {
            if(string.IsNullOrEmpty(splitStr))
            {
                Debug.LogError("GetUITexturePackingName  Fail,Not Avalib Path " + texturePath);
                return "";
            }

            int index = texturePath.IndexOf(EditorDefine.S_UITextureTopRelativePath);
            string relativePath = texturePath.Substring(index + EditorDefine.S_UITextureTopRelativePath.Length);
            return System.IO.Path.GetFileNameWithoutExtension(relativePath.Replace(@"\", splitStr).Replace(@"/", splitStr));

        }


    }
}