using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    ///  框架版本控制器 (增加框架版本信息)
    /// </summary>
    public class AddFrameWorkVersion : EditorWindow
    {

        #region  Data 
        private static AddFrameWorkVersion m_AddFrameWorkVersionWin;
        private List<VersionInfor> m_FrameWorkVersion = new List<VersionInfor>();
        private VersionInfor m_LastPreviousVersion = null;
        private VersionInfor m_NewVersion = null;

        private Vector2 m_ScrollRect = Vector2.zero;
        private Vector2 m_SubScrollRect = Vector2.zero;


        private Rect m_PreviousVersionRect;
        private Rect m_CurVersionRect;
        private Rect m_VersionInforRect = new Rect(0, 0, 0, 0);
        #endregion

        #region 资源
        private Texture _GrayImage;
        /// <summary>
        /// 灰色背景图
        /// </summary>
        private Texture m_GrayImage
        {
            get
            {
                if (_GrayImage == null)
                    _GrayImage = Resources.Load<Texture>("EditorImage/Gray_Img");
                return _GrayImage;
            }
        }


        private int m_DefaultFontSize = 0;
        private Color m_DefaultFontColor;
        private GUISkin m_DefaultSkin;
        #endregion


        [MenuItem("Tools/框架版本控制/增加版本信息")]
        private static void ShowFrameWorkVersion()
        {
            m_AddFrameWorkVersionWin = EditorWindow.GetWindow<AddFrameWorkVersion>("增加版本控制");
            m_AddFrameWorkVersionWin.minSize = new Vector2(400, 600);
            m_AddFrameWorkVersionWin.Show();
        }



        /// <summary>
        /// 获取本地的版本信息
        /// </summary>
        private void GetLocalVersionInfor()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(EditorDefine.S_FrameWorkVersionLogPath);
            if (textAsset == null)
            {
                Debug.LogError("GetLocalVersionInfor Fail,Path " + EditorDefine.S_FrameWorkVersionLogPath);
                return;
            }
            FromJson(textAsset);
        }

        private void FromJson(TextAsset textAsset)
        {
            JsonData jsonDa = JsonMapper.ToObject(textAsset.text);
            m_FrameWorkVersion.Clear();
            m_LastPreviousVersion = null;
            if (jsonDa == null) return;
            JsonData subVersionData = null;
            for (int dex = 0; dex < jsonDa.Count; ++dex)
            {
                VersionInfor versionInfor = new VersionInfor();
                versionInfor.m_MainVersion = int.Parse(jsonDa[dex]["m_MainVersion"].ToString());
                versionInfor.m_OtherVersionInfor = jsonDa[dex]["m_OtherVersionInfor"].ToString();
                subVersionData = jsonDa[dex]["m_MainVersion"]["m_SubVersion"];

                for (int subIndex = 0; subIndex < subVersionData.Count; ++subIndex)
                {
                    versionInfor.m_SubVersion.Add(int.Parse(subVersionData[subIndex].ToString()));
                }
                m_FrameWorkVersion.Add(versionInfor);
            }

            if (m_FrameWorkVersion.Count != 0)
                m_LastPreviousVersion = m_FrameWorkVersion[0];

        }


        private void OnGUI()
        {
            #region  保存编辑器的设置
            m_DefaultSkin = GUI.skin;
            GUI.skin = EditorDefine.S_CustomerGUISkin;
            #endregion
   //           m_ScrollRect= EditorGUILayout.BeginScrollView(m_ScrollRect, false, true);

            EditorGUILayout.BeginVertical();
            #region 版本提示
            GUILayout.BeginArea(new Rect(0, 0, EditorGUIUtility.currentViewWidth, 20));
            if (m_LastPreviousVersion == null)
            {
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUILayout.Label("添加第一个主版本：", GUILayout.Width(200));
                m_LastPreviousVersion = new VersionInfor();
                List<int> subVersion = new List<int>();
                subVersion.Add(0);
                subVersion.Add(0);
                subVersion.Add(1);
                m_LastPreviousVersion.DefaultVersion(subVersion, 1, "第一个版本");
            }

            if (m_NewVersion == null)
                m_NewVersion = m_LastPreviousVersion;

            GUILayout.EndArea();
            #endregion


            //      m_ScrollRect = GUILayout.BeginScrollView(m_ScrollRect, false, true);

            #region     上一个版本信息

            m_PreviousVersionRect = new Rect(0, 20, EditorGUIUtility.currentViewWidth, 100);
            GUI.DrawTexture(m_PreviousVersionRect, m_GrayImage, ScaleMode.ScaleAndCrop);
            GUILayout.BeginArea(m_PreviousVersionRect, new GUIContent("上一个版本的信息"));

            #region 整个版本号
            GUILayout.BeginArea(new Rect(0, 20, EditorGUIUtility.currentViewWidth, 30));
            EditorGUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth));
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("上一个版本号：", GUILayout.Width(100));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(m_LastPreviousVersion.ToString(), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            #endregion


            #region 主版本

            GUILayout.BeginArea(new Rect(0, 50, EditorGUIUtility.currentViewWidth, 30));
            EditorGUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth));
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("上一个主版本：", GUILayout.Width(100));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(m_LastPreviousVersion.m_MainVersion.ToString(), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            #region    子版本

            GUILayout.BeginArea(new Rect(0, 80, EditorGUIUtility.currentViewWidth, 30));
            EditorGUILayout.BeginHorizontal();
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("子版本：", GUILayout.Width(100));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(m_LastPreviousVersion.GetSubVersion(), GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion


            GUILayout.EndArea();
            #endregion

            #region  要添加得版本

            #region 编辑新版本
            m_CurVersionRect = m_PreviousVersionRect.AddRect(new Rect(0, m_PreviousVersionRect.height + 50, 0, m_AddFrameWorkVersionWin.position.height - m_PreviousVersionRect.height - 400));
            GUI.DrawTexture(m_CurVersionRect, m_GrayImage, ScaleMode.ScaleAndCrop); //这里需要先绘制背景图在后在绘制区域
           GUILayout.BeginArea(m_CurVersionRect);
            EditorGUILayout.BeginVertical();

            #region  主版本
            EditorGUILayout.BeginHorizontal();
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("主版本：", GUILayout.Width(100));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            m_NewVersion.m_MainVersion = int.Parse(GUILayout.TextField(m_NewVersion.m_MainVersion.ToString(), GUILayout.Width(100)));
            EditorGUILayout.EndHorizontal();
            #endregion

            #region 子版本

            EditorGUILayout.BeginHorizontal();
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("子版本：", GUILayout.Width(100));

            for (int dex = 0; dex < m_NewVersion.m_SubVersion.Count; ++dex)
            {
                m_NewVersion.m_SubVersion[dex] = int.Parse(GUILayout.TextField(m_NewVersion.m_SubVersion[dex].ToString(), GUILayout.Width(50)));
            }

            EditorGUILayout.EndHorizontal();
            #endregion

            #region  版本描述
            m_VersionInforRect = m_CurVersionRect.ReduceRect(0, -300, 0, 800);
       
            m_SubScrollRect = GUILayout.BeginScrollView(m_SubScrollRect, false, true);
            m_NewVersion.m_OtherVersionInfor = GUILayout.TextArea(m_NewVersion.m_OtherVersionInfor.ToString(), GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(m_VersionInforRect.height - 20));
            GUILayout.EndScrollView();

            #endregion

            #endregion

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("添加新版本")))
            {
              //  AddGameFramWorkVersion();
            }

            EditorGUILayout.EndVertical();
         GUILayout.EndArea();
            #endregion


            EditorGUILayout.EndVertical();
      //         EditorGUILayout.EndScrollView();

            #region  恢复编辑器的颜色等设置
            //GUI.skin.label.fontSize = m_DefaultFontSize;
            //GUI.color = m_DefaultFontColor;
            GUI.skin = EditorDefine.S_CustomerGUISkin;
            #endregion

        }


        private void AddGameFramWorkVersion()
        {
            for (int dex = 0; dex < m_FrameWorkVersion.Count; ++dex)
            {
                if (m_FrameWorkVersion[dex].IsEqual(m_NewVersion))
                {
                    Debug.LogError("已经包含了相同版本号的版本 " + m_FrameWorkVersion[dex].ToString());
                    return;
                }
            }
            m_FrameWorkVersion.Add(m_NewVersion);
            string data = JsonMapper.ToJson(m_FrameWorkVersion);
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            if (System.IO.Directory.Exists(EditorDefine.S_FrameWorkVersionLogPath))
                System.IO.Directory.CreateDirectory(EditorDefine.S_FrameWorkVersionLogPath);

            if (System.IO.File.Exists(EditorDefine.S_FrameWorkVersionLogPath + ".txt"))
                System.IO.File.Create(EditorDefine.S_FrameWorkVersionLogPath + ".txt");

            FileStream stream = new FileStream(EditorDefine.S_FrameWorkVersionLogPath + ".txt", FileMode.Truncate, FileAccess.ReadWrite);
            stream.Write(byteData, 0, byteData.Length);
            stream.Close();
            AssetDatabase.Refresh();
            //System.IO.File.WriteAllText(EditorDefine.S_FrameWorkVersionLogPath + ".txt", data,System.Text.Encoding.UTF8,)

        }

    }
}