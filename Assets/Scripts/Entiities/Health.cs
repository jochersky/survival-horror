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
        if (!other.TryGetComponent(out Damage damage)) return;
        if (!(_currentHealth >= 0f)) return;
        
        OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage.DamageAmt);
        _currentHealth -= damage.DamageAmt;
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0f) return;
        
        OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage);
        _currentHealth -= damage;
        Debug.Log(_currentHealth);
        if (_currentHealth <= 0f) Debug.Log("died");
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }
}
