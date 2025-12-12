using System;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageData damageData;
    [SerializeField] private Collider hitbox;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Health health;

    [Header("Properties")] 
    [SerializeField] private float cameraShakeIntensity = 1f;
    [SerializeField] private float cameraShakeTime = 1f;
    [SerializeField] private bool deactivateOnCollision = false;
    [SerializeField] private bool deactivateOnTrigger = false;
    [SerializeField] private bool deactivateOnDeath = false;
    
    public float DamageAmt => damageData.damageAmt;

    private void Start()
    {
        if (deactivateOnDeath)
        {
            health.OnDeath += Deactivate;
            health.OnBackToFull += Activate;
        }
    }

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
        if (other.transform.TryGetComponent(out Health h))
        {
            h.TakeDamage(damageData.damageAmt);
            CameraShake.Instance.ShakeCurrentCamera(cameraShakeIntensity, cameraShakeTime);
        }
        else if (deactivateOnCollision)
            Deactivate();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Health h))
        {
            h.TakeDamage(damageData.damageAmt);
            CameraShake.Instance.ShakeCurrentCamera(cameraShakeIntensity, cameraShakeTime);
        }
        else if (deactivateOnTrigger)
            Deactivate();
    }
}
