using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultItem : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private Text m_Label;
    [SerializeField] private Text m_Value;

    public void Set(Sprite icon, string label, string value)
    {
        m_Icon.sprite = icon;
        m_Label.text = label;
        m_Value.text = value;
    }
}
