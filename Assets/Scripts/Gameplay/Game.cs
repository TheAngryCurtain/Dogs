using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameMode m_GameMode;

    public void Setup(GameMode mode)
    {
        m_GameMode = mode;
        m_GameMode.Init();
    }

    private void Update()
    {
        if (m_GameMode != null)
        {
            m_GameMode.Update();
            bool complete = m_GameMode.CheckEndCondition();
            if (complete)
            {
                m_GameMode.Complete();

                // TODO
                // maybe transition to Summary Screen?
                // then go back to front end?
            }
        }
    }
}
