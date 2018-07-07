using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameFrameWork
{
    /// <summary>
    /// 提供IO 常用功能
    /// </summary>
    public class IoUtility :Singleton_Static<IoUtility>
    {

        /// <summary>
        /// 将一个目录下所有的文件复制到指定的目录(会先删除指定目录下的文件)
        /// </summary>
        /// <param name="sourcesDirc"></param>
        /// <param name="destionationDirc"></param>
        public void ForceCopyDirectoryFile(string sourcesDirc,string destionationDirc)
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


    }
}