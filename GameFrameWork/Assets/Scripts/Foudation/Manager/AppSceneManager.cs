using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFrameWork
{
    /// <summary>
    /// 场景加载模式
    /// </summary>
    public enum LoadSceneModeEnum
    {
        /// <summary>
        /// 释放所有需要释放的场景
        /// </summary>
        ReleasAll,  //释放所有的场景
        /// <summary>
        ///加载新场景前先卸载自己 
        /// </summary>
        ReleaseSelf,  
        /// <summary>
        /// 只是加载新场景 不管其他加载的场景
        /// </summary>
        KeepPrevious,  
    }

    /// <summary>
    /// 场景管理器
    /// </summary>
    public class AppSceneManager : Singleton_Static<AppSceneManager>
    {
        private List<Scene> m_CurLoadedScene = new List<Scene>(); //所有加载的场景  最上面的是最后加载的
        private static bool S_IsInitialed = false;
        private static HashSet<string> S_AllNeedUnLoadScene = new HashSet<string>();  //所以需要异步卸载的场景
        private static System.Action S_OnUnLoadSceneCompleteAct = null;
        /// <summary>
        /// 场景文件相对于Resoureces 的目录
        /// </summary>
        private const string S_SceneDirectory = "Scene/";  
        private static HashSet<SceneNameEnum> S_AllIgnoreUnLoadScene = new HashSet<SceneNameEnum>(new SceneNameEnum[] { SceneNameEnum.ApplicationEntry });  //所有忽略需要卸载的场景名

        /// <summary>
        /// 标识是否是在卸载场景的状态  如果是 则不能加载其他的场景 否则容易异常
        /// </summary>
        public bool IsUnLoadingScene { private set; get; }


        #region  初始化

        /// <summary>
        /// 初始化场景管理器
        /// </summary>
        public void OnApplicationStart()
        {
            if (S_IsInitialed)
                return;
            S_IsInitialed = true;
            IsUnLoadingScene = false;
            UnLoadAllUnUseScene();
            Scene current = SceneManager.GetActiveScene();
            if (m_CurLoadedScene.Contains(current) == false)
                m_CurLoadedScene.Add( current);
        }


        /// <summary>
        /// 检测是否已经加载了这个场景  (如果已经加载返回索引 否则返回-1)
        /// </summary>
        /// <param name="scenaName"></param>
        /// <returns></returns>
        private int CheckIfSceneLoaded(string scenaName)
        {
            for (int dex = 0; dex < m_CurLoadedScene.Count; ++dex)
            {
                if (m_CurLoadedScene[dex].name == scenaName)
                {
                    return dex;
                }
            }
            return -1;
        }

        #endregion


        #region 初始化/异步卸载场景

        private void UnLoadAllUnUseScene()
        {
            foreach (var item in m_CurLoadedScene)
            {
                Debug.LogError("应该不会进入到这里了 查一查这里");
                if (S_AllIgnoreUnLoadScene.Contains(SceneNameHelper.GetSceneName( item.name)) == false)
                {
                    S_AllNeedUnLoadScene.Add(item.name);

                    SceneManager.UnloadSceneAsync(item);
                }  //卸载已经加载的场景
            }

            if (S_AllNeedUnLoadScene.Count > 0)
                SceneManager.sceneUnloaded += OnUnLoadSceneComplete;
            else
                OnCompleteUnLoadScene();
        }

        /// <summary>
        /// 没有需要卸载的场景或者已经完成资源卸载
        /// </summary>
        private void OnCompleteUnLoadScene()
        {
            if (S_AllNeedUnLoadScene.Count != 0)
                return;
            if (S_OnUnLoadSceneCompleteAct != null)
                S_OnUnLoadSceneCompleteAct();
        }

        /// <summary>
        /// 场景异步卸载完成回调
        /// </summary>
        /// <param name="unloadScene"></param>
        private void OnUnLoadSceneComplete(Scene unloadScene)
        {
            S_AllNeedUnLoadScene.Remove(unloadScene.name);
            OnCompleteUnLoadScene();
        }
        #endregion


        #region  加载场景资源
       
        /// <summary>
        /// 加载场景资源 (优先加载AssetBundle 资源) （不要直接使用字符串场景名）
        /// </summary>
        /// <param name="sceneNameEnum"></param>
        /// <param name="isSingleLoad">加载场景模式 (true 标识加载完成删除其他的场景资源  否则 保留其他的场景资源 )</param>
        /// <param name="callback">=true 标识场景加载成功 否则失败</param>
        public void LoadScene(SceneNameEnum sceneNameEnum, LoadSceneModeEnum loadMode,  System.Action<bool> callback, 
            List<SceneNameEnum> ignoreUnLoadScene,System.Action onCompleteUnLoadSceneAct=null)
        {
            if(IsUnLoadingScene)
            {
                Debug.LogError("卸载场景时候加载会有异常 请先确认判断有没有完成资源卸载 然后处理加载逻辑");
                return;
            }

            string sceneName = sceneNameEnum.ToString();
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("加载场景失败 ，未知的场景资源名称" + sceneName);
                if (callback != null)
                    callback(false);
                return;
            }

            int index = CheckIfSceneLoaded(sceneName);
            if (index != -1)
            {
                Debug.LogError("The Scene Is Already Loaded " + sceneName);
                Scene scene = m_CurLoadedScene[index];
                m_CurLoadedScene.RemoveAt(index);
                m_CurLoadedScene.Add(scene); //更新当前加载的场景到最后面 

                if (callback != null)
                    callback(true);
                return;
            }

            DealWithLoadMode(loadMode, ignoreUnLoadScene, ()=> {
                if (onCompleteUnLoadSceneAct != null)
                    onCompleteUnLoadSceneAct.Invoke();

                ResourcesLoader.SceneLoader.LoadScene(S_SceneDirectory + sceneName, LoadAssetModel.Async, (loader) =>
                {
                    if (loader == null || loader.IsCompleted == false || loader.IsError)
                    {
                        Debug.LogError("场景资源加载失败");
                        if (callback != null)
                            callback(false);
                        return;
                    }
                    EventCenter.Instance.StartCoroutine(InstantiateScene(sceneName, LoadSceneMode.Additive, callback));
                });
            });  //先卸载资源然后 再加载新的场景

         
        }





        /// <summary>
        /// 加载场景资源 (优先加载AssetBundle 资源) （不要直接使用字符串场景名）
        /// </summary>
        /// <param name="sceneNameEnum"></param>
        /// <param name="isSingleLoad">加载场景模式 (true 标识加载完成删除其他的场景资源  否则 保留其他的场景资源 )</param>
        /// <param name="callback"></param>
        public void LoadScene(SceneNameEnum sceneNameEnum, LoadSceneModeEnum loadMode, System.Action<bool> callback,  System.Action onCompleteUnLoadSceneAct=null)
        {
            LoadScene(sceneNameEnum, loadMode,callback,new List<SceneNameEnum>(), onCompleteUnLoadSceneAct);
        }

        #region  卸载其他场景
        /// <summary>
        /// 加载完成时候 卸载其他的场景资源
        /// </summary>
        /// <param name="loadMode"></param>
        /// <param name="ignoreUnLoadScene"></param>
        private void DealWithLoadMode(LoadSceneModeEnum loadMode, List<SceneNameEnum> ignoreUnLoadScene, System.Action onCompleteUnLoadAct)
        {
            if (ignoreUnLoadScene == null)
                ignoreUnLoadScene = new List<SceneNameEnum>();
            List<Scene> allNeedUnLoadScene = new List<Scene>();  //需要卸载的场景
            List<int> allNeedRemoveSceneIndex = new List<int>();
            SceneNameEnum sceneName = SceneNameEnum.ApplicationEntry;
            #region  根据不同的加载模式获取需要卸载的场景列表

            switch (loadMode)
            {
                case LoadSceneModeEnum.ReleasAll:
                    for (int dex = 0; dex < m_CurLoadedScene.Count; ++dex)
                    {
                        sceneName = (SceneNameEnum)System.Enum.Parse(typeof(SceneNameEnum), m_CurLoadedScene[dex].name);
                        if (ignoreUnLoadScene.Contains(sceneName) == false&& S_AllIgnoreUnLoadScene.Contains(sceneName)==false)
                        {
                            allNeedRemoveSceneIndex.Add(dex);
                            allNeedUnLoadScene.Add(m_CurLoadedScene[dex]);
                        }
                    }
                    break;
                case LoadSceneModeEnum.ReleaseSelf:
                    if (m_CurLoadedScene.Count > 0)
                    {
                        Debug.Log("m_CurLoadedScene  " + m_CurLoadedScene.Count);
                        sceneName = (SceneNameEnum)System.Enum.Parse(typeof(SceneNameEnum), m_CurLoadedScene[m_CurLoadedScene.Count - 1].name);
                        if (ignoreUnLoadScene.Contains(sceneName) == false && S_AllIgnoreUnLoadScene.Contains(sceneName) == false)
                        {
                            allNeedRemoveSceneIndex.Add(m_CurLoadedScene.Count - 1);
                            allNeedUnLoadScene.Add(m_CurLoadedScene[m_CurLoadedScene.Count - 1]);
                        }
                    }
                    break;
                case LoadSceneModeEnum.KeepPrevious:
                    break;
                default:
                    Debug.LogEditorInfor("没有定义的加载场景类型  " + loadMode);
                    break;
            }
            #endregion

            allNeedRemoveSceneIndex.Reverse();
           for (int dex=0;dex< allNeedRemoveSceneIndex.Count;++dex)
           {
                m_CurLoadedScene.RemoveAt(allNeedRemoveSceneIndex[dex]);
           }

            EventCenter.Instance.StartCoroutine(UnLoadScenes(allNeedUnLoadScene, onCompleteUnLoadAct));
        }

       /// <summary>
       /// 卸载资源 
       /// </summary>
       /// <param name="allNeedUnLoadScene"></param>
       /// <param name="onCompleteUnLoadAct"></param>
       /// <returns></returns>
        private IEnumerator UnLoadScenes(List<Scene> allNeedUnLoadScene, System.Action onCompleteUnLoadAct)
        {
            IsUnLoadingScene = true;
            if (allNeedUnLoadScene.Count>0)
            {
                //for (int dex = 0; dex < allNeedUnLoadScene.Count; ++dex)
                //{
                //}  //先处理加载器释放资源 避免由于这里执行需要时间导致其他地方判断有问题

                for (int dex = 0; dex < allNeedUnLoadScene.Count; ++dex)
                {
                    ResourcesLoader.SceneLoader.UnLoadScene(S_SceneDirectory + allNeedUnLoadScene[dex].name);
                    yield return SceneManager.UnloadSceneAsync(allNeedUnLoadScene[dex].name); //注意这里如果加载的是AssetBundle  资源场景 则获得的场景索引(sceneBuildIndex)=-1 
                    yield return null;
                }
                System.GC.Collect();
            }
            IsUnLoadingScene = false;
            if (onCompleteUnLoadAct != null)
                onCompleteUnLoadAct();
        }


        #endregion

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator InstantiateScene(string sceneName, LoadSceneMode loadMode, System.Action<bool> callback)
        {
            AsyncOperation async =   SceneManager.LoadSceneAsync(sceneName, loadMode);
            yield return async;

            if (async == null || async.isDone == false)
            {
                if (callback != null)
                    callback(false);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);  //等一帧(不确定是否有必要 考虑到场景加载完 有一段时间是不能立刻改变场景的 TODO)

            Scene currentLoad = SceneManager.GetSceneByName(sceneName);
            m_CurLoadedScene.Add(currentLoad);
            Debug.LogInfor("InstantiateScene  " + currentLoad.name);
            if (callback != null)
                callback(true);
        }

        #endregion

    }
}
