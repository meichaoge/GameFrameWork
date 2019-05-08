using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{

    /// <summary>
    /// Unity 内置功能的扩展
    /// </summary>
    public class UnityBuildInUtility: Singleton_Static<UnityBuildInUtility>
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