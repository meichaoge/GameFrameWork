using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    /// <summary>
    /// 单例的Mono  类 如果不存在则会自动创建一个空对象并附加这个脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class Singleton_Mono_AutoCreate<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static object obj = new object();
        private static T m_Instance = null;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = ResourcesMgr.Instance.Instantiate(typeof(T).Name);
                    m_Instance = go.GetAddComponent<T>();
                }
                return m_Instance;
            }
        }



        public static T GetInstance()
        {
            return Instance;
        }




#if UNITY_EDITOR
        /// <summary>
        /// 当在编辑器挂在脚本时调用
        /// </summary>
        protected virtual void Reset()
        {
            try
            {
                if (GameObject.FindObjectsOfType<T>().Length > 1)
                {
                    GameObject.DestroyImmediate(gameObject.GetComponent<T>());
                    Debug.LogError("There are Already Exit " + typeof(T));
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

#endif

        /// <summary>
        /// 确保在运行时动态创建和添加组件依然保持只有一个对象
        /// </summary>
        protected virtual void Awake()
        {
            GetInstance(false);  //Makesure the other Component is destroyed
        }

        private static T GetInstance(bool isIgnoreCheck = true)
        {
            if (m_Instance != null && isIgnoreCheck)
                return m_Instance;
#if UNITY_EDITOR
            try
            {
#endif
                lock (obj)
                {
                    var result = GameObject.FindObjectsOfType<T>();
                    if (result.Length == 0)
                    {
                        //   Debug.LogError("MonoSingleton ... Not Initialed :" + typeof(T));
                        m_Instance = new GameObject(typeof(T).Name).AddComponent<T>();  //测试发现当运行时如果报错，下一次运行会生成多个对象
                    }
                    else if (result.Length == 1)
                    {
                        m_Instance = result[0];
                    }
                    else
                    {
                        m_Instance = result[result.Length - 1];  //Keep the First Initialed one  Be The Avalable
                        Debug.LogError("There are " + result.Length + " " + typeof(T));
                        for (int dex = 0; dex < result.Length - 1; ++dex)
                            GameObject.DestroyImmediate(result[dex]);
                    }
                }

#if UNITY_EDITOR
            }
            catch (System.Exception ex)
            {
                Debug.LogError("GetInstance >> " + ex.ToString());
            }
#endif

            return m_Instance;
        }

        protected virtual void OnDestroy()
        {
            m_Instance = null;
            obj = null;
        }

    }
}