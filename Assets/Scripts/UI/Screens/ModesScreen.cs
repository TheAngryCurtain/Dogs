using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class ModesScreen : UIBaseScreen
{
    public enum eMode { None = -1, Catch };

    [SerializeField] private UIMenu m_Menu;
    [SerializeField] private UIMenu m_SubMenu;
    [SerializeField] private Text m_Description;

    private eMode m_PreviousMode = eMode.None;
    private eMode m_CurrentMode = eMode.None;
    private UIMenu m_ActiveMenu;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemHighlighted += OnMenuItemHighlighted;
        m_Menu.OnItemSelected += OnMenuItemSelected;

        //m_SubMenu.OnItemHighlighted += OnSubMenuItemHighlighted;
        m_SubMenu.OnItemSelected += OnSubMenuItemSelected;

        m_Menu.PopulateMenu();

        m_ActiveMenu = m_Menu;
    }

    public override void Shutdown()
    {
        m_Menu.OnItemHighlighted -= OnMenuItemHighlighted;
        m_Menu.OnItemSelected -= OnMenuItemSelected;

        //m_SubMenu.OnItemHighlighted -= OnSubMenuItemHighlighted;
        m_SubMenu.OnItemSelected -= OnSubMenuItemSelected;

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
                    if (m_CurrentMode != eMode.None)
                    {
                        m_CurrentMode = m_PreviousMode;
                        m_ActiveMenu.RemoveMenuFocus();

                        m_ActiveMenu.ClearMenu();

                        m_ActiveMenu = m_Menu;
                        m_ActiveMenu.RefocusMenu();

                        m_HasSubSectionFocus = false;
                    }
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

        m_ActiveMenu = m_SubMenu;
        m_ActiveMenu.PopulateMenu();
        m_ActiveMenu.RefocusMenu();

        m_HasSubSectionFocus = true;
    }

    public void OnMenuItemHighlighted(UIMenuItemInfo item)
    {
        m_Description.text = item.m_Description;
    }

    public void OnSubMenuItemSelected(int index)
    {
        // set this here for now, but will need game data structure later
        GameManager.Instance.m_NumOfPlayers = index + 1;
        GameManager.Instance.m_Mode = m_CurrentMode;

        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Locations);
    }

    public void OnSubMenuItemHighlighted(UIMenuItemInfo item)
    {
        //m_Description.text = item.m_Description;
    }
}
