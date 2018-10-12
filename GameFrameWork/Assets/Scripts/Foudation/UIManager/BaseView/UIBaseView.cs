﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// UI 界面的类型 （页面/弹窗/组件/飘字）
    /// </summary>
    public enum WindowTypeEnum
    {
        None,  //无定义的  需要设置类型 
        Page,   //整屏界面 同时只有一个在显示
        PopUp, //弹窗
        Widget, //组件
        PopTip, //飘字
    }

    /// <summary>
    /// 所有UIView  的父类，所有的UI子类(页面或者弹框)必须继承这个类
    /// 注意 ；
    /// </summary>
    public abstract class UIBaseView : MonoBehaviour
    {

        public RectTransform rectransform
        {
            get
            {
                return transform as RectTransform;
            }
        }

        #region 界面属性定义
        [Header("窗口类型  必须设置")]
        [SerializeField]
        protected WindowTypeEnum m_WindowType = WindowTypeEnum.None;
        /// <summary>
        /// 窗口类型
        /// </summary>
        public WindowTypeEnum WindowType { get { return m_WindowType; } }


        [Header("是否在界面隐藏是销毁  必须设置")]
        [SerializeField]
        protected bool m_AutoDestroyOnDiable = false;

        #endregion

        #region 事件
        protected System.Action<UIBaseView> m_OnCompleteShowAct = null;  //完成显示时候调用

        #endregion

        #region State 
        /// <summary>
        /// 标识是否是打开状态(并不能作为判断当前View是否可见的标识)
        /// </summary>
        public bool IsOpen { get { return gameObject.activeSelf; } }

        /// <summary>
        /// 标识是否正在显示窗口
        /// </summary>
        protected bool IsShowingWindow { get; private set; }


        /// <summary>
        /// 标识是否完成了显示初始化
        /// </summary>
        public bool IsCompleteShow { get; private set; }

        #endregion

        #region Unity Frame 
        protected virtual void Awake()
        {
            IsShowingWindow = false;
            IsCompleteShow = false;
            UIManager.Instance.RecordView(this);
        }
        protected virtual void Start()
        {
            LoadUIConfigString();
        }

        protected virtual void OnEnable()
        {
            // IsOpen = true;  //2018/09/27 修改 这里不能执行这个 否则第一次创建的时候就已经打开了 后面的逻辑不好处理
        }

        protected virtual void OnDisable()
        {
            IsShowingWindow = false;
        }

        protected virtual void OnDestroy()
        {
            if (UIManager.Instance!=null)
            UIManager.Instance.UnRecordView(this);
        }

        //protected virtual void Update()  {  }

        #endregion


        #region 扩展接口
        /// <summary>
        /// 加载当前视图相关的语言配置 (在Start() 中调用  相关赋值操作请滞后)
        /// </summary>
        protected virtual void LoadUIConfigString()
        {

        }




        /// <summary>
        /// 外部调用 显示窗口
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void ShowWindow(params object[] parameter)
        {
            if (IsShowingWindow)
            {
                Debug.LogError("当前UI已经是打开状态 请使用接口 FlushWindow() 刷新界面!!!! 这里自动转化成 FlushWindow() 接口调用");
                FlushWindow(parameter);
                return;
            }
            IsCompleteShow = false;
            IsShowingWindow = true;
            gameObject.SetActive(true);

        }
        /// <summary>
        /// 子类重写这个方法实现 协程显示界面
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnEnumerateShowWindow()
        {
            yield return null;
        }

        /// <summary>
        /// 直接初始化
        /// </summary>
        protected virtual void OnShowWindow()
        {

        }


        /// <summary>
        /// 完成打开窗口 
        /// </summary>
        protected virtual void OnCompleteShowWindow()
        {
            IsCompleteShow = true;
        }






        /// <summary>
        /// 外部调用 刷新窗口
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void FlushWindow(params object[] parameter)
        {
            if (IsOpen == false)
            {
                Debug.LogError("没有打开的界面需要调用  ShowWindow()!!!  这里自动转化成 ShowWindow() 接口调用   ");
                ShowWindow(parameter);
                return;
            }
        }
        /// <summary>
        /// 子类重写这个方法实现 协程刷新界面  只有打开了界面才能执行这个接口
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnEnumerateFlushWindow()
        {
            yield return null;
        }

        /// <summary>
        /// 直接初始化
        /// </summary>
        protected virtual void OnFlushWindow()
        {

        }
        /// <summary>
        /// 完成刷新窗口 
        /// </summary>
        protected virtual void OnCompleteFlushWindow()
        {
            IsCompleteShow = true;
        }





        /// <summary>
        /// / 外部调用 关闭窗口
        /// </summary>
        public virtual void HideWindow(params object[] parameter)
        {
            if(IsCompleteShow==false)
            {
                Debug.LogEditorInfor("HideWindow Exception !!! Is Not Complete ShowWindow Process " + gameObject.name);
            }
        }
        /// <summary>
        /// /子类重写这个方法实现 协程关闭界面
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnHideWindow(params object[] parameter)
        {
            yield return null;
        }
        /// <summary>
        /// 完成刷新窗口 (必须在 HideWindow中调用这个接口 否则无法关闭界面  )
        /// </summary>
        protected virtual void OnCompleteHideWindow()
        {
            IsCompleteShow = true;
            gameObject.SetActive(false);
        }


        public virtual void DestroyView()
        {
            GameObject.Destroy(gameObject);
        }


        #endregion


    }
}