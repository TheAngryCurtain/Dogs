using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : UIBaseScreen
{
    [SerializeField] private Slider m_LoadingSlider;

    private AsyncOperation m_AsyncLoadOp;

    public override void Initialize()
    {
        base.Initialize();

        int sceneIndex = GameManager.Instance.m_LocationIndex + 1; // TODO need a map of scene names to indices
        StartCoroutine(LoadLevel(sceneIndex));
    }

    private IEnumerator LoadLevel(int sceneIndex)
    {
        m_AsyncLoadOp = SceneManager.LoadSceneAsync(sceneIndex);

        while (!m_AsyncLoadOp.isDone)
        {
            m_LoadingSlider.value = m_AsyncLoadOp.progress;

            yield return null;
        }

        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.GameHUD);
    }
}
