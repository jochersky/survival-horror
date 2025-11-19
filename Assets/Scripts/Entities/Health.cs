using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityTime = 0.5f;
    // [SerializeField] private Collider[] colliders;
    
    private float _currentHealth;
    private bool _isInvulnerable;

    public float MaxHealth => maxHealth;
    
    public delegate void HealthChanged(float oldHealth, float newHealth);
    public delegate void Died();
    public event HealthChanged OnHealthChanged;
    public event Died OnDeath;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out Damage damage))
        {
            TakeDamage(damage.DamageAmt);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
        {
            TakeDamage(damage.DamageAmt);
        }
    }

    public void Heal(float amount)
    {
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
