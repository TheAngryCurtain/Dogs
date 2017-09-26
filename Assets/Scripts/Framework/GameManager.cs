﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // TODO really need a data structure for all of this...
    public int m_NumOfPlayers;
    public int m_LocationIndex;
    public UI.Enums.ScreenId m_ScreenIDToLoad;
    public ModesScreen.eMode m_Mode;
    public ModesScreen.eDifficulty m_Difficulty;

    [SerializeField] private GameObject m_SmallDogPrefab;
    [SerializeField] private GameObject m_DiscPrefab;
    [SerializeField] private GameObject m_launcherPrefab;
    [SerializeField] private GameObject m_DiscTrackerPrefab;

    [SerializeField] private GameObject m_GamePrefab;
    [SerializeField] private GameModeData[] m_ModeData;
    public GameModeData[] ModeData { get { return m_ModeData; } }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // TODO should probably create and move this to a Game Monobehaviour
        VSEventManager.Instance.AddListener<GameplayEvents.OnGameStartEvent>(InitGame);
    }

    public override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        VSEventManager.Instance.RemoveListener<GameplayEvents.OnGameStartEvent>(InitGame);

        base.OnDestroy();
    }

    private void InitGame(GameplayEvents.OnGameStartEvent e)
    {
        // Spawn Player and World Objects
        Instantiate(m_launcherPrefab, null); // spawn at it's prefab position?

        Instantiate(m_DiscTrackerPrefab, null);
        
        GameObject dogObj = (GameObject)Instantiate(m_SmallDogPrefab, null);
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnPlayerSpawnedEvent(dogObj));

        GameObject discObj = (GameObject)Instantiate(m_DiscPrefab, null);
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnDiscSpawnedEvent(discObj));
        

        // Spawn Game
        GameObject gameObj = (GameObject)Instantiate(m_GamePrefab, Vector3.zero, Quaternion.identity);
        Game game = gameObj.GetComponent<Game>();

        // Setup Mode
        switch (m_Mode)
        {
            case ModesScreen.eMode.Catch:
                game.Setup(new CatchMode());
                break;

            default:
            case ModesScreen.eMode.None:
                Debug.LogErrorFormat("No Mode Selected! This shouldn't happen.");
                break;
        }

        game.Init();

        // TODO
        // then, make sure the disc and launcher are set up to fire (maybe a "Ready?" text, then a delay and "Catch!" right before launching)
        // Then, once the catch is made, award the points (DONE)_and reset the disc
        // Once the 3 misses are done, end the game
        // show a results screen
        // TODO gameplay - add consecutive catch combo?
        // TODO gameplay - add 
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        // Notify Level Loaded
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnLevelLoadedEvent(scene.buildIndex));

#if UNITY_EDITOR
        // Rebuild lighting.
        if (UnityEditor.Lightmapping.giWorkflowMode == UnityEditor.Lightmapping.GIWorkflowMode.Iterative)
        {
            DynamicGI.UpdateEnvironment();
        }
#endif
    }
}
