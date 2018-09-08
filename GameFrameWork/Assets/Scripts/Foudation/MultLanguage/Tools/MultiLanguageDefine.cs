using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GameFrameWork
{
    public class MultiLanguageDefine {}


    #region Rectransform 组件的属性定义
    [System.Serializable]

    public class RectransformToggle
    {
        public bool m_IsEnableAnchorPosition;
        public bool m_IsEnableSize;
        public bool m_IsEnableAnchorMax;
        public bool m_IsEnableAnchorMin;
        public bool m_IsEnablePivot;
        public bool m_IsEnableAngle;
        public bool m_IsEnableScale;

    }//控制Rectransform 那些属性是开启的

    [System.Serializable]
    public class RectransformBaseProperty
    {
        public Vector2 m_AnchorPosition = Vector2.zero;
        public Vector2 m_Size = Vector2.zero;
        public Vector2 m_AnchorMax = Vector2.zero;
        public Vector2 m_AnchorMin = Vector2.zero;
        public Vector2 m_Pivot = Vector2.zero;
        public Vector3 m_Angle = Vector3.zero;
        public Vector3 m_Scale = Vector3.zero;

        public RectransformBaseProperty() { }

        public RectransformBaseProperty(RectransformBaseProperty other)
        {
            m_AnchorPosition = other.m_AnchorPosition;
            m_Size = other.m_Size;
            m_AnchorMax = other.m_AnchorMax;
            m_AnchorMin = other.m_AnchorMin;
            m_Pivot = other.m_Pivot;
            m_Angle = other.m_Angle;
            m_Scale = other.m_Scale;
        }

        ////赋值保存RectransformBaseProperty 的值
        //public void CloneRectransfromValue(RectransformBaseProperty data)
        //{
        //    this.m_AnchorPosition = data.m_AnchorPosition;
        //    this.m_Size = data.m_Size;
        //    this.m_AnchorMax = data.m_AnchorMax;
        //    this.m_AnchorMin = data.m_AnchorMin;
        //    this.m_Pivot = data.m_Pivot;
        //    this.m_Angle = data.m_Angle;
        //    this.m_Scale = data.m_Scale;
        //}

        public void CloneFromRectTransform(RectTransform rtf)
        {
            m_AnchorPosition = rtf.anchoredPosition;
            m_Size = rtf.sizeDelta;
            m_AnchorMax = rtf.anchorMax;
            m_AnchorMin = rtf.anchorMin;
            m_Angle = rtf.localEulerAngles;
            m_Scale = rtf.localScale;
            m_Pivot = rtf.pivot;
        }

        public void CloneToRectTransform(RectTransform rtf)
        {
            rtf.anchoredPosition = m_AnchorPosition;
            rtf.sizeDelta = m_Size;
            rtf.anchorMax = m_AnchorMax;
            rtf.anchorMin = m_AnchorMin;
            rtf.localEulerAngles = m_Angle;
            rtf.localScale = m_Scale;
            rtf.pivot = m_Pivot;
        }
    }//
    #endregion


    #region  Image 组件属性定义

    [System.Serializable]
    public class ImageProperty
    {
        public bool m_IsPreserveAspect;
        public Color m_ImageColor = Color.white;
        public Image.Type m_ImageType = Image.Type.Simple;
        public Image.FillMethod m_FillMethod = Image.FillMethod.Radial360;
        public float m_FillAmount = 0;

        [Header("---只在m_FillMethod== Image.FillMethod.Radial180 时候有效--")]
        public Image.Origin180 m_fillOrigin180 = Image.Origin180.Bottom;
        [Header("---只在m_FillMethod== Image.FillMethod.Radial360 时候有效--")]
        public Image.Origin360 m_fillOrigin360 = Image.Origin360.Bottom;
        [Header("---只在m_FillMethod== Image.FillMethod.Radial90 时候有效--")]
        public Image.Origin90 m_fillOrigin90 = Image.Origin90.BottomLeft;
        [Header("---只在m_FillMethod== Image.FillMethod.Horizontal 时候有效--")]
        public Image.OriginHorizontal m_fillOriginHorizontal = Image.OriginHorizontal.Left;
        [Header("---只在m_FillMethod== Image.FillMethod.Vertical 时候有效--")]
        public Image.OriginVertical m_fillOriginVertical = Image.OriginVertical.Bottom;


        public ImageProperty() { }

        public ImageProperty(ImageProperty other)
        {
            m_ImageColor = other.m_ImageColor;
            m_ImageType = other.m_ImageType;
            m_IsPreserveAspect = other.m_IsPreserveAspect;
            m_FillMethod = other.m_FillMethod;

            switch (other.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    m_fillOrigin180 = other.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    m_fillOrigin360 = other.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    m_fillOrigin90 = other.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    m_fillOriginHorizontal = other.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    m_fillOriginVertical = other.m_fillOriginVertical;
                    break;
            }
            m_FillAmount = other.m_FillAmount;
        }

        public void CloneFromImage(Image image)
        {
            this.m_ImageColor = image.color;
            this.m_IsPreserveAspect = image.preserveAspect;
            this.m_ImageType = image.type;

            this.m_FillMethod = image.fillMethod;
            this.m_FillAmount = image.fillAmount;

            switch (image.fillMethod)
            {
                case Image.FillMethod.Radial180:
                    this.m_fillOrigin180 = (Image.Origin180)image.fillOrigin;
                    break;
                case Image.FillMethod.Radial360:
                    this.m_fillOrigin360 = (Image.Origin360)image.fillOrigin;
                    break;
                case Image.FillMethod.Radial90:
                    this.m_fillOrigin90 = (Image.Origin90)image.fillOrigin;
                    break;
                case Image.FillMethod.Horizontal:
                    this.m_fillOriginHorizontal = (Image.OriginHorizontal)image.fillOrigin;
                    break;
                case Image.FillMethod.Vertical:
                    this.m_fillOriginVertical = (Image.OriginVertical)image.fillOrigin;
                    break;
            }
        }

        public void CloneToImage(Image image)
        {
            image.type = this.m_ImageType;
            image.color = this.m_ImageColor;
            image.preserveAspect = this.m_IsPreserveAspect;
            if (image.type != Image.Type.Filled) return;

            image.fillMethod = this.m_FillMethod;
            switch (this.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    image.fillOrigin = (int)this.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    image.fillOrigin = (int)this.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    image.fillOrigin = (int)this.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    image.fillOrigin = (int)this.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    image.fillOrigin = (int)this.m_fillOriginVertical;
                    break;
            }
            image.fillAmount = this.m_FillAmount;
        }
    }//图片可配置的属性

    [System.Serializable]
    public class ImageConfige
    {
        public string m_LanguageName;  //显示用的
        public Language m_Language;
        public Sprite m_SourceImage = null;
        public ImageProperty m_ImageProperty;
        public ImageConfige()
        {
            m_LanguageName = m_Language.ToString();
            m_ImageProperty = new ImageProperty();
        }

        public ImageConfige(Language language)
        {
            m_Language = language;
            m_LanguageName = m_Language.ToString();
            m_ImageProperty = new ImageProperty();
        }

        public ImageConfige(Language language, ImageProperty imageProperty)
        {
            m_Language = language;
            m_LanguageName = m_Language.ToString();
            m_ImageProperty = new ImageProperty(imageProperty);
        }
    } //每一种语言类型对应的配置属性

    [System.Serializable]
    public class TotalImageConfige
    {
        [Header("对应的值为true，标识当前属性对应的Rectransform 属性字段 会被更新")]
        public List<ImageConfige> m_ImageLanguageConfige = new List<ImageConfige>();
    }

    #endregion

    #region TextMeshPro组件
    /// <summary>
    /// TextMeshPro组件属性
    /// </summary>
    [System.Serializable]
    public class TextMeshProProperty
    {
        public TMPro.TMP_FontAsset font;
        public Material fontSharedMaterial;
        public TMPro.FontStyles fontStyle;
        public Color color;
        public TMPro.VertexGradient colorGradient;
        public float fontSize;
        public TMPro.TextAlignmentOptions alignment;
        public bool enableWordWrapping;
        public TMPro.TextOverflowModes overflowMode;

        public void CloneFromTextMeshPro(TMPro.TextMeshProUGUI text)
        {
            this.font = text.font;
            this.fontSharedMaterial = text.fontSharedMaterial;
            this.fontStyle = text.fontStyle;
            this.color = text.color;
            this.colorGradient = text.colorGradient;
            this.fontSize = text.fontSize;
            this.alignment = text.alignment;
            this.enableWordWrapping = text.enableWordWrapping;
            this.overflowMode = text.overflowMode;
        }

        public void CloneToTextMeshPro(TMPro.TextMeshProUGUI text)
        {
            text.font = this.font;
            text.fontSharedMaterial = this.fontSharedMaterial;
            text.fontStyle = this.fontStyle;
            text.color = this.color;
            text.colorGradient = this.colorGradient;
            text.fontSize = this.fontSize;
            text.alignment = this.alignment;
            text.enableWordWrapping = this.enableWordWrapping;
            text.overflowMode = this.overflowMode;
        }
    }
    #endregion
}