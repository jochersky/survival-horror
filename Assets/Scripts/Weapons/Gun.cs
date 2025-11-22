using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : Weapon
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject projectileDecal;
    // must be assigned when the prefab has already been instanced
    private Camera _cam;
    private InputActionMap _playerActions;

    [Header("Gun Properties")] 
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
    public int BulletsRemaining { get { return _bulletsRemaining; } set { _bulletsRemaining = value; } }

    public delegate void ReloadComplete(Gun gun);
    public event ReloadComplete OnReloadComplete;

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
        
        int amt = WeaponManager.instance.GetAmmoAmount(this, maxMagazineSize - _bulletsRemaining);
        if (amt == 0) return;
        _reload = StartCoroutine(Reload(_bulletsRemaining + amt));
    }

    public override void AimAttack()
    {
        if(!_isFiring) ShootGun();
    }

    private void ShootGun()
    {
        if (_isReloading || _isFiring) return;
        // reload if trying to fire and there are bullets to reload with
        if (_bulletsRemaining <= 0)
        {
            int amt = WeaponManager.instance.GetAmmoAmount(this, maxMagazineSize - _bulletsRemaining);
            if (amt == 0) return;
            _reload = StartCoroutine(Reload(_bulletsRemaining + amt));
        }
        
        Vector3 firingDirection = (_cam.transform.forward - projectileSpawners[0].transform.forward).normalized;

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit camHit, maxBulletDistance, _mask))
            firingDirection = camHit.point - projectileSpawners[0].transform.position;

        if (Physics.Raycast(projectileSpawners[0].transform.position, firingDirection, out RaycastHit hit, maxBulletDistance, _mask))
        {
            if (hit.transform.TryGetComponent(out Health health))
                health.TakeDamage(damage);
                
            // place slightly inside of object so that decal doesn't flicker
            Vector3 pos = hit.point - hit.normal * 0.1f;
            Instantiate(projectileDecal, pos, Quaternion.LookRotation(hit.normal), hit.transform);
        }

        _fire = StartCoroutine(Fire());
    }
    
    private IEnumerator Reload(int amt)
    {
        _isReloading = true;
        
        float timer = 0;
        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _bulletsRemaining = amt;
        OnReloadComplete?.Invoke(this);
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
        OnFireComplete?.Invoke(this);
        _isFiring = false;
    }
}
