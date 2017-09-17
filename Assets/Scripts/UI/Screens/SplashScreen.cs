using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Rewired;

public class SplashScreen : UIBaseScreen
{
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);

        switch (data.actionId)
        {
            case RewiredConsts.Action.UI_Submit:
                if (data.GetButtonDown())
                {
                    UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.MainMenu);
                }
                break;
        }
    }
}
