using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class LocationsScreen : UIBaseScreen
{
    [SerializeField] private UIMenu m_Menu;
    [SerializeField] private Text m_Description;
    [SerializeField] private Image m_Thumb;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemHighlighted += OnItemHighlighted;
        m_Menu.OnItemSelected += OnItemSelected;
        
        m_Menu.PopulateMenu();
    }

    public override void Shutdown()
    {
        m_Menu.OnItemHighlighted -= OnItemHighlighted;
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
        UIManager.Instance.LoadLevelWithScreen(index + 2, UI.Enums.ScreenId.GameReady);
    }

    public void OnItemHighlighted(UIMenuItemInfo item)
    {
        m_Description.text = item.m_Description;
        m_Thumb.sprite = item.m_ThumbnailSprite;
    }
}
