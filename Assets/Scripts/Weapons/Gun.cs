using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    // references
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject projectileDecal;
    private InputActionMap _playerActions;
    private Camera _cam;

    [Header("Gun Stats")] 
    [SerializeField] private float damage = 35f;
    [SerializeField] private int maxMagazineSize = 8;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxBulletDistance = 100f;
    
    // input actions
    private InputAction m_ZoomAction;
    private InputAction m_AttackAction;
    private InputAction m_ReloadAction;
    
    // variables to store optimized setter/getter parameter IDs
    private int _isZoomingHash;
    private int _isShootingHash;
    
    private bool _isZooming;
    private bool _isPressingFire;
    private int _bulletsRemaining;
    
    private Coroutine _reload;
    private bool _isReloading;
    private Coroutine _fire;
    private bool _isFiring;

    private LayerMask _mask;

    private void Awake()
    {
        _cam = Camera.main;
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_ZoomAction = actions.FindAction("Zoom");
        m_ZoomAction.started += OnZoom;
        m_ZoomAction.canceled += OnZoom;
        m_AttackAction = actions.FindAction("Attack");
        m_AttackAction.started += OnAttack;
        m_AttackAction.canceled += OnAttack;
        m_ReloadAction = actions.FindAction("Reload");
        m_ReloadAction.started += OnReload;

        // Assign layers that the gun can interact with
        _mask = LayerMask.GetMask("EnemyHurtbox", "Environment");
        
        // set the parameter hash references
        _isZoomingHash = Animator.StringToHash("isZooming");
        _isShootingHash = Animator.StringToHash("isShooting");
        
        _bulletsRemaining = maxMagazineSize;
    }
    
    private void OnEnable()
    {
        // enable the character controls action map
        _playerActions.Enable();
    }

    private void OnDisable()
    {
        // disable the character controls action map
        _playerActions.Disable();
    }

    private void Update()
    {
        if (_isZooming)
        {
            CalculateShot();
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        _isZooming = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isPressingFire = context.ReadValueAsButton();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (_isReloading || _bulletsRemaining == maxMagazineSize) return;
        
        _reload = StartCoroutine(Reload());
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
        animator.SetBool(_isShootingHash, true);
        animator.SetBool(_isZoomingHash, false);
        
        float timer = 0;
        while (timer < fireRate)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        _bulletsRemaining--;
        _isFiring = false;
        animator.SetBool(_isShootingHash, false);
        animator.SetBool(_isZoomingHash, true);
    }

    private void CalculateShot()
    {
        transform.LookAt(-_cam.transform.forward * 1000f);
                
        Vector3 firingDirection = (_cam.transform.forward - projectileSpawners[0].transform.forward).normalized;
            
        Debug.DrawRay(projectileSpawners[0].transform.position, firingDirection * maxBulletDistance, Color.red);
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward * maxBulletDistance, Color.green);
            
        Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit camHit, maxBulletDistance, _mask);

        // adjust where the bullet will hit if something collides with a ray going out of the camera
        firingDirection = (camHit.transform ? 
            camHit.point - projectileSpawners[0].transform.position : 
            (_cam.transform.forward - projectileSpawners[0].transform.forward).normalized);
            
        if (_isPressingFire)
        {
            ShootBullet(firingDirection);
        }
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
}
