using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.ResourcesLoader
{
    /// <summary>
    /// 这个基类定义那些会在程序运行期间频繁加载和卸载的资源(图片/声音等)
    /// 这里每个加载器记录了加载的资源被哪个物体拥有，且在该物体上加载了一个脚本LoaderResourceHandlerTag
    /// </summary>
    public   abstract class ApplicationLoader_Alone : ApplicationLayerBaseLoader
    {
        /// <summary>
        /// 记录这个资源被那个物体引用的关系 (确保一个资源只能被一个物体拥有一次引用)
        /// </summary>
        [System.Serializable]
        protected class LoaderRequsterInfor
        {
            public object m_RequsterInfor;//加载器加载的资源的引用对象
            public int m_HashCode; //加载器加载的资源的引用对象的HaseCode
            public ApplicationLoader_Alone m_Loader;  //加载器

            public LoaderRequsterInfor(object requestTarget, ApplicationLoader_Alone loader)
            {
                m_RequsterInfor = requestTarget;
                m_HashCode = m_RequsterInfor.GetHashCode();
                m_Loader = loader;
            }
            public LoaderRequsterInfor(object requestTarget, int hashcode, ApplicationLoader_Alone loader)
            {
                m_RequsterInfor = requestTarget;
                m_HashCode = hashcode;
                m_Loader = loader;
            }
        }


        /// <summary>
        /// 记录当前资源被哪些物体拥有
        /// </summary>
        protected Dictionary<int, LoaderRequsterInfor> m_ResourceHolderInfor = new Dictionary<int, LoaderRequsterInfor>();
        private HashSet<int> m_DeleteLoaderInfor = new HashSet<int>(); //当前引用不存在的对象 删除记录


        public override void InitialLoader()
        {
            base.InitialLoader();
            IsDontDestroyOnLoaded = false;
        }


        #region 编辑器处理
#if UNITY_EDITOR
        [SerializeField]
        private List<LoaderRequsterInfor> m_ShowResourcesHolderInfor = new List<LoaderRequsterInfor>();

        private void UpdateView()
        {
            m_ShowResourcesHolderInfor.Clear();
            foreach (var item in m_ResourceHolderInfor)
            {
                m_ShowResourcesHolderInfor.Add(item.Value);
            }
        }
#endif
        #endregion

        #region 引用计数维护
        protected override void AddReference(Transform requestTarget, string url)
        {
            if (TryAddRecord(requestTarget, url))
                base.AddReference(requestTarget, url);  //如果没有被记录过则添加引用计数
        }


        public override void ReduceReference(BaseAbstracResourceLoader loader, bool isForcesDelete)
        {
            if (TryDeleteRecord(m_RequesterTarget))
                base.ReduceReference(loader, isForcesDelete); //如果没有被记录过则添加引用计数
        }

        #endregion


        /// <summary>
        /// 加载之前判断处理一些资源逻辑 (加Tag 脚本标识和减少旧资源的索引)
        /// </summary>
        /// <param name="requestTarget"></param>
        /// <param name="url"></param>
        protected virtual void TryLoadAsset(Transform requestTarget, string url)
        {
            if (requestTarget == null) return;
            LoaderResourceHandlerTag tag = requestTarget.GetAddComponent<LoaderResourceHandlerTag>();
            tag.TryLoadAsset(url, this);
        }

        /// <summary>
        /// 增加引用需要自己调用返回后执行
        /// 默认规则是资源再整个过程中是不变的 ，每个资源只能被一个物体拥有一次
        /// 会在加载的物体上挂载对应的Tag脚本以及记录资源信息
        /// </summary>
        /// <param name="requestTarget">如果挂载的物体为null 则不考虑 返回true</param>
        /// <returns></returns>
        protected virtual bool TryAddRecord(Transform requestTarget, string url)
        {
            m_RequesterTarget = requestTarget;
            if (requestTarget == null)
                return true;
            TryLoadAsset(requestTarget, url); //2018/07/16 加入资源标识

            int hashcode = requestTarget.GetHashCode();
            if (m_ResourceHolderInfor.ContainsKey(hashcode))
                return false;

            LoaderRequsterInfor resquestInfor = new LoaderRequsterInfor(requestTarget, hashcode, this);
            m_ResourceHolderInfor.Add(hashcode, resquestInfor);
#if UNITY_EDITOR
            UpdateView();
#endif
            return true;
        }

        /// <summary>
        /// 删除物体的引用记录  减少引用需要自己调用返回后执行
        /// </summary>
        /// <param name="requestTarget"></param>
        /// <returns></returns>
        protected virtual bool TryDeleteRecord(object requestTarget)
        {
            if (requestTarget == null)
                return false;
            int hashcode = requestTarget.GetHashCode();
            if (m_ResourceHolderInfor.ContainsKey(hashcode) == false)
                return false;

            m_ResourceHolderInfor.Remove(hashcode);
#if UNITY_EDITOR
            UpdateView();
#endif
            return true;
        }


        /// <summary>
        /// 检测当前资源是否还在被引用
        /// </summary>
        public void CheckLoaderIsReference()
        {
            foreach (var item in m_ResourceHolderInfor)
            {
                if (item.Value.m_RequsterInfor == null)
                {
                    m_DeleteLoaderInfor.Add(item.Key);
                    item.Value.m_Loader.ReduceReference(item.Value.m_Loader, false);
                }
            }

            foreach (var item in m_DeleteLoaderInfor)
            {
#if UNITY_EDITOR
                Debug.LogInfor(string.Format("[UnRefence : url ={0}  ,targetHashCode ={1}", m_ResourceHolderInfor[item].m_Loader.m_ResourcesUrl, item));
#endif
                m_ResourceHolderInfor.Remove(item);
            }
            if (m_DeleteLoaderInfor.Count != 0)
                m_DeleteLoaderInfor.Clear();

        }



    }
}