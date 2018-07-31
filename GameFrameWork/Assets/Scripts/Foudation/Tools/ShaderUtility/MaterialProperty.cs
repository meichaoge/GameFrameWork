using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 材质球关联的Shader 中属性字段的信息
    /// </summary>
    public class MaterialPropertyItem : MonoBehaviour
    {
        public string m_PropertyName;
        public int m_PropertyIndex;  //属性索引
        public ShaderPropertyEnum m_ShaderPropertyEnum;  //Shader 属性的字段类型
        public object m_PropertyValue;

        public override string ToString()
        {
#if UNITY_EDITOR
            return string.Format("m_PropertyIndex={0},m_PropertyName={1},m_ShaderPropertyEnum={2},m_PropertyValue={3}",
            m_PropertyIndex, m_PropertyName, m_ShaderPropertyEnum, m_PropertyValue);
# else
            return base.ToString();
#endif
        }
    }

    /// <summary>
    /// 当前材质球关联的Shader 所有的属性值
    /// </summary>
    public class MeterialPropertysInfor
    {
        public int m_MaterialIndex;  //索引
        public string m_MeterialName;  //材质球名称
        /// <summary>
        /// 当前Shader中定义的属性
        /// </summary>
        public Dictionary<int, MaterialPropertyItem> m_AllShaderPropertys = new Dictionary<int, MaterialPropertyItem>();

        public void AddPropertyItemInfor(int dataIndex, MaterialPropertyItem propertyItem)
        {
#if UNITY_EDITOR
            foreach (var item in m_AllShaderPropertys.Values)
            {
                if (item == null)
                {
                    Debug.LogEditorInfor("当前材质球中有一个材质球属性为空  Index=" + m_MaterialIndex);
                    continue;
                }
            }

#endif

            if (m_AllShaderPropertys.ContainsKey(dataIndex))
            {
                Debug.LogError("重复的材质球Shader 属性ID={0}  ，确认需要传值正确 ??? ", dataIndex);
                return;
            }
            m_AllShaderPropertys.Add(dataIndex, propertyItem);
        }

    }


    /// <summary>
    /// 一个物体Redenr 下所有的材质球信息
    /// </summary>
    public class GameObjectRenderPropertyInfor
    {
        public List<MeterialPropertysInfor> m_AllConnectedMaterialsInfor = new List<MeterialPropertysInfor>();  //所有关联的材质球信息

        /// <summary>
        /// 清理数据 
        /// </summary>
        public void ClearRenderPropertyInfor()
        {
            if (m_AllConnectedMaterialsInfor == null || m_AllConnectedMaterialsInfor.Count == 0) return;

            foreach (var item in m_AllConnectedMaterialsInfor)
            {
                if (item != null)
                    item.m_AllShaderPropertys.Clear();
            }
            m_AllConnectedMaterialsInfor.Clear();
        }

        /// <summary>
        /// 添加材质球信息 (可能是空)
        /// </summary>
        /// <param name="materialInfor"></param>
        public void AddMaterialInfor(MeterialPropertysInfor materialInfor)
        {
            m_AllConnectedMaterialsInfor.Add(materialInfor);
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (var item in m_AllConnectedMaterialsInfor)
            {
                builder.Append(item.ToString());
            }
            return builder.ToString();
#else
            return base.ToString();
#endif
        }

    }



}