using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 整个应用程序的主管理器 负责管理其他模块
    /// </summary>
    public class ApplicationMgr : Singleton_Mono<ApplicationMgr>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        private void FixedUpdate()
        {
            ResourcesLoaderMgr.Tick();  
        }


    }
}