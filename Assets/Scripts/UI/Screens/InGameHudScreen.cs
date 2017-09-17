using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UnityEngine.SceneManagement;

public class InGameHudScreen : UIBaseScreen
{
    [SerializeField] private Slider m_ThrowMeter;
    [SerializeField] private Slider m_HydrationMeter;
    [SerializeField] private Text m_ScoreText;
    [SerializeField] private Text m_CongratsText;

    [SerializeField] private Color m_PositiveColor;
    [SerializeField] private Color m_NegativeColor;

    private float m_Score = 0f;
    private string[] m_Messages = new string[]
    {
            "Good!", "Great!", "Excellent!"
    };

    public override void Initialize()
    {
        base.Initialize();

        m_ThrowMeter.gameObject.SetActive(false);
        UpdateScore(0);
    }

    public void UpdateThrowMeter(float percent)
    {
        if (percent == 0f)
        {
            m_ThrowMeter.gameObject.SetActive(false);
        }
        else if (!m_ThrowMeter.gameObject.activeSelf)
        {
            m_ThrowMeter.gameObject.SetActive(true);
        }

        m_ThrowMeter.value = percent;
    }

    public void UpdateHydration(float percent)
    {
        m_HydrationMeter.value = percent;
    }

    public void UpdateScore(float amount)
    {
        m_Score += amount;
        m_ScoreText.text = string.Format("{0:0}", m_Score);
    }

    public void UpdateMessage(float screenTime, string msg, Color color)
    {
        m_CongratsText.color = color;
        m_CongratsText.text = msg;

        if (screenTime > 0f)
        {
            StartCoroutine(ClearMessage(screenTime));
        }
    }

    private IEnumerator ClearMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateMessage(-1, "", Color.white);
    }

    public void OnCatchMade(IDog.eActionType action)
    {
        float baseScoreForCatch = 50f;
        float modifier = 1f;
        switch (action)
        {
            default:
            case IDog.eActionType.None:
                break;

            case IDog.eActionType.Jump:
                modifier = 1.5f;
                break;

            case IDog.eActionType.Special:
                modifier = 2f;
                break;
        }

        UpdateMessage(2f, m_Messages[(int)action], m_PositiveColor);
        UpdateScore(baseScoreForCatch * modifier);
    }
}
