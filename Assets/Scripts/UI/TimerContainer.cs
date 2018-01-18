using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerContainer : MonoBehaviour
{
    [SerializeField] private Text m_TimerText;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<UIEvents.UpdateTimeRemainingEvent>(OnTimeUpdated);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<UIEvents.UpdateTimeRemainingEvent>(OnTimeUpdated);
    }

    private void OnTimeUpdated(UIEvents.UpdateTimeRemainingEvent e)
    {
        // activate this UI element if it receives the event
        if (!m_TimerText.gameObject.activeInHierarchy)
        {
            m_TimerText.gameObject.SetActive(true);
        }

        m_TimerText.text = string.Format("{0:00}:{1:00}", e.SecondsRemaining / 60, e.SecondsRemaining % 60);
    }
}
