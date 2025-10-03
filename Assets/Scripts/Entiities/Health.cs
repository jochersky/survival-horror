using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;
    
    [SerializeField] private float maxHealth = 100f;
    
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
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0f) return;
        
        OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage);
        _currentHealth -= damage;
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }
}
