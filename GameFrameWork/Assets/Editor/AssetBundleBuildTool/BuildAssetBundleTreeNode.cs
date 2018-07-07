using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 树形节点选中状态
    /// </summary>
    public enum TreeNodeSelectState
    {
        AllSelected, //全部选择
        NoSelected, //全部非选择
        SomeSelected, //有些选择了

    }


    /// <summary>
    /// 打包生成AssetBundle 时候显示需要打包目录的树形节点
    /// </summary>
    [System.Serializable]
    public class BuildAssetBundleTreeNode : BaseTreeNodeInfor
    {
     /// <summary>
     /// 是否需要被选中(只要有子项是选中的则选中 否则非选择)
     /// </summary>
        protected bool m_IsSelected = false;
        public bool IsSelected { get { return GetTreeNodeIsSelectedState(); } set { SetTreeNodeIsSelectedState(value); } }   //是否选中
  //      public TreeNodeSelectState m_TreeNodeSelectState { get; protected set; } //节点的选择状态

        public BuildAssetBundleTreeNode(string name) : base(name)
        {
          
        }


        protected bool GetTreeNodeIsSelectedState()
        {
            if (IsTreeNode)
            {
                //if (m_IsSelected)
                //    m_TreeNodeSelectState = TreeNodeSelectState.AllSelected;
                //else
                //    m_TreeNodeSelectState = TreeNodeSelectState.NoSelected;
                return m_IsSelected;
            }


            bool isHasItemSelected = false;
            foreach (var item in m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    if ((item as BuildAssetBundleTreeNode).IsSelected)
                        isHasItemSelected = (item as BuildAssetBundleTreeNode).IsSelected;
                }
                else
                {
                    bool isSubINodeItemSelected = false;
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        isSubINodeItemSelected = GetSubTreeNodeItemIsSelected(subItem);
                        if (isSubINodeItemSelected)
                        {
                            isHasItemSelected = true;
                            return isHasItemSelected;
                        }
                    }//递归遍历子目录
                }
            }
            return isHasItemSelected;
        }
        /// <summary>
        /// 递归遍历子目录
        /// </summary>
        /// <param name="nodeInfor"></param>
        /// <returns></returns>
        protected bool GetSubTreeNodeItemIsSelected(BaseTreeNodeInfor nodeInfor)
        {
            if (nodeInfor.IsTreeNode)
                return (nodeInfor as BuildAssetBundleTreeNode).m_IsSelected;

            bool isHasItemSelected = false;
            foreach (var item in nodeInfor.m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    if( (item as BuildAssetBundleTreeNode).m_IsSelected)
                    {
                        isHasItemSelected = true;
                    }
                }
                else
                {
                    bool isSubINodeItemSelected = false;
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        isSubINodeItemSelected = GetSubTreeNodeItemIsSelected(subItem);
                        if (isSubINodeItemSelected)
                        {
                            isHasItemSelected = true;
                            return isHasItemSelected;
                        }
                    }//递归遍历子目录
                }
            }
            return isHasItemSelected;
        }


        protected void SetTreeNodeIsSelectedState(bool isselected)
        {
            if (IsTreeNode)
            {
                m_IsSelected = isselected;
                return;
            }

            foreach (var item in m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    (item as BuildAssetBundleTreeNode).m_IsSelected = isselected;
                }
                else
                {
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        SetSubTreeNodeIsSelectedState(subItem, isselected);
                    }
                }
            }
            m_IsSelected = isselected;
        }
        protected void SetSubTreeNodeIsSelectedState(BaseTreeNodeInfor target, bool isselected)
        {
            if (target.IsTreeNode)
            {
                (target as BuildAssetBundleTreeNode).m_IsSelected = isselected;
                return;
            }

            foreach (var item in target.m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    (item as BuildAssetBundleTreeNode).m_IsSelected = isselected;
                }
                else
                {
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        SetSubTreeNodeIsSelectedState(subItem, isselected);
                    }
                }
            }
              (target as BuildAssetBundleTreeNode).m_IsSelected = isselected;
        }





    }
}