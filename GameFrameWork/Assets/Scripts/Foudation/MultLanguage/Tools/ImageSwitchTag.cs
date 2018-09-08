using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFrameWork
{
    [RequireComponent(typeof(Image))]
    /// <summary>
    /// 多语言切换工具
    /// </summary>
    public class ImageSwitchTag : MonoBehaviour
    {
        #region UI
        private Image _TargetImage;
        private Image m_TargetImage
        {
            get
            {
                if (_TargetImage == null)
                    _TargetImage = transform.GetComponent<Image>();
                return _TargetImage;
            }
            set
            {
                _TargetImage = value;
            }
        }

        private RectTransform m_Rectransform { get { return transform as RectTransform; } }
        #endregion

        #region 编辑器下切换语言的工具
#if UNITY_EDITOR
        [SerializeField]
        [Header("标识当前编辑的语言版本 确定修改完成后右键脚本可以选择是否保存到对应的配置语言中")]
        private Language m_CurrentEditorLanguage;

#endif
        #endregion

        #region Data
        public TotalImageConfige m_TotalImageConfigure = new TotalImageConfige();
#if UNITY_EDITOR
        private List<ImageConfige> m_TotalImageLanguageConfigure = new List<ImageConfige>();  //为了防止修改list 导致重复的定义而使用的
#endif

        #endregion

        #region Unity 编辑代码

#if UNITY_EDITOR

        #region 编辑菜单

        [ContextMenu("保存当前图片的属性更改到配置m_ImageLanguageConfige中")]
        public void ApplyEditorProperty()
        {
            ImageConfige config = GetTargetImageProperty(m_CurrentEditorLanguage);
            for (int dex = 0; dex < m_TotalImageConfigure.m_ImageLanguageConfige.Count; ++dex)
            {
                if (m_TotalImageConfigure.m_ImageLanguageConfige[dex].m_Language == config.m_Language)
                {
                    //   m_ImageLanguageConfige[dex] = config;
                    m_TotalImageConfigure.m_ImageLanguageConfige[dex] = CloneImageConfigureValue(m_TotalImageConfigure.m_ImageLanguageConfige[dex], ref config);
                    OnEditorConfigure();
                    return;
                }
            }
            m_TotalImageConfigure.m_ImageLanguageConfige.Add(config);
            OnEditorConfigure();
        }

        [ContextMenu("切换到当前需要显示的语言版本(_CurrentEditorLanguage 的值)")]
        public void SwitchToShowingLanguageImage()
        {
            ShowImageViewBaseOnLanguage(m_CurrentEditorLanguage);
        }

        #endregion

        #region Mono Frame

        private void Reset()
        {
            GetLanguageConfigureOnLanguageType(ref m_TotalImageConfigure.m_ImageLanguageConfige);
        }

        ////当修改属性时候
        //private void OnValidate()
        //{
        //    OnEditorConfigure();
        //}
        #endregion

        /// <summary>
        /// 当编辑属性列表时候
        /// </summary>
        private void OnEditorConfigure()
        {
            if (m_TotalImageLanguageConfigure.Count == 0)
                GetLanguageConfigureOnLanguageType(ref m_TotalImageLanguageConfigure);

            Language repeatLanguage = Language.Chinese;
            if (IsLanguageConfigureEnable(m_TotalImageConfigure.m_ImageLanguageConfige, ref repeatLanguage))
            {
                m_TotalImageLanguageConfigure = m_TotalImageConfigure.m_ImageLanguageConfige;
            }
            else
            {
                Debug.LogError("当前更改的语言配置有问题 有重复的语言 " + repeatLanguage);
                m_TotalImageConfigure.m_ImageLanguageConfige = m_TotalImageLanguageConfigure; //保持修改前的属性不变
            }
        }


        #region 数据的初始化
        /// <summary>
        /// 根据语言的枚举类型
        /// </summary>
        /// <param name="languageContainer"></param>
        private void GetLanguageConfigureOnLanguageType(ref List<ImageConfige> languageConfigContainer)
        {
            if (languageConfigContainer == null)
                languageConfigContainer = new List<ImageConfige>();
            var allLanguage = System.Enum.GetValues(typeof(Language));
            ImageConfige firstConfigure = null;
            foreach (var language in allLanguage)
            {
                ImageConfige config = null;
                Language languageType = (Language)System.Enum.Parse(typeof(Language), language.ToString());
                if (languageConfigContainer.Count == 0)
                {
                    config = GetTargetImageProperty(languageType);
                    //     GetSpritePathByLanguage(ref sptitePath,config.m_SourceImage, GameSettings.GetLanguageName(config.m_Language));
                    firstConfigure = config;
                }
                else
                {
                    config = new ImageConfige(languageType, firstConfigure.m_ImageProperty);
                }
                if (IsExitLanguageConfigure(config.m_Language, m_TotalImageConfigure.m_ImageLanguageConfige)) continue;
                languageConfigContainer.Add(config);
            }
        }

        /// <summary>
        ///自动移动图片 (TODO  )
        /// </summary>
        /// <param name="path"></param>
        /// <param name="source"></param>
        /// <param name="languageName"></param>
        private void GetSpritePathByLanguage(ref string path, Sprite source, string languageName)
        {
            //if (source == null) return;
            //string assetPath = AssetDatabase.GetAssetPath(source);
            //string directionPath = System.IO.Path.GetDirectoryName(assetPath);
            //Debug.LogInfor("directionPath =" + directionPath);
            //if(directionPath=="Resources")
            //{
            //    Debug.LogInfor("使用Unity 内置的资源"+ source.name);
            //    return;
            //}
            //string[] paths = directionPath.Split('/');
            //int index = -1;
            //bool isNeedCreateDirectory = false;
            //for (int dex=0;dex<paths.Length;++dex)
            //{
            //    if(paths[dex]=="Main_New")
            //    {
            //        index = dex;
            //        isNeedCreateDirectory = true;
            //        break;
            //    }

            //    if (paths[dex] == "Main")
            //    {
            //        index = dex;
            //        isNeedCreateDirectory = true;
            //        break;
            //    }

            //    if (paths[dex] == languageName)
            //    {
            //        isNeedCreateDirectory = false;
            //        index = dex;
            //        break;
            //    }
            //}

            //if(index==-1)
            //{
            //    Debug.LogError("当前图片不在管理的目录 请手动处理");
            //    return;
            //}

            //if(isNeedCreateDirectory)
            //{
            //    string relativePath = "";
            //    for (int dex= index+1;dex< paths.Length;++dex)
            //    {
            //        relativePath += paths[dex];
            //        if (dex != paths.Length - 1)
            //            relativePath += "/";
            //    }
            //    Debug.Log("relativePath=" + relativePath);
            //    string NewPath = Application.dataPath + "/Art/UI/Localization_NEW/";
            //    Debug.Log("NewPath=" + NewPath);
            //    var allLanguage = System.Enum.GetValues(typeof(Language));
            //    foreach (var item    in allLanguage)
            //    {
            //        Language language = (Language)System.Enum.Parse(typeof(Language), item.ToString());
            //        NewPath += GameSettings.GetLanguageName(language) + "/";
            //        string pathRelative = relativePath + "/" + System.IO.Path.GetFileName(assetPath);
            //        Debug.LogInfor("language=" + language + "   ::" + NewPath + "   :::" + pathRelative);
            //        string directoryPath = System.IO.Path.GetDirectoryName(NewPath + pathRelative);
            //        Debug.Log("directoryPath==" + directoryPath);
            //        if (GameSettings.GetLanguageName(language)== languageName)
            //        {
            //            Debug.Log("From:" + assetPath + " \n+TO:" + (NewPath + pathRelative));
            //            System.IO.Directory.CreateDirectory(directoryPath);
            //            System.IO.File.Move(assetPath, NewPath+ pathRelative);
            //            System.IO.File.Move(assetPath, NewPath + pathRelative + ".meta");

            //        }
            //        //else
            //        //{
            //        //    System.IO.Directory.CreateDirectory(directoryPath);
            //        //    Debug.Log("From:" + AssetDatabase.GetAssetPath(source) + " \n+TO:" + (NewPath + pathRelative));
            //        //    System.IO.File.Copy(AssetDatabase.GetAssetPath(source), NewPath + pathRelative);
            //        //}
            //    }
            //}
        }


        //获取第一个语言对应的配置 取当前脚本挂载对象上的属性
        private ImageConfige GetTargetImageProperty(Language firstLanguage)
        {
            ImageConfige _configure = new ImageConfige();
            _configure.m_Language = firstLanguage;
            _configure.m_SourceImage = m_TargetImage.sprite;
            InitialedImageProperty(ref _configure.m_ImageProperty);
            return _configure;
        }



        //初始化图片的基本属性
        private void InitialedImageProperty(ref ImageProperty imageProperty)
        {
            if (m_TargetImage == null) return;
            if (imageProperty == null)
                imageProperty = new ImageProperty();

            imageProperty.m_ImageColor = m_TargetImage.color;
            imageProperty.m_IsPreserveAspect = m_TargetImage.preserveAspect;
            imageProperty.m_ImageType = m_TargetImage.type;

            imageProperty.m_FillMethod = m_TargetImage.fillMethod;
            imageProperty.m_FillAmount = m_TargetImage.fillAmount;

            switch (m_TargetImage.fillMethod)
            {
                case Image.FillMethod.Radial180:
                    imageProperty.m_fillOrigin180 = (Image.Origin180)m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Radial360:
                    imageProperty.m_fillOrigin360 = (Image.Origin360)m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Radial90:
                    imageProperty.m_fillOrigin90 = (Image.Origin90)m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Horizontal:
                    imageProperty.m_fillOriginHorizontal = (Image.OriginHorizontal)m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Vertical:
                    imageProperty.m_fillOriginVertical = (Image.OriginVertical)m_TargetImage.fillOrigin;
                    break;
            }
        }
        #endregion


#endif
        #endregion

        private void Awake()
        {
            ShowImageViewBaseOnLanguage(ApplicationConfig.Instance.CurLanguageType);
        }


        //根据语言类型获取对应的配置
        private ImageConfige GetLanguageConfigure(Language language)
        {
            for (int dex = 0; dex < m_TotalImageConfigure.m_ImageLanguageConfige.Count; ++dex)
            {
                if (m_TotalImageConfigure.m_ImageLanguageConfige[dex].m_Language == language)
                    return m_TotalImageConfigure.m_ImageLanguageConfige[dex];
            }
            Debug.LogError("没有对应的语言配置" + language);
            return null;
        }
        //检测当前语言是否已经配置过
        private bool IsExitLanguageConfigure(Language language, List<ImageConfige> dataSource)
        {
            for (int dex = 0; dex < dataSource.Count; ++dex)
            {
                if (dataSource[dex].m_Language == language)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检测参数对应的语言配置是否可用(有语言配置相同)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsLanguageConfigureEnable(List<ImageConfige> data, ref Language repeatLanguage)
        {
            List<ImageConfige> readyCompareList = new List<ImageConfige>();  //已经匹配的不重复项

            for (int dex = 0; dex < data.Count; ++dex)
            {
                if (IsExitLanguageConfigure(data[dex].m_Language, readyCompareList))
                {
                    repeatLanguage = data[dex].m_Language;
                    return false;
                }
                readyCompareList.Add(data[dex]);
            }
            return true;
        }

        #region 切换视图
        /// <summary>
        /// 根据参数给定的语言切换图片
        /// </summary>
        /// <param name="language"></param>
        public void ShowImageViewBaseOnLanguage(Language language)
        {
            ImageConfige newImageConfige = GetLanguageConfigure(language);
            if (newImageConfige == null)
            {
                Debug.LogError(string.Format("当前图片{0  }没有配置对应的语言{1}  ", gameObject.name, language));
                return;
            }
            InitialedImageProperty(newImageConfige);
        }


        /// <summary>
        /// 切换设置Image 属性
        /// </summary>
        /// <param name="config"></param>
        private void InitialedImageProperty(ImageConfige config)
        {
            m_TargetImage.sprite = config.m_SourceImage;
            if (config.m_SourceImage == null)
                Debug.LogInfor("当前语言" + config.m_Language + "  Name=" + gameObject.name + "  图片没有找到");
            m_TargetImage.type = config.m_ImageProperty.m_ImageType;
            m_TargetImage.color = config.m_ImageProperty.m_ImageColor;
            m_TargetImage.preserveAspect = config.m_ImageProperty.m_IsPreserveAspect;
            if (m_TargetImage.type != Image.Type.Filled) return;

            m_TargetImage.fillMethod = config.m_ImageProperty.m_FillMethod;
            SetImageFillOriginProperty(ref config.m_ImageProperty);
            m_TargetImage.fillAmount = config.m_ImageProperty.m_FillAmount;
        }
        //根据Image 的Fillmothed 设置fillorgin的值
        private void SetImageFillOriginProperty(ref ImageProperty data)
        {
            switch (data.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    m_TargetImage.fillOrigin = (int)data.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    m_TargetImage.fillOrigin = (int)data.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    m_TargetImage.fillOrigin = (int)data.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    m_TargetImage.fillOrigin = (int)data.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    m_TargetImage.fillOrigin = (int)data.m_fillOriginVertical;
                    break;
            }

        }
        private void SetImageFillOriginProperty(ref ImageProperty source, ref ImageProperty data)
        {
            switch (data.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    source.m_fillOrigin180 = data.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    source.m_fillOrigin360 = data.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    source.m_fillOrigin90 = data.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    source.m_fillOriginHorizontal = data.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    source.m_fillOriginVertical = data.m_fillOriginVertical;
                    break;
            }
        }


        #endregion

        //将sources 数据赋值到target中
        private ImageConfige CloneImageConfigureValue(ImageConfige target, ref ImageConfige sources)
        {
            target.m_ImageProperty = sources.m_ImageProperty;
            return target;
        }

    }
}

