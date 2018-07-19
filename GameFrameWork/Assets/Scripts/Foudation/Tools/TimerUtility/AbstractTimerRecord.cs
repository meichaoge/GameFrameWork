using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    ///  计时器基类
    /// </summary>
    public abstract class AbstractTimerRecord 
    {

        public System.Action<float, int> m_CallbackAct = null;  //回调 当前的计时以及Hashcode
        /// <summary>
        /// 唯一标识当前计时器对象
        /// </summary>
        public int m_HashCode { protected set; get; }
        public float m_SpaceTime;  //间隔
        protected float m_InitialedTime {  get; private set; } //初始化时候的时间


        public virtual void InitialTimer()
        {
            m_HashCode = GetHashCode();
            m_InitialedTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 每一帧调用一次
        /// </summary>
        public abstract void TimeTicked();

    }
}