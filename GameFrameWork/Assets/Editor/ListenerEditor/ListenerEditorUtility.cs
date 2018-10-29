using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 监听 Unity 编辑器
    /// </summary>
    public class ListenerEditorUtility : Editor
    {

        #region 监听Unity 打包前后的事件

        private static bool isBeforeBuildFlag = false;
        public static System.Action OnBeforeBuildPlayerEvent = null;
        public static System.Action<BuildTarget, string> OnPostBuildPlayerEvent = null;

        [PostProcessScene]
        private static void OnProcessScene()
        {
            if (!isBeforeBuildFlag && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogInfor("Unity标准Build 前处理函数  .. OnProcessScene");

                isBeforeBuildFlag = true;

                EditorUtility.DisplayDialog("XX", "aa", "确定");

                if (OnBeforeBuildPlayerEvent != null)
                    OnBeforeBuildPlayerEvent();
            }
        }

        /// <summary>
        /// Unity标准Build后处理函数
        /// </summary>
        [PostProcessBuild()]
        private static void OnPostBuildPlayer(BuildTarget target, string pathToBuiltProject)
        {
            Debug.LogInfor("Unity标准Build后处理函数  .. OnPostBuildPlayer");
            if (OnPostBuildPlayerEvent != null)
            {
                OnPostBuildPlayerEvent(target, pathToBuiltProject);
            }

            UnityEngine.Debug.Log(string.Format("Success build ({0}) : {1}", target, pathToBuiltProject));
        }

        /// <summary>
        /// 编译完成回调
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Debug.LogEditorInfor("编译代码回调");
        }


        #endregion


        #region 监听Prefab 保存的事件
        [InitializeOnLoadMethod]
        static void StartInitializeOnLoadMethod()
        {
            PrefabUtility.prefabInstanceUpdated = delegate (GameObject instance)
            {
                //prefab保存的路径
                Debug.Log(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(instance)));
            };
        }

        #endregion

        #region 监听保存事件
        public static void OnWillSaveAssets(string[] names)
        {
            foreach (string name in names)
            {
                if (name.EndsWith(".unity"))
                {
                    Scene scene = SceneManager.GetSceneByPath(name);
                    Debug.Log("ListenSaveAsset 监听到你正在保存场景资源 ：" + scene.name);
                }
                else
                {
                    Debug.LogInfor(" name= " + name);
                }
            }
        }
        #endregion


    }
}