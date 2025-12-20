using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Damage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageData damageData;
    [SerializeField] private Collider hitbox;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Health health;

    [Header("Properties")] 
    [SerializeField] private float cameraShakeIntensity = 1f;
    [SerializeField] private float cameraShakeTime = 0.15f;
    [SerializeField] private bool deactivateOnCollision = false;
    [SerializeField] private bool deactivateOnTrigger = false;
    [SerializeField] private bool deactivateOnDeath = false;
    
    public float DamageAmt => damageData.damageAmt;

    public delegate void DamageDone();
    public event DamageDone OnDamageDone;

    private void Start()
    {
        if (deactivateOnDeath)
        {
            health.OnDeath += Deactivate;
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
        
        if (other.transform.TryGetComponent(out Hurtbox hb))
        {
            // hurtbox sends event to call TakeDamage on connected health object
            OnDamageDone?.Invoke();
            CameraShake.Instance.ShakeCurrentCamera(cameraShakeIntensity, cameraShakeTime);
        }
        else if (other.transform.TryGetComponent(out Health h))
        {
            h.TakeDamage(damageData.damageAmt);
            OnDamageDone?.Invoke();
            CameraShake.Instance.ShakeCurrentCamera(cameraShakeIntensity, cameraShakeTime);
        }
        else if (deactivateOnCollision)
            Deactivate();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Hurtbox hb))
        {
            // hurtbox sends event to call TakeDamage on connected health object
            OnDamageDone?.Invoke();
            CameraShake.Instance.ShakeCurrentCamera(cameraShakeIntensity, cameraShakeTime);
        }
        else if (other.transform.TryGetComponent(out Health h))
        {
            h.TakeDamage(damageData.damageAmt);
            OnDamageDone?.Invoke();
            CameraShake.Instance.ShakeCurrentCamera(cameraShakeIntensity, cameraShakeTime);
        }
        else if (deactivateOnTrigger)
            Deactivate();
    }
}
