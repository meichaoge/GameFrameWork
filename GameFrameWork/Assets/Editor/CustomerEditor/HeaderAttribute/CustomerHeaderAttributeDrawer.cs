using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace GameFrameWork.EditorExpand
{

    /// <summary>
    /// 重写CustomerHeaderAttribute 在Inspector 界面显示
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomerHeaderAttribute))]
    public class CustomerHeaderAttributeDrawer : DecoratorDrawer
    {
        private GUIStyle style = new GUIStyle();


        public override void OnGUI(Rect position)
        {
            CustomerHeaderAttribute headAttribute = (CustomerHeaderAttribute)attribute;  //获取自定义的标签
            Color color = ColorUtility.Instance.htmlToColor(headAttribute.m_Color);

            #region 重绘GUI
            position = EditorGUI.IndentedRect(position);
            style.normal.textColor = color;
            GUI.Label(position, headAttribute.m_Ttitle, style);
            #endregion


        }



    }
}
#endif