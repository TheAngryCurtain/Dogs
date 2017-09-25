using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class ModesScreen : UIBaseScreen
{
    // TODO move these
    public enum eMode { None = -1, Catch };
    public enum eDifficulty { None = -1, Easy, Medium, Hard};

    [SerializeField] private UIMenu m_Menu;
    [SerializeField] private UIMenu m_PlayersSubMenu;
    [SerializeField] private UIMenu m_DifficultySubMenu;
    [SerializeField] private Text m_Description;

    private eMode m_PreviousMode = eMode.None;
    private eMode m_CurrentMode = eMode.None;
    private eDifficulty m_PreviousDifficulty = eDifficulty.None;
    private eDifficulty m_CurrentDifficulty = eDifficulty.None;
    private UIMenu m_ActiveMenu;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemHighlighted += OnMenuItemHighlighted;
        m_Menu.OnItemSelected += OnMenuItemSelected;

        //m_SubMenu.OnItemHighlighted += OnSubMenuItemHighlighted;
        m_PlayersSubMenu.OnItemSelected += OnPlayerSubMenuSelected;
        m_DifficultySubMenu.OnItemSelected += OnDifficultySubMenuSelected;

        m_Menu.PreSetMenuDataForModes(GameManager.Instance.ModeData);
        m_Menu.PopulateMenu();

        m_ActiveMenu = m_Menu;
    }

    public override void Shutdown()
    {
        m_Menu.OnItemHighlighted -= OnMenuItemHighlighted;
        m_Menu.OnItemSelected -= OnMenuItemSelected;

        //m_SubMenu.OnItemHighlighted -= OnSubMenuItemHighlighted;
        m_PlayersSubMenu.OnItemSelected -= OnPlayerSubMenuSelected;
        m_DifficultySubMenu.OnItemSelected -= OnDifficultySubMenuSelected;

        base.Shutdown();
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);

        switch (data.actionId)
        {
            case RewiredConsts.Action.UI_Cancel:
                if (data.GetButtonDown())
                {
                    m_ActiveMenu.RemoveMenuFocus();
                    m_ActiveMenu.ClearMenu();

                    if (m_ActiveMenu == m_PlayersSubMenu)
                    {
                        m_CurrentMode = m_PreviousMode;
                        m_ActiveMenu = m_DifficultySubMenu;
                    }
                    else if (m_ActiveMenu == m_DifficultySubMenu)
                    {
                        m_CurrentDifficulty = m_PreviousDifficulty;
                        m_ActiveMenu = m_Menu;

                        m_HasSubSectionFocus = false;
                    }

                    //m_ActiveMenu.PopulateMenu();
                    m_ActiveMenu.RefocusMenu();
                }
                break;
        }

        m_ActiveMenu.HandleInput(data);
    }

    public void OnMenuItemSelected(int index)
    {
        m_PreviousMode = m_CurrentMode;
        m_CurrentMode = (eMode)index;
        m_ActiveMenu.RemoveMenuFocus();

        m_ActiveMenu = m_DifficultySubMenu;
        m_ActiveMenu.PopulateMenu();
        m_ActiveMenu.RefocusMenu();

        m_HasSubSectionFocus = true;
    }

    public void OnMenuItemHighlighted(UIMenuItemInfo item)
    {
        m_Description.text = item.m_Description;
    }

    public void OnPlayerSubMenuSelected(int index)
    {
        // set this here for now, but will need game data structure later
        GameManager.Instance.m_NumOfPlayers = index + 1;
        GameManager.Instance.m_Mode = m_CurrentMode;
        GameManager.Instance.m_Difficulty = m_CurrentDifficulty;

        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Locations);
    }

    public void OnDifficultySubMenuSelected(int index)
    {
        m_PreviousDifficulty = m_CurrentDifficulty;
        m_CurrentDifficulty = (eDifficulty)index;

        m_ActiveMenu.RemoveMenuFocus();
        //m_ActiveMenu.ClearMenu();

        m_ActiveMenu = m_PlayersSubMenu;
        m_ActiveMenu.PopulateMenu();
    }

    public void OnSubMenuItemHighlighted(UIMenuItemInfo item)
    {
        //m_Description.text = item.m_Description;
    }
}
