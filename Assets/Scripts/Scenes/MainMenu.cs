using UnityEditor;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject SettingsScreen;

    private void Start()
    {
        SettingsScreen.SetActive(false);
    }

    public void StartButtonPressed()
    {
        SceneSwapManager.SwapScene(sceneToLoad);
    }

    public void SettingsButtonPressed()
    {
        SettingsScreen.SetActive(true);
    }

    public void QuitButtonPressed()
    {
        // TODO: change to use this when creating a build
        // Application.Quit();
        EditorApplication.ExitPlaymode();
    }

    public void CloseSettings()
    {
        SettingsScreen.SetActive(false);
    }
}
