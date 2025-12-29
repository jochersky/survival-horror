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

    public static void SwapScene(SceneField scene)
    {
        Instance.StartCoroutine(Instance.FadeOutThenChangeScene(scene));
    }

    private IEnumerator FadeOutThenChangeScene(SceneField scene)
    {
        // start fading to vlack
        
        // keep fading out

        SceneManager.LoadScene(scene);
        return null;
    }
}
