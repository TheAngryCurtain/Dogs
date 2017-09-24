using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissContainer : MonoBehaviour
{
    [SerializeField] private Transform m_Transform;
    [SerializeField] private GameObject m_MissIconPrefab;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<UIEvents.UpdateMissedCatchEvent>(UpdateMisses);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<UIEvents.UpdateMissedCatchEvent>(UpdateMisses);
    }

    private void UpdateMisses(UIEvents.UpdateMissedCatchEvent e)
    {
        for (int i = 0; i < m_Transform.childCount; i++)
        {
            Destroy(m_Transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < e.Misses; i++)
        {
            Instantiate(m_MissIconPrefab, m_Transform);
        }
    }
}
