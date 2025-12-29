using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;

    public void StartButtonPressed()
    {
        SceneSwapManager.SwapScene(sceneToLoad);
    }
}
