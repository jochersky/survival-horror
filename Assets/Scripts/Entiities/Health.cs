using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    
    private float _currentHealth;
    
    public delegate void HealthChanged(float oldHealth, float newHealth);
    public event HealthChanged OnHealthChanged;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
        {
            if (_currentHealth >= 0f)
            {
                OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage.DamageAmt);
                _currentHealth -= damage.DamageAmt;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth >= 0f)
        {
            OnHealthChanged?.Invoke(_currentHealth, _currentHealth - damage);
            _currentHealth -= damage;
        }
    }
}
