using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 资源加载模式(同步/异步)
    /// </summary>
    public enum LoadAssetModel
    {
        ///    Sync, //同步加载 当前帧返回
        Async, //协程异步加载
    }

    /// <summary>
    /// 资源加载时候选择的路径
    /// </summary>
    [System.Serializable]
    public enum LoadAssetPathEnum
    {
        PersistentDataPath,  //外部的资源目录
        ResourcesPath, //Resources路径
                       // StreamingAssetsPath, //
                       //  EditorAssetDataPath,  //编辑器下路径
        None,
    }

    /// <summary>
    /// 资源的类型  会根据资源类型处理不同的
    /// </summary>
    public enum AssetTypeTag
    {
        None,  //  不做处理
        ShaderAsset,  //Shader资源
        Material,  //材质球
    }
    /// <summary>
    /// 加载器返回资源的类型 
    /// </summary>
    public enum ReturnAssetType
    {
        None,  //无定义的错误类型  （BridgeLoader 可能出现）
               //   AssetBundle,  //返回的资源是一个AssetBundle  (AssetBundleLoader)
        WWW,  //返回WWW对象  (WWWLoader)
        ByteArray, //返回一个Byte数组(ByteLoader)
        Object,   // 返回可实例化的对象  (AssetBundleLoader)
    }


    /// <summary>
    /// 使用桥接器加载资源需要指定资源最终的加载类型便于转化
    /// </summary>
    public enum LoadAssetType
    {
        ByteArray,
        String,
        AudioClip,
        Texture2D,
        AssetBundle, 
    }

    ///// <summary>
    ///// 资源类型和扩展名对应关系
    ///// </summary>
    //public class AssetTypeAndExtension
    //{
    //    public AssetTypeTag m_AssetTypeTag;  //资源类型
    //    public string m_ExtensionName; //扩展名
    //}
}