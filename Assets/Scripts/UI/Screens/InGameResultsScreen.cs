using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class InGameResultsScreen : UIBaseScreen
{
    public override void Initialize()
    {
        base.Initialize();

        int modeIndex = (int)GameManager.Instance.m_Mode;
        GameModeData modeData = GameManager.Instance.ModeData[modeIndex];
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);

        switch (data.actionId)
        {
            case RewiredConsts.Action.UI_Confirm: // Retry
                if (data.GetButtonDown())
                {
                    UIManager.Instance.LoadLevelWithScreen((int)eScene.DogPark, UI.Enums.ScreenId.GameReady); // TODO set this up a better way. 1 is dog park
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
