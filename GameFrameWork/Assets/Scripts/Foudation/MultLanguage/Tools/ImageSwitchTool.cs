using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    public class ImageSwitchTool : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField]
        [Header(" 右键脚本切换语言切换到当前值对应的语言")]
        private Language m_CurrentShowLanguage;
        public List<ImageSwitchTag> m_AllSubImageTags = new List<ImageSwitchTag>();


        [ContextMenu("刷新子节点的ImageSwitchTg引用")]
        public void FlushImageSwitchTag()
        {
            GetAllImageSwitchTag();
            Debug.Log("刷新引用完成");
        }

        [ContextMenu("一键切换到对应的语言")]
        private void SwitchImageToCN()
        {
            for (int dex = 0; dex < m_AllSubImageTags.Count; ++dex)
            {
                m_AllSubImageTags[dex].ShowImageViewBaseOnLanguage(m_CurrentShowLanguage);
            }
            Debug.Log("一键切换到对应的语言"+ m_CurrentShowLanguage);
        }

   

        private void Reset()
        {
            GetAllImageSwitchTag();
        }

        private void GetAllImageSwitchTag()
        {
            m_AllSubImageTags.Clear();
            transform.GetComponentsInChildren<ImageSwitchTag>(true, m_AllSubImageTags);
        }

#endif

    }

}