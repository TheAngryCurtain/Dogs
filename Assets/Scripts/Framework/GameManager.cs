using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public System.Action<Transform> OnPlayerSpawned;
    public System.Action<Transform> OnDiscSpawned;
    public System.Action OnLevelLoaded;

    public int m_NumOfPlayers;
    public int m_LocationIndex;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0) // TODO need an intellegent way of making sure this works for all levels. 1 is the DogPark currently
        {
            // Spawn Game
            GameObject gameObj = (GameObject)Instantiate(m_GamePrefab, Vector3.zero, Quaternion.identity);
            Game game = gameObj.GetComponent<Game>();

            // TODO create and pass in game mode to Game
            // TODO attach listeners to game Mode

            // Spawn Player and World Objects
            GameObject dogObj = (GameObject)Instantiate(m_SmallDogPrefab, new Vector3(20f, 0.25f, 20f), Quaternion.identity);
            if (OnPlayerSpawned != null)
            {
                OnPlayerSpawned(dogObj.transform);
            }

            GameObject discObj = (GameObject)Instantiate(m_DiscPrefab, new Vector3(20f, 5f, 20f), Quaternion.identity);
            DiscController disc = discObj.GetComponent<DiscController>();
            if (OnDiscSpawned != null)
            {
                OnDiscSpawned(discObj.transform);
            }

            GameObject launcherObj = (GameObject)Instantiate(m_DiscPrefab, null); // spawn at it's prefab position?
            DiscLauncher launcher = launcherObj.GetComponent<DiscLauncher>();
            if (launcher != null)
            {
                launcher.AssignDisc(disc);
            }

            // Notify Level Loaded
            if (OnLevelLoaded != null)
            {
                OnLevelLoaded();
            }

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
