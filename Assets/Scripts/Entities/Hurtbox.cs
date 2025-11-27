using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private float damageMult = 1f;
    
    public delegate void damageTaken(float damageAmt);
    public event damageTaken OnDamageTaken;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
            OnDamageTaken?.Invoke(damage.DamageAmt * damageMult);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out Damage damage))
            OnDamageTaken?.Invoke(damage.DamageAmt * damageMult);
    }

    public void TakeDamage(float damageAmt)
    {
        OnDamageTaken?.Invoke(damageAmt * damageMult);
    }
}
