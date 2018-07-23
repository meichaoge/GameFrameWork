using GameFrameWork.ResourcesLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
            PrefabLoader.LoadAsset(parent, url, (loader) =>
            {
                #region  加载成功后的处理逻辑

                if (loader.IsCompleted && loader.IsError)
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
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
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

            SpriteLoader.LoadAsset(targetImag.transform, url, (loader) =>
            {
                #region  加载成功后的处理逻辑
                if (loader.IsCompleted && loader.IsError)
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
            TextAssetLoader.LoadAsset(url, (loader) =>
           {
                #region  加载成功后的处理逻辑
                if (loader.IsCompleted && loader.IsError)
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
            FontLoader.LoadAsset(url, (loader) =>
            {
                #region  加载成功后的处理逻辑
                if (loader.IsCompleted && loader.IsError)
                {
                    Debug.LogError("LoadAsset   Fail,Not Exit At Path= " + url);
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

    }
}