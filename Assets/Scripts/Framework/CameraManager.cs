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

        GameManager.Instance.OnPlayerSpawned += OnDogSpawned;
        GameManager.Instance.OnDiscSpawned += OnDiscSpawned;
        GameManager.Instance.OnLevelLoaded += OnLevelLoaded;
    }

    public override void OnDestroy()
    {
        GameManager.Instance.OnPlayerSpawned -= OnDogSpawned;
        GameManager.Instance.OnDiscSpawned -= OnDiscSpawned;
        GameManager.Instance.OnLevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        m_CamController.enabled = true;
    }

    private void OnDogSpawned(Transform dog)
    {
        m_CamController.SetFollowTarget(dog);

        IDog d = dog.GetComponent<IDog>();
        if (d != null)
        {
            d.SetFollowCamera(m_Camera);
        }
    }

    private void OnDiscSpawned(Transform disc)
    {
        m_CamController.SetLockTarget(disc);
    }
}
