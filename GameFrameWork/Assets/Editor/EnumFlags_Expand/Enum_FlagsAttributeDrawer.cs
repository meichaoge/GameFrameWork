using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 绘制EnumFlagsAttribute 的表现效果
    /// </summary>
    [CustomPropertyDrawer(typeof(Enum_FlagsAttribute))]
    public class Enum_FlagsAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        //    Debug.Log("CurSelect EnumFlag Value=" + property.intValue);
        }


    }
}
