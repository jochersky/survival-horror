using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeOutImage;
    [Range(0.1f, 10f), SerializeField] private float fadeOutSpeed = 5f;
    [Range(0.1f, 10f), SerializeField] private float fadeInSpeed = 5f;

    [SerializeField] private Color fadeOutStartColor;
    
    public bool IsFadingOut { get; private set; }
    public bool IsFadingIn { get; private set; }
    
    public static SceneFadeManager Instance { get; private set; }

    private void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        fadeOutStartColor.a = 0f;
    }

    private void Update()
    {
        if (IsFadingOut)
        {
            if (fadeOutImage.color.a < 1f)
            {
                fadeOutStartColor.a += Time.deltaTime * fadeOutSpeed;
                fadeOutImage.color = fadeOutStartColor;
            }
            else
            {
                IsFadingOut = false;
            }
        }

        if (IsFadingIn)
        {
            if (fadeOutImage.color.a > 0f)
            {
                fadeOutStartColor.a -= Time.deltaTime * fadeInSpeed;
                fadeOutImage.color = fadeOutStartColor;
            }
            else
            {
                IsFadingIn = false;
            }
        }
    }

    public void StartFadingOut()
    {
        fadeOutImage.color = fadeOutStartColor;
        IsFadingOut = true;
    }

    public void StartFadingIn()
    {
        if (fadeOutImage.color.a < 1f) return; 
        
        fadeOutImage.color = fadeOutStartColor;
        IsFadingIn = true;
    }
}
