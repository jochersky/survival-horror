using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Texture fullHealthTexture;
    [SerializeField] private Texture threeFourthsHealthTexture;
    [SerializeField] private Texture halfHealthTexture;
    [SerializeField] private Texture quarterHealthTexture;
    [SerializeField] private Texture emptyHealthTexture;
    private RawImage healthBarImage;

    private void Start()
    {
        healthBarImage = GetComponent<RawImage>();
        health.OnHealthChanged += UpdateHealth;
    }

    private void UpdateHealth(float oldHealth, float newHealth)
    {
        float healthBarRatio = newHealth / health.MaxHealth;
        Debug.Log(healthBarRatio);
        if (healthBarRatio >= 1)
        {
            healthBarImage.texture = fullHealthTexture;
        }
        else if (healthBarRatio < 1 && healthBarRatio >= 0.67)
        {
            healthBarImage.texture = threeFourthsHealthTexture;
        }
        else if (healthBarRatio < 0.67 && healthBarRatio >= 0.33)
        {
            healthBarImage.texture = halfHealthTexture;
        } 
        else if (healthBarRatio < 0.33 && healthBarRatio > 0)
        {
            healthBarImage.texture = quarterHealthTexture;    
        }
        else if (healthBarRatio <= 0)
        {
            healthBarImage.texture = emptyHealthTexture;
        }
    }
}
