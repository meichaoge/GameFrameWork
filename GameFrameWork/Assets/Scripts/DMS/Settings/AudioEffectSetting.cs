using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    /// <summary>
    /// 声音资源配置信息 (有些时候声音资源播放的路径/音量大小...是配置的 在这里读取)
    /// </summary>
    public class AudioEffectSetting : Singleton_Static<AudioEffectSetting>, ISetting
    {
        #region  ISetting 接口信息

        private bool m_IsEnable = false;
        /// <summary>
        /// 标识当前Setting  是否正确完成初始化
        /// </summary>
        public bool IsEnable { get { return m_IsEnable; } private set { m_IsEnable = value; } }

        #endregion

        #region 配置数据
        private Dictionary<ButtonAudioEnum, ButtonAudioConfInfor> m_AllButtonAudioConfigInfors = new Dictionary<ButtonAudioEnum, ButtonAudioConfInfor>();
        #endregion

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialSetting();
        }

        #region  读取和解析配置文件
        public void InitialSetting()
        {
            if (IsEnable)
            {
                Debug.LogError("已经进行了初始化");
                return;
            }

            ResourcesMgr.Instance.LoadFile(Define_Config.ButtonAudioConfigPath, LoadAssetModel.Sync, (dataStr) =>
            {
                if (string.IsNullOrEmpty(dataStr))
                {
                    IsEnable = false;
                    return;
                }
                AnalysisConfig(dataStr);
            });
        }

        private void AnalysisConfig(string data)
        {
            m_IsEnable = true;
            try
            {
                JsonData json = JsonMapper.ToObject(data);
                for (int dex = 0; dex < json.Count; ++dex)
                {
                    ButtonAudioConfInfor infor = new ButtonAudioConfInfor();
                    infor.ButtonAudio = (ButtonAudioEnum)System.Enum.Parse(typeof(ButtonAudioEnum), json[dex]["ButtonAudio"].ToString());
                    infor.AudioPath = json[dex]["AudioPath"].ToString();
                    infor.AudioName = json[dex]["AudioName"].ToString();
                    infor.Volume = float.Parse(json[dex]["Volume"].ToString());
                    infor.IsLoop = bool.Parse(json[dex]["IsLoop"].ToString());

                    if (m_AllButtonAudioConfigInfors.ContainsKey(infor.ButtonAudio))
                    {
                        Debug.LogError("AnalysisConfig Fail,重复的配置项 " + infor.ButtonAudio);
                        continue;
                    }
                    m_AllButtonAudioConfigInfors.Add(infor.ButtonAudio, infor);
                }
            }
            catch (Exception ex)
            {
                m_IsEnable = false;
                Debug.LogError(string.Format("解析配置文件{0}出错 {1}", Define_Config.ButtonAudioConfigPath, ex.ToString()));
            }
        }

        #endregion


        #region  接口


        /// <summary>
        /// 根据按钮的枚举类型获取数据
        /// </summary>
        /// <param name="buttonAudioEnum"></param>
        /// <returns></returns>
        public ButtonAudioConfInfor GetButtonAudioConfigByType(ButtonAudioEnum buttonAudioEnum)
        {
            ButtonAudioConfInfor result = null;
            if (m_AllButtonAudioConfigInfors.TryGetValue(buttonAudioEnum, out result))
                return result;

            Debug.LogError("GetButtonAudioConfigByType Fail,Not Find This Type:" + buttonAudioEnum);
            return null;
        }
        #endregion



    }
}