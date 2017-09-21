using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int m_NumOfPlayers;
    public int m_LocationIndex;
    public ModesScreen.eMode m_Mode;

    [SerializeField] private GameObject m_SmallDogPrefab;
    [SerializeField] private GameObject m_DiscPrefab;
    [SerializeField] private GameObject m_launcherPrefab;

    [SerializeField] private GameObject m_GamePrefab;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        base.OnDestroy();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex != 0) // TODO need an intellegent way of making sure this works for all levels. 1 is the DogPark currently
        {
            // Spawn Player and World Objects
            GameObject dogObj = (GameObject)Instantiate(m_SmallDogPrefab, new Vector3(20f, 0.25f, 20f), Quaternion.identity);
            VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnPlayerSpawnedEvent(dogObj));

            GameObject discObj = (GameObject)Instantiate(m_DiscPrefab, new Vector3(20f, 5f, 20f), Quaternion.identity);
            VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnDiscSpawnedEvent(discObj));
            
            Instantiate(m_launcherPrefab, null); // spawn at it's prefab position?

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


            // Notify Level Loaded
            VSEventManager.Instance.TriggerEvent(new GameplayEvents.OnLevelLoadedEvent());

#if UNITY_EDITOR
            // Rebuild lighting.
            if (UnityEditor.Lightmapping.giWorkflowMode == UnityEditor.Lightmapping.GIWorkflowMode.Iterative)
            {
                DynamicGI.UpdateEnvironment();
            }
#endif

        }
    }
}
