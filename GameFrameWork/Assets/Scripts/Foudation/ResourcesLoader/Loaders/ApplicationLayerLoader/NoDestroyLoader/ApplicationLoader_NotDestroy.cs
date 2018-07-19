using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 一些不会在运行期主动被间卸载的资源 (比如文本配置/Shader 等)
    /// </summary>
    public class ApplicationLoader_NotDestroy : ApplicationLayerBaseLoader
    {
        public override void InitialLoader()
        {
            base.InitialLoader();
            IsDontDestroyOnLoaded = true;
        }

        protected override void AddReference(GameObject requestTarget, string url)
        {
            ReferCount = 1;
        }

        protected override void ReduceReference(Type loaderType, bool isForcesDelete)
        {
          
        }


    }
}