using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private Transform[] projectileSpawners;
    [SerializeField] private GameObject projectileDecal;
    
    [Header("Gun Stats")]
    [SerializeField] private int maxMagazineSize = 8;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxBulletDistance = 100f;
    
    private InputSystem_Actions _actions;
    private InputSystem_Actions.PlayerActions _playerActions;
    
    private bool _isZooming;
    private bool _isPressingFire;
    private int _bulletsRemaining;
    
    private Coroutine _reload;
    private bool _isReloading;
    private Coroutine _fire;
    private bool _isFiring;

    private void Awake()
    {
        _actions = new InputSystem_Actions(); // Asset object
        _playerActions = _actions.Player;     // Extract action map object
        
        // Subscribe the player input callbacks
        _playerActions.AddCallbacks(this);
        
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
    
    public void OnMove(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
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
