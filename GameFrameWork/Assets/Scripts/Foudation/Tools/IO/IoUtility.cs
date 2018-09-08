using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    /// <summary>
    /// 提供IO 常用功能
    /// </summary>
    public class IoUtility : Singleton_Static<IoUtility>
    {

        /// <summary>
        /// 将一个目录下所有的文件复制到指定的目录(会先删除指定目录下的文件)
        /// </summary>
        /// <param name="sourcesDirc"></param>
        /// <param name="destionationDirc"></param>
        public void ForceCopyDirectoryFile(string sourcesDirc, string destionationDirc)
        {
            if (!Directory.Exists(destionationDirc))
            {
                Directory.CreateDirectory(destionationDirc);
            }
            else
            {
                Directory.Delete(destionationDirc, true);
                Directory.CreateDirectory(destionationDirc);
            }

            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(sourcesDirc);
            FileInfo[] files = directoryInfo.GetFiles();
            //复制所有文件  
            foreach (FileInfo file in files)
            {
                Debug.Log("file " + file.DirectoryName);
                file.CopyTo(Path.Combine(destionationDirc, file.Name));
            }
            //最后复制目录  
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                ForceCopyDirectoryFile(Path.Combine(sourcesDirc, dir.Name), Path.Combine(destionationDirc, dir.Name));
            }

        }

        /// <summary>
        /// 将制定的信息保存到本地的外部存储目录， 如果不存在则创建这个文件夹(总目录是)
        /// </summary>
        /// <param name="filePath">相对于Resources 路径</param>
        /// <param name="content">写入的内容</param>
        /// <param name="isIngoreLanguage">是否忽略多语言(true 标识不同语言都写入在同一个目下，否则按照语言名分类文件)</param>
        /// <param name="isAppend">是追加方式还是截断方式写入</param>
        /// <returns></returns>
        public bool SaveLocalDataOutStore(string filePath, string content, bool isIngoreLanguage = false, bool isAppend = true)
        {
            bool result = true;
            FileStream filestream = null;
            string realPath = "";  //外部存储路径
            if (isIngoreLanguage)
                realPath = string.Format("{0}{1}", ConstDefine.S_AssetBundleTopPath, filePath);
            else
                realPath = string.Format("{0}{1}/{2}", ConstDefine.S_AssetBundleTopPath, LanguageMgr.Instance.GetCurLanguageStr(), filePath);
            try
            {
                if (System.IO.File.Exists(realPath) == false)
                {
                    #region 创建新文件并写数据
                    string directionaryPath = System.IO.Path.GetDirectoryName(realPath);
                    if (System.IO.Directory.Exists(directionaryPath) == false)
                        System.IO.Directory.CreateDirectory(directionaryPath);

                    filestream = System.IO.File.Create(realPath);
                    byte[] date = Encoding.UTF8.GetBytes(content);
                    filestream.Write(date, 0, date.Length);
                    #endregion
                }
                else
                {
                    if (isAppend)
                        filestream = System.IO.File.Open(realPath, FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite);
                    else
                        filestream = System.IO.File.Open(realPath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    byte[] date = Encoding.UTF8.GetBytes(content);
                    filestream.Write(date, 0, date.Length);
                }
            }
            catch (System.Exception ex)
            {
                result = false;
                Debug.LogError("SaveLocalData Fail," + ex.ToString());
            }
            finally
            {
                if (filestream != null)
                    filestream.Close();
            }

            return result;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 将制定的信息保存到本地Resources中 如果不存在则创建这个文件夹(总目录是)
        /// </summary>
        /// <param name="filePath">相对于Resources 路径</param>
        /// <param name="content">写入的内容</param>
        /// <param name="isIngoreLanguage">是否忽略多语言(true 标识不同语言都写入在同一个目下，否则按照语言名分类文件)</param>
        /// <param name="isAppend">是追加方式还是截断方式写入</param>
        /// <returns></returns>
        public bool SaveLocalDataResourceStore(string filePath, string content, bool isIngoreLanguage = false, bool isAppend = true)
        {
            bool result = true;
            FileStream filestream = null;
            string realPath = "";  //外部存储路径
            if (isIngoreLanguage)
                realPath = string.Format("{0}{1}", ConstDefine.S_ResourcesPath, filePath);
            else
                realPath = string.Format("{0}{1}/{2}", ConstDefine.S_ResourcesPath, LanguageMgr.Instance.GetCurLanguageStr(), filePath);
            try
            {
                if (System.IO.File.Exists(realPath) == false)
                {
                    #region 创建新文件并写数据
                    string directionaryPath = System.IO.Path.GetDirectoryName(realPath);
                    if (System.IO.Directory.Exists(directionaryPath) == false)
                        System.IO.Directory.CreateDirectory(directionaryPath);

                    filestream = System.IO.File.Create(realPath);
                    byte[] date = Encoding.UTF8.GetBytes(content);
                    filestream.Write(date, 0, date.Length);
                    #endregion
                }
                else
                {
                    if (isAppend)
                        filestream = System.IO.File.Open(realPath, FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite);
                    else
                        filestream = System.IO.File.Open(realPath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    byte[] date = Encoding.UTF8.GetBytes(content);
                    filestream.Write(date, 0, date.Length);
                }
            }
            catch (System.Exception ex)
            {
                result = false;
                Debug.LogError("SaveLocalData Fail," + ex.ToString());
            }
            finally
            {
                if (filestream != null)
                    filestream.Close();
            }
            AssetDatabase.Refresh();
            return result;
        }

#endif


    }
}