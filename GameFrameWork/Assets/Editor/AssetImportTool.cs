using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFrameWork.EditorExpand
{
    public class AssetImportTool
    {
        public static bool S_IsEnableImportTeture=true;
      //  private bool isEnable = true;

        [MenuItem("Tools/资源导入设置/启用图片格式设置")]
        public static void  EnableReImportTexture()
        {
            if (S_IsEnableImportTeture)
            {
                Debug.LogEditorInfor("已经开启图片导入功能 ");
                return;
            }
            S_IsEnableImportTeture = true;
            Debug.LogEditorInfor("EnableReImportTexture  开启图片导入功能 ");
        }

        [MenuItem("Tools/资源导入设置/禁用图片格式设置")]
        public static void DisableReImportTexture()
        {
            if (S_IsEnableImportTeture==false)
            {
                Debug.LogEditorInfor("已经禁用图片导入功能 ");
                return;
            }
            S_IsEnableImportTeture = false;
            Debug.LogEditorInfor("DisableReImportTexture  禁用图片导入功能 ");
        }

    }
}