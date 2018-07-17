using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace GameFrameWork
{
    /// <summary>
    /// 应用程序版本中心 (编辑器下也会有框架信息)
    /// </summary>
    [System.Serializable]
    public class VersionInfor
    {
        public int m_MainVersion = 0;  //主版本
        public List<int> m_SubVersion = new List<int>();  //子版本
        public string m_OtherVersionInfor = ""; //版本附加信息 比如日期等

        public static string S_MainSplitChar = "_";//主与子版本之间的分隔符
        public static string S_SplitChar = ".";//子版本之间的分隔符

        public string m_Version { get { return ToString(); } }


        public void DefaultVersion(List<int> subVersion, int mainVersion = 1, string otherVersionInfor = "")
        {
            m_MainVersion = mainVersion;
            m_OtherVersionInfor = otherVersionInfor;
            m_SubVersion.Clear();
            m_SubVersion.AddRange(subVersion);
        }



        public bool IsEqual(VersionInfor infor)
        {
            if (infor == null) return false;

            if (m_MainVersion != infor.m_MainVersion) return false;

            if (infor.m_SubVersion.Count != m_SubVersion.Count) return false;

            for (int dex = 0; dex < m_SubVersion.Count; ++dex)
            {
                if (m_SubVersion[dex] != infor.m_SubVersion[dex]) return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(m_MainVersion.ToString());
            builder.Append(S_MainSplitChar);
            for (int dex = 0; dex < m_SubVersion.Count; ++dex)
            {
                builder.Append(m_SubVersion[dex]);
                if (dex != m_SubVersion.Count - 1)
                    builder.Append(S_SplitChar);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取子版本号
        /// </summary>
        /// <returns></returns>
        public string GetSubVersion()
        {
            StringBuilder builder = new StringBuilder();
            for (int subindex = 0; subindex < m_SubVersion.Count; ++subindex)
            {
                builder.Append(m_SubVersion[subindex].ToString());
                if (subindex != m_SubVersion.Count - 1)
                    builder.Append(S_SplitChar);
            }
            return builder.ToString();

        }
    }
}