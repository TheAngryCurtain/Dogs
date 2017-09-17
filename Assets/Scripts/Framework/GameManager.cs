using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int m_NumOfPlayers;
    public int m_LocationIndex;

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
        if (scene.buildIndex == 1) // TODO need an intellegent way of making sure this works for all levels. 1 is the DogPark currently
        {
            // TODO
            // spawn dog
            // keep camera from UI, but change to perspective (may be able to use perspective all the way through the game?)
            // assign camera to dog, and dog to camera (as target)
            // spawn frisbee
            // assign frisbee to camera (lock target)
            // 

            // OTHER
            // set up game and game mode classes
            // set up pause menu screen
            // let it quit back to front end
        }
    }
}
