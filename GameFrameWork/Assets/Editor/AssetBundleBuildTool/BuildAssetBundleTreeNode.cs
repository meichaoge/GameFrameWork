using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 打包生成AssetBundle 时候显示需要打包目录的树形节点
    /// </summary>
    [System.Serializable]
    public class BuildAssetBundleTreeNode : BaseTreeNodeInfor
    {
        //public SuperToggle m_IsSelectedToggle;  //是否选中
        protected bool m_IsSelected = false;
        public bool IsSelected { get { return GetTreeNodeIsSelectedState(); } set { SetTreeNodeIsSelectedState(value); } }   //是否选中

        public BuildAssetBundleTreeNode(string name) : base(name)
        {
          
        }


        protected bool GetTreeNodeIsSelectedState()
        {
            if (IsTreeNode)
                return m_IsSelected;

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