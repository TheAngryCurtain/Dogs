using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class ModesScreen : UIBaseScreen
{
    // TODO move these
    public enum eMode {
        None = -1,
        Catch,
        Soccer
    };

    public enum eSubMode {
        None = -1,
        Strikes, Timed,
        Targets, GoalKeeper
        };

    public enum eDifficulty {
        None = -1,
        Easy, Medium, Hard
    };

    [SerializeField] private UIMenu m_Menu;
    [SerializeField] private UIMenu m_ModeTypeSubMenu;
    [SerializeField] private UIMenu m_DifficultySubMenu;
    [SerializeField] private Text m_Description;

    private eMode m_PreviousMode = eMode.None;
    private eMode m_CurrentMode = eMode.None;
    private eSubMode m_PreviousSubMode = eSubMode.None;
    private eSubMode m_CurrentSubMode = eSubMode.None;
    private eDifficulty m_PreviousDifficulty = eDifficulty.None;
    private eDifficulty m_CurrentDifficulty = eDifficulty.None;
    private UIMenu m_ActiveMenu;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemHighlighted += OnMenuItemHighlighted;
        m_Menu.OnItemSelected += OnMenuItemSelected;

        //m_SubMenu.OnItemHighlighted += OnSubMenuItemHighlighted;
        m_ModeTypeSubMenu.OnItemSelected += OnModeTypeSubMenuSelected;
        m_ModeTypeSubMenu.OnItemHighlighted += OnModeTypeSubMenuHighlighted;

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
        m_ModeTypeSubMenu.OnItemSelected -= OnModeTypeSubMenuSelected;
        m_ModeTypeSubMenu.OnItemHighlighted -= OnModeTypeSubMenuHighlighted;

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

                    if (m_ActiveMenu == m_DifficultySubMenu)
                    {
                        m_CurrentDifficulty = m_PreviousDifficulty;
                        m_ActiveMenu = m_ModeTypeSubMenu;
                    }
                    else if (m_ActiveMenu == m_ModeTypeSubMenu)
                    {
                        m_CurrentMode = m_PreviousMode;
                        m_ActiveMenu = m_Menu;

                        m_HasSubSectionFocus = false;
                    }

                    //m_ActiveMenu.PopulateMenu();
                    m_ActiveMenu.RefocusMenu();

                    // audio
                    VSEventManager.Instance.TriggerEvent(new AudioEvents.RequestUIAudioEvent(true, AudioManager.eUIClip.Back));
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

        m_ActiveMenu = m_ModeTypeSubMenu;

        GameModeData currentData = GameManager.Instance.ModeData[index];
        m_ActiveMenu.PreSetMenuDataForSubModes(currentData);
        m_ActiveMenu.PopulateMenu();

        m_ActiveMenu.RefocusMenu();

        m_HasSubSectionFocus = true;
    }

    public void OnMenuItemHighlighted(UIMenuItemInfo item)
    {
        m_Description.text = item.m_Description;
    }

    public void OnModeTypeSubMenuSelected(int index)
    {
        // set this here for now, but will need game data structure later
        // this is now forced to single player for the time being
        //GameManager.Instance.m_NumOfPlayers = index + 1;
        int modesOffset = 2;
        GameManager.Instance.m_SubMode = (eSubMode)(index + modesOffset);

        m_ActiveMenu = m_DifficultySubMenu;
        m_ActiveMenu.PopulateMenu();
    }

    public void OnDifficultySubMenuSelected(int index)
    {
        m_PreviousDifficulty = m_CurrentDifficulty;
        m_CurrentDifficulty = (eDifficulty)index;

        GameManager.Instance.m_Mode = m_CurrentMode;
        GameManager.Instance.m_Difficulty = m_CurrentDifficulty;

        m_ActiveMenu.RemoveMenuFocus();
        //m_ActiveMenu.ClearMenu();

        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Locations);
    }

    public void OnModeTypeSubMenuHighlighted(UIMenuItemInfo item)
    {
        m_Description.text = item.m_Description;
    }
}
