using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageData damageData;
    [SerializeField] private Collider hitbox;
    [SerializeField] private Transform followTransform;
    
    [Header("Properties")]
    [SerializeField] private bool deactivateOnCollision = false;
    [SerializeField] private bool deactivateOnTrigger = false;
    
    public float DamageAmt => damageData.damageAmt;
    
    private void Update()
    {
        if (followTransform)
        {
            hitbox.transform.position = followTransform.position;
            hitbox.transform.rotation = followTransform.rotation;
        }
    }

    public void Activate()
    {
        hitbox.enabled = true;
    }

    public void Deactivate()
    {
        hitbox.enabled = false;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (deactivateOnCollision)
            Deactivate();
        
        if (other.transform.TryGetComponent(out Health health))
            health.TakeDamage(damageData.damageAmt);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (deactivateOnTrigger)
            Deactivate();
        
        if (other.transform.TryGetComponent(out Health health))
            health.TakeDamage(damageData.damageAmt);
    }
}
