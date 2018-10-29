using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    public class UIMainMenuItem : MonoBehaviour
    {
        #region UI
        private Button m_btnMenuHead;
        private GameObject m_goRedTip;

        #endregion

        #region Data 
        private MainMenuEnum m_MainMenuEnum;
        private System.Action<UIMainMenuItem> m_ClickCallback;
        #endregion

        #region Frame
        private void Awake()
        {
            this.InitView();
        }

        private void InitView()
        {
            Button btnMenuHead = transform.Find("MenuHead").gameObject.GetComponent<Button>();
            GameObject goRedTip = transform.Find("MenuHead/RedTip").gameObject;

            //**
            m_btnMenuHead = btnMenuHead;
            m_goRedTip = goRedTip;

        }
        #endregion

        public void InitialedMenuItem(MainMenuEnum menuEnum, System.Action<UIMainMenuItem> clickCallback)
        {
            m_MainMenuEnum = menuEnum;
            m_ClickCallback = clickCallback;
        }


        private void ShowMenuView()
        {
            switch (m_MainMenuEnum)
            {
                case MainMenuEnum.God:
                    break;
                case MainMenuEnum.Union:
                    break;
                case MainMenuEnum.Knapsack:
                    break;
                case MainMenuEnum.Task:
                    break;
                case MainMenuEnum.Mall:
                    break;
                case MainMenuEnum.Friend:
                    break;
                case MainMenuEnum.Mail:
                    break;
                case MainMenuEnum.Rank:
                    break;
                case MainMenuEnum.TimeLimitActivity:
                    break;
                case MainMenuEnum.Welfare:
                    break;
                case MainMenuEnum.Activity:
                    break;
                case MainMenuEnum.FirstRecharge:
                    break;
                case MainMenuEnum.NewServerActivity:
                    break;
                case MainMenuEnum.Notice:
                    break;
                case MainMenuEnum.WorldBoss:
                    break;
                case MainMenuEnum.AreanAward:
                    break;
                default:
                    Debug.LogError("ShowMenuView Fail, 没有定义的类型 " + m_MainMenuEnum);
                    break;
            }
        }




    }
}