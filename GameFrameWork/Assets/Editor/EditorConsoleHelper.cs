using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 编辑器下Console 面板帮助类
    /// </summary>
    public class EditorConsoleHelper : Singleton_Static<EditorHelper>
    {
        /// <summary>
        /// 运行时可以清空Console
        /// </summary>
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            if (type == null)
            {
                type = assembly.GetType("UnityEditor.LogEntries");
            }
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}