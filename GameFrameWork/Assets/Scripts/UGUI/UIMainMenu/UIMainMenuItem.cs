using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    public class UIMainMenuItem : UIBaseView
  {
       #region UI
     private Button m_btnMenuHead ;
private GameObject m_goRedTip ;

      #endregion

     #region Frame
    protected override void Awake()
    {
      base.Awake();
         this.InitView();
    }

    private void  InitView() {
      			Button btnMenuHead = transform.Find("MenuHead").gameObject.GetComponent<Button>();
			GameObject goRedTip = transform.Find("MenuHead/RedTip").gameObject;

      //**
     m_btnMenuHead=btnMenuHead;
m_goRedTip=goRedTip;

    }
    #endregion
  }
}