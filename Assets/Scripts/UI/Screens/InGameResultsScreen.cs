using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class InGameResultsScreen : UIBaseScreen
{
    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ResultItemPrefab;

    public override void Initialize()
    {
        base.Initialize();

        int modeIndex = (int)GameManager.Instance.m_Mode;
        GameModeData modeData = GameManager.Instance.ModeData[modeIndex];
        GameMode mode = GameManager.Instance.CurrentGame.Mode;

        PopulateFields(mode.Statistics);
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

                    // audio
                    VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Back));

                    // stop level audio
                    VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestLevelAudioEvent(false));

                }
                break;
        }
    }

    private void PopulateFields(StatsCollection collection)
    {
        int count = (int)StatsCollection.eStatType.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject resultsItemObj = (GameObject)Instantiate(m_ResultItemPrefab, m_ContentContainer);
            UIResultItem resultsItem = resultsItemObj.GetComponent<UIResultItem>();
            if (resultsItem != null)
            {
                StatsCollection.eStatType type = (StatsCollection.eStatType)i;
                resultsItem.Set(null, collection.GetStatLabel(type), collection.GetFormatedStat(type));
            }
        }
    }
}
