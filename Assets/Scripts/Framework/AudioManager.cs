using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum eGamePlayClip { Whistle, Fanfare, Small_Clap, Med_Clap, Big_Clap, Praise_B, Praise_G, Disappointment };
    public enum eUIClip { Navigate, Confirm, Back }

    [SerializeField] private AudioSource m_LoopSource;
    [SerializeField] private AudioSource m_PointSource;

    [SerializeField] private AudioClip[] m_AmbientLoops;
    [SerializeField] private AudioClip[] m_GameplayClips;
    [SerializeField] private AudioClip[] m_UIClips;

    private void Start()
    {
        VSEventManager.Instance.AddListener<AudioEvents.RequestLevelAudioEvent>(OnLevelAudioRequested);
        VSEventManager.Instance.AddListener<AudioEvents.RequestGameplayAudioEvent>(OnGameplayAudioRequested);
        VSEventManager.Instance.AddListener<AudioEvents.RequestUIAudioEvent>(OnUIAudioRequested);
    }

    private void OnLevelAudioRequested(AudioEvents.RequestLevelAudioEvent e)
    {
        if (e.Play)
        {
            // get the ambient loop for the current level
            AudioClip levelClip = m_AmbientLoops[GameManager.Instance.m_LocationIndex - 2]; // 2 because it's scene 2, but location 0

            m_LoopSource.clip = levelClip;
            m_LoopSource.Play();
        }
        else
        {
            m_LoopSource.Stop();
        }
    }

    private void OnGameplayAudioRequested(AudioEvents.RequestGameplayAudioEvent e)
    {
        if (e.Play)
        {
            AudioClip gameplayClip = m_GameplayClips[(int)e.GameplayClip];

            m_PointSource.clip = gameplayClip;
            m_PointSource.Play();
        }
        else
        {
            m_PointSource.Stop();
        }
    }

    private void OnUIAudioRequested(AudioEvents.RequestUIAudioEvent e)
    {
        if (e.Play)
        {
            AudioClip uiClip = m_UIClips[(int)e.UIClip];

            m_PointSource.clip = uiClip;
            m_PointSource.Play();
        }
        else
        {
            m_PointSource.Stop();
        }
    }
}
