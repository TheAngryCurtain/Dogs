using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] m_MissBases;
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
        // active this UI element if it receives the event
        if (!m_MissBases[0].activeInHierarchy)
        {
            for (int i = 0; i < m_MissBases.Length; i++)
            {
                m_MissBases[i].SetActive(true);
            }
        }

        // update
        for (int i = 0; i < e.Misses; i++)
        {
            m_MissMarkers[i].SetActive(true);
        }
    }
}
