using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscLauncher : MonoBehaviour
{
    [SerializeField] private Transform m_DiscReset;
    public Vector3 ResetPosition { get { return m_DiscReset.position; } }

    [SerializeField] private Transform m_DiscLaunch;

    private DiscController m_Disc;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameplayEvents.OnDiscSpawnedEvent>(OnDiscSpawned);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.OnDiscSpawnedEvent>(OnDiscSpawned);
    }

    private void OnDiscSpawned(GameplayEvents.OnDiscSpawnedEvent e)
    {
        m_Disc = e.DiscObj.GetComponent<DiscController>();
    }

    private void Launch()
    {
        m_Disc.transform.position = m_DiscLaunch.position;

        //float randThrow = UnityEngine.Random.Range(100f, 115f);
        //float randCurve = UnityEngine.Random.Range(-0.3f, 0.3f);

        //Debug.LogFormat("Throw Force: {0}, Curve Amount: {1}", randThrow, randCurve);

        m_Disc.Throw(m_DiscLaunch.forward, 65f, 0f);
    }

    private void Reset()
    {
        m_Disc.Reset();
        m_Disc.transform.position = ResetPosition;
    }
}
