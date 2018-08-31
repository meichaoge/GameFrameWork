﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.HotUpdate
{
    /// <summary>
    /// 热更新AssetBundle 
    /// </summary>
    public class AsssetBundleUpdateManager : AssetUpdateManagerBase
    {

        protected override void InitialedUpdateMgr()
        {
            base.InitialedUpdateMgr();
            HotAssetManagerEnum = HotAssetEnum.AssetBundleAsset;
            m_LocalAssetConfigurePath = ConstDefine.S_AssetBundleTopPath;
            m_LocalAssetConfigureFileName = string.Format("{0}_{1}", AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(), "AssetBundleInfor.txt");

            Debug.LogInfor("InitialedUpdateMgr HotAssetManagerEnum=" + m_LocalAssetConfigurePath);
            Debug.LogInfor("InitialedUpdateMgr m_LocalAssetConfigureFileName=" + m_LocalAssetConfigureFileName);



        }




        protected override void GetLocalAssetRecordInfor(string assetText)
        {
             base.GetLocalAssetRecordInfor(assetText);
         //   m_LocalAssetRecord = JsonMapper.ToObject<HotAssetBaseRecordInfor>(assetText);
        }



        protected override void GetServerAssetRecordInfor(string assetText)
        {
            //  base.GetServerAssetRecordInfor(assetText);
            m_ServerAssetRecord = JsonMapper.ToObject<HotAssetBaseRecordInfor>(assetText);
            //Debug.Log("GetServerAssetRecordInfor " + m_ServerAssetRecord.m_Version);
            //foreach (var item in m_ServerAssetRecord.m_AllAssetRecordsDic)
            //{
            //    Debug.Log("" + item.Value.m_MD5Code + "  ;;;" + item.Value.m_ByteSize+"\n");
            //}
        }

        protected override string GetAssetDownLoadPath(string assetName)
        {
            string path= string.Format("{0}{1}/{2}", m_ServerAssetConfInfor.m_ServerAssetPath, AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(), assetName);
          //  Debug.Log("path=" + path);
            return path;
        }


        protected override string GetAssetRelativePathByUrl(string url)
        {
            string abundleDwonPath = string.Format("{0}{1}/", m_ServerAssetConfInfor.m_ServerAssetPath, AssetBundleMgr.Instance.GetHotAssetBuildPlatformName());
            int index = url.IndexOf(abundleDwonPath);
            if (index == -1)
            {
                Debug.LogError("GetABundlePathByUrl Fail url=" + url);
                return "";
            }

            //Debug.Log(url.Substring(index + abundleDwonPath.Length));
            return url.Substring(index + abundleDwonPath.Length);
        }

    }
}