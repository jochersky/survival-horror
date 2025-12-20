using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityTime = 0.5f;
    [SerializeField] private Hurtbox[] hurtboxes;
    
    private float _currentHealth;
    private bool _isInvulnerable;

    public float MaxHealth => maxHealth;
    
    public delegate void HealthChanged(float oldHealth, float newHealth);
    public event HealthChanged OnHealthChanged;
    public delegate void Died();
    public event Died OnDeath;
    public delegate void BackToFull();
    public event BackToFull OnBackToFull;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void Start()
    {
        if (hurtboxes.Length > 0)
        {
            foreach (Hurtbox h in hurtboxes)
            {
                h.health = this;
                h.OnDamageTaken += TakeDamage;
            }
        }
    }

    public void Revive()
    {
        _currentHealth = maxHealth;
        OnBackToFull?.Invoke();
    }

    public void Heal(float amount)
    {
        AudioManager.Instance.PlaySFX(SfxType.Heal);
        if (_currentHealth >= maxHealth) return;
        OnHealthChanged?.Invoke(_currentHealth, _currentHealth + amount);
        _currentHealth += amount;
    }

    public void TakeDamage(float damage)
    {
        if (_isInvulnerable || _currentHealth <= 0f) return;
        
        OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage);
        StartCoroutine(Invulnerable());
        _currentHealth -= damage;
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }

    IEnumerator Invulnerable()
    {
        _isInvulnerable = true;
        float timer = 0;
        while (timer < invulnerabilityTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _isInvulnerable = false;
    }
}
