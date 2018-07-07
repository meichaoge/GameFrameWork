using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
  

    /// <summary>
    /// 树形结构节点信息
    /// </summary>
    [System.Serializable]
    public class BaseTreeNodeInfor
    {


        public string m_ViewName;  //显示的节点名
        public List<BaseTreeNodeInfor> m_AllSubNodesInfor = new List<BaseTreeNodeInfor>(); //子节点 （嵌套结构）
        public bool IsTreeNode {
            get
            {
                if (m_AllSubNodesInfor.Count > 0)
                    return false;
                return true;
            }
        } //标识是否是叶子节点
        private bool m_IsOn = false;
        public bool IsOn { get { return GetTreeNodeIsOnState(); } set { SetTreeNodeIsOnState(value); } }  //是否显示这个节点

        public BaseTreeNodeInfor(string name)
        {
            //m_IsShowToggle = new SuperToggle(this);
            m_ViewName = name;
        }



        protected bool GetTreeNodeIsOnState()
        {
            if (IsTreeNode)
                return m_IsOn;

            bool isHasItemOn = false;
            foreach (var item in m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    if(item.m_IsOn)
                    {
                        isHasItemOn = true;
                    }
                }
                else
                {
                    bool isSubINodeItemOn = false;
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        isSubINodeItemOn = GetSubTreeNodeItemIsOn(subItem);
                        if (isSubINodeItemOn)
                        {
                            isHasItemOn = true;
                            return isHasItemOn;
                        }
                    }//递归遍历子目录
                }
            }
            return isHasItemOn;
        }

        /// <summary>
        /// 递归遍历子目录
        /// </summary>
        /// <param name="nodeInfor"></param>
        /// <returns></returns>
        protected bool GetSubTreeNodeItemIsOn(BaseTreeNodeInfor nodeInfor)
        {
            if (nodeInfor.IsTreeNode)
                return nodeInfor.m_IsOn;

            bool isHasItemOn = false;
            foreach (var item in nodeInfor.m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    if( item.m_IsOn)
                    {
                        isHasItemOn = true;
                    }
                }
                else
                {
                    bool isSubINodeItemOn = false;
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        isSubINodeItemOn = GetSubTreeNodeItemIsOn(subItem);
                        if (isSubINodeItemOn)
                        {
                            isHasItemOn = true;
                            return isHasItemOn;
                        }
                    }//递归遍历子目录
                }
            }
            return isHasItemOn;
        }


        protected void SetTreeNodeIsOnState(bool isOn)
        {
            if (IsTreeNode)
            {
                m_IsOn = isOn;
                return;
            }

            foreach (var item in m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    item.m_IsOn = isOn;
                }
                else
                {
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        SetSubTreeNodeIsOnState(subItem, isOn);
                    }
                }
            }
            m_IsOn = isOn;
        }

        protected void SetSubTreeNodeIsOnState(BaseTreeNodeInfor target, bool isOn)
        {
            if (target.IsTreeNode)
            {
                target.m_IsOn = isOn;
                return;
            }

            foreach (var item in target.m_AllSubNodesInfor)
            {
                if (item.IsTreeNode)
                {
                    item.m_IsOn = isOn;
                }
                else
                {
                    foreach (var subItem in item.m_AllSubNodesInfor)
                    {
                        SetSubTreeNodeIsOnState(subItem, isOn);
                    }
                }
            }
            target.m_IsOn = isOn;
        }

    }


    /// <summary>
    /// 树形结构基类
    /// </summary>
    [System.Serializable]
    public class BaseTreeViewInfor
    {
        public List<BaseTreeNodeInfor> m_TopTreeNodesInfor = new List<BaseTreeNodeInfor>();  //最顶层的节点
      
    }



}
