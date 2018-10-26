using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 管理整个项目中的所有UI界面(不需要销毁)
    /// </summary>
    public class UIManager : Singleton_Static<UIManager>
    {
        //***这里不使用Stack 栈是因为会存在操作栈中间元素的行为
        private Dictionary<string, UIBaseView> m_AllExitView = new Dictionary<string, UIBaseView>(); //记录所有打开的UI(不包含组件类型的UI ,由所属的界面自己控制)


        #region 创建/获取UI
        /// <summary>
        /// 创建UI 异步加载方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="callback"></param>
        /// <param name="isActivate">是否生成后保持激活</param>
        /// <param name="isResetTransProperty">是否重置属性</param>
        /// <param name="viewName">默认为预制体名</param>
        public void CreateUIAsync<T>(string viewPath, Transform parentTrans, System.Action<T> callback, bool isActivate, bool isResetTransProperty = true, string viewName = "") where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                Debug.LogError("CreateUI Fail, View Path Is Not Avaliable");
                return;
            }
            ResourcesMgr.Instance.InstantiateAsync(viewPath, parentTrans, (obj) =>
           {
               if (obj != null && string.IsNullOrEmpty(viewName) == false)
                   obj.name = viewName;

               T viewScript = obj.GetAddComponent<T>();
                //Debug.LogEditorInfor("CreateUI  " + viewScript.gameObject.activeSelf+ "           viewPath="+ viewPath+ "            isActivate="+ isActivate);
                obj.SetActive(isActivate);

               if (callback != null)
                   callback.Invoke(viewScript);
           }, true, isResetTransProperty);
        }

        /// <summary>
        /// 创建UI 同步加载方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="callback"></param>
        /// <param name="isActivate">是否生成后保持激活</param>
        /// <param name="isResetTransProperty">是否重置属性</param>
        /// <param name="viewName">默认为预制体名</param>
        public T CreateUISync<T>(string viewPath, Transform parentTrans, bool isActivate, bool isResetTransProperty = true, string viewName = "") where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                Debug.LogError("CreateUI Fail, View Path Is Not Avaliable   viewPath=" + viewPath);
                return null;
            }
            T result = null;

            GameObject obj = ResourcesMgr.Instance.InstantiateSync(viewPath, parentTrans, true, isResetTransProperty);
            if (obj == null)
                return result;
            if (string.IsNullOrEmpty(viewName) == false)
                obj.name = viewName;

            T viewScript = obj.GetAddComponent<T>();
            //Debug.LogEditorInfor("CreateUI  " + viewScript.gameObject.activeSelf+ "           viewPath="+ viewPath+ "            isActivate="+ isActivate);
            obj.SetActive(isActivate);
            result = viewScript;

            return result;
        }

        /// <summary>
        /// 获取已经存在的UIView
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public T GetExitUIInstance<T>(string viewName) where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Debug.LogError("GetUI Fail Not Exit");
                return null;
            }
            if (m_AllExitView.ContainsKey(viewName))
                return m_AllExitView[viewName] as T;
            return null;
        }

        /// <summary>
        ///  获取已经存在的UIView
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetExitUIInstance<T>() where T : UIBaseView
        {
            foreach (var item in m_AllExitView.Values)
            {
                if (item.GetType() == typeof(T))
                {
                    return item as T;
                }
            }
            return null;
        }


        /// <summary>
        /// 强制获取一个组件 没有就创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="parentTrans"></param>
        /// <param name="callback"></param>
        /// <param name="isActivate"></param>
        /// <param name="isResetTransProperty"></param>
        /// <param name="viewName"></param>
        public void ForceGetUIAsync<T>(string viewPath, Transform parentTrans, System.Action<T> callback, bool isActivate = true, bool isResetTransProperty = true) where T : UIBaseView
        {
            ForceGetUIAsync<T>(viewPath, System.IO.Path.GetFileNameWithoutExtension(viewPath), parentTrans, callback, isActivate, isResetTransProperty);
        }

        /// <summary>
        /// 强制获取一个组件 没有就创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="parentTrans"></param>
        /// <param name="callback"></param>
        /// <param name="isActivate"></param>
        /// <param name="isResetTransProperty"></param>
        /// <param name="viewName"></param>
        public void ForceGetUIAsync<T>(string viewPath, string viewName, Transform parentTrans, System.Action<T> callback, bool isActivate = true, bool isResetTransProperty = true) where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = System.IO.Path.GetFileNameWithoutExtension(viewPath);

            T view = GetExitUIInstance<T>(viewName);
            if (view != null)
            {
                if (view.transform.parent != parentTrans)
                    view.transform.SetParent(parentTrans);
                if (isResetTransProperty)
                    view.transform.ResetTransProperty();

                view.gameObject.SetActive(isActivate);

                if (callback != null)
                    callback(view);
                return;
            }

            CreateUIAsync<T>(viewPath, parentTrans, callback, isActivate, isResetTransProperty, viewName);
        }



        public T ForceGetUISync<T>(string viewPath, Transform parentTrans, bool isActivate = true, bool isResetTransProperty = true) where T : UIBaseView
        {
            return ForceGetUISync<T>(viewPath, System.IO.Path.GetFileNameWithoutExtension(viewPath), parentTrans, isActivate, isResetTransProperty);
        }
        public T ForceGetUISync<T>(string viewPath, string viewName, Transform parentTrans, bool isActivate = true, bool isResetTransProperty = true) where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = System.IO.Path.GetFileNameWithoutExtension(viewPath);
            T view = GetExitUIInstance<T>(viewName);
            if (view != null)
            {
                if (view.transform.parent != parentTrans)
                    view.transform.SetParent(parentTrans);
                if (isResetTransProperty)
                    view.transform.ResetTransProperty();

                view.gameObject.SetActive(isActivate);
                return view;
            }

            return CreateUISync<T>(viewPath, parentTrans, isActivate, isResetTransProperty, viewName);
        }


        #endregion

        #region  辅助
        /// <summary>
        /// 记录存在的UI
        /// </summary>
        /// <param name="view"></param>
        public void RecordView(UIBaseView view)
        {
            if (view == null || string.IsNullOrEmpty(view.name))
            {
                Debug.LogError("RecordView" + (view == null));
                return;
            }
            if (m_AllExitView.ContainsKey(view.name) == false)
                m_AllExitView.Add(view.name, view);
        }

        /// <summary>
        /// 取消记录
        /// </summary>
        /// <param name="view"></param>
        public void UnRecordView(UIBaseView view)
        {
            if (view == null || string.IsNullOrEmpty(view.name))
            {
                Debug.LogError("UnRecordView" + (view == null));
                return;
            }
            if (m_AllExitView.ContainsKey(view.name))
                m_AllExitView.Remove(view.name);
        }

        #endregion






    }
}