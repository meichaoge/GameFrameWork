//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using UnityEngine;

/// <summary>
/// Class containing methods to ease debugging while developing a game.
/// 重载了Untiy引擎自带的Debug
/// 彭鑫
/// 2017年8月24日10:04:13
/// </summary>
///  新增日志级别控制 public static LogLevel S_LogLevel = LogLevel.Log  以及 Infor 级别的日志(LogInfor)     注：当S_LogLevel=S_LogLevel.None时候无日志输出

//public static class Debug
//{
//    public enum LogLevel
//    {
//        Log = 0,
//        Warning,
//        Assert,
//        Infor,
//        Exception,
//        Error,

//        None             //无日志级别
//    }


//    public static LogLevel S_LogLevel = LogLevel.Log;  //当前使用的日记级别

//    public static string m_InforColor = "#5EAB15FF"; //Infor 级别的日志颜色



//    [Conditional("UNITY_ASSERTIONS")]
//    public static void Assert(bool condition)
//    {
//        if (S_LogLevel > LogLevel.Assert) return;
//        UnityEngine.Debug.Assert(condition);
//    }
//    [Conditional("UNITY_ASSERTIONS")]
//    public static void Assert(bool condition, string message)
//    {
//        if (S_LogLevel > LogLevel.Assert) return;
//        UnityEngine.Debug.Assert(condition, message);
//    }

//    [Conditional("UNITY_ASSERTIONS")]
//    public static void Assert(bool condition, object message)
//    {
//        if (S_LogLevel > LogLevel.Assert) return;
//        UnityEngine.Debug.Assert(condition, message);
//    }



//    [Conditional("UNITY_ASSERTIONS")]
//    public static void Assert(bool condition, object message, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Assert) return;
//        UnityEngine.Debug.Assert(condition, message, context);
//    }
//    [Conditional("UNITY_ASSERTIONS")]
//    [Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
//    public static void Assert(bool condition, string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Assert) return;
//        UnityEngine.Debug.Assert(condition, format, args);
//    }

//    public static void Log(object message)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.Log(message);
//    }

//    public static void Log(object message, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.Log(message, context);
//    }

//    新增 高于Debug级别的日志
//    public static void LogInfor(object message)
//    {
//        if (S_LogLevel > LogLevel.Infor) return;
//        message = string.Format("<color={0}> {1}</color>", m_InforColor, message);
//        UnityEngine.Debug.Log(message);
//    }
//    新增 高于Debug级别的日志
//    public static void LogInfor(object message, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Infor) return;
//        message = string.Format("<color={0}> {1}</color>", m_InforColor, message);
//        UnityEngine.Debug.Log(message, context);
//    }




//    [Conditional("UNITY_ASSERTIONS")]
//    public static void LogAssertion(object message)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.LogAssertion(message);
//    }

//    [Conditional("UNITY_ASSERTIONS")]
//    public static void LogAssertion(object message, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.LogAssertion(message, context);
//    }

//    [Conditional("UNITY_ASSERTIONS")]
//    public static void LogAssertionFormat(string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.LogAssertionFormat(format, args);
//    }

//    [Conditional("UNITY_ASSERTIONS")]
//    public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.LogAssertionFormat(context, format, args);
//    }

//    public static void LogError(object message)
//    {
//        if (S_LogLevel > LogLevel.Error) return;
//        UnityEngine.Debug.LogError(message);
//    }

//    public static void LogError(UnityEngine.Object message)
//    {
//        if (S_LogLevel > LogLevel.Error) return;
//        UnityEngine.Debug.LogError(message);
//    }

//    public static void LogError(object message, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Error) return;
//        UnityEngine.Debug.LogError(message, context);
//    }

//    public static void LogErrorFormat(string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Error) return;
//        UnityEngine.Debug.LogErrorFormat(format, args);
//    }

//    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Error) return;
//        UnityEngine.Debug.LogErrorFormat(context, format, args);
//    }

//    public static void LogException(Exception exception)
//    {
//        if (S_LogLevel > LogLevel.Exception) return;
//        UnityEngine.Debug.LogException(exception);
//    }

//    public static void LogException(Exception exception, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Exception) return;
//        UnityEngine.Debug.LogException(exception, context);
//    }


//    public static void LogFormat(string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.LogFormat(format, args);
//    }

//    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Log) return;
//        UnityEngine.Debug.LogFormat(context, format, args);
//    }

//    public static void LogWarning(object message)
//    {
//        if (S_LogLevel > LogLevel.Warning) return;
//        UnityEngine.Debug.LogWarning(message);
//    }

//    public static void LogWarning(object message, UnityEngine.Object context)
//    {
//        if (S_LogLevel > LogLevel.Warning) return;
//        UnityEngine.Debug.LogWarning(message, context);
//    }

//    public static void LogWarningFormat(string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Warning) return;
//        UnityEngine.Debug.LogWarningFormat(format, args);
//    }

//    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
//    {
//        if (S_LogLevel > LogLevel.Warning) return;
//        UnityEngine.Debug.LogWarningFormat(context, format, args);
//    }

//    #region Unity 编辑器下的绘制

//    public static void DrawLine(Vector3 start, Vector3 end, Color color)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawLine(start, end, color);
//#endif
//    }

//    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawLine(start, end, color, duration);
//#endif
//    }


//    public static void DrawLine(Vector3 start, Vector3 end)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawLine(start, end);
//#endif
//    }

//    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f, bool depthTest = true)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
//#endif
//    }

//    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0f, bool depthTest = true)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
//#endif
//    }
//    public static void DrawRay(Vector3 start, Vector3 dir)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawRay(start, dir);
//#endif
//    }
//    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawRay(start, dir, color);
//#endif
//    }

//    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.DrawRay(start, dir, color, duration);
//#endif
//    }
//    #endregion


//}
