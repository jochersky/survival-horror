using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance { get; private set; }

    private void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void SwapScene(SceneField scene)
    {
        Instance.StartCoroutine(Instance.FadeOutThenChangeScene(scene));
    }

    private IEnumerator FadeOutThenChangeScene(SceneField scene)
    {
        SceneFadeManager.Instance.StartFadingOut();
        
        while (SceneFadeManager.Instance.IsFadingOut) 
        {
            yield return null;
        }
        
        SceneManager.LoadScene(scene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneFadeManager.Instance.StartFadingIn();
    }
}
