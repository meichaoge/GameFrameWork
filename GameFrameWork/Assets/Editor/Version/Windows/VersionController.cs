//using LitJson;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;


//namespace GameFrameWork.EditorExpand
//{
//    /// <summary>
//    /// 框架版本控制器 (显示版本信息)
//    /// </summary>
//    public class VersionController : EditorWindow
//    {
//        #region  Data 
//        private static VersionController m_VersionControllerWin;
//        private List<VersionInfor> m_FrameWorkVersion = new List<VersionInfor>();
//        private Vector2 m_ScrollRect = Vector2.zero;
//        private Vector2 m_SubScrollRect = Vector2.zero;
//        #endregion



//        [MenuItem("Tools/框架版本控制/显示版本信息")]
//        private static void ShowFrameWorkVersion()
//        {
//            m_VersionControllerWin = EditorWindow.GetWindow<VersionController>("版本控制");
//            m_VersionControllerWin.minSize = new Vector2(400, 400);
//            m_VersionControllerWin.maxSize = new Vector2(400, 600);
//            m_VersionControllerWin.Show();
//        }



//        /// <summary>
//        /// 获取本地的版本信息
//        /// </summary>
//        private void GetLocalVersionInfor()
//        {
//            TextAsset textAsset = Resources.Load<TextAsset>(EditorDefine.S_FrameWorkVersionLogPath);
//            if (textAsset == null)
//            {
//                Debug.LogError("GetLocalVersionInfor Fail,Path " + EditorDefine.S_FrameWorkVersionLogPath);
//                return;
//            }
//            FromJson(textAsset);
//        }

//        private void FromJson(TextAsset textAsset)
//        {
//            JsonData jsonDa = JsonMapper.ToObject(textAsset.text);
//            m_FrameWorkVersion.Clear();
//            if (jsonDa == null) return;
//            JsonData subVersionData = null;
//            for (int dex = 0; dex < jsonDa.Count; ++dex)
//            {
//                VersionInfor versionInfor = new VersionInfor();
//                versionInfor.m_MainVersion = int.Parse(jsonDa[dex]["m_MainVersion"].ToString());
//                versionInfor.m_OtherVersionInfor = jsonDa[dex]["m_OtherVersionInfor"].ToString();
//                 subVersionData = jsonDa[dex]["m_MainVersion"]["m_SubVersion"];

//                for (int subIndex=0;subIndex< subVersionData.Count;++subIndex)
//                {
//                    versionInfor.m_SubVersion.Add(int.Parse(subVersionData[subIndex].ToString()));
//                }
//                m_FrameWorkVersion.Add(versionInfor);
//            }
//        }


//        private void OnGUI()
//        {
//            EditorGUILayout.BeginVertical();
//            m_ScrollRect = GUILayout.BeginScrollView(m_ScrollRect, true, true);
//            for (int dex=0;dex< m_FrameWorkVersion.Count;++dex)
//            {
//                EditorGUILayout.BeginVertical();

//                EditorGUILayout.BeginHorizontal();
//                GUILayout.Label("主版本：", GUILayout.Width(150));
//                GUILayout.Label(m_FrameWorkVersion[dex].ToString(), GUILayout.ExpandWidth(true));
//                EditorGUILayout.EndHorizontal();



//                EditorGUILayout.BeginHorizontal();
//                GUILayout.Label("子版本：", GUILayout.Width(150));
//                GUILayout.Label("", GUILayout.Width(50));
//                for (int subindex=0;subindex< m_FrameWorkVersion[dex].m_SubVersion.Count;++subindex)
//                {
//                    GUILayout.Label(m_FrameWorkVersion[dex].m_SubVersion[subindex].ToString(), GUILayout.Width(50));
//                    GUILayout.Label(".", GUILayout.Width(10));
//                }
//                EditorGUILayout.EndHorizontal();


//                m_SubScrollRect = EditorGUILayout.BeginScrollView(m_SubScrollRect, false, false);
//                EditorGUILayout.BeginVertical();
//                GUILayout.Label(m_FrameWorkVersion[dex].m_OtherVersionInfor.ToString(), GUILayout.Width(350), GUILayout.ExpandHeight(true));
//                EditorGUILayout.EndVertical();

//                EditorGUILayout.EndVertical();
//            }

//            GUILayout.EndScrollView();
//            EditorGUILayout.EndVertical();
//        }

//    }
//}