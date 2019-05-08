using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork.UGUI
{

    /// <summary>
    /// Widget 组件父类
    /// </summary>
    public class UIBaseWidgetView : UIBaseView
    {
        #region  Widget 公共操作
        private static Dictionary<Transform, List<UIBaseWidgetView>> m_AllWidgetView = new Dictionary<Transform, List<UIBaseWidgetView>>(); //当前显示的widget  

        #endregion

        #region WidgetFrame

        /// <summary>
        /// 当前弹窗属于哪个视图(可能是null)
        /// </summary>
        public Transform BelongParent { get; protected set; }

        /// <summary>
        /// 显示后多久自动隐藏 0 标识不隐藏
        /// </summary>
        public float HideDelayTime = 0.5f;

        /// <summary>
        /// 标识是否是单例的组件
        /// </summary>
        public bool IsSinglenton = false;

        protected bool m_IsDestroyedHide = false;
        #endregion

        #region WidgetFrame

        protected override void Awake()
        {
            m_WindowType = WindowTypeEnum.Widget;
            base.Awake();
        }


        public override void ShowWindow(UIParameterArgs parameter)
        {
#if UNITY_EDITOR
            if (parameter == null || parameter.ParemeterCount < 3)
            {
                Debug.LogEditorInfor("ShowWindow Fail, 没有传入当前弹窗所属的界面");
                return;
            }
#endif
            BelongParent = parameter.GetParameterByIndex(0) as Transform;
            HideDelayTime = float.Parse(parameter.GetParameterByIndex(1).ToString());
            IsSinglenton = bool.Parse(parameter.GetParameterByIndex(2).ToString());

            base.ShowWindow(parameter);
        }


        public override void HideWindow(UIParameterArgs parameter)
        {
            m_IsDestroyedHide = bool.Parse(parameter.GetParameterByIndex(0).ToString());
            base.HideWindow(parameter);
        }


        protected override void OnCompleteHideWindow()
        {
            base.OnCompleteHideWindow();

            if (m_IsDestroyedHide)
            {
                Debug.LogEditorInfor("当前组件在隐藏时候自动销毁自己");
                GameObject.Destroy(gameObject);
            }
        }

        #endregion


        #region  Widget 公共操作
        /// <summary>
        /// 打开一个Widget 
        /// </summary>
        /// <param name="parentTrans"></param>
        /// <param name="showTime">=0 标识一直存在</param>
        /// <param name="isSingleton">=true 标识只会创建一个</param>
        /// <param name="parameter"></param>
        public void OpenWidget(Transform parentTrans, float showTime, bool isSingleton, UIParameterArgs parameter)
        {
            if (parameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }

#if UNITY_EDITOR
            if (this.WindowType != WindowTypeEnum.Widget)
            {
                Debug.LogError("OpenWidget Fail,Not Page Window " + this.name);
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
                        Debug.LogError("存在已经销毁的Widget  view=" + this + "  parentTrans=" + parentTrans);
                }
#endif
                #region 当前Widget 存在
                if (isSingleton)
                {
                    if (m_AllWidgetView[parentTrans].Count > 1)
                    {
                        Debug.LogError("OpenWidget  Fail,Not Singlton Widget" + this.name);
                        return;
                    }
                    if (m_AllWidgetView[parentTrans].Count == 0)
                        m_AllWidgetView[parentTrans].Add(this);
                }
                else
                {
                    m_AllWidgetView[parentTrans].Add(this);
                }
                ShowWidgetView(this, parentTrans, showTime, isSingleton, parameter);
                #endregion
            }
            else
            {
                #region 当前Widget 第一次创建
                List<UIBaseWidgetView> widgetViews = new List<UIBaseWidgetView>();
                widgetViews.Add(this);
                m_AllWidgetView.Add(parentTrans, widgetViews);

                ShowWidgetView(this, parentTrans, showTime, isSingleton, parameter);
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
        private void ShowWidgetView(UIBaseWidgetView view, Transform parentTrans, float showTime, bool isSingleton, UIParameterArgs parameter)
        {
            if (view.rectransform.parent != parentTrans)
                view.rectransform.SetParent(parentTrans);
            view.rectransform.ResetRectTransProperty();

            UIParameterArgs parameter1 = UIParameterArgs.Create(parentTrans, showTime, isSingleton);
            view.ShowWindow(Helper.Instance.MegerUIParameter(parameter1, parameter));
        }


        /// <summary>
        /// 关闭打开的Widget  
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isDestroyWidget"></param>
        public void CloseWidget(bool isDestroyWidget, UIParameterArgs parameter)
        {
            if (m_AllWidgetView.ContainsKey(this.BelongParent) == false)
            {
                Debug.LogError("CloseWidget Fail,Not Exit " + this.name);
                return;
            }
            if (parameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }
            int Index = -1;
            for (int dex = 0; dex < m_AllWidgetView[this.BelongParent].Count; ++dex)
            {
                if (m_AllWidgetView[this.BelongParent][dex] == this)
                {
                    Index = dex;
                    break;
                }
            }

            if (Index == -1)
            {
                Debug.LogError("CloseWidget Fail,Not Exit " + this.name);
                return;
            }

            UIParameterArgs parameter1 = UIParameterArgs.Create(isDestroyWidget);
            this.HideWindow(Helper.Instance.MegerUIParameter(parameter1, parameter));
            if (isDestroyWidget)
            {
                m_AllWidgetView[this.BelongParent].RemoveAt(Index);
            }

        }


        /// <summary>
        /// 关闭所有关联改物体的Widget
        /// </summary>
        /// <param name="targetTrans"></param>
        public static void CloseAttachWidget(Transform targetTrans, bool isDestroyWidget, UIParameterArgs parameter)
        {
            if (m_AllWidgetView.ContainsKey(targetTrans) == false) return;
            if (parameter == null)
            {
                Debug.LogError("parameter 不要使用null  使用默认的 UIParameterArgs.Create() 无参数方法替代");
                return;
            }

            for (int dex = 0; dex < m_AllWidgetView[targetTrans].Count; ++dex)
            {
                UIParameterArgs parameter1 = UIParameterArgs.Create(isDestroyWidget);
                m_AllWidgetView[targetTrans][dex].HideWindow(Helper.Instance.MegerUIParameter(parameter1, parameter));
            }

            if (isDestroyWidget)
                m_AllWidgetView[targetTrans].Clear();
        }

        #endregion
    }
}