using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.HotUpdate
{
    /// <summary>
    /// 创建热更资源加载器的工厂
    /// </summary>
    public class AssetUpdateManagerFactory
    {
        /// <summary>
        /// 根据不同的资源类型获取对用的资源工厂
        /// </summary>
        /// <param name="assetEnum"></param>
        /// <returns></returns>
        public static AssetUpdateManagerBase CreateUpdateMgr(HotAssetEnum assetEnum)
        {
            switch (assetEnum)
            {
                case HotAssetEnum.AssetBundleAsset:
                    return new AsssetBundleUpdateManager();

                case HotAssetEnum.LuaAsset:
                    Debug.LogError("没有处理Lua  TODO");
                    return null;

                default:
                    Debug.LogError("没有处理定义的资源类型 " + assetEnum);
                    return null;
            }

        }

    }
}