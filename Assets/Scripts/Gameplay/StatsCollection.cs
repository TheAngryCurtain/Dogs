using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    //public Sprite m_Icon; // TODO
    public string m_Label;
    public float m_Value;

    public Stat(string label, float value)
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
            m_Stats.Add(new Stat(m_StatLabels[i], 0f));
        }
    }

    public void UpdateStat(eStatType type, float value)
    {
        m_Stats[(int)type].m_Value = value;
    }

    public float GetStat(eStatType type)
    {
        return m_Stats[(int)type].m_Value;
    }

    public string GetFormatedStat(eStatType type)
    {
        string s = string.Empty;
        switch (type)
        {
            case eStatType.Difficulty:
                int diff = (int)m_Stats[(int)type].m_Value;
                s = ((ModesScreen.eDifficulty)diff).ToString();
                break;

            case eStatType.CatchPercent:
                int percent = (int)(m_Stats[(int)type].m_Value * 100);
                s = string.Format("{0}%", percent);
                break;

            default:
                s = (m_Stats[(int)type].m_Value).ToString();
                break;
        }

        return s;
    }

    public string GetStatLabel(eStatType type)
    {
        return m_StatLabels[(int)type];
    }
}
