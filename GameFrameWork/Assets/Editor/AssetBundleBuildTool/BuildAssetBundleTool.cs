using GameFrameWork.HotUpdate;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 打包Resource 目录下BuildAssetBundlePath 目录中的资源(如果不存在需要创建一个目录)
    /// </summary>
    public class BuildAssetBundleTool 
    {
        private static BuildTarget S_CurrentBuildTarget = BuildTarget.StandaloneWindows64;  //打包的资源平台
        private static string S_AssetBundleOutPath { get { return ConstDefine.S_StreamingAssetPath +"/"+ AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(S_CurrentBuildTarget); } }  //AssetBundle 输出目录
        /// <summary>
        ///需要打包的AssetBundle 资源  Key :文件相对resource路径，value所属的AssetBundleName
        /// </summary>
        private static Dictionary<string, string> S_AllFileNeedBuildAssetBundleRecord = new Dictionary<string, string>();
        private static HotAssetBaseRecordInfor S_HotAssetBaseRecordInfor = new HotAssetBaseRecordInfor();  //记录打包后的资源 AssetBundle 信息

        private static BuildAssetBundleWindows S_BuildAssetBundleWindows;  //打包资源的自定义窗口


        public static void BegingPackAssetBundle(BuildTarget target, BuildAssetBundleWindows editorWin)
        {
            S_CurrentBuildTarget = target;
            S_BuildAssetBundleWindows = editorWin;
            PackAssetBundle();
        }

        /// <summary>
        /// 开始打包AssetBundle
        /// </summary>
        private static void PackAssetBundle()
        {
            S_HotAssetBaseRecordInfor.AllAssetRecordsDic.Clear();  //清理本地记录的数据
            S_AllFileNeedBuildAssetBundleRecord.Clear();
            ClearAllPreviousAssetBundleName();
            GetAndSetNeedPackAssetName(ConstDefine.S_ResourcesPath);
            CallAPIBuildAssetBundle();  //生成AssetBundle
            AssetDatabase.Refresh(); 

            //***将生成的AssetBundle  移动到Applicaiton.persistentDataPath 中
            CopyAndMoveAssetBundleAsset();
            CreateAssetBundleDepends(); //创建依赖关系字典
            SaveAllDepdenceToLocalFile();

            AssetDatabase.Refresh();
            S_BuildAssetBundleWindows.OnCompleteBuildAssetBundle();
        }


        /// <summary>
        /// 清理所有资源的AssetBundle 名,避免打包不必要的资源
        /// </summary>
        private static void ClearAllPreviousAssetBundleName()
        {
            string[] previousAssetName = AssetDatabase.GetAllAssetBundleNames();  //获得当前所有设置AssetBundle 名
            if (previousAssetName == null || previousAssetName.Length == 0) return;
            // Debug.Log("Before .. " + previousAssetName.Length);
            for (int dex = 0; dex < previousAssetName.Length; ++dex)
                AssetDatabase.RemoveAssetBundleName(previousAssetName[dex], true);

            //Debug.Log("End  .. " + AssetDatabase.GetAllAssetBundleNames().Length);
        }

        #region  获取需要打包的文件和目录
        /// <summary>
        /// 获取Resources 下所有的文件夹和文件 并且检测是否需要打包
        /// </summary>
        /// <param name="resourcePath"></param>
        private static void GetAndSetNeedPackAssetName(string resourcePath)
        {
            string[] directorys = System.IO.Directory.GetDirectories(resourcePath, "*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directorys)
            {
                if (S_BuildAssetBundleWindows.CheckIfNeedPacked(directory))
                    SearchAllSubDirectorys(directory);
            }

            string[] files = System.IO.Directory.GetFiles(resourcePath, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (S_BuildAssetBundleWindows.CheckIfNeedPacked(file))
                    SetAssetBundleNameByPath(file);
            }
        }

        /// <summary>
        /// 获得所有的文件并设置AssetBundle 名
        /// </summary>
        private static void SearchAllSubDirectorys(string resourcePath)
        {
            //Debug.Log("GetAllFileAndSetAssetBundleName Path:  " + resourcePath);
            string[] _SubDire = Directory.GetDirectories(resourcePath); //获得所有的子文件夹
            foreach (var item in _SubDire)
                SearchAllSubDirectorys(item);

            string[] _ContainFiles = Directory.GetFiles(resourcePath);  //获得所有的文件
            foreach (var file in _ContainFiles)
            {
                SetAssetBundleNameByPath(file);
            }
        }

        #endregion

        /// <summary>
        /// 根据文件路径设置AssetBundle Name  
        /// 规则： 跳过 (.meta) ,(.prefab/.unity  单独打包)  其他资源按照文件所在的文件夹打包
        /// </summary>
        /// <param name="filePath"></param>
        private static void SetAssetBundleNameByPath(string filePath)
        {
            if (Path.GetExtension(filePath) == ".meta")
            {
                // Debug.Log(".meta 文件跳过 ");
                return;
            }

            string fileAssetPath = filePath.Substring(filePath.IndexOf(string.Format("{0}/", ConstDefine.S_AssetName))); //相对于Assets目录 方便AssetImporter 使用
            int index = fileAssetPath.IndexOf(string.Format("{0}/", ConstDefine.S_ResourcesName));
            string filePathRelativeResource = fileAssetPath.Substring(index+ ConstDefine.S_ResourcesName.Length+1);//相对于Resource的路径
            //相对于Resource路径下不带扩展名的文件名
            string filePathRelativeResourceWithoutExtension = filePathRelativeResource.Substring(0, filePathRelativeResource.IndexOf(Path.GetExtension(filePathRelativeResource)));
  //          Debug.Log("fileAssetPath=" + fileAssetPath + "   filePathRelativeResource=" + filePathRelativeResource);
  //          Debug.Log("filePathRelativeResourceWithNoExtension=" + filePathRelativeResourceWithoutExtension);

            string assetName = "";
            string extensionName = Path.GetExtension(filePath);
            if (extensionName == ".prefab" || extensionName == ".unity" )//|| extensionName == ".asset")
            { //Prefab 和 Scene 文件单独打包成AssetBundle
                assetName = filePathRelativeResourceWithoutExtension.Replace(@"\", "/") + ConstDefine.AssetBundleExtensionName;
            }
            else
            {  //其他文件按照目录设置文件名 打包到一个AssetBundle
                string fileDirec = Path.GetDirectoryName(filePathRelativeResourceWithoutExtension);  //当前文件的目录
                string _fileName = Path.GetFileName(fileDirec); //获得当前目录的名字
                ///以目录+"/"+最后一个目录 为AssetBundleName
                assetName = string.Format("{0}/{1}{2}", fileDirec, _fileName, ConstDefine.AssetBundleExtensionName).Replace(@"\", "/");
                //                 (fileDirec + "/" + _fileName).Replace(@"\", "/") + ConstDefine.AssetBundleExtensionName; 
            }
   //         Debug.Log("当前文件夹路径是 " + filePath + " 当前文件AssetName= " + assetName);

            //AssetImporter.GetAtPath()  获取 Assets目录下资源必须带后缀名
            AssetImporter _impoter = AssetImporter.GetAtPath(fileAssetPath);   //**** Assets/Resources/AssetBundle_Path/Obj/obj1.prefab
            _impoter.assetBundleName = assetName.ToLower();  //所有的资源AssetBundleName在加载时候会被识别成小写

            string fileAssetRecordPath = filePathRelativeResource.Replace(@"\", "/");
            string fileAssetBundleRecordName = assetName.ToLower();
            if (S_AllFileNeedBuildAssetBundleRecord.ContainsKey(fileAssetRecordPath))
            {
                Debug.LogError("重复的资源路径" + filePathRelativeResource);
                return;
            }
            S_AllFileNeedBuildAssetBundleRecord.Add(fileAssetRecordPath, fileAssetBundleRecordName); //记录资源路径到AssetBundle路径
        }

        /// <summary>
        /// 调用API 生成AssetBundle 资源
        /// </summary>
        private static void CallAPIBuildAssetBundle()
        {
            CleanAssetBundleOutputPath();
            BuildPipeline.BuildAssetBundles(S_AssetBundleOutPath, BuildAssetBundleOptions.DeterministicAssetBundle, S_CurrentBuildTarget);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 清理输出目录 (循环删除包含的子目录)
        /// </summary>
        private static void CleanAssetBundleOutputPath()
        {
            Debug.LogInfor("CleanAssetBundleOutputPath >>>" + S_AssetBundleOutPath);
            if (Directory.Exists(S_AssetBundleOutPath))
                Directory.Delete(S_AssetBundleOutPath, true);

            Directory.CreateDirectory(S_AssetBundleOutPath);
        }

        private  static void CopyAndMoveAssetBundleAsset()
        {
            IoUtility.Instance.ForceCopyDirectoryFile(ConstDefine.S_StreamingAssetPath, ConstDefine.S_AssetBundleTopPath);
        }


        /// <summary>
        /// 创建依赖关系
        /// </summary>
        private static void CreateAssetBundleDepends()
        {
            RecordPackAbundleMainifestInfor(AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(S_CurrentBuildTarget));  //不同平台的打包主AssetBundle
            RecordPackAbundleMainifestInfor(AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(S_CurrentBuildTarget) + ".manifest");  // AssetBundleManifest

            #region 遍历记录所有的AssetBundle
            string[] allAssets = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllAssetBundles();//获得所有的AssetBundleName
            foreach (var item in allAssets)
            {
                string[] depences = AssetBundleMgr.Instance.S_AssetBundleManifest.GetAllDependencies(item); //获得当前资源的所有依赖关系
                string path = S_AssetBundleOutPath + "/" + item;  //获取打包后的资源AssetBundle 绝对路径

                #region 记录当前AssetBundle 信息
                AssetBundleRecordeInfor _infor = new AssetBundleRecordeInfor();
                _infor.m_MD5Code = MD5Helper.GetFileMD5(path);
                System.IO.FileInfo fileInfor = new System.IO.FileInfo(path);
                _infor.m_ByteSize = (int)fileInfor.Length;
      //          _infor.m_DependeceAssetNamePath.AddRange(depences);
                if (S_HotAssetBaseRecordInfor.AllAssetRecordsDic.ContainsKey(item))
                {
                    Debug.LogError("重复的AssetBundleName=" + item);
                    break;
                }
                S_HotAssetBaseRecordInfor.AllAssetRecordsDic.Add(item, _infor);  //记录当前的 AssetBundle 资源
                #endregion

                #region 记录当前文件 .meta信息
                AssetBundleRecordeInfor _metaInfor = new AssetBundleRecordeInfor();
                _metaInfor.m_MD5Code = MD5Helper.GetFileMD5(path + ".meta");
                FileInfo mataFileInfor = new System.IO.FileInfo(path + ".meta");
                _metaInfor.m_ByteSize = (int)mataFileInfor.Length;
                if (S_HotAssetBaseRecordInfor.AllAssetRecordsDic.ContainsKey(item + ".meta"))
                {
                    Debug.LogError("重复的AssetBundleName=" + item + ".meta");
                    break;
                }
                S_HotAssetBaseRecordInfor.AllAssetRecordsDic.Add(item + ".meta", _metaInfor);  //记录当前的 AssetBundle 资源
                #endregion

            }

            AssetBundleMgr.Instance.m_MainAssetBundle.Unload(true);  //避免打包后编辑器加载报错
            #endregion
        }

        /// <summary>
        /// 记录不同平台打包下生成的主 mainAssetBundle 和 mainFest 信息
        /// </summary>
        /// <param name="fileName"></param>
        private static void RecordPackAbundleMainifestInfor(string fileName)
        {
            string PlatformABundlePath = S_AssetBundleOutPath + "/" + fileName;
            //if(System.IO.File.Exists(PlatformABundlePath))
            //    System.IO.File.Create() 
            //当AssetBundle
            AssetBundleRecordeInfor _infor = new AssetBundleRecordeInfor();
            System.IO.FileInfo fileInfor = new System.IO.FileInfo(PlatformABundlePath);
            _infor.m_ByteSize = (int)fileInfor.Length;
            _infor.m_MD5Code = MD5Helper.GetFileMD5(PlatformABundlePath);
            S_HotAssetBaseRecordInfor.AllAssetRecordsDic.Add(fileName, _infor);


            //.meta
            AssetBundleRecordeInfor _metaInfor = new AssetBundleRecordeInfor();
            System.IO.FileInfo metaFileInfor = new System.IO.FileInfo(PlatformABundlePath + ".meta");
            _metaInfor.m_ByteSize = (int)metaFileInfor.Length;
            _metaInfor.m_MD5Code = MD5Helper.GetFileMD5(PlatformABundlePath + ".meta");
            S_HotAssetBaseRecordInfor.AllAssetRecordsDic.Add(fileName + ".meta", _metaInfor);
        }


        /// <summary>
        /// 保存到本地
        /// </summary>
        private static void SaveAllDepdenceToLocalFile()
        {
            string msg = LitJson.JsonMapper.ToJson(S_HotAssetBaseRecordInfor);
            string configRecordPath = string.Format("{0}{1}", ConstDefine.S_AssetBundleTopPath,
                AssetBundleMgr.Instance.GetHotAssetBuildPlatformName(S_CurrentBuildTarget)+ ConstDefine.S_AssetBundleBuildRecordConfigureName);

            string directoryPath = System.IO.Path.GetDirectoryName(configRecordPath);
            if (System.IO.Directory.Exists(directoryPath) == false)
                System.IO.Directory.CreateDirectory(directoryPath);

            if (System.IO.File.Exists(configRecordPath))
            {
                System.IO.File.Delete(configRecordPath);
            }
            System.IO.File.WriteAllText(configRecordPath, msg);
            Debug.LogInfor("SaveAllDepdenceToLocalFile " + configRecordPath);
        }

    }
}