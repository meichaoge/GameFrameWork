using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 屏幕点击特效(会生成一个特效 然后使用内部的循环机制保证不会一直创建销毁)
    /// </summary>
    public class PointDownScreenEffect : MonoBehaviour
    {
        [Header("如果UI相机不是主相机 这这里需要指定值")]
        public Camera m_UICamera;


        /// <summary>
        /// 记录特效对象和存在的时间
        /// </summary>
        [System.Serializable]
        public class EffectRecord
        {
            public GameObject m_EffectObj;
            public float m_RemainTime;  //剩余存活时间
            public EffectRecord(GameObject go, float time)
            {
                m_EffectObj = go;
                m_RemainTime = time;
            }
        }

        private Queue<GameObject> m_EffectObjs = new Queue<GameObject>(); //生成的特效
        private List<EffectRecord> m_AllShowingEffectObj = new List<EffectRecord>(); //记录所有正在显示特效
        public float m_MaxAliveTime = 0.5f;  //每个特效存在时间

        private Vector3 m_MouseCanvasPostion; //点击时候鼠标的位置 转换到中心点再屏幕正中心的坐标


        private void Awake()
        {
            if (m_UICamera == null)
                m_UICamera = Camera.main;

            if (m_UICamera.orthographic == false)
            {
                Debug.LogError("UICamera 应该是 透视(perspective)模式"); //透视模式下相机的Size 影响特效的大小
            }

            if (m_UICamera.nearClipPlane > 1)
            {
                Debug.LogError("UICamera 近视点要小于1 否则无法看到特效");
            }

        }


        void Update()
        {
            UpdateEffectObjsState();

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; ++i)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)
                        {
                            m_MouseCanvasPostion = new Vector3(touch.position.x, touch.position.y, 1);
                            m_MouseCanvasPostion = m_UICamera.ScreenToWorldPoint(m_MouseCanvasPostion);
                            GameObject go = GetEffectObject();
                            go.transform.localPosition = m_UICamera.transform.InverseTransformPoint(m_MouseCanvasPostion);
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                m_MouseCanvasPostion = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
                m_MouseCanvasPostion = m_UICamera.ScreenToWorldPoint(m_MouseCanvasPostion);
                GameObject go = GetEffectObject();
                go.transform.localPosition = m_UICamera.transform.InverseTransformPoint(m_MouseCanvasPostion);
            }
        }

        void OnDisable()
        {
            m_EffectObjs.Clear();
            m_AllShowingEffectObj.Clear();
        }

        /// <summary>
        /// 从队列中获取一个可用的特效
        /// </summary>
        /// <returns></returns>
        private GameObject GetEffectObject()
        {
            if (m_EffectObjs.Count != 0)
            {
                GameObject obj = m_EffectObjs.Dequeue();
                if (obj != null)
                {
                    obj.SetActive(true);

                    EffectRecord record2 = new EffectRecord(obj, m_MaxAliveTime);
                    m_AllShowingEffectObj.Add(record2);
                    return obj;
                }
            } //获取之前生成的Effect

            GameObject go = new GameObject("特效");// ResourceMgr.instance.Instantiate(EffectResPath.ScreenEffectItemPath, m_UICamera.transform);
            go.transform.SetParent(m_UICamera.transform);
            go.transform.localPosition = m_UICamera.transform.InverseTransformPoint(m_MouseCanvasPostion); ;
            go.transform.localScale = Vector3.one;

            EffectRecord record = new EffectRecord(go, m_MaxAliveTime);
            m_AllShowingEffectObj.Add(record);
            return go;
        }

        /// <summary>
        /// 更新所有正在显示的特效的状态
        /// </summary>
        void UpdateEffectObjsState()
        {
            if (m_AllShowingEffectObj.Count > 0)
            {
                for (int dex = 0; dex < m_AllShowingEffectObj.Count; ++dex)
                {
                    m_AllShowingEffectObj[dex].m_RemainTime -= Time.deltaTime;
                }
                if (m_AllShowingEffectObj[0].m_RemainTime <= 0)
                {
                    m_AllShowingEffectObj[0].m_EffectObj.SetActive(false);
                    m_EffectObjs.Enqueue(m_AllShowingEffectObj[0].m_EffectObj);  //缓存这个特效资源方便后面使用
                    m_AllShowingEffectObj.RemoveAt(0);
                }
            }
        }

    }
}