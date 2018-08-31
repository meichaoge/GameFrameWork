using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 手动挂载在哪些不会因为场景切换而删除的对象上
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        public static HashSet<GameObject> S_DontDestroyObjs = new HashSet<GameObject>();

        private void Awake()
        {
            if (S_DontDestroyObjs.Contains(gameObject) == false)
            {
                S_DontDestroyObjs.Add(gameObject);
            }
            else
            {
                Debug.LogEditorInfor("当前物体{0}已经被加入到不会自动销毁的队列中，确保是否添加多个脚本了", gameObject);
            }
            GameObject.DontDestroyOnLoad(gameObject);
        }








        /// <summary>
        /// 删除物体
        /// </summary>
        /// <param name="go"></param>
        public static void DestroyedObj(GameObject go)
        {
            if(S_DontDestroyObjs.Contains(go))
            {
                S_DontDestroyObjs.Remove(go);
            }

            GameObject.Destroy(go);
        }





    }
}