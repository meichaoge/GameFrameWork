using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// 计时器基类
    /// </summary>
    public  class Timer_CountDown: AbstractTimerRecord
    {
        public float m_DeadTime;  //倒计时时长
        public float m_LastRecordTime=0;


        public override void InitialTimer()
        {
            base.InitialTimer();
            m_DeadTime += m_InitialedTime;
        }

        public override void TimeTicked()
        {
            if (Time.realtimeSinceStartup- m_DeadTime >= 0)
            {
                if (m_CallbackAct != null)
                    m_CallbackAct(0, m_HashCode);
                TimeTickUtility.Instance.UnRegisterTimer_Delay(this);  //标记为要删除
                return;
            }

            m_LastRecordTime = Time.realtimeSinceStartup;
            if (m_LastRecordTime - m_InitialedTime >= m_SpaceTime)
            {
                if (m_CallbackAct != null)
                    m_CallbackAct(m_DeadTime- m_InitialedTime, m_HashCode);
                m_LastRecordTime -= m_SpaceTime;
            }

        }

    }
}