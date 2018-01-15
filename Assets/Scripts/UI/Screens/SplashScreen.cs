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

                    // audio
                    // for now, use a gameplay event because there is no proper effect for this yet
                    // randomly pick b or g
                    int random = UnityEngine.Random.Range((int)AudioManager.eGamePlayClip.Praise_B, (int)(AudioManager.eGamePlayClip.Praise_G + 1));
                    VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestGameplayAudioEvent(true, (AudioManager.eGamePlayClip)random));
                }
                break;
        }
    }
}
