using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 打包程序的 窗口
    /// </summary>
    public class BuildToolWindow : EditorWindow
    {
        private static BuildToolWindow m_BuildToolWindow = null;

        [MenuItem("Tools/Build Application/打包项目资源设置", false, 1)]
        private static void ShowBuildApplicationWin()
        {
            m_BuildToolWindow = EditorWindow.GetWindow<BuildToolWindow>("打包项目");

            m_BuildToolWindow.minSize = new Vector2(400, 400);
            m_BuildToolWindow.maxSize = new Vector2(800, 600);
            m_BuildToolWindow.Show();
        }

        [PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            Debug.Log(pathToBuiltProject);
        }




        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent(string.Format("当前选择的语言: {0}", LanguageMgr.Instance.GetCurLanguageStr())), GUILayout.Width(300));
            GUILayout.Space(10);

            GUILayout.BeginVertical("Box");
            int willRemoveCount = BuildTool.S_MutiLanguageResourcesInfor.AllWillMoveOutResources.Count;
            EditorGUILayout.SelectableLabel(string.Format("已经记录了 {0} 个需要打包前移除的资源目录", willRemoveCount));
            int alreadyRecordCount = BuildTool.S_MoveOutResourceaRecordInfor.m_AllWillMoveInAssetPath.Count;
            EditorGUILayout.SelectableLabel(string.Format("已经记录了 {0} 个需要导入的资源目录", alreadyRecordCount));

            GUILayout.EndVertical();
            #region  菜单按钮

            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace(); //使用空白填充
            if (GUILayout.Button(new GUIContent("移除多语言资源"), GUILayout.Width(180), GUILayout.Height(40)))
            {
                if (alreadyRecordCount > 0)
                {
                    if (EditorUtility.DisplayDialog("提示", "已经记录了部分需要导入的资源，确定需要添加记录吗？", "确定", "取消") == false)
                        return;
                }

                if (willRemoveCount > 0)
                    BuildTool.MoveOutUnUseResources(AppConfigSetting.Instance.LanguageType);
                Debug.LogEditorInfor("移除其他多语言资源 选择" + AppConfigSetting.Instance.LanguageType);
                EditorUtility.DisplayDialog("打包前移除资源", string.Format("打包前移除非{0}语言的资源到目录{1}", AppConfigSetting.Instance.LanguageType,
                   EditorDefine.S_BuildAppMultLanguageTempStorePath), "已知晓");
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("重新导入多语言资源"), GUILayout.Width(180), GUILayout.Height(40)))
            {
                if (alreadyRecordCount > 0)
                    BuildTool.MoveInAllMoveOutResources();
                Debug.LogEditorInfor("重新导入多语言资源 ");
                EditorUtility.DisplayDialog("打包后恢复资源", string.Format("还原所有已经被移动到{0}的资源", EditorDefine.S_BuildAppMultLanguageTempStorePath), "已知晓");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            #endregion

        }

    }
}