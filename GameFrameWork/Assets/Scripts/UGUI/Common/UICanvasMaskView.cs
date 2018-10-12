using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 全屏Mask 
    /// </summary>
    public class UICanvasMaskView : UIBaseWidgetView
    {
        #region UI
        private Image m_imgWaitImage;
        #endregion

        #region Data 
        /// <summary>
        /// 记录所有请求打开这个界面的引用 Key=被哪个物体打开 value=请求次数
        /// </summary>
        private Dictionary<Transform, int> m_AllReferenceInfor = new Dictionary<Transform, int>();

        private const float S_DelayShowLoadingTime = 5f;  //延时5秒显示转圈
        private float m_RotateAngle = 100f;
   //     private Tweener m_WaitConectTweenner = null;
        private bool m_IsShowWaite = false;
#if UNITY_EDITOR
        private List<Transform> m_AllReference_EditorOnly = new List<Transform>();
        private List<int> m_AllReferenceCount_EditorOnly = new List<int>();

#endif

        #endregion




        #region Frame
        protected override void Awake()
        {
            base.Awake();
            this.InitView();
        }

        private void InitView()
        {
            Image imgWaitImage = transform.Find("WaitImage").gameObject.GetComponent<Image>();

            //**
            m_imgWaitImage = imgWaitImage;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke("ShowWaitConnectView");
            m_IsShowWaite = false;
            //if (m_WaitConectTweenner != null && m_WaitConectTweenner.IsPlaying())
            //    m_WaitConectTweenner.Complete();
        }


        public override void ShowWindow(params object[] parameter)
        {
            if (parameter != null && parameter.Length == 0)
            {
                Debug.LogError("全屏Mske 必须传入哪个物体请求打开这个界面的参数");
                return;
            }

            base.ShowWindow(parameter);
            if (parameter == null)
                AddReference(null);
            else
                AddReference(parameter[0] as Transform);
            ShowDefaultView();

            OnCompleteShowWindow();
        }

      


        public override void FlushWindow(params object[] parameter)
        {
            if (parameter != null && parameter.Length == 0)
            {
                Debug.LogError("全屏Mske 必须传入哪个物体请求打开这个界面的参数");
                return;
            }


            base.FlushWindow(parameter);
            if (parameter == null)
                AddReference(null);
            else
                AddReference(parameter[0] as Transform);
            ShowDefaultView();

            OnCompleteFlushWindow();
        }


        /// <summary>
        /// 需要至少一个参数  第一个参数bool 类型判断是强制关闭 //第二个参数是Transform 参数 // 第三个参数为bool 类型判断是释放一次引用还是多个
        /// </summary>
        /// <param name="parameter"></param>
        public override void HideWindow(params object[] parameter)
        {
            if (parameter == null || parameter.Length < 1)
            {
                Debug.LogError("全屏Mske 必须传入哪个物体请求打开这个界面的参数");
                return;
            }

            bool isForceClose = bool.Parse(parameter[0].ToString());
            if (isForceClose)
            {
                ReleasAllTransformReference();
            }
            else
            {
                Transform target = parameter[1] as Transform;
                bool isReleaseOnece = bool.Parse(parameter[2].ToString());
                if (isReleaseOnece)
                    ReleaseReference(target);
                else
                    ReleasTransformReference(target);
            }

            if (CheckIfNeedCloseView() == false) return;

            base.HideWindow(parameter);
            OnCompleteHideWindow();
        }
        #endregion

        #region 显示视图
        private void ShowDefaultView()
        {
            m_imgWaitImage.gameObject.SetActive(false);

            Invoke("ShowWaitConnectView", S_DelayShowLoadingTime);
        }



        private void ShowWaitConnectView()
        {
            m_imgWaitImage.gameObject.SetActive(true);
            m_IsShowWaite = true;
            //if (m_WaitConectTweenner != null && m_WaitConectTweenner.IsPlaying()) return;
            //m_WaitConectTweenner= m_imgWaitImage.transform.DORotate(Vector3.zero, 1f, RotateMode.FastBeyond360).SetLoops(0);
        }




        void Update()
        {
#if UNITY_EDITOR

            m_AllReference_EditorOnly.Clear();
            m_AllReference_EditorOnly.AddRange(m_AllReferenceInfor.Keys);

            m_AllReferenceCount_EditorOnly.Clear();
            m_AllReferenceCount_EditorOnly.AddRange(m_AllReferenceInfor.Values);
#endif

            if(m_IsShowWaite)
            {
                m_imgWaitImage.transform.Rotate(Vector3.back, Time.deltaTime * m_RotateAngle);
            }

        }

        #endregion

        #region  辅助 记录当前Mask 被哪些界面打开
        /// <summary>
        /// 转换Null  参数
        /// </summary>
        /// <param name="target"></param>
        private void CheckTargetTrans(ref Transform target)
        {
            if (target == null)
            {
                target = UIManagerHelper.Instance.UICanvasTrans;
                Debug.LogEditorInfor("全局引用Mask 自动转移到  UIManagerHelper.Instance.transform 上");
            }
        }

        /// <summary>
        /// 记录当前参数对象请求打开Mask 次数
        /// </summary>
        /// <param name="target"></param>
        private void AddReference(Transform target)
        {
            CheckTargetTrans(ref target);


            if (m_AllReferenceInfor.ContainsKey(target))
                m_AllReferenceInfor[target]++;
            else
                m_AllReferenceInfor.Add(target, 1);
        }


        /// <summary>
        /// 记录当前参数对象请求打开Mask 次数  释放引用
        /// </summary>
        /// <param name="target"></param>
        private void ReleaseReference(Transform target)
        {
            CheckTargetTrans(ref target);
            if (m_AllReferenceInfor.ContainsKey(target))
            {
                if (m_AllReferenceInfor[target] == 1)
                    m_AllReferenceInfor.Remove(target);
                else
                    m_AllReferenceInfor[target]--;
            }
        }


        /// <summary>
        /// 真真关闭界面时候需要判断一下是否可以关闭
        /// </summary>
        /// <returns></returns>
        private bool CheckIfNeedCloseView()
        {
            if (m_AllReferenceInfor == null || m_AllReferenceInfor.Count == 0) return true;
            return false;
        }

        /// <summary>
        /// 释放一个Transform 所有的引用
        /// </summary>
        /// <param name="target"></param>
        public void ReleasTransformReference(Transform target)
        {
            CheckTargetTrans(ref target);
            if (m_AllReferenceInfor.ContainsKey(target))
            {
                m_AllReferenceInfor.Remove(target);
            }
        }

        /// <summary>
        /// 强制释放所有的引用
        /// </summary>
        public void ReleasAllTransformReference()
        {
            m_AllReferenceInfor.Clear();
        }

        #endregion

    }
}