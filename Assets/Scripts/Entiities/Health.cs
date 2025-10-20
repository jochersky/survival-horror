using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    // [SerializeField] private Collider[] colliders;
    
    private float _currentHealth;
    
    public delegate void HealthChanged(float oldHealth, float newHealth);
    public delegate void Died();
    public event HealthChanged OnHealthChanged;
    public event Died OnDeath;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
        {
            Debug.Log(other.name + " damaging " + name);
            TakeDamage(damage.DamageAmt);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0f) return;
        
        OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage);
        _currentHealth -= damage;
        Debug.Log(name + " health now at " + _currentHealth);
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }
}
