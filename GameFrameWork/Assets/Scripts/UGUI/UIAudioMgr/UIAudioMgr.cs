using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UGUI
{
    /// <summary>
    /// 声音的频道信息
    /// </summary>
    public enum AudioChannelEnum
    {
        BgChannel = 1, //背景音 (只有一个)
        Channel2 = 2,//其他的音效 (这里可以定义多个)
    }

    public class UIAudioMgr : UIBaseView
    {
        #region UI
        private AudioSource m_AudBgAudio;
        private AudioSource m_AudOtheAudio1;

        #endregion

        #region Data
        private string m_AudBgAudioName;
        private string m_AudOtheAudioName;
        private float m_Volume;
        private bool m_IsLoop;
        private bool m_NeedReplay;
        #endregion

        #region Frame
        protected override void Awake()
        {
            base.Awake();
            this.InitView();
            m_WindowType = WindowTypeEnum.Widget;

        }

        private void InitView()
        {
            AudioSource AudBgAudio = transform.Find("BgAudio").gameObject.GetComponent<AudioSource>();
            AudioSource AudOtheAudio1 = transform.Find("OtheAudio1").gameObject.GetComponent<AudioSource>();

            //**
            m_AudBgAudio = AudBgAudio;
            m_AudOtheAudio1 = AudOtheAudio1;

        }


        public override void ShowWindow(params object[] parameter)
        {


            base.ShowWindow(parameter);
        }


        #endregion
    }
}