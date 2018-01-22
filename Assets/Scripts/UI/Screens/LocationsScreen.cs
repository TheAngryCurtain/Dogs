using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

[System.Serializable]
public class LocationData
{
    public eScene m_Scene;
    public string m_Name;
    public string m_Description;
    public Sprite m_Thumbnail;
}

public class LocationsScreen : UIBaseScreen
{
    [SerializeField] private UIMenu m_Menu;
    [SerializeField] private Text m_Description;
    [SerializeField] private Image m_Thumb;

    private GameModeData m_Data;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemHighlighted += OnItemHighlighted;
        m_Menu.OnItemSelected += OnItemSelected;

        m_Data = GameManager.Instance.ModeData[(int)GameManager.Instance.m_Mode];
        m_Menu.PreSetMenuDataForLocations(m_Data);
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
        int sceneID = (int)m_Data.m_Locations[index].m_Scene;
        UIManager.Instance.LoadLevelWithScreen(sceneID, UI.Enums.ScreenId.GameReady);
    }

    public void OnItemHighlighted(UIMenuItemInfo item)
    {
        m_Description.text = item.m_Description;
        m_Thumb.sprite = item.m_ThumbnailSprite;
    }
}
