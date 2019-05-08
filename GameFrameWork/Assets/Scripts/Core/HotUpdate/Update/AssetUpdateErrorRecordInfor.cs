using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.HotUpdate
{
    /// <summary>
    /// 记录下载过程中的错误
    /// </summary>
    public class AssetUpdateErrorRecordInfor
    {
        public AssetUpdateManagerBase UpdateManager;
        public string ErrorDescription;
        public AssetUpdateErrorCode ErrrorCode = AssetUpdateErrorCode.None;

        public AssetUpdateErrorRecordInfor(AssetUpdateManagerBase manager, string description, AssetUpdateErrorCode code)
        {
            UpdateManager = manager;
            ErrorDescription = description;
            ErrrorCode = code;
        }

    }
}