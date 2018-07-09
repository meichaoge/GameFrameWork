using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameFrameWork.EditorExpand
{

    /// <summary>
    /// 创建序列化资源 Asset 工具类
    /// </summary>
    public static class ScriptableObjectUtility
    {
        /// <summary>
        /// 创建Unity 序列化资源Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateUnityAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
             string assetPathAndName = EditorDialogUtility.SaveFileDialog("保存Asset资源", "", "", "asset");
            Debug.Log("assetPathAndName= " + assetPathAndName);
            assetPathAndName = assetPathAndName.Substring(assetPathAndName.IndexOf("Assets"));

            Debug.Log("CreateUnityAsset >>>path :" + assetPathAndName);
            AssetDatabase.CreateAsset(asset, assetPathAndName); //创建资源Asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }


}
