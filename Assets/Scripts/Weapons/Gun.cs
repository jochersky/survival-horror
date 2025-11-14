using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : Weapon
{
    [Header("References")]
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject projectileDecal;
    // must be assigned when the prefab has already been instanced
    private Camera _cam;

    [Header("Gun Stats")] 
    [SerializeField] private float damage = 35f;
    [SerializeField] private int maxMagazineSize = 8;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxBulletDistance = 100f;
    
    // input actions
    private InputAction m_ReloadAction;

    private bool _initialized;
    private int _bulletsRemaining;
    
    private Coroutine _reload;
    private bool _isReloading;
    private Coroutine _fire;
    private bool _isFiring;

    private LayerMask _mask;
    
    // getters and setters
    public int BulletsRemaining { get { return _bulletsRemaining; } }

    private void Awake()
    {
        _cam = Camera.main;
        
        // Assign layers that the gun can interact with
        _mask = LayerMask.GetMask("EnemyHurtbox", "Environment");
        
        _bulletsRemaining = maxMagazineSize;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (_isReloading || _bulletsRemaining == maxMagazineSize) return;
        
        _reload = StartCoroutine(Reload());
    }

    public override void SwingAttack()
    {
        
    }

    public override void AimAttack()
    {
        if(!_isFiring) CalculateShot();
    }

    private void CalculateShot()
    {
        // transform.LookAt(-cam.transform.forward * 1000f);
                
        Vector3 firingDirection = (_cam.transform.forward - projectileSpawners[0].transform.forward).normalized;
            
        Debug.DrawRay(projectileSpawners[0].transform.position, firingDirection * maxBulletDistance, Color.red);
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward * maxBulletDistance, Color.green);
            
        Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit camHit, maxBulletDistance, _mask);

        // adjust where the bullet will hit if something collides with a ray going out of the camera
        firingDirection = (camHit.transform ? 
            camHit.point - projectileSpawners[0].transform.position : 
            (_cam.transform.forward - projectileSpawners[0].transform.forward).normalized);
            
        ShootBullet(firingDirection);
    }

    private void ShootBullet(Vector3 firingDirection)
    {
        if (!_isReloading && !_isFiring && _bulletsRemaining > 0)
        {
            Physics.Raycast(projectileSpawners[0].transform.position, firingDirection, out RaycastHit hit, maxBulletDistance, _mask);
            
            if (hit.transform && hit.transform.TryGetComponent(out Health health))
                health.TakeDamage(damage);
                
            // place slightly inside of object so that decal doesn't flicker
            Vector3 pos = hit.point - hit.normal * 0.1f;
            GameObject decal = Instantiate(projectileDecal, pos, Quaternion.LookRotation(hit.normal));

            _fire = StartCoroutine(Fire());
        }
        else if (_bulletsRemaining <= 0)
        {
            _reload = StartCoroutine(Reload());
        }
    }
    
    private IEnumerator Reload()
    {
        _isReloading = true;
        
        float timer = 0;
        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _bulletsRemaining = maxMagazineSize;
        _isReloading = false;
    }

    private IEnumerator Fire()
    {
        _isFiring = true;
        
        float timer = 0;
        while (timer < fireRate)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        _bulletsRemaining--;
        _isFiring = false;
    }
}
