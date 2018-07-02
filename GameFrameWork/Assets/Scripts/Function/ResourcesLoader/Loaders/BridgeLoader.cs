using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 按照先
    /// </summary>
    public class BridgeLoader : BaseAbstracResourceLoader
    {
        public HashSet<System.Action<object>> m_AllCompleteAct = new HashSet<System.Action<object>>();  //加载完成回调

        public static void LoadAsset(string url, System.Action<object> onCompleteAct, LoadAssetModel loadModel = LoadAssetModel.Async)
        {
            bool isLoaderExit = false;
            BridgeLoader bridgeLoader = ResourcesLoaderMgr.GetLoaderInstance<BridgeLoader>(url, ref isLoaderExit);
            bridgeLoader.m_AllCompleteAct.Add(onCompleteAct);

            if (isLoaderExit)
            {
                if (bridgeLoader.IsCompleted)
                    bridgeLoader. OnCompleteLoad(bridgeLoader.IsError, bridgeLoader.Description, bridgeLoader.ResultObj);  //如果当前加载器已经完成加载 则手动触发事件
                return;  //如果已经存在 且当前加载器还在加载中，则只需要等待加载完成则回调用回调
            }

            LoadAssetPathEnum curLoadAssetPathEnum = ApplicationMgr.Instance.GetFirstPriortyAssetPathEnum();  //加载的优先级
              
            if (ResourcesLoaderMgr.GetAssetPathOfLoadAssetPath(ref url, curLoadAssetPathEnum, true) == PathResultEnum.Valid)
            {


              //  bridgeLoader.OnCompleteLoad(true, string.Format("Path is Invalidate {0}", url), null);
                return;
            } //当前资源路径有效

        }





        protected override void OnCompleteLoad(bool isError, string description, object result, float process = 1)
        {
            base.OnCompleteLoad(isError, description, result, process);
            foreach (var completeAct in m_AllCompleteAct)
            {
                if (completeAct != null)
                    completeAct(ResultObj);
            }
            m_AllCompleteAct.Clear();
        }


        public override void Dispose()
        {
        }
    }
}