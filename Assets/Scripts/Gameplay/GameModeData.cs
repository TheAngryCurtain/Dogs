using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameModeData
{
    public Sprite m_Icon;
    public string m_Name;

    public GameObject[] m_Prefabs;
    public LocationData[] m_Locations;

    [TextArea(3, 5)] public string m_Description;

    public GameSubModeData[] m_SubModes;
}

[System.Serializable]
public class GameSubModeData
{
    public string m_Name;
    public GameObject[] m_Prefabs;

    [TextArea(3, 5)] public string m_Description;
    [TextArea(5, 8)] public string m_Rules;
}