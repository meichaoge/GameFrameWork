﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameFrameWork
{
    /// <summary>
    /// TextMeshProUGUI本地化组件
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class LocalizationImage : LocalizationBase
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
            public ImageProperty property = new ImageProperty();
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

            Image image = GetComponent<Image>();
            if (image == null)
                return;
            config.property.CloneFromImage(image);
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

            Image image = GetComponent<Image>();
            if (image == null)
                return;
            config.property.CloneToImage(image);
        }
        #endregion
    }
}
