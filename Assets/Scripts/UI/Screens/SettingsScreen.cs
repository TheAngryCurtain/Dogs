using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Rewired;

public class SettingsScreen : UIBaseScreen
{
    [SerializeField] private UIMenu m_Menu;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemSelected += OnItemSelected;
        m_Menu.PopulateMenu();
    }

    public override void Shutdown()
    {
        m_Menu.OnItemSelected -= OnItemSelected;

        base.Shutdown();
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);
        m_Menu.HandleInput(data);
    }

    public void OnItemSelected(int index)
    {
        Debug.Log(index);
        switch (index)
        {
            default:
                Debug.Log("TODO");
                break;
        }
    }
}
