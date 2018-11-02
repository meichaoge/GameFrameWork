using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    /// <summary>
    /// 循环列表项的基类
    /// </summary>
    public class LoopScrollRectItemBase : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("编辑器下显示当前项的索引")]
        [SerializeField]
        private int DateIndex_EditorShow;

        //private void Update()
        //{
        //    DateIndex_EditorShow = m_DataIndex;
        //}
#endif

        /// <summary>
        /// 当前项的数据索引ID 
        /// </summary>
        public int m_DataIndex { get; protected set; }

        public RectTransform rectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }

        /// <summary>
        /// 初始化或者刷新视图
        /// </summary>
        /// <param name="idx"></param>
        public virtual void InitialedScrollCellItem(int idx)
        {
            m_DataIndex = idx;
#if UNITY_EDITOR
            DateIndex_EditorShow = m_DataIndex;
#endif


            string name = "Cell " + idx.ToString();
            gameObject.name = name;
        }


    }
}