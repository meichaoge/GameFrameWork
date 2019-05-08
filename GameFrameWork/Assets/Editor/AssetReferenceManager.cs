using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 编辑器下UI资源管理
/// 要求所有的图片都是Resources下预制体加载，而不是直接加载
/// UI资源和特效资源单独放置
/// </summary>
public class AssetReferenceManager
{

    //[MenuItem("Tools/删除所有本地保存的数据")]
    //private static void DelateAllPlayerPrefs()
    //{
    //    PlayerPrefs.DeleteAll();
    //}
    #region 编辑器菜单
    [MenuItem("Tools/资源管理/获取Resources下Prefabs依赖信息")]
    private static void GeAllPrefabsAssetInfor()
    {
        GetAllPrefabRelatveInfor();
    }


    #region 文件

    [MenuItem("Tools/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_全部")]
    private static void GetImgAssetReferencePrefab_All()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }
        List<string> allSeleteAssets = new List<string>();
        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            allSeleteAssets.Add(assetPath);
         //   GetAllPrefabRelatveInfor();
            ShowAllImageReference(allSeleteAssets);
            return;
        }
        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }

    [MenuItem("Tools/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_无引用")]
    private static void GetImgAssetReferencePrefab_NoReference()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }
        List<string> allSeleteAssets = new List<string>();
        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            allSeleteAssets.Add(assetPath);
         //   GetAllPrefabRelatveInfor();
            ShowAllImageReference_NoReference(allSeleteAssets);
            return;
        }
        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }


    [MenuItem("Tools/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_引用")]
    private static void GetImgAssetReferencePrefab_Reference()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }
        List<string> allSeleteAssets = new List<string>();
        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            allSeleteAssets.Add(assetPath);
         //   GetAllPrefabRelatveInfor();
            ShowAllImageReference_Reference(allSeleteAssets);
            return;
        }
        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }


    [MenuItem("Tools/资源管理/图片管理(文件)/查找当前图片资源被引用的预制体信息_自动设置无用资源格式")]
    private static void GetImgAssetReferencePrefab_AutoSet()
    {
        var selectobjs = Selection.objects;
        if (selectobjs.Length == 0)
            return;

        if (selectobjs.Length > 1)
        {
            Debug.LogError("只能选择一个图片资源");
            return;
        }
        List<string> allSeleteAssets = new List<string>();
        string assetPath = AssetDatabase.GetAssetPath(selectobjs[0]);
        if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            allSeleteAssets.Add(assetPath);
        //    GetAllPrefabRelatveInfor();
            ShowAndAutoSetAllImageReference_NoReference(allSeleteAssets);
            return;
        }
        Debug.LogError("选择的资源不是图片资源 " + assetPath);
    }

    #endregion

    #region 文件夹

    [MenuItem("Tools/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_全部")]
    private static void GetImgsAssetReferencePrefab_All()
    {
        var selectobjs = Selection.objects;
        List<string> allImageAsset = new List<string>(selectobjs.Length);
        foreach (var item in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(item);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] { assetPath });

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
             //   Debug.Log(spritePath);
            }
        }

    //    GetAllPrefabRelatveInfor();
        ShowAllImageReference(allImageAsset);
    }

    [MenuItem("Tools/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_没引用")]
    private static void GetImgsAssetReferencePrefab_NoReference()
    {
        var selectobjs = Selection.objects;
        List<string> allImageAsset = new List<string>(selectobjs.Length);
        foreach (var item in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(item);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] { assetPath });

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

    //    GetAllPrefabRelatveInfor();
        ShowAllImageReference_NoReference(allImageAsset);
    }

    [MenuItem("Tools/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_引用")]
    private static void GetImgsAssetReferencePrefab_Reference()
    {
        var selectobjs = Selection.objects;
        List<string> allImageAsset = new List<string>(selectobjs.Length);
        foreach (var item in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(item);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] { assetPath });

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

     //   GetAllPrefabRelatveInfor();
        ShowAllImageReference_Reference(allImageAsset);
    }

    [MenuItem("Tools/资源管理/图片管理(文件夹)/所有图片资源被引用的预制体信息_自动设置无用资源格式")]
    private static void GetImgsAssetReferencePrefab_AutoSet()
    {
        var selectobjs = Selection.objects;
        List<string> allImageAsset = new List<string>(selectobjs.Length);
        foreach (var item in selectobjs)
        {
            string assetPath = AssetDatabase.GetAssetPath(item);
            string[] containSprites = AssetDatabase.FindAssets("t:Sprite", new string[] { assetPath });

            foreach (var spriteAsset in containSprites)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteAsset);
                allImageAsset.Add(spritePath);
            }
        }

     //   GetAllPrefabRelatveInfor();
        ShowAndAutoSetAllImageReference_NoReference(allImageAsset);
    }
    #endregion


    #endregion


    private static Dictionary<string, List<string>> mAllPrefabsDepdence = new Dictionary<string, List<string>>();  //所有记录的Prefab资源的引用


    #region 处理接口
    private  static void GetAllPrefabRelatveInfor()
    {
        mAllPrefabsDepdence.Clear();
        string[] assetGuidList = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });  //获取Resources下所有的预制体
        foreach (var assetguid in assetGuidList)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetguid);
            string[] depdence = AssetDatabase.GetDependencies(assetPath);  //获取当前资源的依赖
            mAllPrefabsDepdence[assetPath] = new List<string>(depdence);
        }
        Debug.Log(string.Format("获取Resources下{0}个预制体的依赖信息", mAllPrefabsDepdence.Count));
    }

    /// <summary>
    /// 获取参数图片资源被哪些资源引用
    /// </summary>
    /// <param name="imagAsset"></param>
    /// <returns></returns>
    private static Dictionary<string,List<string>> GetImageAssetReference(List<string> imagAsset) 
    {
        if(mAllPrefabsDepdence.Count==0)
        {
            GetAllPrefabRelatveInfor();
        }


        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>(imagAsset.Count);

        foreach (var imgItem in imagAsset)
        {
            List<string> reference = new List<string>(10);  //那些预制体资源引用
            foreach (var prefabsAsset in mAllPrefabsDepdence)
            {
                if(prefabsAsset.Value.Contains(imgItem))
                {
                    reference.Add(prefabsAsset.Key);
                }
            }
            result[imgItem] = reference;
        }

        return result;
    }



    /// <summary>
    /// 显示所有的结果
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }
            Debug.Log("End---------------------Asset;" + item.Key);
        }
    }

    /// <summary>
    /// 只显示无引用的
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference_NoReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;

            Debug.Log(string.Format("没有被引用的资源 AssetName={0,-30}   Path={1,-100};",System.IO.Path.GetFileName(item.Key), item.Key));
        }

        Debug.Log("完成显示无引用的图片信息");
    }

    /// <summary>
    /// 只显示有引用的
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAllImageReference_Reference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            if (item.Value.Count == 0)
                continue;

            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }
            Debug.Log("End---------------------Asset;" + item.Key);
        }
        Debug.Log("完成显示有引用的图片信息");

    }

    /// <summary>
    /// 显示所欲没有被引用的资源，并且自动设置格式
    /// </summary>
    /// <param name="imagAsset"></param>
    private static void ShowAndAutoSetAllImageReference_NoReference(List<string> imagAsset)
    {
        Dictionary<string, List<string>> result = GetImageAssetReference(imagAsset);
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;

            Debug.Log("Begin---------------------Asset;" + item.Key);
            foreach (var subitem in item.Value)
            {
                Debug.Log(subitem);
            }
            Debug.Log("End---------------------Asset;" + item.Key);
        }


        int count = 0;
        //***设置导入格式
        foreach (var item in result)
        {
            if (item.Value.Count != 0)
                continue;
            ++count;
            TextureImporter impoter = AssetImporter.GetAtPath(item.Key) as TextureImporter;
            if(impoter.textureType != TextureImporterType.Default)
            {
                impoter.textureType = TextureImporterType.Default;
                impoter.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("一共设置了" + count + "个资源的格式");

    }

    #endregion


}
