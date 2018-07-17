using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 记录框架需要做的事情
    /// </summary>
    public class FrameWorkTodoLog : ScriptableObject
    {
        public string m_Version; //版本号
        public List<string> m_JobsItems = new List<string>();  //需要做的事情
    }
}