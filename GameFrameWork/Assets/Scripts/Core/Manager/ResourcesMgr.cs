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
        private HashSet<GameObject> m_AllNotDestroyOnLoadObj = new HashSet<GameObject>(); // 所有不会在场景加载时候销毁的对象

        #region  实例化对象
        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Instantiate(string goName) 
        {
            return new GameObject(goName);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public T Instantiate<T>(T original) where T : Object
        {
            return GameObject.Instantiate<T>(original, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public T Instantiate<T>(T original, Transform parent) where T : Object
        {
            return Instantiate<T>(original, parent, true);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instantiate<T>(original, position, rotation, null);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            ResourcesLoadTraceMgr.Instance.RecordTraceInstantiate(original.name);
            return GameObject.Instantiate<T>(original, position, rotation, parent);
        }

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            ResourcesLoadTraceMgr.Instance.RecordTraceInstantiate(original.name);
            return GameObject.Instantiate<T>(original, parent, worldPositionStays);
        }
        #endregion

        #region  记录不会销毁的对象
        /// <summary>
        /// 记录不会被销毁的对象
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool MarkNotDestroyOnLoad(GameObject go)
        {
            if(go==null)
            {
                Debug.LogError("MarkNotDestroyOnLoad Fail, Parameter is Null");
                return false;
            }

            if(m_AllNotDestroyOnLoadObj.Contains(go))
            {
                Debug.LogError("MarkNotDestroyOnLoad Fail,Alrady Record " + go.name);
                return false;
            }
            m_AllNotDestroyOnLoadObj.Add(go);
            GameObject.DontDestroyOnLoad(go);
            return true;
        }
        #endregion

        #region 加载不同资源的接口 (内部使用加载器加载资源)

        #region 加载并生成GameObject

        /// <summary>
        /// 根据指定的路径加载一个预制体资源并生成对应的实例 (异步加载)
        /// </summary>
        /// <param name="url">资源唯一路径</param>
        /// <param name="parent">实例生成后挂载在那个父节点下</param>
        /// <param name="callback">加载资源成功后的回调</param>
        /// <param name="isActivate">默认为tue 标识生成的实例为激活状态</param>
        /// <param name="isResetTransProperty">默认为tue 标识是否重置生成对象的Transform 属性</param>
        /// <returns></returns>
        public void InstantiateAsync(string url, Transform parent, System.Action<GameObject> callback, bool isActivate = true, bool isResetTransProperty = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback(null);
                return;
            }

            Instantiate(url, parent, LoadAssetModel.Async, callback, isActivate, isResetTransProperty);
        }

        /// <summary>
        /// 根据指定的路径加载一个预制体资源并生成对应的实例 (同步加载)
        /// </summary>
        /// <param name="url">资源唯一路径</param>
        /// <param name="parent">实例生成后挂载在那个父节点下</param>
        /// <param name="isActivate">默认为tue 标识生成的实例为激活状态</param>
        /// <param name="isResetTransProperty">默认为tue 标识是否重置生成对象的Transform 属性</param>
        /// <returns></returns>
        public GameObject InstantiateSync(string url, Transform parent, bool isActivate = true, bool isResetTransProperty = true)
        {
            GameObject loadGameObject = null;
            if (string.IsNullOrEmpty(url))
            {
                return loadGameObject;
            }

            Instantiate(url, parent, LoadAssetModel.Sync, (go) => { loadGameObject = go; }, isActivate, isResetTransProperty);
            return loadGameObject;
        }

        /// <summary>
        /// 根据指定的路径加载一个预制体资源并生成对应的实例
        /// </summary>
        /// <param name="url">资源唯一路径</param>
        /// <param name="parent">实例生成后挂载在那个父节点下</param>
        /// <param name="callback">加载资源成功后的回调</param>
        /// <param name="isActivate">默认为tue 标识生成的实例为激活状态</param>
        /// <param name="isResetTransProperty">默认为tue 标识是否重置生成对象的Transform 属性</param>
        /// <returns></returns>
        private void Instantiate(string url, Transform parent, LoadAssetModel loadModel, System.Action<GameObject> callback, bool isActivate = true, bool isResetTransProperty = true)
        {
#if UNITY_EDITOR
            Debug.LogEditorInfor(string.Format("[ResourcesMgr ] Instantiate Begin >>>>  url={0}  LoadModel={1} Time={2}   RederFrameCont={3}", url, loadModel, Time.realtimeSinceStartup, Time.renderedFrameCount));

#endif


            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                    callback(null);
                return;
            }


            PrefabLoader.LoadAsset(parent, url, loadModel, (loader) =>
             {
                 #region  加载成功后的处理逻辑
                 ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
                 if (loader == null || loader.ResultObj == null || (loader.IsCompleted && loader.IsError))
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

                 if (isResetTransProperty)
                 {
                     if (go.transform is RectTransform)
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

#if UNITY_EDITOR
                 Debug.LogEditorInfor(string.Format("[ResourcesMgr ] Instantiate Complete <<<<<  url={0}  LoadModel={1} Time={2}  RederFrameCont={3}", url, loadModel, Time.realtimeSinceStartup, Time.renderedFrameCount));
#endif

                 if (callback != null)
                     callback.Invoke(go);

                 #endregion
             });
        }


        #endregion


        #region  加载Sprite
        /// <summary>
        /// 加载已经打成Prefab的预制体的资源的Sprite 精灵(同步加载)
        /// </summary>
        /// <param name="targetImag"></param>
        /// <param name="url"></param>
        public Sprite LoadSpriteSync(string url, UnityEngine.UI.Image targetImag)
        {
            Sprite result = null;
            if (CheckLoadSpriteParameter(url, targetImag) == false)
                return result;
            LoadSprite(url, targetImag, LoadAssetModel.Sync, (sprite) => { result = sprite; });
            return result;
        }

        /// <summary>
        /// 加载已经打成Prefab的预制体的资源的Sprite 精灵（异步加载）
        /// </summary>
        /// <param name="targetImag"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void LoadSpriteAsync(string url, UnityEngine.UI.Image targetImag, System.Action<Sprite> callback)
        {
            if (CheckLoadSpriteParameter(url, targetImag) == false)
            {
                if (callback != null)
                    callback.Invoke(null);
                return;
            }
            LoadSprite(url, targetImag, LoadAssetModel.Async, callback);
        }
        /// <summary>
        /// 检查参数是否合法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="targetImag"></param>
        /// <returns></returns>
        private bool CheckLoadSpriteParameter(string url, UnityEngine.UI.Image targetImag)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("CheckLoadSpriteParameter  Fail,Url IsNullOrEmpty");
                return false;
            }
            if (targetImag == null)
            {
                Debug.LogError("CheckLoadSpriteParameter Fail,Target Image is Null");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载已经打成Prefab的预制体的资源的Sprite 精灵
        /// </summary>
        /// <param name="targetImag"></param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        private void LoadSprite(string url, UnityEngine.UI.Image targetImag, LoadAssetModel loadModel, System.Action<Sprite> callback)
        {
            SpriteLoader.LoadAsset(targetImag.transform, url, loadModel, (loader) =>
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

        #endregion

        #region  加载有多个语言版本的Sprite资源
        public Sprite LoadSpecialSpriteSync(string url, UnityEngine.UI.Image targetImag)
        {
            //** StringUtility 写到一半
            return null;
        }
        #endregion

        #region  加载File 配置文件(text)
        /// <summary>
        /// 加载各种配置文件(配置文件只会被加载一次 ，加载后不会被主动销毁) (同步加载)
        /// </summary>
        /// <param name="url"></param>
        public string LoadFileSync(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadFileSync Fail  路径为null");
                return result;
            }

            LoadFile(url, LoadAssetModel.Sync, (asset) => { result = asset; });
            return result;
        }
        /// <summary>
        /// 加载各种配置文件(配置文件只会被加载一次 ，加载后不会被主动销毁)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void LoadFileAsync(string url, System.Action<string> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadFileAsync Fail  路径为null");
                if (callback != null)
                    callback("");
                return;
            }
            LoadFile(url, LoadAssetModel.Async, callback);
        }

        /// <summary>
        /// 加载各种配置文件(配置文件只会被加载一次 ，加载后不会被主动销毁)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        private void LoadFile(string url, LoadAssetModel loadModel, System.Action<string> callback)
        {
            TextAssetLoader.LoadAsset(url, loadModel, (loader) =>
            {
                ResourcesLoadTraceMgr.Instance.RecordTraceResourceInfor(loader);
                #region  加载成功后的处理逻辑
                if (loader == null || (loader.IsCompleted && loader.IsError))
                {
                    Debug.LogInfor("LoadFile   Fail,Not Exit At Path= " + url);
                    if (callback != null)
                        callback.Invoke(null);
                    return;
                } //加载资源出错

                if (callback != null)
                    callback.Invoke(loader.ResultObj.ToString());
                #endregion
            });
        }

        #endregion

        #region 加载字体
        /// <summary>
        /// 加载字体资源 (同步)
        /// </summary>
        /// <param name="url"></param>
        public Font LoadFontSync(string url)
        {
            Font result = null;
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadFontSync url IsNullOrEmpty");
                return result;
            }
            LoadFont(url, LoadAssetModel.Sync, (font) => { result = font; });
            return result;
        }

        /// <summary>
        /// 加载字体资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void LoadFontAsync(string url, System.Action<Font> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadFontAsync url IsNullOrEmpty");
                if (callback != null)
                    callback(null);
                return;
            }
            LoadFont(url, LoadAssetModel.Async, callback);
        }
        /// <summary>
        /// 加载字体资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        private void LoadFont(string url, LoadAssetModel loadModel, System.Action<Font> callback)
        {
            FontLoader.LoadFontAsset(url, loadModel, (loader) =>
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
        #endregion

        #region  加载材质球
        /// <summary>
        /// 加载材质球
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        public Material LoadMaterialSync(string url, Transform target)
        {
            Material result = null;
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadMaterialSync Fail url IsNullOrEmpty");
                return result;
            }
            LoadMaterial(url, target, LoadAssetModel.Sync, (material) => { result = material; });
            return result;
        }

        /// <summary>
        /// 加载材质球
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        public void LoadMaterialAsync(string url, Transform target, System.Action<Material> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadMaterialAsync Fail url IsNullOrEmpty");
                if (callback != null)
                    callback(null);
                return;
            }
            LoadMaterial(url, target, LoadAssetModel.Async, callback);
        }
        /// <summary>
        /// 加载材质球
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="callback"></param>
        private void LoadMaterial(string url, Transform target, LoadAssetModel loadModel, System.Action<Material> callback)
        {
            MaterialLoader.LoadAsset(target, url, loadModel, (loader) =>
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

        #endregion

        #region  加载声音
        /// <summary>
        /// 加载声音资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        public AudioClip LoadAudioSync(string url, Transform parent)
        {
            AudioClip result = null;
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadAudioSync Fail url IsNullOrEmpty");
                return result;
            }
            LoadAudio(url, parent, LoadAssetModel.Sync, (audio) => { result = audio; });
            return result;
        }


        /// <summary>
        /// 加载声音资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        public void LoadAudioAsync(string url, Transform parent, System.Action<AudioClip> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("LoadAudioAsync Fail url IsNullOrEmpty");
                if (callback != null)
                    callback(null);
                return;
            }
            LoadAudio(url, parent, LoadAssetModel.Async, callback);
        }

        /// <summary>
        /// 加载声音资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        private void LoadAudio(string url, Transform parent, LoadAssetModel loadModel, System.Action<AudioClip> callback)
        {

            AudioLoader.LoadAudioClip(parent, url, loadModel, (loader) =>
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


                 AudioClip clip = loader.ResultObj as AudioClip;

                 if (callback != null)
                     callback.Invoke(clip);
                 #endregion
             });
        }

        #endregion


        #endregion

    }
}