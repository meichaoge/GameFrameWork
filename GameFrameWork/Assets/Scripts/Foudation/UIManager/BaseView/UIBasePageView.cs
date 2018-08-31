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
   


        protected override void Awake()
        {
            m_WindowType = WindowTypeEnum.Page;
            base.Awake();
        }


      

    }
}