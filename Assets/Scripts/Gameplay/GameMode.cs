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

public class BaseCatchMode : GameMode
{
    protected const string HIGH_SCORE_SAVE_NAME = "DogsTM_HighScore";

    protected float m_Score;
    protected string[] m_Messages = new string[]
    {
        "Good!", "Great!", "Excellent!"
    };
    protected int m_CountDownRemaining;

    public float m_MaxDiscDistanceForce = 65f;
    public float[] m_DifficultyMinDistForces = new float[] { 60f, 55f, 50f };
    public float[] m_DifficultyMinDiscCurves = new float[] { 0f, 0.1f, 0.25f };

    // stats
    protected int[] m_Catches = new int[3];
    protected int m_ConsecutiveCatches = 0;
    protected bool m_PreviousWasMiss = false;
    protected int m_TotalThrows = 0;

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
            // audio
            VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, AudioManager.eGamePlayClip.Whistle));

            VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent(endMessage, 1f, InGameHudScreen.eMessageAlignment.Neutral));
            callback();

            StartRound();
        }
    }

    protected virtual void StartRound()
    {

    }

    public override void Update()
    {

    }

    public override void Init()
    {
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.ResetDiscEvent());
        VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestLevelAudioEvent(true));

        VSEventManager.Instance.AddListener<GameplayEvents.DogCatchDiscEvent>(OnDogCatchDisc);
        VSEventManager.Instance.AddListener<GameplayEvents.DiscTouchGroundEvent>(OnDiscTouchGround);

        m_Collection = new StatsCollection();

        m_CountDownRemaining = 3;
        //m_CurrentNumDrops = 0;
        m_Score = 0;

        StartCountDown();
    }

    protected virtual bool ShouldReset()
    {
        return true;
    }

    public override bool CheckEndCondition()
    {
        return !ShouldReset();
    }

    public override void Complete()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.DogCatchDiscEvent>(OnDogCatchDisc);
        VSEventManager.Instance.RemoveListener<GameplayEvents.DiscTouchGroundEvent>(OnDiscTouchGround);

        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.GameResults);
    }

    protected virtual void LaunchDisc()
    {
        int difficultyIndex = (int)GameManager.Instance.m_Difficulty;
        float minForce = m_DifficultyMinDistForces[difficultyIndex];
        float curve = m_DifficultyMinDiscCurves[difficultyIndex];

        VSEventManager.Instance.TriggerEvent(new GameplayEvents.LaunchDiscEvent(minForce, m_MaxDiscDistanceForce, curve));

        m_TotalThrows += 1;
    }

    protected virtual void OnDogCatchDisc(GameplayEvents.DogCatchDiscEvent e)
    {
        float baseScoreForCatch = 50f;
        float modifier = 1f;

        StatsCollection.eStatType catchType = StatsCollection.eStatType.GoodCatches;
        AudioManager.eGamePlayClip clapAmount = AudioManager.eGamePlayClip.Small_Clap;
        int catchesIndex = 0;
        switch (e.ActionType)
        {
            default:
            case IDog.eActionType.None:
                break;

            case IDog.eActionType.Jump:
                catchType = StatsCollection.eStatType.GreatCatches;
                clapAmount = AudioManager.eGamePlayClip.Med_Clap;
                catchesIndex = 1;
                modifier = 1.5f;
                break;

            case IDog.eActionType.Special:
                catchType = StatsCollection.eStatType.ExcellentCatches;
                clapAmount = AudioManager.eGamePlayClip.Big_Clap;
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

        // audio
        VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, clapAmount));

        Utils.Instance.ActAfterDelay(3f, Reset);
    }

    private void Reset()
    {
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent("Reset!", 1f, InGameHudScreen.eMessageAlignment.Neutral));
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.ResetDiscEvent());

        Utils.Instance.ActAfterDelay(1f, ReadyCountDown);
    }

    protected virtual void OnDiscTouchGround(GameplayEvents.DiscTouchGroundEvent e)
    {
        m_PreviousWasMiss = true;
        m_ConsecutiveCatches = 0;
        if (ShouldReset()) 
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

            // for now, save high score in playerprefs
            int highScore = PlayerPrefs.GetInt(HIGH_SCORE_SAVE_NAME);
            if (m_Score > highScore)
            {
                highScore = (int)m_Score;
                PlayerPrefs.SetInt(HIGH_SCORE_SAVE_NAME, highScore);
            }

            m_Collection.UpdateStat(StatsCollection.eStatType.HighScore, highScore);
            m_Collection.UpdateStat(StatsCollection.eStatType.Score, m_Score);
            m_Collection.UpdateStat(StatsCollection.eStatType.Difficulty, (int)GameManager.Instance.m_Difficulty);
        }

        // audio
        VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, AudioManager.eGamePlayClip.Disappointment));
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMessageEvent("Miss!", 1f, InGameHudScreen.eMessageAlignment.Negative));
    }
}

public class TimedCatchMode : BaseCatchMode
{
    private int m_MaxTimeSeconds = 120;
    private int m_CurrentSeconds;

    private float m_CurrentTime;
    private bool m_RoundStarted = false;
    private bool m_DiscInAir = false;

    public override void Init()
    {
        m_CurrentSeconds = m_MaxTimeSeconds;
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateTimeRemainingEvent(m_CurrentSeconds));

        base.Init();
    }

    protected override void StartRound()
    {
        base.StartRound();

        m_CurrentTime = Time.time;
        m_RoundStarted = true;
    }

    public override void Update()
    {
        if (m_RoundStarted && m_CurrentSeconds > 0 && Time.time - m_CurrentTime > 1f)
        {
            m_CurrentSeconds -= 1;
            VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateTimeRemainingEvent(m_CurrentSeconds));

            m_CurrentTime = Time.time;
        }

        base.Update();
    }

    protected override void LaunchDisc()
    {
        base.LaunchDisc();

        m_DiscInAir = true;
    }

    protected override void OnDiscTouchGround(GameplayEvents.DiscTouchGroundEvent e)
    {
        base.OnDiscTouchGround(e);

        m_DiscInAir = false;
    }

    protected override void OnDogCatchDisc(GameplayEvents.DogCatchDiscEvent e)
    {
        base.OnDogCatchDisc(e);

        m_DiscInAir = false;
    }

    public override bool CheckEndCondition()
    {
        return m_CurrentSeconds == 0 && !m_DiscInAir;
    }
}

public class StrikeCatchMode : BaseCatchMode
{
    private int m_MaxNumDrops = 3;
    private int m_CurrentNumDrops;

    public override void Init()
    {
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMissedCatchEvent(0));

        m_CountDownRemaining = 3;
        m_CurrentNumDrops = 0;
        m_Score = 0;

        base.Init();
    }

    protected override bool ShouldReset()
    {
        return m_CurrentNumDrops < m_MaxNumDrops;
    }

    public override bool CheckEndCondition()
    {
        return m_CurrentNumDrops == m_MaxNumDrops;
    }

    protected override void OnDiscTouchGround(GameplayEvents.DiscTouchGroundEvent e)
    {
        m_CurrentNumDrops += 1;
        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateMissedCatchEvent(m_CurrentNumDrops));

        base.OnDiscTouchGround(e);
    }
}