using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 监听编辑器下资源导入
    /// </summary>
    public class ListenAssetImport : AssetPostprocessor
    {

        #region 监听图片的导入
        /// <summary>
        /// 导入图片后执行
        /// </summary>
        public void OnPreprocessTexture()
        {
            Debug.Log("OnPreProcessTexture=" + this.assetPath);
            TextureImporter impor = this.assetImporter as TextureImporter;
            if (this.assetPath.StartsWith(string.Format("Assets/{0}", EditorDefine.S_UITextureTopRelativePath)))
            {
                OnPresProcessUITextureSetting(impor);
            } //导入UI资源
        }

        /// <summary>
        /// 导入图片后执行 （这里必须有参数否则报错）
        /// </summary>
        public void OnPostprocessTexture(Texture2D texture)
        {

        }


        /// <summary>
        /// 导入UI图片资源
        /// </summary>
        /// <param name="impor"></param>
        private void OnPresProcessUITextureSetting(TextureImporter textureImporter)
        {

            ////textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.spritePackingTag = EditorHelper.Instance.GetUITexturePackingName(this.assetPath); ///所属的图集

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

        #endregion


        #region  导入模型
        public void OnPreprocessModel()
        {
            ModelImporter impor = this.assetImporter as ModelImporter;
            //impor.isReadable = false;
            if (this.assetPath.StartsWith(string.Format("Assets/{0}", EditorDefine.S_ModelTopRelativePath)))
                return;
            Debug.Log("OnPreprocessModel=" + this.assetPath);
            impor.importMaterials = false;  //导入模型的时候不导入材质球而是自动生成避免不能修改材质球为(TODO 查的)
        }
        #endregion


    }
}