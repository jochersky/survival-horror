using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private DamageData damageData;
    [SerializeField] private Collider hitbox;
    [SerializeField] private Transform followTransform;
    
    public float DamageAmt => damageData.damageAmt;

    public void Start()
    {
        // hitbox.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        hitbox.transform.position = followTransform.position;
    }

    public void Activate()
    {
        hitbox.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        hitbox.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Health health))
        {
            health.TakeDamage(damageData.damageAmt);
        }
    }
}
