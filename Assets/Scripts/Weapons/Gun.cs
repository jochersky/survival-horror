using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    // references
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject projectileDecal;
    private InputActionMap _playerActions;
    
    [Header("Gun Stats")]
    [SerializeField] private int maxMagazineSize = 8;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxBulletDistance = 100f;
    
    // input actions
    private InputAction m_ZoomAction;
    private InputAction m_AttackAction;
    private InputAction m_ReloadAction;
    
    private bool _isZooming;
    private bool _isPressingFire;
    private int _bulletsRemaining;
    
    private Coroutine _reload;
    private bool _isReloading;
    private Coroutine _fire;
    private bool _isFiring;

    private void Awake()
    {
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_ZoomAction = actions.FindAction("Zoom");
        m_ZoomAction.started += OnZoom;
        m_ZoomAction.canceled += OnZoom;
        m_AttackAction = actions.FindAction("Attack");
        m_AttackAction.started += OnAttack;
        m_ReloadAction = actions.FindAction("Reload");
        m_ReloadAction.started += OnReload;
        
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
            transform.LookAt(-Camera.main.transform.forward * 1000f);
                
            Vector3 firingDirection = (Camera.main.transform.forward - projectileSpawners[0].transform.forward).normalized;
            Debug.DrawRay(projectileSpawners[0].transform.position, firingDirection * 1000f, Color.red);
            
            if (_isPressingFire)
            {
                if (!_isReloading && !_isFiring && _bulletsRemaining > 0)
                {
                    Physics.Raycast(projectileSpawners[0].transform.position, firingDirection * 1000f, out RaycastHit hit, maxBulletDistance);
                
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
        Debug.Log("Reloading");
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
