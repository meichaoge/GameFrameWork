using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{


    /// 选择的语言资源信息配置 (移动其他语言的资源后需要记录保留的资源的目录)
    /// </summary>
    public class MoveOutResourceaRecord : ScriptableObject
    {
        public List<MoveOutResourcesInfor> m_AllWillMoveInAssetPath = new List<MoveOutResourcesInfor>();

        public void AddRecord(MoveOutResourcesInfor infor)
        {
            for (int dex = 0; dex < m_AllWillMoveInAssetPath.Count; ++dex)
            {
                if (m_AllWillMoveInAssetPath[dex].AssetDestinationTopDirectoryPath == infor.AssetDestinationTopDirectoryPath &&
                    m_AllWillMoveInAssetPath[dex].AssetSourceTopDirectoryPath == infor.AssetSourceTopDirectoryPath)
                {
                    Debug.LogError("AddRecord Fail,Allready Record " + infor);
                    return;

                }
            }


            m_AllWillMoveInAssetPath.Add(infor);
        }

    }


    /// <summary>
    /// 单个移动信息记录
    /// </summary>
    [System.Serializable]
    public class MoveOutResourcesInfor
    {
        /// <summary>
        /// 资源的原始目录
        /// </summary>
        public string AssetSourceTopDirectoryPath;
        /// <summary>
        /// 资源移动到哪个目录
        /// </summary>
        public string AssetDestinationTopDirectoryPath;

        public MoveOutResourcesInfor(string fromPath, string destinationPath)
        {
            AssetSourceTopDirectoryPath = fromPath;
            AssetDestinationTopDirectoryPath = destinationPath;
        }

        public override string ToString()
        {
            return string.Format("From ={0} To ={1}", AssetSourceTopDirectoryPath, AssetDestinationTopDirectoryPath);
        }
    }
}