using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 打包程序控制系统
    /// </summary>
    public class BuildTool : Singleton_Static<BuildTool>
    {
        private bool m_IsIgnoreLanguage = false;
        /// <summary>
        /// 是否忽略由于不同语言带来的资源目录需要区分的问题 主要用在多语言版本中
        /// </summary>
        public bool IsIgnoreLanguage { get { return m_IsIgnoreLanguage; } }

        private static MutiLanguageResourcesInfor _MutiLanguageResourcesInfor = null;
        /// <summary>
        /// 记录需要移除的多语言资源
        /// </summary>
        public static MutiLanguageResourcesInfor S_MutiLanguageResourcesInfor
        {
            get
            {
                if(_MutiLanguageResourcesInfor==null)
                    _MutiLanguageResourcesInfor= AssetDatabase.LoadAssetAtPath<MutiLanguageResourcesInfor>(EditorDefine.S_BuildAppMultLanguageAssetPath);
                return _MutiLanguageResourcesInfor;
            }
        }

        private static MoveOutResourceaRecord _MoveOutResourceaRecord = null;
        /// <summary>
        /// 记录需要移动回来的多语言资源
        /// </summary>
        public static MoveOutResourceaRecord S_MoveOutResourceaRecordInfor
        {
            get
            {
                if(_MoveOutResourceaRecord==null)
                    _MoveOutResourceaRecord= AssetDatabase.LoadAssetAtPath<MoveOutResourceaRecord>(EditorDefine.S_MoveOutMultLanguageAssetPath);
                return _MoveOutResourceaRecord;
            }
        }





        /// <summary>
        /// 将所有的非参数语言的多语言资源移除到其他目录不打包进去
        /// </summary>
        /// <param name="selectLanguage"></param>
        public static void MoveOutUnUseResources(Language selectLanguage)
        {
            GetMulitLanguageResourceConfig(selectLanguage);
        }

        /// <summary>
        /// 获取配置多语言资源对象
        /// </summary>
        private static void GetMulitLanguageResourceConfig(Language selectLanguage)
        {
            if (S_MutiLanguageResourcesInfor == null)
            {
                Debug.LogError("GetMulitLanguageResourceConfig Fail  S_MutiLanguageResourcesInfor is Null");
                return;
            }

            if (S_MoveOutResourceaRecordInfor == null)
            {
                Debug.LogError("GetMulitLanguageResourceConfig Fail S_MoveOutResourceaRecordInfor is Null");
                return;
            }

            Debug.LogEditorInfor(EditorDefine.S_BuildAppMultLanguageTempStorePath);
            if (System.IO.Directory.Exists(EditorDefine.S_BuildAppMultLanguageTempStorePath) == false)
                System.IO.Directory.CreateDirectory(EditorDefine.S_BuildAppMultLanguageTempStorePath);

            IoUtility.Instance.GetDirectoryParentDcirectory(EditorDefine.S_BuildAppMultLanguageTempStorePath);

            foreach (var item in S_MutiLanguageResourcesInfor.AllWillMoveOutResources)
            {
                TryToMoveMultLanguageResources(item, selectLanguage);
            }
        }

        /// <summary>
        /// 根据记录的项 根据不同情况合理解析出需要移动出去的目录
        /// </summary>
        /// <param name="item"></param>
        /// <param name="selectLanguage"></param>
        /// <returns></returns>
        private static void TryToMoveMultLanguageResources(MutiLanguageResourceItem item, Language selectLanguage)
        {
            string realPath = string.Empty;
            if (item.IsIgnoreLanguage)
            {
                realPath = string.Format("{0}/{1}", Application.dataPath, item.AssetRelativePath);
                if (TryMoveDirectory(realPath, item.AssetRelativePath,string.Empty))
                    AssetDatabase.Refresh();
                return;
            } //移动不需要考虑不同语言版本的资源

            var languages = System.Enum.GetValues(typeof(Language));
            foreach (var langua in languages)
            {
                Language languageEnum = (Language)System.Enum.Parse(typeof(Language), langua.ToString());
                if (languageEnum == selectLanguage)
                    continue;

                realPath = string.Format("{0}/{1}/{2}", Application.dataPath, item.AssetRelativePath, langua.ToString()); //包含一个语言目录
                if (TryMoveDirectory(realPath, item.AssetRelativePath, languageEnum.ToString()) == false)
                    continue;
                AssetDatabase.Refresh();
            } //移动需要考虑不同语言版本的资源 自动加入一个语言目录
        }

        /// <summary>
        /// 根据目录移动目录中所有的资源
        /// </summary>
        /// <param name="realPath"></param>
        /// <param name="relativePath"></param>
        /// <param name="languageDirectory">当不需要增加多语言目录时候为null 即可</param>
        /// <returns></returns>
        private static bool TryMoveDirectory(string realPath, string relativePath,string languageDirectory)
        {
            if (string.IsNullOrEmpty(languageDirectory) == false)
                relativePath = string.Format("{0}/{1}", relativePath, languageDirectory); //增加一个语言目录

            if (System.IO.Directory.Exists(realPath) == false)
            {
                Debug.LogError("GetMulitLanguageResourceConfig Fail ,path Not Exit " + relativePath);
                return false;
            }

        //    string parentDirectory = IoUtility.Instance.GetDirectoryParentDcirectory(realPath);  //获取参数路径的父级目录
            string storePath = string.Format("{0}/{1}", EditorDefine.S_BuildAppMultLanguageTempStorePath, relativePath);

            MoveOutResourcesInfor infor = new MoveOutResourcesInfor(realPath, storePath);
            S_MoveOutResourceaRecordInfor.AddRecord(infor); //记录数据
            IoUtility.Instance.ForceMoveDirectoryFile(realPath, storePath);
            return true;
        }




        /// <summary>
        /// 将移除出去的资源重新导入进来
        /// </summary>
        public static void MoveInAllMoveOutResources()
        {
            if (S_MoveOutResourceaRecordInfor == null)
            {
                Debug.LogError("MoveInAllMoveOutResources Fail S_MoveOutResourceaRecordInfor is Null");
                return;
            }

            try
            {
                foreach (var item in S_MoveOutResourceaRecordInfor.m_AllWillMoveInAssetPath)
                {
                    IoUtility.Instance.ForceMoveDirectoryFile(item.AssetDestinationTopDirectoryPath, item.AssetSourceTopDirectoryPath);
                    AssetDatabase.Refresh();
                }

                if (System.IO.Directory.Exists(EditorDefine.S_BuildAppMultLanguageTempStorePath))
                    Directory.Delete(EditorDefine.S_BuildAppMultLanguageTempStorePath, true);  //删除无用的空目录
                S_MoveOutResourceaRecordInfor.m_AllWillMoveInAssetPath.Clear();  //清空数据记录
            }
            catch (System.Exception ex)
            {
                Debug.LogError("MoveInAllMoveOutResources " + ex.ToString());
            }
           
        }


    }
}