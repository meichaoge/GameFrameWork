using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 多语言资源信息配置 (记录哪些目录资源需要在打包时候被移动出去)
    /// </summary>
    public class MutiLanguageResourcesInfor : ScriptableObject
    {
        public List<MutiLanguageResourceItem> AllWillMoveOutResources = new List<MutiLanguageResourceItem>();

    }

    /// <summary>
    /// 多语言资源项
    /// </summary>
    [System.Serializable]
    public class MutiLanguageResourceItem
    {
        /// <summary>
        /// 相对于Assets 目录路径
        /// </summary>
        [Tooltip("相对于Assets 目录路径")]
        public string AssetRelativePath;
        /// <summary>
        /// =false 则会自动按照支持的语言添加一个子语言目录 ,=true标识不需要考虑有多个语言版本资源
        /// </summary>
        [Tooltip("=false 则会自动按照支持的语言添加一个子语言目录 ,=true标识不需要考虑有多个语言版本资源")]
        public bool IsIgnoreLanguage = false;

    }


}