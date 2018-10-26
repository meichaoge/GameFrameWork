using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


namespace GameFrameWork
{
    /// <summary>
    /// 点击时候Button 的缩放效果
    /// </summary>
    public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        [SerializeField]
        private bool m_IsTargetEnable = true;//标识当前Button 是否是开启状态 未开启的音效默认音效不一致

        Vector3 m_InitialScale;
        //[Header("当==1 时候没有缩放效果")]
        //public float m_MaxScaleRate = 0.95f;
        [Header("当==1 时候没有缩放效果")]
        public float m_MaxScaleRatio = 0.95f;

        [Header("是都需要开启点击音效")]
        public bool m_IsNeedAudiouEffect = true;
        [Header("是否是作用在父节点上")]
        [SerializeField]
        private bool m_IsEffectParent = false;

        private float m_volume = 0.15f;//音量


        public Action<Vector3> onPointerDown;
        public Action<Vector3> onPointerUp;
        private AudioSource m_AudioSource;
        public ButtonAudioEnum BtnAudioEnum = ButtonAudioEnum.DefaultClickAudio;

        private void Awake()
        {
            m_AudioSource = transform.GetAddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            m_AudioSource.reverbZoneMix = 0;

            if (m_IsEffectParent)
                m_InitialScale = transform.parent.localScale;
            else
                m_InitialScale = transform.localScale;

        }
        private void Start() { }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("ButtonEffect OnPointerDown: " + gameObject.name);
            PlayAudioClip();
            if (m_MaxScaleRatio == 1f) return;

            if (m_IsEffectParent)
                transform.parent.localScale = m_InitialScale * m_MaxScaleRatio;
            else
                transform.localScale = m_InitialScale * m_MaxScaleRatio;

            if (onPointerDown != null)
            {
                onPointerDown.Invoke(m_InitialScale * m_MaxScaleRatio);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Debug.Log("ButtonEffect OnPointerUp: " + gameObject.name);
            if (m_MaxScaleRatio == 1f) return;
            if (m_IsEffectParent)
                transform.parent.localScale = m_InitialScale;
            else
                transform.localScale = m_InitialScale;

            if (onPointerUp != null)
            {
                onPointerUp.Invoke(m_InitialScale);
            }
        }

        private void PlayAudioClip()
        {
            if (m_IsNeedAudiouEffect == false) return;
            ButtonAudioConfInfor audio = AudioEffectSetting.Instance.GetButtonAudioConfigByType(BtnAudioEnum);

            if (m_AudioSource.clip == null || m_AudioSource.clip.name != audio.AudioName)
            {
                if (audio == null)
                {
                    Debug.LogError("PlayAudio  Fail");
                    return;
                }
                m_volume = audio.Volume;
                m_AudioSource.clip = ResourcesMgr.Instance.LoadAudioSync(audio.AudioPath, transform);
            
            }

            if (m_AudioSource.clip == null) return;
            m_AudioSource.loop = false;
            m_AudioSource.volume = m_volume;
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
        }



    }
}