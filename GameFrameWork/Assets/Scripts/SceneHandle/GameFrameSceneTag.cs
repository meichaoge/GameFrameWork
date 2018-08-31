using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 每一个场景中都需要有一个物体挂载这个脚本 并设置标签为 GameSceneTag 
    /// 该物体的 activate 状态不要控制  交给场景加载管理
    /// </summary>
    public class GameFrameSceneTag : MonoBehaviour
    {
        /// <summary>
        /// 唯一标识当前场景是否被记载了
        /// </summary>
        public bool IsSceneLoaded { get; private set; }
        /// <summary>
        ///只会被初始化一次 即场景加载时候
        /// </summary>
        public string SceneName { get; private set; }

        /// <summary>
        /// 场景被加载时候调用
        /// </summary>
        public System.Action<string > m_OnSceneLoadAct = null;
        /// <summary>
        /// 场景被卸载时候调用
        /// </summary>
        public System.Action<string> m_OnSceneUnLoadAct = null;
     

        public void OnCompleteLoadScene(string sceneName)
        {
            if(IsSceneLoaded)
            {
                Debug.LogError("Scene Already Loaded " + sceneName);
                return;
            }
            Debug.Log("OnCompleteLoadScene  <<");
            IsSceneLoaded = true;
            SceneName = sceneName;
            if (m_OnSceneLoadAct != null)
                m_OnSceneLoadAct.Invoke(SceneName);
        }

        private void Awake()
        {
            IsSceneLoaded = false;
            Debug.Log("Awake  >>");
        }

        private void Start()
        {
            Debug.Log("Start  >>");
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable  >>");
            if (IsSceneLoaded)
            {
                IsSceneLoaded = false;
                if (m_OnSceneUnLoadAct != null)
                    m_OnSceneUnLoadAct.Invoke(SceneName);
            }
        }







    }
}