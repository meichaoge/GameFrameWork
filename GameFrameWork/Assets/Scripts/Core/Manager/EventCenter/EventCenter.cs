using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameFrameWork
{
    /// <summary>
    /// 提供常用的定帧刷新/协程执行接口
    /// </summary>
    public class EventCenter : Singleton_Mono<EventCenter>
    {
        /// <summary>
        /// 更新速率
        /// </summary>
        public enum UpdateRate
        {
            NormalFrame = 1,
            DelayOneFrame = 2,
            DelayTwooFrame = 3,
        }

#region  Data

        public delegate void UpdateHandle();
        List<UpdateHandle> m_AllUpdateEvent_Normal = new List<UpdateHandle>();
        List<UpdateHandle> m_AllUpdateEvent_OneFrameDelay = new List<UpdateHandle>();
        List<UpdateHandle> m_AllUpdateEvent_TwoFrameDelay = new List<UpdateHandle>();

        //FixedUpodate
        List<UpdateHandle> m_AllFixedUpdateEvent_Normal = new List<UpdateHandle>();
        List<UpdateHandle> m_AllFixedUpdateEvent_OneFrameDelay = new List<UpdateHandle>();
        List<UpdateHandle> m_AllFixedUpdateEvent_TwoFrameDelay = new List<UpdateHandle>();

        #endregion

        #region  变量
        /// <summary>
        /// 当前系统更新了多少帧
        /// </summary>
        public int CurFrameCount { get; private set; }
 #endregion


        #region 编辑器下设置
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            gameObject.name = "EventCenter";
        }
#endif
        #endregion


        protected override void Awake()
        {
            base.Awake();
            Loom.GetInstance();
        }

        private int updateCountTime = 0;
        #region UpdateEvent
        private void Update()
        {
            ++CurFrameCount;
            if (m_AllUpdateEvent_Normal.Count > 0)
            {//Normal
                ExcuteEvent(ref m_AllUpdateEvent_Normal);
            }//if

            ++updateCountTime;
            if (updateCountTime % 2 == 0)
            {//OneFrame Dlaye
                if (m_AllUpdateEvent_OneFrameDelay.Count != 0)
                    ExcuteEvent(ref m_AllUpdateEvent_OneFrameDelay);
            }
            else if (updateCountTime % 3 == 0)
            {//Tworame Dlaye
                if (m_AllUpdateEvent_TwoFrameDelay.Count != 0)
                    ExcuteEvent(ref m_AllUpdateEvent_TwoFrameDelay);
            }
        }

        public void AddUpdateEvent(UpdateHandle handle, UpdateRate _rate)
        {
            if (_rate == UpdateRate.NormalFrame)
            {
                if (m_AllUpdateEvent_Normal.Contains(handle))
                    return;
                m_AllUpdateEvent_Normal.Add(handle);
            }//if
            else if (_rate == UpdateRate.DelayOneFrame)
            {
                if (m_AllUpdateEvent_OneFrameDelay.Contains(handle)) return;
                m_AllUpdateEvent_OneFrameDelay.Add(handle);
            }//if
            else if (_rate == UpdateRate.DelayTwooFrame)
            {
                if (m_AllUpdateEvent_TwoFrameDelay.Contains(handle)) return;
                m_AllUpdateEvent_TwoFrameDelay.Add(handle);
            }//if
        }

        public void RemoveUpdateEvent(UpdateHandle handle, UpdateRate _rate)
        {
            if (m_AllUpdateEvent_Normal.Contains(handle))
            {
                m_AllUpdateEvent_Normal.Remove(handle);
                return;
            }
            if (m_AllUpdateEvent_OneFrameDelay.Contains(handle))
            {
                m_AllUpdateEvent_OneFrameDelay.Remove(handle);
                return;
            }

            if (m_AllUpdateEvent_TwoFrameDelay.Contains(handle))
            {
                m_AllUpdateEvent_TwoFrameDelay.Remove(handle);
                return;
            }

        }

        #endregion

        private int fixedCountTime = 0;
        #region FixedUpdate
        private void FixedUpdate()
        {
            ++fixedCountTime;
            if (m_AllFixedUpdateEvent_Normal.Count > 0)
            {//Normal
                ExcuteEvent(ref m_AllFixedUpdateEvent_Normal);
            }//if


            if (fixedCountTime % 2 == 0)
            {//OneFrame Dlaye
                if (m_AllFixedUpdateEvent_OneFrameDelay.Count != 0)
                    ExcuteEvent(ref m_AllFixedUpdateEvent_OneFrameDelay);
            }
            else if (fixedCountTime % 3 == 0)
            {//Tworame Dlaye
                if (m_AllFixedUpdateEvent_TwoFrameDelay.Count != 0)
                    ExcuteEvent(ref m_AllFixedUpdateEvent_TwoFrameDelay);
            }
        }


        public void AddFixedUpdateEvent(UpdateHandle handle, UpdateRate _rate)
        {
            if (_rate == UpdateRate.NormalFrame)
            {
                if (m_AllFixedUpdateEvent_Normal.Contains(handle))
                    return;
                m_AllFixedUpdateEvent_Normal.Add(handle);
            }//if

            if (_rate == UpdateRate.DelayOneFrame)
            {
                if (m_AllFixedUpdateEvent_OneFrameDelay.Contains(handle)) return;
                m_AllFixedUpdateEvent_OneFrameDelay.Add(handle);
                return;
            }//if

            if (_rate == UpdateRate.DelayTwooFrame)
            {
                if (m_AllFixedUpdateEvent_TwoFrameDelay.Contains(handle)) return;
                m_AllFixedUpdateEvent_TwoFrameDelay.Add(handle);
                return;
            }//if
        }

        public void RemoveFixedUpdateEvent(UpdateHandle handle, UpdateRate _rate)
        {
            if (m_AllUpdateEvent_Normal.Contains(handle))
            {
                return;
            }

            if (m_AllFixedUpdateEvent_OneFrameDelay.Contains(handle))
            {
                m_AllFixedUpdateEvent_OneFrameDelay.Remove(handle);
                return;
            }

            if (m_AllFixedUpdateEvent_TwoFrameDelay.Contains(handle))
            {
                m_AllFixedUpdateEvent_TwoFrameDelay.Remove(handle);
                return;
            }

        }

        #endregion

        /// <summary>
        /// 执行所有的事件
        /// </summary>
        /// <param name="listData"></param>
        private void ExcuteEvent(ref List<UpdateHandle> listData)
        {
            if (listData == null || listData.Count == 0) return;

            for (int _dex = 0; _dex < listData.Count; ++_dex)
            {
                if (listData[_dex] != null)
                    listData[_dex]();
            }
        }

        #region Enumerator Action
        /// <summary>
        /// 延迟一定时间之间执行操作
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public void DelayDoEnumerator(float time, System.Action action)
        {
            StartCoroutine(DelayDoAction(time, action));
        }
        IEnumerator DelayDoAction(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            if (action != null)
                action();
        }



        /// <summary>
        /// 当前帧最后时刻执行操作
        /// Action will do At The Last of The Frame  
        /// </summary>
        /// <param name="action"></param>
        public void CurrentFameLastDoAction(System.Action action)
        {
            StartCoroutine(FramLastDoAction(action));
        }
        IEnumerator FramLastDoAction(System.Action action)
        {
            yield return new WaitForEndOfFrame();
            if (action != null)
                action();
        }



        /// <summary>
        /// 下一帧执行操作
        /// </summary>
        /// <param name="action"></param>
        public void NextFrameDoAction(System.Action action)
        {
            StartCoroutine(NextFramDoAction(action));
        }
        IEnumerator NextFramDoAction(System.Action action)
        {
            yield return null;
            if (action != null)
                action();
        }




        ///// <summary>
        ///// DownLoad Texture Action
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="filePath"></param>
        ///// <param name="finishDownAct"></param>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public IEnumerator DownLoadAction(string url, string filePath, Action<string, string> finishDownAct, DownLoadTetureHandler action)
        //{
        //    //++TextureDownLoadHepler.CurrentTaskCount;
        //    yield return null;
        //    WWW ww = new WWW(url);
        //    yield return ww;
        //    //  --TextureDownLoadHepler.CurrentTaskCount;
        //    if (ww.isDone && string.IsNullOrEmpty(ww.error))
        //    {
        //        Debug.Log("WWW DownLoad Success");
        //        byte[] data;
        //        yield return data = ww.texture.EncodeToJPG(); ;

        //        if (action != null)
        //            action(true, url, filePath, data, finishDownAct);
        //        yield break;
        //    }

        //    Debug.LogError("Texture DownLoad Fail!!! url=" + url + "   error=" + ww.error);
        //    if (action != null)
        //        action(false, url, filePath, null, finishDownAct);

        //}


        #endregion



    }
}
