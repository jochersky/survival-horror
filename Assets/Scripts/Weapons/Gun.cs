using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : Weapon
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject projectileDecal;
    [SerializeField] private TrailRenderer bulletTrail;
    // must be assigned when the prefab has already been instanced
    private Camera _cam;
    private InputActionMap _playerActions;

    [Header("Gun Properties")] 
    [SerializeField] private float damage = 35f;
    [SerializeField] private int maxMagazineSize = 8;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxBulletDistance = 100f;
    [SerializeField] private float bulletSpeed = 100f;
    
    // input actions
    private InputAction m_ReloadAction;

    private bool _initialized;
    private int _bulletsRemaining;
    
    private Coroutine _reload;
    private bool _isReloading;
    private Coroutine _fire;
    private bool _isFiring = false;

    private LayerMask _mask;
    
    // getters and setters
    public int BulletsRemaining { get { return _bulletsRemaining; } set { _bulletsRemaining = value; } }

    public delegate void RequestReload();
    public event RequestReload OnRequestReload;
    
    public delegate void ReloadComplete(Gun gun);
    public event ReloadComplete OnReloadComplete;

    public delegate void RequestFire();
    public event RequestFire OnRequestFire;

    public delegate void FireComplete(Gun gun);
    public event FireComplete OnFireComplete;

    private void Awake()
    {
        _cam = Camera.main;
        
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_ReloadAction = actions.FindAction("Reload");
        m_ReloadAction.started += OnReload;
        
        // Assign layers that the gun can interact with
        _mask = LayerMask.GetMask("EnemyHurtbox", "Environment");
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (_isReloading || _bulletsRemaining == maxMagazineSize) return;
        TryReload();
    }

    public override void AimAttack()
    {
        TryFire();
    }
    
    private void TryFire()
    {
        if (_isReloading || _isFiring) return;
        
        // reload if trying to fire and there are bullets to reload with
        if (_bulletsRemaining <= 0) TryReload();
        
        // request a fire from the player state machine
        OnRequestFire?.Invoke();
    }

    public void FireGun()
    {
        Vector3 firingDirection = (_cam.transform.forward - projectileSpawners[0].transform.forward).normalized;

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit camHit, maxBulletDistance, _mask))
            firingDirection = camHit.point - projectileSpawners[0].transform.position;
        
        if (Physics.Raycast(projectileSpawners[0].transform.position, firingDirection, out RaycastHit hit, maxBulletDistance, _mask))
        {
            TrailRenderer trail = Instantiate(bulletTrail, projectileSpawners[0].position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit.point));
            
            // place slightly inside of object so that decal doesn't flicker
            Vector3 pos = hit.point - hit.normal * 0.1f;
            Instantiate(projectileDecal, pos, Quaternion.LookRotation(hit.normal), hit.transform);
            
            if (hit.transform.TryGetComponent(out Hurtbox hurtbox)) hurtbox.TakeDamage(damage);
        }
        else
        {
            TrailRenderer trail = Instantiate(bulletTrail, projectileSpawners[0].position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, firingDirection * maxBulletDistance));
        }

        _bulletsRemaining--;
    }

    private void TryReload()
    {
        // check we can even reload in the first place: have bullets missing and bullets to reload with in inventory
        if (_bulletsRemaining == maxMagazineSize || WeaponManager.instance.AmmoCount() == 0) return;
        
        // request a reload from player state machine
        OnRequestReload?.Invoke();
    }
    
    public void ReloadGun()
    {
        int amt = WeaponManager.instance.GetAmmoAmount(this, maxMagazineSize);
        if (amt == 0) return;
        _bulletsRemaining = amt;
        OnReloadComplete?.Invoke(this);
    }

    // private IEnumerator Fire()
    // {
    //     _isFiring = true;
    //     
    //     float timer = 0;
    //     while (timer < fireRate)
    //     {
    //         timer += Time.deltaTime;
    //         yield return null;
    //     }
    //     
    //     _bulletsRemaining--;
    //     OnFireComplete?.Invoke(this);
    //     _isFiring = false;
    // }

    private IEnumerator SpawnTrail(TrailRenderer tr, Vector3 target)
    {
        Vector3 startPos = tr.transform.position;
        float distance = Vector3.Distance(startPos, target);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            tr.transform.position = Vector3.Lerp(startPos, target, 1 - (remainingDistance / distance));
            remainingDistance -= bulletSpeed * Time.deltaTime;
            yield return null;
        }
        tr.transform.position = target;
        Destroy(tr.gameObject, tr.time);
    }
}
