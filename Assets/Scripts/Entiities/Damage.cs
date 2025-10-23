using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private DamageData damageData;
    [SerializeField] private Collider hitbox;
    [SerializeField] private Transform followTransform;
    
    public float DamageAmt => damageData.damageAmt;

    public void Start()
    {
        hitbox.enabled = false;
    }
    
    private void Update()
    {
        if (followTransform)
            hitbox.transform.position = followTransform.position;
    }

    public void Activate()
    {
        hitbox.enabled = true;
    }

    public void Deactivate()
    {
        hitbox.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Health health))
        {
            health.TakeDamage(damageData.damageAmt);
        }
    }
}
