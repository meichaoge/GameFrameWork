using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 提供Shader 的常用操作
    /// </summary>
    public class ShaderUtility : Singleton_Static<ShaderUtility>
    {

#if UNITY_EDITOR

        /// <summary>
        /// 获取当前Render 组件关联的所有材质球的Shader属性定义信息
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="gameObjectRenderInfor"></param>
        public void GetMaterialShaderProperty(Renderer renderTarget, ref GameObjectRenderPropertyInfor gameObjectRenderInfor)
        {
            if (renderTarget == null) return;
            if (gameObjectRenderInfor == null)
                gameObjectRenderInfor = new GameObjectRenderPropertyInfor();
            else
                gameObjectRenderInfor.ClearRenderPropertyInfor();


            if (renderTarget.sharedMaterials == null || renderTarget.sharedMaterials.Length == 0) return;
            for (int dex = 0; dex < renderTarget.sharedMaterials.Length; ++dex)
            {
                #region 解析当前Render 关联的所有材质球
                MeterialPropertysInfor materialInfor = new MeterialPropertysInfor();
                materialInfor.m_MaterialIndex = dex;

                Material mat = renderTarget.sharedMaterials[dex];
                if (mat == null)
                {
                    Debug.LogEditorInfor(string.Format("Target Object : {0}  ShaderMaterial Index={1} is Null,Please Check is Right ?"));
                    gameObjectRenderInfor.AddMaterialInfor(materialInfor);
                    continue;
                }
                materialInfor.m_MeterialName = mat.name;
                #region  解析Shader中每一个定义的属性

                int shaderPropertysCount = ShaderUtil.GetPropertyCount(mat.shader);  //获取一个Shader中定义的属性个数
                for (int subIndex = 0; subIndex < shaderPropertysCount; ++subIndex)
                {
                    MaterialPropertyItem propertyItemInfor = new MaterialPropertyItem();
                    propertyItemInfor.m_PropertyIndex = subIndex;
                    propertyItemInfor.m_PropertyName = ShaderUtil.GetPropertyName(mat.shader, subIndex);
                    propertyItemInfor.m_ShaderPropertyEnum = ShaderPropertyType2ShaderPropertyEnum(ShaderUtil.GetPropertyType(mat.shader, subIndex));
                    propertyItemInfor.m_PropertyValue = GetPropertyValue(mat, propertyItemInfor.m_PropertyName, propertyItemInfor.m_ShaderPropertyEnum);
                    materialInfor.AddPropertyItemInfor(propertyItemInfor.m_PropertyIndex, propertyItemInfor);

                    //**** 使用 mat.GetXXX()  接口根据不同的 ShaderPropertyType 返回一个值
           //         Debug.LogEditorInfor(propertyItemInfor.ToString());
                }

                #endregion

                gameObjectRenderInfor.AddMaterialInfor(materialInfor);
                #endregion

            }
        }

        /// <summary>
        /// 获取当前Render 组件关联的所有材质球的Shader属性定义信息
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <returns></returns>
        public GameObjectRenderPropertyInfor GetMaterialShaderProperty(Renderer renderTarget)
        {
            GameObjectRenderPropertyInfor renderPropertyInfor = new GameObjectRenderPropertyInfor();
            GetMaterialShaderProperty(renderTarget, ref renderPropertyInfor);
            return renderPropertyInfor;
        }


        /// <summary>
        /// Editor 环境转成本地的类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public ShaderPropertyEnum ShaderPropertyType2ShaderPropertyEnum(ShaderUtil.ShaderPropertyType propertyType)
        {
            switch (propertyType)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    return ShaderPropertyEnum.Color;
                case ShaderUtil.ShaderPropertyType.Vector:
                    return ShaderPropertyEnum.Vector;
                case ShaderUtil.ShaderPropertyType.Float:
                    return ShaderPropertyEnum.Float;
                case ShaderUtil.ShaderPropertyType.Range:
                    return ShaderPropertyEnum.Range;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    return ShaderPropertyEnum.TexEnv;
                default:
                    return ShaderPropertyEnum.Color;
                    break;
            }
        }


#endif

        /// <summary>
        /// 获取某个材质球上Shader中某个属性字段的值
        /// </summary>
        /// <param name="matTarget"></param>
        /// <param name="propertyName">属性字段名</param>
        /// <param name="propertyType">属性字段的类型</param>
        /// <returns></returns>
        public object GetPropertyValue(Material matTarget, string propertyName, ShaderPropertyEnum propertyType)
        {
            if (matTarget == null) return null;

            switch (propertyType)
            {
                case ShaderPropertyEnum.Float:
                case ShaderPropertyEnum.Range:
                    return matTarget.GetFloat(propertyName);
                case ShaderPropertyEnum.Color:
                    return matTarget.GetColor(propertyName);
                case ShaderPropertyEnum.TexEnv:
                    return null;
                case ShaderPropertyEnum.Vector:
                    return matTarget.GetFloatArray(propertyName);
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// 设置 某个材质球上Shader中某个属性字段的值
        /// </summary>
        /// <param name="matTarget"></param>
        /// <param name="propertyName">属性字段名</param>
        /// <param name="propertyValue">要设置属性字段值</param>
        /// <param name="propertyType">属性字段的类型</param>
        public void SetPropertyValue(Material matTarget, string propertyName, object propertyValue, ShaderPropertyEnum propertyType)
        {
            if (matTarget == null) return;

            switch (propertyType)
            {
                case ShaderPropertyEnum.Float:
                case ShaderPropertyEnum.Range:
                    matTarget.SetFloat(propertyName, (float)propertyValue);
                    break;
                case ShaderPropertyEnum.Color:
                    matTarget.SetColor(propertyName, (Color)propertyValue);
                    break;
                case ShaderPropertyEnum.TexEnv:
                    break;
                case ShaderPropertyEnum.Vector:
                    matTarget.SetFloatArray(propertyName, (float[])propertyValue);
                    break;
                default:
                    break;
            }
        }


    }
}