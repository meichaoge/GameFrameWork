using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace GameFrameWork
{
    /// <summary>
    /// RectTransform本地化组件
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class LocalizationRectTransform : LocalizationBase
    {
        /// <summary>
        /// 配置
        /// </summary>
        [System.Serializable]
        public class Config
        {
            [Header("语言名称")]
            public string languageName;
            [Header("是否启用配置，默认不启用，保存配置自动改为启用")]
            public bool enable;
            [HideInInspector]
            public Language languageType;
            [Header("配置数据")]
            public RectransformBaseProperty property = new RectransformBaseProperty();
        }

        #region 变量属性
        [Header("多语言配置数据")]
        public List<Config> configs = new List<Config>();
        #endregion

        #region 编辑器操作
#if UNITY_EDITOR
        /// <summary>
        /// 重置配置
        /// </summary>
        protected override void ResetConfig()
        {
            base.ResetConfig();

            configs.Clear();
            foreach (var obj in System.Enum.GetValues(typeof(Language)))
            {
                Language languageType = (Language)obj;
                Config config = new Config();
                config.languageName = languageType.ToString();
                config.languageType = languageType;
                configs.Add(config);
            }
        }

        /// <summary>
        /// 保存视图
        /// </summary>
        protected override void SaveView(Language languageType)
        {
            Config config = configs.FirstOrDefault(x => x.languageType == languageType);
            if (config == null)
                return;
            if (!config.enable)
                config.enable = true;

            RectTransform rtf = transform as RectTransform;
            if (rtf == null)
                return;
            config.property.CloneFromRectTransform(rtf);
        }
#endif
        #endregion

        #region 视图操作
        /// <summary>
        /// 初始化视图
        /// </summary>
        protected override void InitView(Language languageType)
        {
            base.InitView(languageType);

            Config config = configs.FirstOrDefault(x => x.languageType == languageType);
            if (config == null)
                return;
            if (!config.enable)
                return;

            RectTransform rtf = transform as RectTransform;
            if (rtf == null)
                return;
            config.property.CloneToRectTransform(rtf);
        }
        #endregion
    }
}
