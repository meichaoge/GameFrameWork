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
        private List<UIBasePageView> m_AllRecordViewPage = new List<UIBasePageView>();  //按照打开的顺序记录了所有打开的页面UI
        private List<UIBasePopupView> m_AllRecordViewPopup = new List<UIBasePopupView>();  //按照打开的顺序记录了所有打开的弹窗UI
        private List<UIBaseTipView> m_AllRecordViewTips = new List<UIBaseTipView>();//按照打开的事件顺序记录了所有打开的飘窗提示UI
        private Dictionary<UIBasePageView, List<UIBasePopupView>> m_LowerPriorityPopupView = new Dictionary<UIBasePageView, List<UIBasePopupView>>(); //当前弹窗优先级不够导致暂时无法显示的界面
        private Dictionary<Transform, List<UIBaseWidgetView>> m_AllWidgetView = new Dictionary<Transform, List<UIBaseWidgetView>>(); //当前显示的widget  
        private Dictionary<string, UIBaseView> m_AllExitView = new Dictionary<string, UIBaseView>(); //记录所有打开的UI(不包含组件类型的UI ,由所属的界面自己控制)

        #region 状态
        /// <summary>
        /// 当前打开的页面
        /// </summary>
        public UIBasePageView CurOpenPage { get; private set; }
        #endregion

        #region 常用的接口

        #region  page 类型界面操作

        /// <summary>
        /// 打开一个页面UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void OpenPage(UIBasePageView view, params object[] parameter)
        {
            if (view == null)
            {
                Debug.LogError("OpenPage Fail,The View Is Null");
                return;
            }

#if UNITY_EDITOR
            if (view.WindowType != WindowTypeEnum.Page)
            {
                Debug.LogError("OpenPage Fail,Not Page Window " + view.name);
                return;
            }
#endif

            if (m_AllRecordViewPage.Contains(view) == false)
            {
                #region 打开一个新的页面
                m_AllRecordViewPage.Add(view);
                ClosePageConnectView(CurOpenPage);

                //if (CurOpenPage != null)
                //{
                //    CurOpenPage.HideWindow();
                //    RemoveWillPopupView(CurOpenPage);  //关闭当前界面所包含的弹窗
                //    CloseAttachWidget(CurOpenPage.transform,false);   //关闭关联的Widget
                //}
                CurOpenPage = view;  //更新当前打开的窗口
                view.ShowWindow(parameter);
                #endregion
                return;
            }
            #region 打开一个已经打开过的页面
            Debug.Log("界面已经被打开过 则需要处理");
            if (view == CurOpenPage)
            {
                #region 当前要打开的界面就是最后打开的页面   则根据条件判断是否只需要刷新

                if (CurOpenPage.IsOpen && CurOpenPage.IsCompleteShow)
                {
                    Debug.Log("界面已经打开了  执行刷新操作 ");
                    CurOpenPage.FlushWindow(parameter);
                }
                else
                {
                    Debug.LogError("当前界面状态异常 正在打开中..... " + CurOpenPage.IsOpen + ":" + CurOpenPage.IsCompleteShow);
                }
                #endregion

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
                if (m_AllRecordViewPage[dex] != view)
                    m_AllRecordViewPage.RemoveAt(dex);
            } //弹出已经打开过的界面
            CurOpenPage = view;
            m_AllRecordViewPage.Add(view);
            view.ShowWindow(parameter);
            #endregion

            #endregion
        }

        /// <summary>
        /// 关闭当前打开的界面 自动打开上一个Page界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="parameter"></param>
        public void ClosePage(params object[] parameter)
        {
            if (CurOpenPage == null) return;
#if UNITY_EDITOR
            if (m_AllRecordViewPage.Count == 0 || m_AllRecordViewPage[m_AllRecordViewPage.Count - 1] != CurOpenPage)
            {
                Debug.LogEditorInfor("ClosePage  Fail, " + (m_AllRecordViewPage.Count == 0));
                return;
            }
#endif
            //if (m_AllRecordViewPage.Count == 1)
            //{
            //    Debug.LogError("只剩下一个界面 无法关闭" + CurOpenPage.name);
            //    return;
            //}
            if(m_AllRecordViewPage.Count>1)
            {
                CurOpenPage = m_AllRecordViewPage[m_AllRecordViewPage.Count - 1];
                CurOpenPage.ShowWindow(parameter);
            } //在打开另一个界面时候回关闭当前界面
            else
            {
                m_AllRecordViewPage.Clear();
                ClosePageConnectView(CurOpenPage);
                CurOpenPage = null;
            } //关闭最后一个页面
        }

        public void ClosePage(UIBasePageView view, params object[] parameter)
        {
            if (view != CurOpenPage) return;

            ClosePage(parameter);
        }


        /// <summary>
        /// 关闭一个页面所有关联的Popup /Widget
        /// </summary>
        /// <param name="view"></param>
        private void  ClosePageConnectView(UIBasePageView view)
        {
            if (view != null)
            {
                view.HideWindow();
                RemoveWillPopupView(view);  //关闭当前界面所包含的弹窗（没有弹出的）
                CloseConnectPopupView(view); //关闭当前界面所包含的弹窗 (已经弹出的)
                CloseAttachWidget(view.transform, false);   //关闭关联的Widget
            }
        }


        #endregion

        #region Popup  弹窗界面

        /// <summary>
        /// 打开一个弹窗界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popupOperate"></param>
        /// <param name="belongPageView">当前弹窗属于哪个Page 管理 ,如果=null则表示属于当前打开的界面 否则指定的参数界面 </param>
        /// <param name="isFailRecord">如果是因为优先级而无法弹出 则表示是否需要记录 如果为false 则当无法显示时候不会自动弹出，而是隐藏了</param>
        /// <param name="parameter"></param>
        /// 
        public void OpenPopUp(UIBasePopupView view, PopupOpenOperateEnum popupOperate, UIBasePageView belongPageView, bool isFailRecord, object[] parameter)
        {
            if (view == null)
            {
                Debug.LogError("OpenPopUp Fail,The View Is Null");
                return;
            }

#if UNITY_EDITOR
            if (view.WindowType != WindowTypeEnum.PopUp)
            {
                Debug.LogError("OpenPopUp Fail,Not Popup Window " + view.name);
                return;
            }
#endif
            Dictionary<int, UIBasePopupView> allWillHidePopupView = GetWillHidePopupView(view, popupOperate, isFailRecord, parameter);
            foreach (var item in allWillHidePopupView)
            {
                item.Value.HideWindow();
                m_AllRecordViewPopup.RemoveAt(item.Key);
            }
            if (belongPageView == null)
                belongPageView = CurOpenPage;
            view.ShowWindow ( Helper.Instance.MegerParameter(new object[] { belongPageView, isFailRecord }, parameter)  ); //注意这里的第一个参数
            m_AllRecordViewPopup.Add(view);
        }


        /// <summary>
        /// 打开一个弹窗界面 （属于当前页面的）
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popupOperate"></param>
        /// <param name="isFailRecord">如果是因为优先级而无法弹出 则表示是否需要记录 如果为false 则当无法显示时候不会自动弹出，而是隐藏了</param>
        /// <param name="parameter"></param>
        /// 
        public void OpenPopUp(UIBasePopupView view, PopupOpenOperateEnum popupOperate, bool isFailRecord, object[] parameter)
        {
            OpenPopUp(view, popupOperate, CurOpenPage, isFailRecord, parameter);
        }

        /// <summary>
        /// 打开一个弹窗界面 （属于当前页面的）
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popupOperate"></param>
        /// <param name="isFailRecord">如果是因为优先级而无法弹出 则表示是否需要记录 如果为false 则当无法显示时候不会自动弹出，而是隐藏了</param>
        public void OpenPopUp(UIBasePopupView view, PopupOpenOperateEnum popupOperate, bool isFailRecord=true)
        {
            OpenPopUp(view, popupOperate, CurOpenPage, isFailRecord, null);
        }


        /// <summary>
        /// 关闭一个弹窗界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="closeOperate"></param>
        public void ClosePopup(UIBasePopupView view, PopupCloseOperateEnum closeOperate)
        {
            if (view == null)
            {
                Debug.LogError("ClosePopup Fail,The View Is Null");
                return;
            }
            view.HideWindow();
            m_AllRecordViewPopup.Remove(view);
            DealHidePopView(view, closeOperate);
        }

        /// <summary>
        /// 处理关闭弹窗时候的逻辑
        /// </summary>
        /// <param name="view"></param>
        /// <param name="closeOperate"></param>
        private void DealHidePopView(UIBasePopupView view, PopupCloseOperateEnum closeOperate)
        {
            if (m_AllRecordViewPopup.Count == 0) return;
            UIBasePopupView nectShowPage = null; //关闭这个弹窗口这个弹窗打开之前的弹窗
            switch (closeOperate)
            {
                case PopupCloseOperateEnum.Simple:
                    break;
                case PopupCloseOperateEnum.AutoPopup:
                    for (int dex = m_AllRecordViewPopup.Count - 1; dex >= 0; --dex)
                    {
                        if (dex > 0 && m_AllRecordViewPopup[dex] == view)
                        {
                            nectShowPage = m_AllRecordViewPopup[dex - 1];
                            break;
                        }
                    }
                    AutoPupupView(nectShowPage);
                    break;
                case PopupCloseOperateEnum.CloseAndClearCurPage:
                    List<int> allPopupViewCurPage = new List<int>();
                    for (int dex = m_AllRecordViewPopup.Count - 1; dex >= 0; --dex)
                    {
                        if (m_AllRecordViewPopup[dex] != view && m_AllRecordViewPopup[dex].BelongPageView == view.BelongPageView)
                            allPopupViewCurPage.Add(dex);
                    }


                    for (int dex = 0; dex < allPopupViewCurPage.Count; ++dex)
                    {
                        m_AllRecordViewPopup[dex].HideWindow();
                    }//关闭弹窗

                    allPopupViewCurPage.Reverse();
                    for (int dex = 0; dex < allPopupViewCurPage.Count; ++dex)
                    {
                        m_AllRecordViewPopup.RemoveAt(allPopupViewCurPage[dex]);
                    }//删除打开记录

                    if (m_LowerPriorityPopupView.ContainsKey(view.BelongPageView))
                        m_LowerPriorityPopupView[view.BelongPageView].Clear();

                    break;
                default:
                    Debug.LogError("没有处理的类型  " + closeOperate);
                    break;
            }

        }



        /// <summary>
        /// 获取需要关闭的弹窗
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popupOperate"></param>
        private Dictionary<int, UIBasePopupView> GetWillHidePopupView(UIBasePopupView popupView, PopupOpenOperateEnum popupOperate, bool isFailRecord, object[] parameter)
        {
            Dictionary<int, UIBasePopupView> allWillHidePopupView = new Dictionary<int, UIBasePopupView>();
            if (m_AllRecordViewPopup.Count == 0) return allWillHidePopupView;

            switch (popupOperate)
            {
                case PopupOpenOperateEnum.KeepPreviousAvailable:
                    break;
                case PopupOpenOperateEnum.HideAllOpenView:
                    for (int dex = 0; dex < m_AllRecordViewPopup.Count; ++dex)
                    {
                        allWillHidePopupView.Add(dex, m_AllRecordViewPopup[dex]);
                    }
                    break;
                case PopupOpenOperateEnum.HideCurPagePopupView:
                    for (int dex = 0; dex < m_AllRecordViewPopup.Count; ++dex)
                    {
                        if (m_AllRecordViewPopup[dex].BelongPageView == popupView.BelongPageView)
                        {
                            allWillHidePopupView.Add(dex, m_AllRecordViewPopup[dex]);
                        }
                    } //获取当前弹窗所属的页面
                    break;
                case PopupOpenOperateEnum.PriorityOrder:
                    if (m_AllRecordViewPopup[m_AllRecordViewPopup.Count - 1].Priority >= popupView.Priority)
                    {
                        if (isFailRecord)
                        {
                            RecordWillPopupView(popupView.BelongPageView, popupView);
                            popupView.FailShowByPriority(parameter);  //记录参数
                        }
                        else
                        {
                            popupView.HideWindow();
                        }
                    }  //加入到待弹出界面中
                    else
                    {
                        allWillHidePopupView.Add(m_AllRecordViewPopup.Count - 1, m_AllRecordViewPopup[m_AllRecordViewPopup.Count - 1]);
                    }
                    break;
                default:
                    Debug.LogError("没有处理的枚举类型 " + popupOperate);
                    break;
            }
            return allWillHidePopupView;
        }





        /// <summary>
        /// 关闭页面时候关闭这个页面所包含的没有弹出的弹窗
        /// </summary>
        /// <param name="pageview"></param>
        private void RemoveWillPopupView(UIBasePageView pageview)
        {
            if (pageview == null)
            {
                Debug.LogError("RemoveWillPopupView Is Null Page");
                return;
            }
            if (m_LowerPriorityPopupView.ContainsKey(pageview) == false) return;
            m_LowerPriorityPopupView[pageview].Clear();
        }
       /// <summary>
       /// 关闭已经打开的弹窗
       /// </summary>
       /// <param name="pageview"></param>
        private void  CloseConnectPopupView(UIBasePageView belongPageView)
        {
            if (belongPageView == null)
            {
                Debug.LogError("ClosePopupView Is Null Page");
                return;
            }

            for (int dex= m_AllRecordViewPopup.Count-1;dex>=0;--dex)
            {
                if(m_AllRecordViewPopup[dex].BelongPageView== belongPageView)
                {
                    m_AllRecordViewPopup[dex].HideWindow();
                    m_AllRecordViewPopup.RemoveAt(dex);
                }
            }
        }



        /// <summary>
        /// 记录需要弹出来的窗口
        /// </summary>
        /// <param name="pageview"></param>
        /// <param name="popupView"></param>
        private void RecordWillPopupView(UIBasePageView pageview, UIBasePopupView popupView)
        {
            if (pageview == null || popupView == null)
            {
                Debug.LogError("RecordWillPopupView Fail " + (pageview == null) + " ::" + (popupView == null));
                return;
            }

            if (m_LowerPriorityPopupView.ContainsKey(pageview) == false)
            {
                List<UIBasePopupView> pagePopupView = new List<UIBasePopupView>();
                pagePopupView.Add(popupView);
                m_LowerPriorityPopupView.Add(pageview, pagePopupView);
            }
            else
            {
                m_LowerPriorityPopupView[pageview].Add(popupView);

#if UNITY_EDITOR
                Debug.LogEditorInfor("排序前的顺序");
                for (int dex = 0; dex < m_LowerPriorityPopupView[pageview].Count; ++dex)
                {
                    Debug.LogEditorInfor("dex={0}   popupView={1 }   priority={2}", dex, m_LowerPriorityPopupView[pageview][dex], m_LowerPriorityPopupView[pageview][dex].Priority);
                }
#endif

                SortPopupViewByPriority(m_LowerPriorityPopupView[pageview], true); //  //按照优先级从低到高排序
                //m_LowerPriorityPopupView[pageview].Sort((a, b) =>
                //{
                //    if (a.Priority > b.Priority)
                //        return 1;
                //    else if (a.Priority == b.Priority)
                //        return 0;
                //    else return -1;
                //});   //按照优先级从低到高排序

#if UNITY_EDITOR
                Debug.LogEditorInfor("排序后的顺序");
                for (int dex = 0; dex < m_LowerPriorityPopupView[pageview].Count; ++dex)
                {
                    Debug.LogEditorInfor("dex={0}   popupView={1 }   priority={2}", dex, m_LowerPriorityPopupView[pageview][dex], m_LowerPriorityPopupView[pageview][dex].Priority);
                }
#endif

            }
        }

        /// <summary>
        /// 当关闭一个弹窗界面时候 检查并弹出优先级比要显示的弹窗优先级高的弹窗
        /// </summary>
        /// <param name="popupView">关闭一个弹窗后下面的一个弹窗 (可以是null)</param>
        /// <returns>标识</returns>
        private void AutoPupupView(UIBasePopupView popupView)
        {
            List<UIBasePopupView> willPopupView = null;
            if (popupView != null)
            {
                if (m_LowerPriorityPopupView.TryGetValue(popupView.BelongPageView, out willPopupView))
                {
                    if (willPopupView.Count == 0) return;
                    if (willPopupView[willPopupView.Count - 1].Priority > popupView.Priority)
                    {
                        UIBasePopupView willShowPopupView = willPopupView[willPopupView.Count - 1]; //将要显示的弹窗
                        willShowPopupView.DelayPopupShowWindow();  //参数已经缓存
                        willPopupView.RemoveAt(willPopupView.Count - 1);  //移除队列
                        m_AllRecordViewPopup.Add(willShowPopupView);  //将要显示的弹窗
                    }
                }
            }
            else
            {
                if (m_LowerPriorityPopupView.TryGetValue(CurOpenPage, out willPopupView))
                {
                    if (willPopupView.Count == 0) return;
                    UIBasePopupView willShowPopupView = willPopupView[willPopupView.Count - 1]; //将要显示的弹窗
                    willShowPopupView.DelayPopupShowWindow();  //参数已经缓存
                    willPopupView.RemoveAt(willPopupView.Count - 1);  //移除队列
                    m_AllRecordViewPopup.Add(willShowPopupView);  //将要显示的弹窗
                } //取当前打开页面的队列表

            }  //可能存在关闭一个顶层弹窗后 后下面没有要显示的弹窗的情况
        }


        public void CloseAllPopupView()
        {

        }


        /// <summary>
        /// 根据弹窗的优先级排序列表(按照从低到高的优先级排序)
        /// </summary>
        /// <param name="popupView"></param>
        /// <param name="isEffetSourceData"></param>
        /// <returns></returns>
        private List<UIBasePopupView> SortPopupViewByPriority(List<UIBasePopupView> popupView, bool isEffetSourceData = false)
        {
            if (popupView == null || popupView.Count == 0)
            {
                return popupView;
            }

            List<UIBasePopupView> result;
            if (isEffetSourceData)
                result = popupView;
            else
            {
                result = new List<UIBasePopupView>();
                result.AddRange(popupView);
            }

            result.Sort((a, b) =>
           {
               if (a.Priority > b.Priority)
                   return 1;
               else if (a.Priority == b.Priority)
                   return 0;
               else return -1;
           });   //按照优先级从低到高排序
            return result;
        }



        #endregion

        #region Tips 类型的界面
        /// <summary>
        /// 打开弹窗 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="autoDestroyTime">自动销毁时间 为0标识不会自毁</param>
        /// <param name="parameter"></param>
        public void OpenTip(UIBaseTipView view, float autoDestroyTime, params object[] parameter)
        {
            if (view == null)
            {
                Debug.LogError("OpenTip Fail,The View Is Null");
                return;
            }

#if UNITY_EDITOR
            if (view.WindowType != WindowTypeEnum.PopTip)
            {
                Debug.LogError("OpenTip Fail,Not Page Window " + view.name);
                return;
            }
#endif

            view.ShowWindow(Helper.Instance. MegerParameter(autoDestroyTime, parameter));
            m_AllRecordViewTips.Add(view);
        }

        /// <summary>
        /// 关闭界面 (包括自动销毁了)
        /// </summary>
        /// <param name="view"></param>
        public void CloseTip(UIBaseTipView view)
        {
            int searchIndex = -1;
            for (int index = 0; index < m_AllRecordViewTips.Count; index++)
            {
                if (m_AllRecordViewTips[index] == view)
                {
                    searchIndex = index;
                    break;
                }
            }

            if (searchIndex != -1)
                m_AllRecordViewTips.RemoveAt(searchIndex);
            else
                Debug.LogError("CloseTip Fail  Not Exit " + view.name);
        }


        #endregion

        #region Widget类型界面
        /// <summary>
        /// 打开一个Widget 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="parentTrans"></param>
        /// <param name="showTime">=0 标识一直存在</param>
        /// <param name="isSingleton">=true 标识只会创建一个</param>
        /// <param name="parameter"></param>
        public void OpenWidget(UIBaseWidgetView view, Transform parentTrans, float showTime, bool isSingleton, params object[] parameter)
        {
            if (view == null)
            {
                Debug.LogError("OpenWidget Fail,The View Is Null");
                return;
            }

#if UNITY_EDITOR
            if (view.WindowType != WindowTypeEnum.Widget)
            {
                Debug.LogError("OpenWidget Fail,Not Page Window " + view.name);
                return;
            }
#endif

            if (parentTrans == null)
                parentTrans = UIManagerHelper.Instance.UICanvasTrans;

            if (m_AllWidgetView.ContainsKey(parentTrans))
            {

#if UNITY_EDITOR
                foreach (var item in m_AllWidgetView[parentTrans])
                {
                    if (item == null)
                        Debug.LogError("存在已经销毁的Widget  view=" + view + "  parentTrans=" + parentTrans);
                }
#endif
                #region 当前Widget 存在
                if (isSingleton)
                {
                    if (m_AllWidgetView[parentTrans].Count > 1)
                    {
                        Debug.LogError("OpenWidget  Fail,Not Singlton Widget" + view.name);
                        return;
                    }
                    if (m_AllWidgetView[parentTrans].Count == 0)
                        m_AllWidgetView[parentTrans].Add(view);
                }
                else
                {
                    m_AllWidgetView[parentTrans].Add(view);
                }
                ShowWidgetView(view, parentTrans, showTime, isSingleton, parameter);
                #endregion
            }
            else
            {
                #region 当前Widget 第一次创建
                List<UIBaseWidgetView> widgetViews = new List<UIBaseWidgetView>();
                widgetViews.Add(view);
                m_AllWidgetView.Add(parentTrans, widgetViews);

                ShowWidgetView(view, parentTrans, showTime, isSingleton, parameter);
                #endregion
            }

        }

        /// <summary>
        /// 显示Widget 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="parentTrans"></param>
        /// <param name="showTime"></param>
        /// <param name="isSingleton"></param>
        /// <param name="parameter"></param>
        private void ShowWidgetView(UIBaseWidgetView view, Transform parentTrans, float showTime, bool isSingleton, params object[] parameter)
        {
            if (view.rectransform.parent != parentTrans)
                view.rectransform.SetParent(parentTrans);
            view.rectransform.ResetRectTransProperty();
            if (view.IsOpen)
                view.FlushWindow(parameter);
            else
                view.ShowWindow(Helper.Instance.MegerParameter(new object[] { parentTrans, showTime, isSingleton }, parameter));
        }


        /// <summary>
        /// 关闭打开的Widget  
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isDestroyWidget"></param>
        public void CloseWidget(UIBaseWidgetView view,bool isDestroyWidget, params object[] parameter)
        {
            if(m_AllWidgetView.ContainsKey(view.BelongParent)==false)
            {
                Debug.LogError("CloseWidget Fail,Not Exit " + view.name);
                return;
            }

            int Index = -1;
            for (int dex=0;dex< m_AllWidgetView[view.BelongParent].Count;++dex)
            {
                if (m_AllWidgetView[view.BelongParent][dex]==view)
                {
                    Index = dex;
                    break;
                }
            }

            if (Index == -1)
            {
                Debug.LogError("CloseWidget Fail,Not Exit " + view.name);
                return;
            }

            view.HideWindow(isDestroyWidget, parameter);
            if (isDestroyWidget)
            {
                m_AllWidgetView[view.BelongParent].RemoveAt(Index);
            }

        }


        /// <summary>
        /// 关闭所有关联改物体的Widget
        /// </summary>
        /// <param name="targetTrans"></param>
        public void CloseAttachWidget(Transform targetTrans, bool isDestroyWidget, params object[] parameter)
        {
            if (m_AllWidgetView.ContainsKey(targetTrans) == false) return;
            for (int dex=0;dex< m_AllWidgetView[targetTrans].Count;++dex)
            {
                m_AllWidgetView[targetTrans][dex].HideWindow(isDestroyWidget, parameter);
            }

            if (isDestroyWidget)
                m_AllWidgetView[targetTrans].Clear();
        }

        #endregion

        #endregion

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
        public void CreateUIAsync<T>(string viewPath, Transform parentTrans, System.Action<T> callback, bool isActivate , bool isResetTransProperty = true, string viewName = "") where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                Debug.LogError("CreateUI Fail, View Path Is Not Avaliable");
                return;
            }
            ResourcesMgr.Instance.Instantiate(viewPath, parentTrans, LoadAssetModel.Async, (obj) =>
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
        /// 创建UI 异步加载方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewPath"></param>
        /// <param name="callback"></param>
        /// <param name="isActivate">是否生成后保持激活</param>
        /// <param name="isResetTransProperty">是否重置属性</param>
        /// <param name="viewName">默认为预制体名</param>
        public void CreateUISync<T>(string viewPath, Transform parentTrans, System.Action<T> callback, bool isActivate , bool isResetTransProperty = true, string viewName = "") where T : UIBaseView
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                Debug.LogError("CreateUI Fail, View Path Is Not Avaliable");
                return;
            }
            ResourcesMgr.Instance.Instantiate(viewPath, parentTrans, LoadAssetModel.Sync, (obj) =>
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
        public T GetExitUIInstance<T>( ) where T : UIBaseView
        {
            foreach (var item in m_AllExitView.Values)
            {
                if(item .GetType()==typeof(T))
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
        public void ForceGetUIAsync<T>(string viewPath, Transform parentTrans, System.Action<T> callback, bool isActivate , bool isResetTransProperty = true, string viewName = "") where T : UIBaseView
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
        public void ForceGetUISync<T>(string viewPath, Transform parentTrans, System.Action<T> callback, bool isActivate, bool isResetTransProperty = true, string viewName = "") where T : UIBaseView
        {
            if(string.IsNullOrEmpty(viewName))
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

            CreateUISync<T>(viewPath, parentTrans, callback, isActivate, isResetTransProperty, viewName);
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