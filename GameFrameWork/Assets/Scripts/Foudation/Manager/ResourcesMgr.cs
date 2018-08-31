using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFrameWork
{
    /// <summary>
    /// 提供加载不同资源的接口 抽象加载器的概念(加载器被封装在命名空间GameFrameWork.ResourcesLoader 中)
    /// </summary>
    public class ResourcesMgr : Singleton_Static<ResourcesMgr>
    {
        #region 加载不同资源的接口 (内部使用加载器加载资源)

        /// <summary>
        /// 根据指定的路径加载一个预制体资源并生成对应的实例
        /// </summary>
        /// <param name="url">资源唯一路径</param>
        /// <param name="parent">实例生成后挂载在那个父节点下</param>
        /// <param name="callback">加载资源成功后的回调</param>
        /// <param name="isActivate">默认为tue 标识生成的实例为激活状态</param>
        /// <param name="isResetTransProperty">默认为tue 标识是否重置生成对象的Transform 属性</param>
        /// 
        /// <returns></returns>
        public void Instantiate(string url, Transform parent, System.Action<GameObject> callback, bool isActivate = true, bool isResetTransProperty = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback(null);
                return;
            }


            PrefabLoader.LoadAsset(parent, url, (loader) =>
            {
                #region  加载成功后的处理逻辑
                ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
                if (loader == null || (loader.IsCompleted && loader.IsError))
                {
                    Debug.LogError("Instantiate  GameObject Fail,Not Exit At Path= " + url);
                    if (callback != null)
                        callback.Invoke(null);
                    return;
                } //加载资源出错
                GameObject prefabGo = loader.ResultObj as GameObject;
                if (prefabGo == null)
                {
                    Debug.LogError("Instantiate  GameObject Fail,Load Result Not GameObject Type " + loader.ResultObj.GetType());
                    if (callback != null)
                        callback.Invoke(null);
                    return;
                }
                if (isActivate == false)
                    prefabGo.SetActive(false);  //临时改变预制体资源的可见性 返回前恢复
                GameObject go = GameObject.Instantiate(prefabGo, parent);
                go.name = prefabGo.name;

                if(isResetTransProperty)
                {
                    if(go.transform is RectTransform)
                    {
                        (go.transform as RectTransform).ResetRectTransProperty();
                    }
                    else
                    {
                        go.transform.ResetTransProperty();
                    }
                }

                if (isActivate == false)
                    prefabGo.SetActive(true);  //恢复可见性

                if (callback != null)
                    callback.Invoke(go);

                #endregion
            });
        }

        /// <summary>
        /// 加载已经打成Prefab的预制体的资源的Sprite 精灵
        /// </summary>
        /// <param name="targetImag"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void LoadSprite(UnityEngine.UI.Image targetImag, string url, System.Action<Sprite> callback)
        {
            if (targetImag == null)
            {
                Debug.LogError("LoadSprite Fail,Target Image is Null");
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback(null);
                return;
            }


            SpriteLoader.LoadAsset(targetImag.transform, url, (loader) =>
            {
                #region  加载成功后的处理逻辑
                ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
                if (loader == null || (loader.IsCompleted && loader.IsError))
                {
                    Debug.LogError("LoadSprite   Fail,Not Exit At Path= " + url);
                    if (callback != null)
                        callback.Invoke(null);
                    return;
                } //加载资源出错

                targetImag.sprite = loader.ResultObj as Sprite;
                if (callback != null)
                    callback.Invoke(targetImag.sprite);
                #endregion
            });
        }

        /// <summary>
        /// 加载各种配置文件(配置文件只会被加载一次 ，加载后不会被主动销毁)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void LoadFile(string url, System.Action<string> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback("");
                return;
            }

            TextAssetLoader.LoadAsset(url, (loader) =>
           {
               ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
               #region  加载成功后的处理逻辑
               if (loader == null || (loader.IsCompleted && loader.IsError))
               {
                   Debug.LogError("LoadFile   Fail,Not Exit At Path= " + url);
                   if (callback != null)
                       callback.Invoke(null);
                   return;
               } //加载资源出错

               if (callback != null)
                   callback.Invoke(loader.ResultObj.ToString());
               #endregion
           });
        }

        /// <summary>
        /// 加载字体资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void LoadFont(string url, System.Action<Font> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback(null);
                return;
            }

            FontLoader.LoadAsset(url, (loader) =>
            {
                ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
                #region  加载成功后的处理逻辑
                if (loader == null || (loader.IsCompleted && loader.IsError))
                {
                    Debug.LogError("LoadFont   Fail,Not Exit At Path= " + url);
                    if (callback != null)
                        callback.Invoke(null);
                    return;
                } //加载资源出错

                if (callback != null)
                    callback.Invoke(loader.ResultObj as Font);
                #endregion
            });
        }
        /// <summary>
        /// 加载材质球
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        public void LoadMaterial(string url, Transform target, System.Action<Material> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback(null);
                return;
            }
            MaterialLoader.LoadAsset(target, url, (loader) =>
            {
                ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
                #region  加载成功后的处理逻辑
                if (loader == null || (loader.IsCompleted && loader.IsError))
                {
                    Debug.LogError("LoadMaterial   Fail,Not Exit At Path= " + url);
                    if (callback != null)
                        callback.Invoke(null);
                    return;
                } //加载资源出错


                Material mat = loader.ResultObj as Material;

                if (callback != null)
                    callback.Invoke(mat);
                #endregion
            });
        }

        /// <summary>
        /// 根据场景资源路径加载场景(可能是AssetBundle 中的场景)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadModel">加载模式</param>
        /// <param name="callback"></param>
        public void LoadScene(string url, LoadSceneMode loadModel, System.Action callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback();
                return;
            }

            SceneLoader.LoadScene(url, (loader) =>
           {
               // ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
               #region  加载成功后的处理逻辑
               if (loader == null || (loader.IsCompleted && loader.IsError))
               {
                   Debug.LogError("LoadMaterial   Fail,Not Exit At Path= " + url);
                   if (callback != null)
                       callback.Invoke();
                   return;
               } //加载资源出错
               string sceneName = System.IO.Path.GetFileNameWithoutExtension(url);
               ApplicationMgr.Instance.StartCoroutine(LoadSceneAsync(sceneName, loadModel, callback));
               #endregion
           });
        }
        private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadModel, System.Action callback)
        {
            AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName, loadModel);
            while (loadAsync != null && loadAsync.isDone == false)
                yield return null;
            if (callback != null)
                callback.Invoke();
        }



        #endregion

    }
}