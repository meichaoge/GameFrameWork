using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 应用层加载器基类 所有实际加载需求的类继承这个，然后内部使用BridgeLoader 加载资源
    /// </summary>
    public class ApplicationLayerBaseLoader : BaseAbstracResourceLoader
    {
        protected BridgeLoader m_BridgeLoader;  //加载不同路径的资源桥接器

    }
}