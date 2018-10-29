using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameFrameWork.EditorExpand
{
    public class MultiLanguageRecourcesMenu : Editor
    {
        [MenuItem("Tools/Build Application/生成将要移动出去的多语言资源 .asset配置")]
        private static void CreateMultiLanguageResourceAsset()
        {
            string directoryPath = System.IO.Path.GetDirectoryName(EditorDefine.S_BuildAppMultLanguageAssetPath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(EditorDefine.S_BuildAppMultLanguageAssetPath);
            ScriptableObjectUtility.CreateUnityAsset<MutiLanguageResourcesInfor>("创建 多语言资源 .asset配置 ", directoryPath, fileName);
            Debug.LogEditorInfor("创建 多语言资源 .asset配置文件 成功");
        }

        [MenuItem("Tools/Build Application/生成保存移动出去的资源 .asset配置文件")]
        private static void CreateSelectLanguageResourceAsset()
        {
            string directoryPath = System.IO.Path.GetDirectoryName(EditorDefine.S_MoveOutMultLanguageAssetPath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(EditorDefine.S_MoveOutMultLanguageAssetPath);
            ScriptableObjectUtility.CreateUnityAsset<MoveOutResourceaRecord>("保存已经移出去的资源信息", directoryPath,fileName);
            Debug.LogEditorInfor("创建 生成保存选择语言资源 .asset配置文件 成功");
        }

    }
}