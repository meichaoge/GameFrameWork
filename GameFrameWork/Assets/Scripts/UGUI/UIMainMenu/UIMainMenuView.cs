using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    /// <summary>
    ///主界面UI
    /// </summary>
    public class UIMainMenuView : UIBasePageView
    {
        #region UI
        private Text m_txtPlayerNameText;
        private Transform m_tfBottomPart;
        private Transform m_tfTopMenuPart;
        private Transform m_tfLeftMenuPart;

        #endregion

        #region  Data 
        //默认的菜单
        private List<MainMenuEnum> m_DefaultBottomMenuEnums = new List<MainMenuEnum>();
        private List<MainMenuEnum> m_DefaultTopMenuEnums = new List<MainMenuEnum>();
        private List<MainMenuEnum> m_DefaultLeftMenuEnums = new List<MainMenuEnum>();

        //**实际显示的菜单
        private List<MainMenuEnum> m_BottomMenuEnums = new List<MainMenuEnum>();
        private List<MainMenuEnum> m_TopMenuEnums = new List<MainMenuEnum>();
        private List<MainMenuEnum> m_LeftMenuEnums = new List<MainMenuEnum>();

        #endregion

        #region Frame
        protected override void Awake()
        {
            base.Awake();
            this.InitView();
            GetAllDefaultMenuEnums_Bottom();
            GetAllDefaultMenuEnums_Top();
            GetAllDefaultMenuEnums_Left();

        }

        private void InitView()
        {
            Text txtPlayerNameText = transform.Find("Content/TopPart/PlayerNameText").gameObject.GetComponent<Text>();
            Transform tfBottomPart = transform.Find("Content/BottomPart");
            Transform tfTopMenuPart = transform.Find("Content/TopMenuPart");
            Transform tfLeftMenuPart = transform.Find("Content/LeftMenuPart");

            //**
            m_txtPlayerNameText = txtPlayerNameText;
            m_tfBottomPart = tfBottomPart;
            m_tfTopMenuPart = tfTopMenuPart;
            m_tfLeftMenuPart = tfLeftMenuPart;

        }

        public override void ShowWindow(UIParameterArgs parameter)
        {
            base.ShowWindow(parameter);

            GetWiilShowConfig(parameter, 0, m_DefaultBottomMenuEnums, ref m_BottomMenuEnums);
            GetWiilShowConfig(parameter, 1, m_DefaultTopMenuEnums, ref m_TopMenuEnums);
            GetWiilShowConfig(parameter, 2, m_DefaultLeftMenuEnums, ref m_LeftMenuEnums);


            CreateBottomMenuItems();
            CreateTopMenuItems();
            CreateLeftMenuItems();


            OnCompleteShowWindow();
        }

        #endregion

        #region  获取默认的配置
        /// <summary>
        /// 默认的底部菜单
        /// </summary>
        private void GetAllDefaultMenuEnums_Bottom()
        {
            m_DefaultBottomMenuEnums.Clear();
            m_DefaultBottomMenuEnums.Add(MainMenuEnum.God);
            m_DefaultBottomMenuEnums.Add(MainMenuEnum.Union);
            m_DefaultBottomMenuEnums.Add(MainMenuEnum.Knapsack);
            m_DefaultBottomMenuEnums.Add(MainMenuEnum.Task);
            m_DefaultBottomMenuEnums.Add(MainMenuEnum.Mall);
        }

        /// <summary>
        /// 默认的顶部菜单
        /// </summary>
        private void GetAllDefaultMenuEnums_Top()
        {
            m_DefaultTopMenuEnums.Clear();
            m_DefaultTopMenuEnums.Add(MainMenuEnum.TimeLimitActivity);
            m_DefaultTopMenuEnums.Add(MainMenuEnum.Welfare);
            m_DefaultTopMenuEnums.Add(MainMenuEnum.Activity);
            m_DefaultTopMenuEnums.Add(MainMenuEnum.FirstRecharge);
            m_DefaultTopMenuEnums.Add(MainMenuEnum.NewServerActivity);
            m_DefaultTopMenuEnums.Add(MainMenuEnum.Notice);
        }

        /// <summary>
        /// 默认的左侧菜单
        /// </summary>
        private void GetAllDefaultMenuEnums_Left()
        {
            m_DefaultLeftMenuEnums.Clear();
            m_DefaultLeftMenuEnums.Add(MainMenuEnum.Friend);
            m_DefaultLeftMenuEnums.Add(MainMenuEnum.Mail);
            m_DefaultLeftMenuEnums.Add(MainMenuEnum.Rank);
        }
        #endregion

        #region 创建视图
        //private void GetWillShowConfig(UIParameterArgs parameter)
        //{
        //    object infor = null;
        //    if (parameter.ParemeterCount >= 1)
        //    {
        //        m_BottomMenuEnums.Clear();
        //        infor = parameter.GetParameterByIndex(0);
        //        if (infor == null)
        //            m_BottomMenuEnums.AddRange(m_DefaultBottomMenuEnums);
        //        else
        //            m_BottomMenuEnums.AddRange(infor as List<MainMenuEnum>);
        //    }

        //    if (parameter.ParemeterCount >= 2)
        //    {
        //        m_TopMenuEnums.Clear();
        //        infor = parameter.GetParameterByIndex(1);
        //        if (infor == null)
        //            m_TopMenuEnums.AddRange(m_DefaultTopMenuEnums);
        //        else
        //            m_TopMenuEnums.AddRange(infor as List<MainMenuEnum>);
        //    }


        //    if (parameter.ParemeterCount >= 3)
        //    {
        //        m_LeftMenuEnums.Clear();
        //        infor = parameter.GetParameterByIndex(2);
        //        if (infor == null)
        //            m_LeftMenuEnums.AddRange(m_DefaultLeftMenuEnums);
        //        else
        //            m_LeftMenuEnums.AddRange(infor as List<MainMenuEnum>);
        //    }
        //}

        /// <summary>
        /// 解析参数个数填充默认的菜单
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="dex"></param>
        /// <param name="defaultMenu"></param>
        /// <param name="willShowMenu"></param>
        private void GetWiilShowConfig(UIParameterArgs parameter, int dex, List<MainMenuEnum> defaultMenu, ref List<MainMenuEnum> willShowMenu)
        {
            willShowMenu.Clear();
            if (parameter.ParemeterCount < dex + 1)
            {
                willShowMenu.AddRange(defaultMenu);
                return;
            } //参数不足
            object infor = parameter.GetParameterByIndex(dex);
            if (infor == null)
                willShowMenu.AddRange(defaultMenu);
            else
                willShowMenu.AddRange(infor as List<MainMenuEnum>);
        }


        /// <summary>
        /// 创建底部菜单
        /// </summary>
        private void CreateBottomMenuItems()
        {

        }

        /// <summary>
        /// 创建顶部菜单
        /// </summary>
        private void CreateTopMenuItems()
        {

        }

        /// <summary>
        /// 创建左侧菜单
        /// </summary>
        private void CreateLeftMenuItems()
        {

        }
        #endregion



    }
}