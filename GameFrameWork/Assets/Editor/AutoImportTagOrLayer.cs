using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 自动导入Tag和Layer 设置  (随包一起打包可以确保导入自己设置的Layer和Tag)  貌似必须打包成package 才会执行
    /// </summary>
    public class AutoImportTagOrLayer : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string s in importedAssets)
            {
                if (s.Equals("Assets/NewBehaviourScript.cs"))
                {
                    AddTag("UICacheItemMark");
                    //增加一个叫ruoruo的layer
                    AddLayer("UnVisualLayer");
                    return;
                }
            }
        }

        static void AddTag(string tag)
        {
            if (!isHasTag(tag))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "tags")
                    {
                        for (int i = 0; i < it.arraySize; i++)
                        {
                            SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(dataPoint.stringValue))
                            {
                                dataPoint.stringValue = tag;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static void AddLayer(string layer)
        {
            if (!isHasLayer(layer))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name.StartsWith("User Layer"))
                    {
                        if (it.type == "string")
                        {
                            if (string.IsNullOrEmpty(it.stringValue))
                            {
                                it.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                    return true;
            }
            return false;
        }

        static bool isHasLayer(string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                    return true;
            }
            return false;
        }

    }
}