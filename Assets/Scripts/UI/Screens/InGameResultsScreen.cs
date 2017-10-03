using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class Stat
{
    //public Sprite m_Icon; // TODO
    public string m_Label;
    public string m_Value;

    public Stat(string label, string value)
    {
        m_Label = label;
        m_Value = value;
    }
}

public class StatsCollection
{
    public enum eStatType
    {
        Difficulty,
        HighScore,
        Score,
        CatchPercent,
        GoodCatches,
        GreatCatches,
        ExcellentCatches,
        ConsecutiveCatches,

        Count
    }

    private string[] m_StatLabels = new string[]
    {
        "Difficulty",
        "Best Score",
        "Round Score",
        "Catch Percentage",
        "Good Catches",
        "Great Catches",
        "Excellent Catches",
        "Consecutive Catches"
    };

    [SerializeField] private List<Stat> m_Stats;

    public StatsCollection()
    {
        int statCount = (int)eStatType.Count;
        m_Stats = new List<Stat>(statCount);

        for (int i = 0; i < statCount; i++)
        {
            m_Stats[i] = new Stat(m_StatLabels[i], string.Empty);
        }
    }

    public void UpdateStat(eStatType type, string value)
    {
        m_Stats[(int)type].m_Value = value;
    }
}

public class InGameResultsScreen : UIBaseScreen
{
    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ResultItemPrefab;

    private StatsCollection m_Collection;

    public override void Initialize()
    {
        base.Initialize();

        int modeIndex = (int)GameManager.Instance.m_Mode;
        GameModeData modeData = GameManager.Instance.ModeData[modeIndex];


        // UGH
        // TODO, These need to be moved to a StatsManager or something that is present during the actual game
        // then pass the collection in here to be displayed
        // perhaps the stats that are relevant can be stored in the game mode and tracked from there?

        InitStats();
    }

    private void InitStats()
    {
        m_Collection = new StatsCollection();
        m_Collection.UpdateStat(StatsCollection.eStatType.Difficulty, GameManager.Instance.m_Difficulty.ToString());

        int numStats = (int)StatsCollection.eStatType.Count;
        for (int i = 1; i < numStats; i++)
        {
            m_Collection.UpdateStat((StatsCollection.eStatType)i, "0");
        }
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);

        switch (data.actionId)
        {
            case RewiredConsts.Action.UI_Confirm: // Retry
                if (data.GetButtonDown())
                {
                    UIManager.Instance.LoadLevelWithScreen((int)eScene.DogPark, UI.Enums.ScreenId.GameReady);
                }
                break;

            case RewiredConsts.Action.UI_Cancel: // Exit
                if (data.GetButtonDown())
                {
                    UIManager.Instance.LoadLevelWithScreen((int)eScene.Main, UI.Enums.ScreenId.MainMenu);
                }
                break;
        }
    }
}
