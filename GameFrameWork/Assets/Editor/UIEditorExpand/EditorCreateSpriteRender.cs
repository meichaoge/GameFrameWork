using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 编辑器下生成Sprite2D对应的预制体
    /// </summary>
    public class EditorCreateSpriteRender //: Editor
    {
        /// <summary>
        /// 根据选择的图片生成预制体并关联这个图片
        /// </summary>
        public static void CreateSpriteRender()
        {
            UnityEngine.Object[] selectObj = Selection.objects;
            if (selectObj.Length == 1)
            {
                SaveSingleSprite(selectObj[0]);
            }
            else
            {
                SaveMultiSprite(selectObj);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据选择的目录生成预制体并关联这个图片
        /// </summary>
        public static void CreateMultiSpriteRenders_Dirctory()
        {
            string dirctoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (System.IO.Directory.Exists(dirctoryPath) == false)
            {
                Debug.LogError("当前选择的不是一个可用的目录路径,创建失败");
                return;
            }

            string savePrefabPath = EditorDialogUtility.SaveFileDialog("保存生成的预制体", Application.dataPath + "/Resources", "只需要选择合适的目录，不需要填写文件名", "prefab");
            if (string.IsNullOrEmpty(savePrefabPath))
            {
                Debug.LogInfor("取消 创建SpriteRender");
                return;
            }

            //Debug.Log(Application.dataPath);
            //Debug.Log("dirctoryPath=" + dirctoryPath);
            var assets = AssetDatabase.FindAssets("t:texture2D", new string[] { dirctoryPath }); //获取当前目录下所有的图片资源GUID
            List<string> allTexture2DAssets = new List<string>();  //所有的图片资源相对资源路径
            foreach (var item in assets)
            {
                allTexture2DAssets.Add(AssetDatabase.GUIDToAssetPath(item));
            }//转成成资源的相对路径

            bool IsNeedReflushAsset = false; //标识是否需要刷新资源
            for (int dex = allTexture2DAssets.Count - 1; dex >= 0; --dex)
            {
                #region 对资源的格式进行处理

                TextureImporter asstImpoter = AssetImporter.GetAtPath(allTexture2DAssets[dex]) as TextureImporter;
                if (asstImpoter == null)
                {
                    IsNeedReflushAsset = true;
                    allTexture2DAssets.RemoveAt(dex);
                    Debug.LogError("理论上不存在这个情况");
                    continue;
                }

                if (asstImpoter.textureType != TextureImporterType.Sprite)
                {
                    IsNeedReflushAsset = true;
                    if (System.IO.Path.GetExtension(allTexture2DAssets[dex]).ToLower() != ".png")
                    {
                        Debug.LogEditorInfor("生成Sprite 关联资源时候 资源不是png格式 " + allTexture2DAssets[dex]);
                        allTexture2DAssets.RemoveAt(dex);
                        continue;
                    }//不是png 图片则不导入 否则强制导入图片
                    TextureImporterHelper.OnPresProcessUITextureSetting(asstImpoter, allTexture2DAssets[dex]);
                }
                #endregion

            }

            if (IsNeedReflushAsset)
                AssetDatabase.Refresh();



            foreach (var sprite2d in allTexture2DAssets)
            {
                CreateOrUpdateSpritePrefab(sprite2d, System.IO.Path.GetDirectoryName(savePrefabPath));
            }
            AssetDatabase.Refresh();
        }


        private static void SaveSingleSprite(UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            CreateOrUpdateSpritePrefab(path);
        }

        private static void SaveMultiSprite(UnityEngine.Object[] selectObj)
        {
            if (selectObj == null || selectObj.Length == 0) return;
            string savePrefabPath = EditorDialogUtility.SaveFileDialog("保存生成的预制体", Application.dataPath + "/Resources", "只需要选择合适的目录，不需要填写文件名", "prefab");
            if (string.IsNullOrEmpty(savePrefabPath))
            {
                Debug.LogInfor("取消 创建SpriteRender");
                return;
            }

            Debug.Log("savePrefabPath=" + savePrefabPath);
            Debug.Log("savePrefabPath=" + System.IO.Path.GetDirectoryName(savePrefabPath));


            foreach (var item in selectObj)
            {
                string path = AssetDatabase.GetAssetPath(item);
                CreateOrUpdateSpritePrefab(path, System.IO.Path.GetDirectoryName(savePrefabPath));
            }
        }

        /// <summary>
        /// 创建或者更新精灵预制体
        /// </summary>
        /// <param name="path"></param>
        /// <param name="savePrefabPath"></param>
        private static void CreateOrUpdateSpritePrefab(string path, string savePrefabPath = "")
        {
            if (System.IO.Path.GetExtension(path).ToLower() != ".png" && System.IO.Path.GetExtension(path).ToLower() != ".jpg")
            {
                Debug.Log("Not Sprite");
                return;
            }
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter; //获取并转换该资源导入器
                                                                                         //  Debug.Log(importer.textureType);
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite; //格式转换
                importer.mipmapEnabled = false;
                importer.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
            } //修改选中图片资源的格式为Sprite

            string prefabName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(savePrefabPath))
            {
                savePrefabPath = EditorDialogUtility.SaveFileDialog("保存生成的预制体", Application.dataPath + "/Resources", prefabName, "prefab");
                if (string.IsNullOrEmpty(savePrefabPath))
                {
                    Debug.LogInfor("取消 创建SpriteRender");
                    return;
                }
            }
            else
            {
                savePrefabPath = string.Format("{0}/{1}.prefab", savePrefabPath, prefabName);
            }

            Debug.Log("CreateSpriteRender  savePrefabPath=" + savePrefabPath);
            //********需要考虑已经存在的时候只需要替换Sprite  TODO


            // Debug.Log("path=" + path  + "           savePrefabPath=" + savePrefabPath);
            GameObject go = new GameObject(prefabName);
            Sprite sources = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            go.AddComponent<SpriteRenderer>().sprite = sources; //设置Sprite的引用关系
            go.layer = LayerMask.NameToLayer("UI");
            GameObject prefab = PrefabUtility.CreatePrefab(savePrefabPath.Substring(savePrefabPath.IndexOf("Assets")), go); //创建预制体资源 路径必须从 Assets开始
            GameObject.DestroyImmediate(go);
        }


    }
}
