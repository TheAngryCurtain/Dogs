using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode
{
    public abstract void Init();
    public abstract void Update();
    public abstract bool CheckEndCondition();
    public abstract void Complete();
}

public class CatchMode : GameMode
{
    private int m_MaxNumDrops = 3;
    private int m_CurrentNumDrops = 0;
    private float m_Score = 0;
    private string[] m_Messages = new string[]
    {
        "Good!", "Great!", "Excellent!"
    };

    public override void Init()
    {
        m_CurrentNumDrops = m_MaxNumDrops;

        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMissedCatchEvent(m_MaxNumDrops)); // TODO change this to zero once the flow of the mode is set up

        VSEventManager.Instance.AddListener<GameplayEvents.DogCatchDiscEvent>(OnDogCatchDisc);
        VSEventManager.Instance.AddListener<GameplayEvents.DiscTouchGroundEvent>(OnDiscTouchGround);
    }

    public override void Update()
    {
        
    }

    public override bool CheckEndCondition()
    {
        return m_CurrentNumDrops == 0;
    }

    public override void Complete()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.DogCatchDiscEvent>(OnDogCatchDisc);
        VSEventManager.Instance.RemoveListener<GameplayEvents.DiscTouchGroundEvent>(OnDiscTouchGround);
    }

    // Mode specific listening functions

    public void OnDogCatchDisc(GameplayEvents.DogCatchDiscEvent e)
    {
        float baseScoreForCatch = 50f;
        float modifier = 1f;
        switch (e.ActionType)
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

        int messageIndex = (int)e.ActionType;
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent(m_Messages[messageIndex], 2f, InGameHudScreen.eMessageAlignment.Positive));

        m_Score += baseScoreForCatch * modifier;
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateScoreEvent(m_Score));
    }

    public void OnDiscTouchGround(GameplayEvents.DiscTouchGroundEvent e)
    {
        if (m_CurrentNumDrops > 0)
        {
            m_CurrentNumDrops -= 1;
        }

        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMissedCatchEvent(m_CurrentNumDrops));
    }
}