using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下配置多语言资源的图片设置 
    /// </summary>
    public class MultLanguageImageTool_Static : MonoBehaviour
    {
        [SerializeField]
        [Header("标识当前编辑的语言版本 确定修改完成后右键脚本可以选择是否保存到对应的配置语言中")]
        private Language m_CurrentEditorLanguage;

        [Space(20)]
        [SerializeField]
        private List<MultLanguageImageTag_Static> m_AllMultLanguageTagOfStatic = new List<MultLanguageImageTag_Static>();

        #region  编辑菜单
        [ContextMenu("刷新子节点中MultLanguageImageTag_Static 引用")]
        private void FlushTagRecord()
        {
            GetAllChildStaticImageTagScript();
            Debug.LogEditorInfor("刷新引用子节点静态图片资源引用");
        }

        [ContextMenu("显示 m_CurrentEditorLanguage 语言对应的Image视图")]
        public void ShowImageOfLanguage()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("运行情况下禁止使用这个接口");
                return;
            }
            for ( int dex=0;dex< m_AllMultLanguageTagOfStatic.Count;++dex)
            {
                m_AllMultLanguageTagOfStatic[dex].ShowImageViewBaseOnLanguage(m_CurrentEditorLanguage);
            }

            Debug.LogEditorInfor(string.Format("完成显示 {0} 语言对应的子节点图片视图", m_CurrentEditorLanguage));
        }
        #endregion


        #region 编辑器下菜单

        private void Reset()
        {
            GetAllChildStaticImageTagScript();
        }
        ///// <summary>
        ///// 当修改属性时候
        ///// </summary>
        //private void OnValidate()
        //{
        //    Debug.Log("AAAA");
        //}
        #endregion

        private void GetAllChildStaticImageTagScript()
        {
            m_AllMultLanguageTagOfStatic.Clear();
            MultLanguageImageTag_Static[] allChildTag = transform.GetComponentsInChildren<MultLanguageImageTag_Static>(true);
            m_AllMultLanguageTagOfStatic.AddRange(allChildTag);
        }

    }
#endif
}