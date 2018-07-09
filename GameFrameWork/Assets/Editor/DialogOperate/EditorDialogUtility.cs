using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFrameWork.EditorExpand { 
    /// <summary>
    /// 编辑器模式下打开各种对话框帮助类
    /// </summary>
    public class EditorDialogUtility 
    {

        /// <summary>
        /// 打开文件对话框获取选择的路径
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">打开目录</param>
        /// <param name="extension">后缀名</param>
        /// <returns></returns>
        public static string  OpenFileDialog(string title, string directory, string extension)
        {
            return EditorUtility.OpenFilePanel(title, directory, extension);
        }

        /// <summary>
        /// 打开保存文件夹的对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="folder">保存的文件夹</param>
        /// <param name="defaultName">默认名</param>
        /// <returns></returns>
        public static string SaveFolderDialog(string title, string folder, string defaultName)
        {
            return EditorUtility.SaveFolderPanel(title, folder, defaultName);
        }

        /// <summary>
        /// 打开保存文件的对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="directory">保存的目录</param>
        /// <param name="defaultName">默认名</param>
        /// <param name="extension">后缀名(不需要 . )</param>
        /// <returns></returns>
        public static string SaveFileDialog(string title, string directory, string defaultName, string extension)
        {
            return EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
        }

    }
}
