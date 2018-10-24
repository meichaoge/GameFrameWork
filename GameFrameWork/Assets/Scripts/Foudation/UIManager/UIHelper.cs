using GameFrameWork.UGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 提供UI常用的辅助接口
    /// </summary>
    public class UIHelper : Singleton_Static<UIHelper>
    {

        #region 显示/隐藏 UI界面

        /// <summary>
        /// 显示飘字提示
        /// </summary>
        /// <param name="content">需要显示的文本</param>
        /// <param name="antoDestroyTime">默认标识不销毁 ( 值不等于0标识显示后多少秒销毁)</param>
        public void ShowTipsViewSync(string content, float antoDestroyTime = 0)
        {
            UIManager.Instance.ForceGetUISync<UITextTipView>(Define_ResPath.UITextTipViewPath, UIManagerHelper.Instance.TipsParentTrans, (tipsView) =>
            {
                if (tipsView == null)
                {
                    Debug.LogError("ShowTipsViewSync  Fail,Not Exit View " + Define_ResPath.UITextTipViewPath);
                    return;
                }
                UIManager.Instance.OpenTip(tipsView, antoDestroyTime, UIParameterArgs.Create( content));
            }, false, true);
        }


        #endregion


       

    }
}