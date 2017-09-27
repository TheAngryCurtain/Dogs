﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eScene { Boot, Main, DogPark };

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        VSEventManager.Instance.AddListener<GameplayEvents.RequestSceneChangeEvent>(OnSceneChangeRequested);
    }

    private void OnDestroy()
    {
        VSEventManager.Instance.RemoveListener<GameplayEvents.RequestSceneChangeEvent>(OnSceneChangeRequested);
    }

    // TODO find a way to return the progress of an async scene load for the loading screen

    private void OnSceneChangeRequested(GameplayEvents.RequestSceneChangeEvent e)
    {
        SceneManager.LoadScene(e.SceneIndex);
    }
}
