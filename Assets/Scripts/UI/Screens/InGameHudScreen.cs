using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class InGameHudScreen : UIBaseScreen
{
    public enum eMessageAlignment { Positive, Neutral, Negative };

    [SerializeField] private Slider m_ThrowMeter;
    [SerializeField] private Slider m_HydrationMeter;
    [SerializeField] private Text m_ScoreText;
    [SerializeField] private Text m_CongratsText;

    [SerializeField] private Color[] m_MessageColors = new Color[3];

    public override void Initialize()
    {
        base.Initialize();

        VSEventManager.Instance.AddListener<UIEvents.UpdateScoreEvent>(UpdateScore);
        VSEventManager.Instance.AddListener<UIEvents.UpdateMessageEvent>(UpdateMessage);
        VSEventManager.Instance.AddListener<UIEvents.UpdateHydrationEvent>(UpdateHydration);

        m_ThrowMeter.gameObject.SetActive(false);

        VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnGameStartEvent(GameManager.Instance.m_Mode, GameManager.Instance.m_SubMode));
    }

    public override void Shutdown()
    {
        base.Shutdown();

        VSEventManager.Instance.RemoveListener<UIEvents.UpdateScoreEvent>(UpdateScore);
        VSEventManager.Instance.RemoveListener<UIEvents.UpdateMessageEvent>(UpdateMessage);
        VSEventManager.Instance.RemoveListener<UIEvents.UpdateHydrationEvent>(UpdateHydration);
    }

    //public void UpdateThrowMeter(float percent)
    //{
    //    if (percent == 0f)
    //    {
    //        m_ThrowMeter.gameObject.SetActive(false);
    //    }
    //    else if (!m_ThrowMeter.gameObject.activeSelf)
    //    {
    //        m_ThrowMeter.gameObject.SetActive(true);
    //    }

    //    m_ThrowMeter.value = percent;
    //}

    private void UpdateHydration(UIEvents.UpdateHydrationEvent e)
    {
        m_HydrationMeter.value = e.HydrationPercent;
    }

    private void UpdateScore(UIEvents.UpdateScoreEvent e)
    {
        m_ScoreText.text = string.Format("{0:0}", e.TotalScore);
    }

    private void UpdateMessage(UIEvents.UpdateMessageEvent e)
    {
        m_CongratsText.color = m_MessageColors[(int)e.Alignment];
        m_CongratsText.text = e.Message;

        if (e.DisplayTime > 0f)
        {
            StartCoroutine(ClearMessage(e.DisplayTime));
        }
    }

    private IEnumerator ClearMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_CongratsText.text = string.Empty;
    }
}
