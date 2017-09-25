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
        VSEventManager.Instance.AddListener<GameplayEvents.LaunchDiscEvent>(LaunchDiscRequest);
        VSEventManager.Instance.AddListener<GameplayEvents.ResetDiscEvent>(ResetDiscRequest);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.OnDiscSpawnedEvent>(OnDiscSpawned);
        VSEventManager.Instance.RemoveListener<GameplayEvents.LaunchDiscEvent>(LaunchDiscRequest);
        VSEventManager.Instance.RemoveListener<GameplayEvents.ResetDiscEvent>(ResetDiscRequest);
    }

    private void LaunchDiscRequest(GameplayEvents.LaunchDiscEvent e)
    {
        Launch(e.MinDiscForce, e.MaxDiscForce, e.MaxDiscCurve);
    }

    private void ResetDiscRequest(GameplayEvents.ResetDiscEvent e)
    {
        Reset();
    }

    private void OnDiscSpawned(GameplayEvents.OnDiscSpawnedEvent e)
    {
        m_Disc = e.DiscObj.GetComponent<DiscController>();
        Reset();
    }

    private void Launch(float min, float max, float curve)
    {
        m_Disc.transform.position = m_DiscLaunch.position;

        float randThrow = UnityEngine.Random.Range(min, max);
        float randCurve = UnityEngine.Random.Range(-curve, curve);

        Debug.LogFormat("Difficulty: {0} > min: {1}, max: {2}, curve: {3} >>>>> rand throw: {4}, rand curve: {5}",
            GameManager.Instance.m_Difficulty.ToString(), min, max, curve, randThrow, randCurve);

        m_Disc.Throw(m_DiscLaunch.forward, randThrow, randCurve);
    }

    private void Reset()
    {
        m_Disc.Reset();
        m_Disc.transform.position = ResetPosition;
    }
}
