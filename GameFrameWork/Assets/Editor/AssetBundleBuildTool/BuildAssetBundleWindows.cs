using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{


    /// <summary>
    /// 打包AssetBundle 的目录文件夹
    /// </summary>
    public class BuildAssetBundleWindows : EditorWindow
    {
        /// <summary>
        /// 显示支持的打包AssetBundle 平台类型
        /// </summary>
        public enum EditorBuildTarget
        {
            Win64,
            Android,
            iOS
        } //目标平台


        private EditorBuildTarget m_BuildTarget;
        private bool isSelectAll = true;  //是否选择所有的文件
        private bool isDeSelectAll = false; //全部非选择

        private static BuildAssetBundleWindows S_BuildAssetBundleWindows;
        public static List<BuildAssetBundleTreeNode> m_AllResourcesTopPath = new List<BuildAssetBundleTreeNode>(); //Resources 目录下顶层的文件名和目录名

        private Vector2 m_ScrollRect = Vector2.zero;
        private float m_ShowToggleWidth = 10;
        private float m_SubItemSpace = 20; //两个层级间距

        [MenuItem("Tools/热更新/打包生成 AssetBundle资源")]
        private static void ShowBuildAssetBundleWin()
        {
            S_BuildAssetBundleWindows = EditorWindow.GetWindow<BuildAssetBundleWindows>("打包AssetBundle");
            S_BuildAssetBundleWindows.minSize = new Vector2(400, 400);
            S_BuildAssetBundleWindows.maxSize = new Vector2(800, 600);
            S_BuildAssetBundleWindows.Show();
            GetAllShowPaths();
        }


        private void OnGUI()
        {
            #region 显示打包平台的枚举项
            GUILayout.Space(10);
            m_BuildTarget = (EditorBuildTarget)EditorGUILayout.EnumPopup("打包目标平台", m_BuildTarget, GUILayout.Width(350));

            #endregion

            #region 提示
            GUILayout.Label("需要打包的资源目录:");
            GUILayout.Space(5);

            GUILayout.Label("LuaAsset目录不需要被打包 ，而是生成LuaAsset目录:");
            GUILayout.Space(5);
            #endregion

            #region 全选、全部非选择

            GUILayout.BeginHorizontal();


            isSelectAll = GUILayout.Toggle(isSelectAll, new GUIContent("全部选中"));
            isDeSelectAll = GUILayout.Toggle(isDeSelectAll, new GUIContent("全部非选中"));

            #region     设置状态

            if (isSelectAll)
            {
                isDeSelectAll = false;
                SelectAllAsset();
            }
            if (isDeSelectAll)
            {
                isSelectAll = false;
                UnSelectAllAsset();
            }
            #endregion

            GUILayout.EndHorizontal();
            #endregion

            #region  显示树形结构
            m_ScrollRect = GUILayout.BeginScrollView(m_ScrollRect, true, true);

            for (int dex = 0; dex < m_AllResourcesTopPath.Count; ++dex)
            {
                ShowSubFilePath(m_AllResourcesTopPath[dex], 1,true);
            }
            GUILayout.EndScrollView();
            GUILayout.Space(15);
            #endregion

            #region  显示打包按钮
           GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("开始打包"), GUILayout.Width(150), GUILayout.Height(50)))
            {
                S_BuildAssetBundleWindows.Close();
                BuildAssetBundleTool.BegingPackAssetBundle(LoalBuildTarget2UnityEngine(m_BuildTarget), S_BuildAssetBundleWindows);
            }
            GUILayout.EndVertical();
            #endregion
        }



        #region GUI 帮助类
        /// <summary>
        /// 全选
        /// </summary>
        private void SelectAllAsset()
        {
            foreach (var item in m_AllResourcesTopPath)
            {
                foreach (var asset in item.m_AllSubNodesInfor)
                {
                    (asset as BuildAssetBundleTreeNode).IsSelected = true;
                }
             (item as BuildAssetBundleTreeNode).IsSelected = true;
            }
        }

        /// <summary>
        /// 全非选
        /// </summary>
        private void UnSelectAllAsset()
        {
            foreach (var item in m_AllResourcesTopPath)
            {
                foreach (var asset in item.m_AllSubNodesInfor)
                {
                    (asset as BuildAssetBundleTreeNode).IsSelected = false;
                }
                (item as BuildAssetBundleTreeNode).IsSelected = false;
            }
        }


        /// <summary>
        /// 递归显示子项
        /// </summary>
        /// <param name="recordInfor"></param>
        /// <param name="layer"></param>
        private void ShowSubFilePath(BuildAssetBundleTreeNode recordInfor, int layer,bool isShow)
        {
            if (isShow == false) return;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(m_SubItemSpace * layer));

            bool isShowNode = GUILayout.Toggle(recordInfor.IsOn, new GUIContent(""), GUILayout.Width(m_ShowToggleWidth));
            if (recordInfor.IsOn != isShowNode)
                recordInfor.IsOn = isShowNode;
            bool isSelect = GUILayout.Toggle(recordInfor.IsSelected, new GUIContent(GetRelativePath(recordInfor.m_ViewName)));
            if (recordInfor.IsSelected != isSelect)
            {
                recordInfor.IsSelected = isSelect;
            }
            GUILayout.EndHorizontal();

            layer += 1;
            foreach (var item in recordInfor.m_AllSubNodesInfor)
            {
                BuildAssetBundleTreeNode node = item as BuildAssetBundleTreeNode;
                if (item.IsTreeNode)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(m_SubItemSpace * layer));
                    bool isShowSubNode = GUILayout.Toggle(item.IsOn, new GUIContent(""), GUILayout.Width(m_ShowToggleWidth));
                    if (item.IsOn != isShowSubNode)
                        item.IsOn = isShowSubNode;

                    bool isSubSelect = GUILayout.Toggle(node.IsSelected, new GUIContent(GetRelativePath(node.m_ViewName)));
                    if (node.IsSelected != isSubSelect)
                        node.IsSelected = isSubSelect;
                    GUILayout.EndHorizontal();
                }
                else
                {
                    ShowSubFilePath(node, layer, item.IsOn);
                }
            }
            GUILayout.EndVertical();
        }

        #endregion


        #region 接口

        /// <summary>
        /// 遍历获取所有Resources 目录下的文件和目录
        /// </summary>
        private static void GetAllShowPaths()
        {
            m_AllResourcesTopPath.Clear();
            //   Debug.LogInfor("AAAAAAAAAA " + ConstDefine.S_ResourcesPath);
            string[] directorys = System.IO.Directory.GetDirectories(ConstDefine.S_ResourcesPath, "*", SearchOption.TopDirectoryOnly);
            string[] files = System.IO.Directory.GetFiles(ConstDefine.S_ResourcesPath, "*", SearchOption.TopDirectoryOnly);

            foreach (var directory in directorys)
            {
                //    Debug.Log("GetAllShowPaths directory=" + directory);
                if (System.IO.Path.GetExtension(directory) != ".meta")
                {
                    BuildAssetBundleTreeNode infor = new BuildAssetBundleTreeNode(directory);
                    infor.IsOn = true;
                    GetSearchAllSubFile(infor, directory);
                    m_AllResourcesTopPath.Add(infor);
                }
            }

            foreach (var file in files)
            {
                //    Debug.Log("GetAllShowPaths file=" + file);
                if (System.IO.Path.GetExtension(file) != ".meta")
                {
                    BuildAssetBundleTreeNode infor = new BuildAssetBundleTreeNode(file);
                    infor.IsOn = true;
                    m_AllResourcesTopPath.Add(infor);
                }
            }

            //     Debug.Log("m_AllResourcesTopPath " + m_AllResourcesTopPath.Count);
        }
        /// <summary>
        /// 遍历子目录
        /// </summary>
        /// <param name="infor"></param>
        /// <param name="path"></param>
        private static void GetSearchAllSubFile(BuildAssetBundleTreeNode infor, string path)
        {
            if (System.IO.Directory.Exists(path) == false) return;

            //        Debug.Log("BBBBB  " + path);
            string[] directorys = System.IO.Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            string[] files = System.IO.Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

            if (directorys.Length == 0 && files.Length == 0) return;

            foreach (var directory in directorys)
            {
                //    Debug.Log("GetSearchAllSubFile directory=" + directory);
                if (System.IO.Path.GetExtension(directory) != ".meta")
                {
                    BuildAssetBundleTreeNode subInfor = new BuildAssetBundleTreeNode(directory);
                    subInfor.IsOn = false;
                    infor.m_AllSubNodesInfor.Add(subInfor);
                    GetSearchAllSubFile(subInfor, directory);
                }
            }

            foreach (var file in files)
            {
                //          Debug.Log("GetSearchAllSubFile file=" + file);
                if (System.IO.Path.GetExtension(file) != ".meta")
                {
                    BuildAssetBundleTreeNode subInfor = new BuildAssetBundleTreeNode(file);
                    subInfor.IsOn = false;
                    GetSearchAllSubFile(subInfor, file);
                    infor.m_AllSubNodesInfor.Add(subInfor);
                }
            }
        }



        /// <summary>
        /// 获取相对于Resources 下的目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            int index = path.IndexOf(ConstDefine.S_ResourcesPath);
            if (index != -1)
            {
                return path.Substring(index + ConstDefine.S_ResourcesPath.Length);
            }
            return "";
        }

        /// <summary>
        /// 检测当前文件或者目录是否需要被打包
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isFile"></param>
        /// <returns></returns>
        public bool CheckIfNeedPacked(string path)
        {
            if (System.IO.Path.GetExtension(path) == ".meta")
            {
                path = path.Substring(0, path.Length - ".meta".Length);
            }

            for (int dex = 0; dex < m_AllResourcesTopPath.Count; ++dex)
            {
                if (m_AllResourcesTopPath[dex].m_ViewName == path)
                {
                    return m_AllResourcesTopPath[dex].IsSelected;
                }
            }

            Debug.LogError("CheckIfNeedPacked Fail,Not Exit Path " + path);
            return false;
        }

        private BuildTarget LoalBuildTarget2UnityEngine(EditorBuildTarget target)
        {
            switch (target)
            {
                case EditorBuildTarget.Win64:
                    return BuildTarget.StandaloneWindows64;
                case EditorBuildTarget.Android:
                    return BuildTarget.Android;
                case EditorBuildTarget.iOS:
                    return BuildTarget.iOS;
                default:
                    Debug.LogError("没有定义的类型 " + target);
                    return BuildTarget.StandaloneWindows;
            }
        }

        #endregion

        public  void OnCompleteBuildAssetBundle()
        {
            string message = string.Format("打包完成，AssetBundle 保存在{0} \n AssetBundle 生成的文件保存在{1}", ConstDefine.S_AssetBundleTopPath, ConstDefine.S_AssetBundleTopPath);

            EditorUtility.DisplayDialog("打包AssetBundle ", message, "已知晓");
            if (S_BuildAssetBundleWindows != null)
                S_BuildAssetBundleWindows.Close();
        }

    }
}