using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// AssetBundle 包含的信息
    /// </summary>
    public class AssetBundleAssetInfor
    {
        public bool IsInitialed = false;
        public HashSet<string> ContainAsset = new HashSet<string>();

    }
}