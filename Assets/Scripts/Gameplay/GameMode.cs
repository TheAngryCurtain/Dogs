using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode
{
    protected StatsCollection m_Collection;
    public StatsCollection Statistics { get { return m_Collection; } }

    public abstract void Init();
    public abstract void Update();
    public abstract bool CheckEndCondition();
    public abstract void Complete();
}

public class CatchMode : GameMode
{
    private int m_MaxNumDrops = 3;
    private int m_CurrentNumDrops;
    private float m_Score;
    private string[] m_Messages = new string[]
    {
        "Good!", "Great!", "Excellent!"
    };

    private int m_CountDownRemaining;

    public float m_MaxDiscDistanceForce = 65f;
    public float[] m_DifficultyMinDistForces = new float[] { 60f, 55f, 50f };
    public float[] m_DifficultyMinDiscCurves = new float[] { 0f, 0.1f, 0.25f };

    // stats
    private int[] m_Catches = new int[3];
    private int m_ConsecutiveCatches = 0;
    private bool m_PreviousWasMiss = false;
    private int m_TotalThrows = 0;

    public override void Init()
    {
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMissedCatchEvent(0));
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.ResetDiscEvent());

        VSEventManager.Instance.AddListener<GameplayEvents.DogCatchDiscEvent>(OnDogCatchDisc);
        VSEventManager.Instance.AddListener<GameplayEvents.DiscTouchGroundEvent>(OnDiscTouchGround);

        m_Collection = new StatsCollection();

        m_CountDownRemaining = 3;
        m_CurrentNumDrops = 0;
        m_Score = 0;

        StartCountDown();
    }

    // ugh, this is messy
    // making a intermediate function to be called with ActAfterDelay
    private void StartCountDown()
    {
        CountDown(m_CountDownRemaining, m_CountDownRemaining.ToString(), "Catch!", LaunchDisc);
    }

    // here is another one
    private void ReadyCountDown()
    {
        CountDown(1, "Ready?", "Catch!", LaunchDisc);
    }

    // TODO make this more general purpose so that it can be used for other things
    private void CountDown(int interations, string delayMessage, string endMessage, System.Action callback)
    {
        if (interations > 0)
        {
            VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent(delayMessage, 1f, InGameHudScreen.eMessageAlignment.Neutral));
            m_CountDownRemaining -= 1;

            Utils.Instance.ActAfterDelay(1f, StartCountDown);
        }
        else
        {
            VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent(endMessage, 1f, InGameHudScreen.eMessageAlignment.Neutral));
            callback();
        }
    }

    public override void Update()
    {
        
    }

    public override bool CheckEndCondition()
    {
        return m_CurrentNumDrops == m_MaxNumDrops;
    }

    public override void Complete()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.DogCatchDiscEvent>(OnDogCatchDisc);
        VSEventManager.Instance.RemoveListener<GameplayEvents.DiscTouchGroundEvent>(OnDiscTouchGround);

        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.GameResults);
    }

    public void OnDogCatchDisc(GameplayEvents.DogCatchDiscEvent e)
    {
        float baseScoreForCatch = 50f;
        float modifier = 1f;

        StatsCollection.eStatType catchType = StatsCollection.eStatType.GoodCatches;
        int catchesIndex = 0;
        switch (e.ActionType)
        {
            default:
            case IDog.eActionType.None:
                break;

            case IDog.eActionType.Jump:
                catchType = StatsCollection.eStatType.GreatCatches;
                catchesIndex = 1;
                modifier = 1.5f;
                break;

            case IDog.eActionType.Special:
                catchType = StatsCollection.eStatType.ExcellentCatches;
                catchesIndex = 2;
                modifier = 2f;
                break;
        }

        int messageIndex = (int)e.ActionType;
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent(m_Messages[messageIndex], 2f, InGameHudScreen.eMessageAlignment.Positive));

        m_Score += baseScoreForCatch * modifier;
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateScoreEvent(m_Score));

        // log catch type stat and count
        m_Catches[catchesIndex] += 1;
        m_Collection.UpdateStat(catchType, m_Catches[catchesIndex]);

        if (!m_PreviousWasMiss)
        {
            m_ConsecutiveCatches += 1;
            float previousHigh = m_Collection.GetStat(StatsCollection.eStatType.ConsecutiveCatches);
            if (m_ConsecutiveCatches > previousHigh)
            {
                m_Collection.UpdateStat(StatsCollection.eStatType.ConsecutiveCatches, m_ConsecutiveCatches);
            }
        }

        m_PreviousWasMiss = false;

        Utils.Instance.ActAfterDelay(3f, Reset);
    }

    private void Reset()
    {
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent("Reset!", 1f, InGameHudScreen.eMessageAlignment.Neutral));
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.ResetDiscEvent());

        Utils.Instance.ActAfterDelay(1f, ReadyCountDown);
    }

    private void LaunchDisc()
    {
        int difficultyIndex = (int)GameManager.Instance.m_Difficulty;
        float minForce = m_DifficultyMinDistForces[difficultyIndex];
        float curve = m_DifficultyMinDiscCurves[difficultyIndex];
        
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.LaunchDiscEvent(minForce, m_MaxDiscDistanceForce, curve));

        m_TotalThrows += 1;
    }

    public void OnDiscTouchGround(GameplayEvents.DiscTouchGroundEvent e)
    {
        m_CurrentNumDrops += 1;
        m_PreviousWasMiss = true;
        m_ConsecutiveCatches = 0;
        if (m_CurrentNumDrops < m_MaxNumDrops)
        {
            Utils.Instance.ActAfterDelay(3f, Reset);
        }
        else
        {
            Debug.Log("GAME OVER");

            // stats
            int totalCatches = 0;
            for (int i = 0; i < m_Catches.Length; i++)
            {
                totalCatches += m_Catches[i];
            }

            float catchPercent = totalCatches / (float)m_TotalThrows;
            m_Collection.UpdateStat(StatsCollection.eStatType.CatchPercent, catchPercent);

            // TODO
            // need to save high score so that it can be loaded and compared to round score

            m_Collection.UpdateStat(StatsCollection.eStatType.Score, m_Score);
            m_Collection.UpdateStat(StatsCollection.eStatType.Difficulty, (int)GameManager.Instance.m_Difficulty);
        }

        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent("Miss!", 1f, InGameHudScreen.eMessageAlignment.Negative));
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMissedCatchEvent(m_CurrentNumDrops));
    }
}