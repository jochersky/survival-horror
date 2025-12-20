using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : Weapon
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private GameObject bloodDecal;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private AudioSource audioSource;
    
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
    [SerializeField] private float muzzleFlashTime = 0.15f;
    [SerializeField] private float cameraShakeIntensity = 1f;
    [SerializeField] private float cameraShakeTime = 0.15f;

    // input actions
    private InputAction m_ReloadAction;

    private bool _initialized;
    private int _bulletsRemaining;
    
    private Coroutine _reload;
    private bool _isReloading = false;
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
        
        muzzleFlash.SetActive(false);
    }

    public void OnReload(InputAction.CallbackContext context)
    {
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
        if (_bulletsRemaining <= 0)
        {
            TryReload();
            return;
        }
        
        _isFiring = true;
        
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
            
            // positon to place decal, slightly inside of object to get better projection
            Vector3 pos = hit.point - hit.normal * 0.05f;
            if (hit.transform.TryGetComponent(out Hurtbox hurtbox))
            {
                // blood decal
                GameObject decal = Instantiate(bloodDecal, pos, Quaternion.LookRotation(hit.normal), hit.transform);
                Vector3 curRot = decal.transform.rotation.eulerAngles;
                decal.transform.eulerAngles = new Vector3(curRot.x, curRot.y, Random.Range(0, 360));
                hurtbox.TakeDamage(damage);
                AudioManager.Instance.PlaySFX(SfxType.PistolHit, audioSource);
            }
            else
            {
                // bullet hole decal
                Instantiate(bulletHoleDecal, pos, Quaternion.LookRotation(hit.normal), hit.transform);
                AudioManager.Instance.PlaySFX(SfxType.PistolFire, audioSource);
            }
        }
        else
        {
            TrailRenderer trail = Instantiate(bulletTrail, projectileSpawners[0].position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, firingDirection * maxBulletDistance));
            AudioManager.Instance.PlaySFX(SfxType.PistolFire, audioSource);
        }
        
        _isFiring = false;
        _bulletsRemaining--;
        CameraShake.Instance.ShakeAimCamera(cameraShakeIntensity, cameraShakeTime);
        AudioManager.Instance.PlaySFX(SfxType.PistolFire, audioSource);
        StartCoroutine(CreateMuzzleFlash());
        OnFireComplete?.Invoke(this);
    }

    private void TryReload()
    {
        if (_isReloading || _bulletsRemaining == maxMagazineSize || WeaponManager.Instance.AmmoCount() == 0) return;
        
        _isReloading = true;
        
        // request a reload from player state machine
        OnRequestReload?.Invoke();
        AudioManager.Instance.PlaySFX(SfxType.PistolReload, audioSource);
    }
    
    public void ReloadGun()
    {
        int amt = WeaponManager.Instance.GetAmmoAmount(this, maxMagazineSize);
        if (amt == 0) return;
        _isReloading = false;
        _bulletsRemaining = amt;
        
        OnReloadComplete?.Invoke(this);
    }

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

    private IEnumerator CreateMuzzleFlash()
    {
        Vector3 curRot = muzzleFlash.transform.rotation.eulerAngles;
        // rotate the muzzle flash mesh for variance in flash
        muzzleFlash.transform.eulerAngles = new Vector3(curRot.x, curRot.y, Random.Range(0, 360));
        muzzleFlash.SetActive(true);
        float timer = 0;
        while (timer < muzzleFlashTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        muzzleFlash.SetActive(false);
    }
}
