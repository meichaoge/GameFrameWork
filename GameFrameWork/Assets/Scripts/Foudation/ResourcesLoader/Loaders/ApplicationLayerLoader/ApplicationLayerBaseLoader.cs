using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 应用层加载器基类 所有实际加载需求的类继承这个，然后内部使用BridgeLoader 加载资源
    /// 注意对于已经加载的资源 需要确认是否需要添加引用或者减少以前的引用
    /// </summary>
    public class ApplicationLayerBaseLoader : BaseAbstracResourceLoader
    {
        /// <summary>
        /// 标识加载器类型 =true 时候不会重复加载 也不会考虑卸载资源问题 
        /// =false 的资源一个物体只能拥有一份 
        /// </summary>
        public bool IsDontDestroyOnLoaded { get; protected set; }

        protected BridgeLoader m_BridgeLoader;  //加载不同路径的资源桥接器
       







    }
}