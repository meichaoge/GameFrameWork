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
        //[MenuItem("Assets/创建精灵 SpriteRender 预制体")]
        public static void CreateSpriteRender()
        {
            UnityEngine.Object selectObj = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(selectObj);
            //  Debug.Log(path + "                     " + System.IO.Path.GetExtension(path).ToLower());
            if (System.IO.Path.GetExtension(path).ToLower() != ".png" && System.IO.Path.GetExtension(path).ToLower() != ".jpg")
            {
                Debug.Log("Not Sprite");
                return;
            }
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter; //获取并转换该资源导入器
                                                                                         //  Debug.Log(importer.textureType);
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite; //轻质转换
                importer.mipmapEnabled = false;
                importer.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
            } //修改选中图片资源的格式为Sprite

            string prefabName = System.IO.Path.GetFileNameWithoutExtension(path);
            string savePrefabPath = EditorDialogUtility.SaveFileDialog("保存生成的预制体", Application.dataPath + "/Resources", prefabName, "prefab");
            if (string.IsNullOrEmpty(savePrefabPath))
            {
                Debug.LogInfor("取消 创建SpriteRender");
                return;
            }
            Debug.Log("CreateSpriteRender  savePrefabPath=" + savePrefabPath);
            //********需要考虑已经存在的时候只需要替换Sprite  TODO


            // Debug.Log("path=" + path  + "           savePrefabPath=" + savePrefabPath);
            GameObject go = new GameObject(prefabName);
            Sprite sources = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            go.AddComponent<SpriteRenderer>().sprite = sources; //设置Sprite的引用关系

            GameObject prefab = PrefabUtility.CreatePrefab(savePrefabPath.Substring(savePrefabPath.IndexOf("Assets")), go); //创建预制体资源 路径必须从 Assets开始
            GameObject.DestroyImmediate(go);
            AssetDatabase.Refresh();
        }

    }
}
