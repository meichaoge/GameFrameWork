
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{

    /// <summary>
    /// 全局的计时器  (由于Time.timescale=0时候FixedUpdate() 不执行， Time.deltaTime=0，且因为缩放比例不一样导致计时会出错，所以需要使用Time.realtimeSinceStartup)
    /// </summary>
    public class TimeTickUtility : Singleton_Static<TimeTickUtility>
    {

        private static float _TimeStartUp = 0;
        /// <summary>
        /// 应用启动时候的时间
        /// </summary>
        public static float S_TimeStartUp
        {
            get { return _TimeStartUp; }
        }
        private Dictionary<int, AbstractTimerRecord> m_AllTimerCallback = new Dictionary<int, AbstractTimerRecord>();
        private List<AbstractTimerRecord> m_AllDeadTimers = new List<AbstractTimerRecord>();  //需要被注销的计时器

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            _TimeStartUp = Time.realtimeSinceStartup; //只赋值一次
        }

        /// <summary>
        /// 启动计时器 空操作是因为在 InitialSingleton 中开始计时了
        /// </summary>
        public void StartUpTimer()
        {
            Debug.LogInfor("计时器开始工作  启动时间：" + S_TimeStartUp);
        }


        public void Tick()
        {
            if (m_AllTimerCallback.Count == 0) return;
            if (m_AllDeadTimers.Count != 0)
            {
                for (int dex = 0; dex < m_AllDeadTimers.Count; ++dex)
                {
                    if (m_AllTimerCallback.ContainsKey(m_AllDeadTimers[dex].m_HashCode))
                        m_AllTimerCallback.Remove(m_AllDeadTimers[dex].m_HashCode);
                }
                m_AllDeadTimers.Clear();
            } //清理过时计时器

            foreach (var item in m_AllTimerCallback.Values)
            {
                item.TimeTicked();
            }
        }

        /// <summary>
        ///注册计时器
        /// </summary>
        /// <param name="spaceTime"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public int RegisterTimer(float spaceTime, System.Action<float,int> callback)
        {
            Timer_Normal recordInfor = new Timer_Normal();
            recordInfor.m_SpaceTime = spaceTime;
            recordInfor.m_CallbackAct = callback;
            recordInfor.m_StartRecordTime = Time.time;

            recordInfor.InitialTimer();

            if (m_AllTimerCallback.ContainsKey(recordInfor.m_HashCode))
            {
                Debug.LogError("RegisterTimer  Fail");
                return 0;
            }
            m_AllTimerCallback.Add(recordInfor.m_HashCode, recordInfor);
            return recordInfor.m_HashCode;
        }


        /// <summary>
        /// 注册倒计时计时器
        /// </summary>
        /// <param name="spaceTime"></param>
        /// <param name="deadTime">倒计时时长</param>
        /// <param name="callback"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public int RegisterCountDownTimer(float spaceTime, float deadTime, System.Action<float, int> callback)
        {
            Timer_CountDown recordInfor = new Timer_CountDown();
            recordInfor.m_SpaceTime = spaceTime;
            recordInfor.m_CallbackAct = callback;
            recordInfor.m_DeadTime = deadTime;

            recordInfor.InitialTimer();
            if (m_AllTimerCallback.ContainsKey(recordInfor.m_HashCode))
            {
                Debug.LogError("RegisterCountDownTimer  Fail");
                return 0;
            }
            m_AllTimerCallback.Add(recordInfor.m_HashCode, recordInfor);
            return recordInfor.m_HashCode;
        }

        /// <summary>
        /// 注销计时器
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public bool UnRegisterTimer(int hashcode)
        {
            if (hashcode == 0)
                Debug.LogError("UnRegisterTimer  注意可能不存在这个计时器");

            if (m_AllTimerCallback.ContainsKey(hashcode))
            {
                m_AllTimerCallback.Remove(hashcode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 标记为要删除的计时器
        /// </summary>
        /// <param name="timer"></param>
        public void UnRegisterTimer_Delay(AbstractTimerRecord timer)
        {
            m_AllDeadTimers.Add(timer);
        }

    }
}