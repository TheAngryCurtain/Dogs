using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class InGameReadyScreen : UIBaseScreen
{
    [SerializeField] private Text m_HeaderLabel;
    [SerializeField] private Text m_RulesLabel;

    public override void Initialize()
    {
        base.Initialize();

        int modeIndex = (int)GameManager.Instance.m_Mode;
        int subModeIndex = (int)GameManager.Instance.m_SubMode;

        switch (modeIndex)
        {
            case (int)ModesScreen.eMode.Catch:
                break;

            case (int)ModesScreen.eMode.Soccer:
                subModeIndex -= 2; // dumb enum offset. Need a way to fix this.
                break;
        }

        GameModeData modeData = GameManager.Instance.ModeData[modeIndex];
        GameSubModeData subModeData = modeData.m_SubModes[subModeIndex];

        m_HeaderLabel.text = string.Format("Welcome to {0}", modeData.m_Name);
        m_RulesLabel.text = subModeData.m_Rules;
    }
    

    // TODO all players will have to ready up here eventually

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);

        switch (data.actionId)
        {
            case RewiredConsts.Action.UI_Confirm:
                if (data.GetButtonDown())
                {
                    UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.GameHUD);
                }
                break;
        }
    }
}
