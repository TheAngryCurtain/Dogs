using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UnityEngine.SceneManagement;

// TODO move this stuff into the scene loader
// find a way to return the progress on an async scene load

public class LoadingScreen : UIBaseScreen
{
    [SerializeField] private Slider m_LoadingSlider;

    private AsyncOperation m_AsyncLoadOp;

    public override void Initialize()
    {
        base.Initialize();

        int sceneIndex = GameManager.Instance.m_LocationIndex; // TODO need a map of scene names to indices
        UI.Enums.ScreenId id = GameManager.Instance.m_ScreenIDToLoad; // TODO need a data structure for this data...
        StartCoroutine(LoadLevel(sceneIndex, id));
    }

    private IEnumerator LoadLevel(int sceneIndex, UI.Enums.ScreenId id)
    {
        m_AsyncLoadOp = SceneManager.LoadSceneAsync(sceneIndex);

        while (!m_AsyncLoadOp.isDone)
        {
            m_LoadingSlider.value = m_AsyncLoadOp.progress;

            yield return null;
        }

        UIManager.Instance.TransitionToScreen(id);
    }
}
