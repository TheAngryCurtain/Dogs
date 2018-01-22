using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // TODO really need a data structure for all of this...
    public int m_NumOfPlayers = 1; // TODO add in multiplayer stuff (used to live on ModesScreen, line 114)
    public int m_LocationIndex;
    public UI.Enums.ScreenId m_ScreenIDToLoad;
    public ModesScreen.eMode m_Mode;
    public ModesScreen.eSubMode m_SubMode;
    public ModesScreen.eDifficulty m_Difficulty;

    [SerializeField] private GameObject m_SmallDogPrefab;

    [SerializeField] private GameObject m_GamePrefab;
    [SerializeField] private GameModeData[] m_ModeData;

    private Game m_Game;

    public GameModeData[] ModeData { get { return m_ModeData; } }
    public Game CurrentGame { get { return m_Game; } }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // TODO should probably create and move this to a Game Monobehaviour
        VSEventManager.Instance.AddListener<GameplayEvents.OnGameStartEvent>(InitGame);

        // move to menu scene
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.RequestSceneChangeEvent((int)eScene.Main));
    }

    public override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        VSEventManager.Instance.RemoveListener<GameplayEvents.OnGameStartEvent>(InitGame);

        base.OnDestroy();
    }

    private void InitGame(GameplayEvents.OnGameStartEvent e)
    {
        // spawn dog
        GameObject dogObj = (GameObject)Instantiate(m_SmallDogPrefab, null);
        VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnPlayerSpawnedEvent(dogObj));
        
        // Spawn Game
        GameObject gameObj = (GameObject)Instantiate(m_GamePrefab, Vector3.zero, Quaternion.identity);
        m_Game = gameObj.GetComponent<Game>();

        GameModeData modeData = m_ModeData[(int)m_Mode];

        // Setup Mode
        switch (m_Mode)
        {
            case ModesScreen.eMode.Catch:
                switch (e.GameSubMode)
                {
                    case ModesScreen.eSubMode.Strikes:
                        m_Game.Setup(new StrikeCatchMode(modeData));
                        break;

                    case ModesScreen.eSubMode.Timed:
                        m_Game.Setup(new TimedCatchMode(modeData));
                        break;
                }
                break;

            case ModesScreen.eMode.Soccer:
                switch (e.GameSubMode)
                {
                    case ModesScreen.eSubMode.Targets:
                        m_Game.Setup(new TargetSoccerMode(modeData));
                        break;

                    case ModesScreen.eSubMode.GoalKeeper:
                        m_Game.Setup(new GoalKeeperSoccerMode(modeData));
                        break;
                }
                break;

            default:
            case ModesScreen.eMode.None:
                Debug.LogErrorFormat("No Mode Selected! This shouldn't happen.");
                break;
        }

        m_Game.Init();
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
