using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 所有的页面界面的父类
    /// </summary>
    public class UIBasePageView : UIBaseView
    {
        #region  Page界面操作
        private  static List<UIBasePageView> m_AllRecordViewPage = new List<UIBasePageView>();  //按照打开的顺序记录了所有打开的页面UI

        /// <summary>
        /// 当前打开的页面
        /// </summary>
        public static UIBasePageView CurOpenPage { get; private set; }
        #endregion


        protected override void Awake()
        {
            m_WindowType = WindowTypeEnum.Page;
            base.Awake();
        }


        #region  page 类型界面操作

        /// <summary>
        /// 打开一个页面UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void OpenPage(UIParameterArgs parameter)
        {
            if (parameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }

#if UNITY_EDITOR
            if (this.WindowType != WindowTypeEnum.Page)
            {
                Debug.LogError("OpenPage Fail,Not Page Window " + this.name);
                return;
            }
#endif

            if (m_AllRecordViewPage.Contains(this) == false)
            {
                #region 打开一个新的页面
                m_AllRecordViewPage.Add(this);
                ClosePageConnectView(CurOpenPage);

                //if (CurOpenPage != null)
                //{
                //    CurOpenPage.HideWindow();
                //    RemoveWillPopupView(CurOpenPage);  //关闭当前界面所包含的弹窗
                //    CloseAttachWidget(CurOpenPage.transform,false);   //关闭关联的Widget
                //}
                CurOpenPage = this;  //更新当前打开的窗口
                this.ShowWindow(parameter);
                #endregion
                return;
            }

            #region 打开一个已经打开过的页面
            Debug.Log("界面已经被打开过 则需要处理");
            if (this == CurOpenPage)
            {
                CurOpenPage.ShowWindow(parameter);
                //#region 当前要打开的界面就是最后打开的页面   则根据条件判断是否只需要刷新
                //if (CurOpenPage.IsActivate && CurOpenPage.IsShowingWindow && CurOpenPage.IsCompleteShow)
                //{
                //    Debug.Log("界面已经打开了  执行刷新操作 ");
                //    CurOpenPage.ShowWindow(parameter);
                //}
                //else
                //{
                //    Debug.LogError("当前界面状态异常 正在打开中..... " + CurOpenPage.IsShowingWindow + ":" + CurOpenPage.IsCompleteShow);
                //}
                //#endregion
                return;
            }//当前要打开的界面就是最后打开的页面   则根据条件判断是否只需要刷新

            #region  弹出已经打开过的界面 并记录当前页面
            //if (CurOpenPage != null)
            //{
            //    CurOpenPage.HideWindow();
            //    RemoveWillPopupView(CurOpenPage);  //关闭当前界面所包含的弹窗
            //    CloseAttachWidget(CurOpenPage.transform, false);   //关闭关联的Widget
            //}
            ClosePageConnectView(CurOpenPage);

            for (int dex = m_AllRecordViewPage.Count - 1; dex >= 0; --dex)
            {
                if (m_AllRecordViewPage[dex] != this)
                    m_AllRecordViewPage.RemoveAt(dex);
            } //弹出已经打开过的界面
            CurOpenPage = this;
            m_AllRecordViewPage.Add(this);
            this.ShowWindow(parameter);
            #endregion

            #endregion
        }

        /// <summary>
        /// 关闭当前打开的界面 自动打开上一个Page界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="parameter"></param>
        public void ClosePage(UIParameterArgs CloseParameter, UIParameterArgs openPageParameter)
        {
            if (CurOpenPage == null) return;
            if (CloseParameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }
#if UNITY_EDITOR
            if (m_AllRecordViewPage.Count == 0 || m_AllRecordViewPage[m_AllRecordViewPage.Count - 1] != CurOpenPage)
            {
                Debug.LogEditorInfor("ClosePage  Fail, " + (m_AllRecordViewPage.Count == 0));
                return;
            }
#endif

            if (CurOpenPage != this) return; //同时只有一个界面时候想关闭的一定是最后一个界面

            //if (m_AllRecordViewPage.Count == 1)
            //{
            //    Debug.LogError("只剩下一个界面 无法关闭" + CurOpenPage.name);
            //    return;
            //}

            HideWindow(CloseParameter);
            m_AllRecordViewPage.Remove(this); //移除当前页面
            if (m_AllRecordViewPage.Count >0)
            {
                CurOpenPage = m_AllRecordViewPage[m_AllRecordViewPage.Count - 1];
                CurOpenPage.ShowWindow(openPageParameter);
            } //在打开另一个界面时候回关闭当前界面
            else
            {
                m_AllRecordViewPage.Clear();
                ClosePageConnectView(CurOpenPage);
                CurOpenPage = null;
            } //关闭最后一个页面
        }



        /// <summary>
        /// 关闭一个页面所有关联的Popup /Widget
        /// </summary>
        /// <param name="view"></param>
        private void ClosePageConnectView(UIBasePageView view)
        {
            if (view != null)
            {
                view.HideWindow(UIParameterArgs.Create());
                UIBasePopupView. RemoveWillPopupView(view);  //关闭当前界面所包含的弹窗（没有弹出的）
                UIBasePopupView. CloseConnectPopupView(view); //关闭当前界面所包含的弹窗 (已经弹出的)
                UIBaseWidgetView. CloseAttachWidget(view.transform, false, UIParameterArgs.Create());   //关闭关联的Widget
            }
        }


        #endregion



    }
}