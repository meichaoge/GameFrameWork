using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 图片导入设置
    /// </summary>
    public class TextureImporterHelper
    {

        /// <summary>
        /// 导入UI图片资源
        /// </summary>
        /// <param name="impor"></param>
        public static void OnPresProcessUITextureSetting(TextureImporter textureImporter,string assetPath)
        {

            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.spritePackingTag = EditorHelper.Instance.GetUITexturePackingName(assetPath); ///所属的图集

            //**Advanced 
            textureImporter.sRGBTexture = true;
            textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;  //使用自带的Alpha
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false; //不可读写

            //***设置不同平台下的图片压缩格式
            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.name = "Android";  // 取值("Standalone"/"iPhone","Android")
            settings.format = TextureImporterFormat.ASTC_RGBA_4x4;
            settings.textureCompression = TextureImporterCompression.CompressedHQ;
            settings.overridden = true;
            settings.compressionQuality = 100;   //图片压缩比例
            settings.maxTextureSize = textureImporter.maxTextureSize;
            textureImporter.SetPlatformTextureSettings(settings);

        }
    }
}