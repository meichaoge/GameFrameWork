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
        /// 自动过滤掉 多语言的路径信息(LocalizationUI/Chinese  or  LocalizationUI/English)
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
            relativePath = System.IO.Path.GetDirectoryName(relativePath); //去除文件名

            if (relativePath.StartsWith(EditorDefine.S_UILocalizationPathFileName))
            {
                var languages = System.Enum.GetValues(typeof(Language));
                string localizationLanguagePath = "";
                foreach (var item in languages)
                {
                    localizationLanguagePath = string.Format("{0}{1}/", EditorDefine.S_UILocalizationPathFileName, item.ToString());
                    if (relativePath.StartsWith(localizationLanguagePath))
                    {
                        relativePath = relativePath.Replace(localizationLanguagePath, "");
                        break;
                    }
                }

                // Debug.LogInfor("relativePath=" + relativePath);
            }//过滤掉由于多语言带来的不同版本资源问题

            return System.IO.Path.GetFileNameWithoutExtension(relativePath.Replace(@"\", splitStr).Replace(@"/", splitStr));

        }


    }
}