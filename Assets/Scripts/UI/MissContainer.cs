using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] m_MissMarkers;

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
        for (int i = 0; i < e.Misses; i++)
        {
            m_MissMarkers[i].SetActive(true);
        }
    }
}
