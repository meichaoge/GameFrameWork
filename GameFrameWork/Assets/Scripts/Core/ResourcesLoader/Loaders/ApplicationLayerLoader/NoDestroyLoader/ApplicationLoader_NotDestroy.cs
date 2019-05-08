using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 一些不会在运行期主动被间卸载的资源 (比如文本配置/Shader 等)
    /// </summary>
    public abstract class ApplicationLoader_NotDestroy : ApplicationLayerBaseLoader
    {
        public override void InitialLoader()
        {
            base.InitialLoader();
            IsDontDestroyOnLoaded = true;
        }

        protected override void AddReference(Transform requestTarget, string url)
        {
            ReferCount = 1;
        }

        public override void ReduceReference(BaseAbstracResourceLoader loader, bool isForcesDelete)
        {
          //***这里不能有减少引用的操作  base.ReduceReference(loader, isForcesDelete);
        }


    }
}