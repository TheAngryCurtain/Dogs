using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameMode m_GameMode;
    private bool m_Complete;

    public GameMode Mode { get { return m_GameMode; } }

    public void Setup(GameMode mode)
    {
        m_GameMode = mode;

        VSEventManager.Instance.AddListener<GameplayEvents.OnGameModeResetEvent>(ResetGame);
    }

    public void Init()
    {
        m_GameMode.Init();
    }

    private void ResetGame(GameplayEvents.OnGameModeResetEvent e)
    {
        Init();
    }

    private void Update()
    {
        if (m_GameMode != null && !m_Complete)
        {
            m_GameMode.Update();
            m_Complete = m_GameMode.CheckEndCondition();
            if (m_Complete)
            {
                m_GameMode.Complete();
            }
        }
    }
}
