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
        /// 创建或者追加内容
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="content">文本内容</param>
        /// <param name="isAppend">是否是追加模式</param>
        public static void CreateOrSetFileContent(string filePath, string content, bool isAppend = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("CreateOrSetFileContent Path is Null  ");
                return;
            }
            byte[] data = Encoding.UTF8.GetBytes(content);
            FileStream operateStream = null;

            try
            {
                string directionaryPath = System.IO.Path.GetDirectoryName(filePath);
                if (System.IO.Directory.Exists(directionaryPath) == false)
                {
                    System.IO.Directory.CreateDirectory(directionaryPath);
                }


                if (File.Exists(filePath) && (isAppend == false))
                {
                    operateStream = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Read, data.Length);//截断
                }
                else
                {
                    operateStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, data.Length);//打开或者创建
                }

                if (isAppend)
                    operateStream.Write(data, (int)operateStream.Length, data.Length);
                else
                    operateStream.Write(data, 0, data.Length);

                operateStream.Flush();// 刷新
            }
            catch (System.Exception e)
            {
                Debug.LogError("CreateOrSetFileContent  " + e);
            }
            finally
            {
                if (operateStream != null)
                    operateStream.Close();

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }



        }

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
        /// 将一个目录文件移动到另一个目录
        /// </summary>
        /// <param name="sourcesDirc"></param>
        /// <param name="destionationDirc"></param>
        public void ForceMoveDirectoryFile(string sourcesDirc, string destionationDirc,bool isTopDirectory=true)
        {
            #region 源目录和目的目录检测

            if (Directory.Exists(sourcesDirc) == false)
            {
                Debug.LogError("ForceMoveDirectoryFile Fail Not Exit " + sourcesDirc);
                return;
            }

            if (!Directory.Exists(destionationDirc))
            {
                Directory.CreateDirectory(destionationDirc);
            }
            else
            {
                if(isTopDirectory)
                {
                    Directory.Delete(destionationDirc,true);
                    Directory.CreateDirectory(destionationDirc);
                }
            }
            #endregion

            DirectoryInfo directoryInfo = new DirectoryInfo(sourcesDirc);
            FileInfo[] files = directoryInfo.GetFiles();
            //移动所有文件  
            foreach (FileInfo file in files)
            {
                //Debug.Log("file " + file.DirectoryName);
                file.MoveTo(Path.Combine(destionationDirc, file.Name));
            }
            //最后移动目录  
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                ForceMoveDirectoryFile(Path.Combine(sourcesDirc, dir.Name), Path.Combine(destionationDirc, dir.Name),false);
            }

            if (isTopDirectory)
            {
                Debug.LogEditorInfor("删除空的目录 " + sourcesDirc);
                Directory.Delete(sourcesDirc, true);  //删除无用的空目录
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

        /// <summary>
        /// 获取参数路径的父级目录
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        public string GetDirectoryParentDcirectory(string directoryPath, char splitStr = '/')
        {
           if(System.IO.Directory.Exists(directoryPath)==false)
            {
                Debug.LogError("GetDirectoryParentDcirectory Fail Dirctory  Not Exit " + directoryPath);
                return string.Empty;
            }

            string[] fileDirectorys = directoryPath.Split(splitStr);
            if (fileDirectorys.Length > 0)
                return fileDirectorys[fileDirectorys.Length - 1];

            return string.Empty;
        }


        /// <summary>
        /// 获取指定目录下的第N级别的父目录
        /// </summary>
        /// <param name="currentPath"></param>
        /// <param name="parentDeeep">父目录的层次</param>
        /// <returns></returns>
        public static string GetFilePathParentDirectory(string currentPath, int parentDeeep)
        {
            if (parentDeeep == 0)
                return currentPath;

            int currentDeep = 0;
            string parentPath = string.Empty;
            string targetPath = currentPath;
            while (currentDeep < parentDeeep)
            {
                parentPath = System.IO.Path.GetDirectoryName(targetPath);
                targetPath = parentPath;
                ++currentDeep;
            }
            return parentPath;
        }



        /// <summary>
        /// byte 单位转换成B/KB/MB/GB单位
        /// </summary>
        /// <param name="byteSize"></param>
        /// <param name="isUptoConvert">=true  标示向上取整，=false 则四舍五入</param>
        /// <returns></returns>
        public static string ByteConversionOthers(int byteSize, bool isUptoConvert = false)
        {
            //转成Byte
            if (isUptoConvert)
                byteSize = Mathf.CeilToInt(byteSize / 8f);
            else
                byteSize = Mathf.FloorToInt(byteSize / 8f);

            string[] units = new string[] { "B", "KB", "MB", "GB", "TB" };
            int count = 0;
            while (byteSize >= 1024)
            {
                if (isUptoConvert)
                {
                    byteSize = Mathf.CeilToInt(byteSize / 1024f);
                }
                else
                {
                    byteSize = Mathf.FloorToInt(byteSize / 1024f);
                }
                count++;
            }
            return string.Format("{0}{1}", byteSize, units[count]);
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