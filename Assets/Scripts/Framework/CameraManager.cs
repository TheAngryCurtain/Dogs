using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera m_Camera;

    private CameraController m_CamController;

    private void Start()
    {
        DontDestroyOnLoad(m_Camera);
        m_CamController = m_Camera.gameObject.GetComponent<CameraController>();
        m_CamController.enabled = false;

        VSEventManager.Instance.AddListener<GameplayEvents.OnPlayerSpawnedEvent>(OnDogSpawned);
        VSEventManager.Instance.AddListener<GameplayEvents.OnDiscSpawnedEvent>(OnDiscSpawned);
        VSEventManager.Instance.AddListener<GameplayEvents.OnLevelLoadedEvent>(OnLevelLoaded);
    }

    public override void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.OnPlayerSpawnedEvent>(OnDogSpawned);
        VSEventManager.Instance.RemoveListener<GameplayEvents.OnDiscSpawnedEvent>(OnDiscSpawned);
        VSEventManager.Instance.RemoveListener<GameplayEvents.OnLevelLoadedEvent>(OnLevelLoaded);
    }

    private void OnLevelLoaded(GameplayEvents.OnLevelLoadedEvent e)
    {
        m_CamController.enabled = true;
    }

    private void OnDogSpawned(GameplayEvents.OnPlayerSpawnedEvent e)
    {
        m_CamController.SetFollowTarget(e.PlayerObj.transform);

        IDog d = e.PlayerObj.GetComponent<IDog>();
        if (d != null)
        {
            d.SetFollowCamera(m_Camera);
        }
    }

    private void OnDiscSpawned(GameplayEvents.OnDiscSpawnedEvent e)
    {
        m_CamController.SetLockTarget(e.DiscObj.transform);
    }
}
