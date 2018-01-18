using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameModeData
{
    public Sprite m_Icon;
    public string m_Name;

    [TextArea(3, 5)] public string m_Description;
    [TextArea(5, 8)] public string m_Rules;
}