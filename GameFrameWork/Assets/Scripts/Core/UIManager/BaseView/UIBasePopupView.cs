using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 弹窗显示的时候对其他已经弹出的弹窗的操作
    /// </summary>
    public enum PopupOpenOperateEnum
    {
        /// <summary>
        /// 保持已经打开的弹窗的状态不改变
        /// </summary>
        KeepPreviousAvailable,
        /// <summary>
        /// 隐藏所有打开的弹窗
        /// </summary>
        HideAllOpenView,
        /// <summary>
        /// 关闭当前属于界面的弹窗
        /// </summary>
        HideCurPagePopupView,
        /// <summary>
        /// 按照优先级显示 (如果已经打开的弹窗优先级更高 则不显示)
        /// </summary>
        PriorityOrder,
    }

    /// <summary>
    /// 关闭弹窗时候的操作
    /// </summary>
    public enum PopupCloseOperateEnum
    {
        /// <summary>
        /// 只关闭自己 不管其他的状态
        /// </summary>
        Simple,
        /// <summary>
        /// 关闭当前界面时候 自动弹出可以弹出的界面
        /// </summary>
        AutoPopup,
        /// <summary>
        /// 关闭当前界面所有已经打开的界面 并且清空优先级不够的未弹出界面
        /// </summary>
        CloseAndClearCurPage, 

    }



    /// <summary>
    /// 弹窗的公共父类
    /// </summary>
    public class UIBasePopupView : UIBaseView
    {
        #region 弹窗的公共操作
        private static List<UIBasePopupView> m_AllRecordViewPopup = new List<UIBasePopupView>();  //按照打开的顺序记录了所有打开的弹窗UI
        private static Dictionary<UIBasePageView, List<UIBasePopupView>> m_LowerPriorityPopupView = new Dictionary<UIBasePageView, List<UIBasePopupView>>(); //当前弹窗优先级不够导致暂时无法显示的界面

        #endregion

        #region 弹窗Frame 数据

        /// <summary>
        /// 当前弹窗属于哪个界面 (可能是null)
        /// </summary>
        public UIBasePageView BelongPageView { get; protected set; }
        /// <summary>
        /// 当前弹窗的优先级 默认是0
        /// </summary>
        public uint Priority { get; protected set; }

        public UIParameterArgs WillPopupParameter { get; protected set; }
        #endregion

        #region  弹窗Frame

        protected override void Awake()
        {
            m_WindowType = WindowTypeEnum.PopUp;
            base.Awake();
        }

        public override void ShowWindow(UIParameterArgs parameter)
        {
#if UNITY_EDITOR
            if (parameter == null || parameter.ParemeterCount < 1)
            {
                Debug.LogEditorInfor("ShowWindow Fail, 没有传入当前弹窗所属的界面");
                return;
            }
#endif
      
            BelongPageView = parameter.GetParameterByIndex(0) as UIBasePageView;
            // object[] newParameter =new object[parameter.Length - 1];
            //   System.Array.Copy(parameter, 0, newParameter, 1, parameter.Length - 1);  //去掉第一个参数
            parameter.CutParameterToFront(0);  //去掉第一个参数
            base.ShowWindow(parameter);
        }

        /// <summary>
        /// 由于某个弹窗关闭 而显示一个因为优先级关闭而不显示的界面的情况
        /// 参数已经缓存在 WillPopupParameter 中
        /// </summary>
        public virtual void DelayPopupShowWindow()
        {
            if (IsActivate == false)
                gameObject.SetActive(true);
        }

        /// <summary>
        /// 记录由于优先级关闭而不能显示的弹窗
        /// </summary>
        public virtual void FailShowByPriority(UIParameterArgs  parameter)
        {
            WillPopupParameter = parameter;
            if (IsActivate)
                gameObject.SetActive(false);
        }

        #endregion

        #region 弹窗的公共操作
        /// <summary>
        /// 打开一个弹窗界面
        /// </summary>
        /// <param name="popupOperate"></param>
        /// <param name="belongPageView">当前弹窗属于哪个Page 管理 ,如果=null则表示属于当前打开的界面 否则指定的参数界面 </param>
        /// <param name="isFailRecord">如果是因为优先级而无法弹出 则表示是否需要记录 如果为false 则当无法显示时候不会自动弹出，而是隐藏了</param>
        /// <param name="parameter"></param>
        /// 
        public void OpenPopUp(PopupOpenOperateEnum popupOperate, UIBasePageView belongPageView, bool isFailRecord, UIParameterArgs parameter)
        {
            if (parameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }
#if UNITY_EDITOR
            if (this.WindowType != WindowTypeEnum.PopUp)
            {
                Debug.LogError("OpenPopUp Fail,Not Popup Window " + this.name);
                return;
            }
#endif
            Dictionary<int, UIBasePopupView> allWillHidePopupView = GetWillHidePopupView(this, popupOperate, isFailRecord, parameter);
            foreach (var item in allWillHidePopupView)
            {
                item.Value.HideWindow(UIParameterArgs.Create());
                m_AllRecordViewPopup.RemoveAt(item.Key);
            }
            if (belongPageView == null)
                belongPageView = UIBasePageView.CurOpenPage;

            UIParameterArgs args0 = UIParameterArgs.Create(belongPageView, isFailRecord);
            this.ShowWindow(Helper.Instance.MegerUIParameter(args0, parameter)); //注意这里的第一个参数
            m_AllRecordViewPopup.Add(this);
        }
        /// <summary>
        /// 打开一个弹窗界面 （属于当前页面的）
        /// </summary>
        /// <param name="popupOperate"></param>
        /// <param name="isFailRecord">如果是因为优先级而无法弹出 则表示是否需要记录 如果为false 则当无法显示时候不会自动弹出，而是隐藏了</param>
        /// <param name="parameter"></param>
        /// 
        public void OpenPopUp( PopupOpenOperateEnum popupOperate, bool isFailRecord, UIParameterArgs parameter)
        {
            OpenPopUp( popupOperate, UIBasePageView.CurOpenPage, isFailRecord, parameter);
        }
        /// <summary>
        /// 打开一个弹窗界面 （属于当前页面的）
        /// </summary>
        /// <param name="popupOperate"></param>
        /// <param name="isFailRecord">如果是因为优先级而无法弹出 则表示是否需要记录 如果为false 则当无法显示时候不会自动弹出，而是隐藏了</param>
        public void OpenPopUp(PopupOpenOperateEnum popupOperate, bool isFailRecord = true)
        {
            OpenPopUp(popupOperate, UIBasePageView.CurOpenPage, isFailRecord, UIParameterArgs.Create());
        }
        /// <summary>
        /// 打开一个弹窗界面 （默认按照优先级打开 ,如果优先级不够则不会弹出）
        /// </summary>
        /// <param name="popupOperate"></param>
        public void OpenPopUp(bool isFailRecord = true)
        {
            OpenPopUp(PopupOpenOperateEnum.PriorityOrder, UIBasePageView.CurOpenPage, isFailRecord, UIParameterArgs.Create());
        }
        



        /// <summary>
        /// 关闭一个弹窗界面
        /// </summary>
        /// <param name="view"></param>
        /// <param name="closeOperate"></param>
        public void ClosePopup( PopupCloseOperateEnum closeOperate)
        {
            this.HideWindow(UIParameterArgs.Create());
            m_AllRecordViewPopup.Remove(this);
            DealHidePopView( closeOperate);
        }
        /// <summary>
        /// 处理关闭当前弹窗时候的逻辑
        /// </summary>
        /// <param name="view"></param>
        /// <param name="closeOperate"></param>
        private void DealHidePopView( PopupCloseOperateEnum closeOperate)
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
                        if (dex > 0 && m_AllRecordViewPopup[dex] == this)
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
                        if (m_AllRecordViewPopup[dex] != this && m_AllRecordViewPopup[dex].BelongPageView == this.BelongPageView)
                            allPopupViewCurPage.Add(dex);
                    }

                    for (int dex = 0; dex < allPopupViewCurPage.Count; ++dex)
                    {
                        m_AllRecordViewPopup[dex].HideWindow(UIParameterArgs.Create());
                    }//关闭弹窗

                    allPopupViewCurPage.Reverse();
                    for (int dex = 0; dex < allPopupViewCurPage.Count; ++dex)
                    {
                        m_AllRecordViewPopup.RemoveAt(allPopupViewCurPage[dex]);
                    }//删除打开记录

                    if (m_LowerPriorityPopupView.ContainsKey(this.BelongPageView))
                        m_LowerPriorityPopupView[this.BelongPageView].Clear();
                    break;
                default:
                    Debug.LogError("没有处理的类型  " + closeOperate);
                    break;
            }

        }



        /// <summary>
        /// 打开一个弹窗时候根据需求 获取需要关闭的弹窗
        /// </summary>
        /// <param name="view"></param>
        /// <param name="popupOperate"></param>
        private Dictionary<int, UIBasePopupView> GetWillHidePopupView(UIBasePopupView popupView, PopupOpenOperateEnum popupOperate, bool isFailRecord, UIParameterArgs parameter)
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
                            popupView.HideWindow(UIParameterArgs.Create());
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
        public static void RemoveWillPopupView(UIBasePageView pageview)
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
        /// 关闭页面时候 关闭关联的已经打开的弹窗
        /// </summary>
        /// <param name="pageview"></param>
        public static void CloseConnectPopupView(UIBasePageView belongPageView)
        {
            if (belongPageView == null)
            {
                Debug.LogError("ClosePopupView Is Null Page");
                return;
            }

            for (int dex = m_AllRecordViewPopup.Count - 1; dex >= 0; --dex)
            {
                if (m_AllRecordViewPopup[dex].BelongPageView == belongPageView)
                {
                    m_AllRecordViewPopup[dex].HideWindow(UIParameterArgs.Create());
                    m_AllRecordViewPopup.RemoveAt(dex);
                }
            }
        }
        //public static void CloseAllPopupView()
        //{
        //    Debug.LogEditorInfor("TODO...");
        //}



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

                SortPopupViewByPriority(m_LowerPriorityPopupView[pageview]); //  //按照优先级从低到高排序
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
                if (m_LowerPriorityPopupView.TryGetValue(UIBasePageView.CurOpenPage, out willPopupView))
                {
                    if (willPopupView.Count == 0) return;
                    UIBasePopupView willShowPopupView = willPopupView[willPopupView.Count - 1]; //将要显示的弹窗
                    willShowPopupView.DelayPopupShowWindow();  //参数已经缓存
                    willPopupView.RemoveAt(willPopupView.Count - 1);  //移除队列
                    m_AllRecordViewPopup.Add(willShowPopupView);  //将要显示的弹窗
                } //取当前打开页面的队列表

            }  //可能存在关闭一个顶层弹窗后 后下面没有要显示的弹窗的情况
        }


     


        /// <summary>
        /// 根据弹窗的优先级排序列表(按照从低到高的优先级排序)
        /// </summary>
        /// <param name="popupView"></param>
        /// <returns></returns>
        private void SortPopupViewByPriority(List<UIBasePopupView> popupView)
        {
            if (popupView == null || popupView.Count == 0)
            {
                return ;
            }

            List<UIBasePopupView> result = new List<UIBasePopupView>(popupView.Count);
            result.AddRange(popupView);

            result.Sort((a, b) =>
            {
                if (a.Priority > b.Priority)
                    return 1;
                else if (a.Priority == b.Priority)
                    return 0;
                else return -1;
            });   //按照优先级从低到高排序
            popupView = result;
        }


        #endregion

    }
}