using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 这个基类定义那些会在程序运行期间只需要加载一次(或者每次找不到的时候去加载的资源) 保存信息后卸载的资源 如配置表
    /// </summary>
    public abstract class ApplicationLoader_AutoUnLoad : ApplicationLayerBaseLoader
    {
      
    }
}