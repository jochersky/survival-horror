using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private Transform[] projectileSpawners;
    
    private InputSystem_Actions _actions;
    private InputSystem_Actions.PlayerActions _playerActions;
    
    private bool _isZooming;
    private bool _isFiring;

    private void Awake()
    {
        _actions = new InputSystem_Actions(); // Asset object
        _playerActions = _actions.Player;     // Extract action map object
        
        // Subscribe the player input callbacks
        _playerActions.AddCallbacks(this);
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
            Debug.DrawRay(projectileSpawners[0].transform.position, firingDirection * 100, Color.red);
            
            if (_isFiring)
            {
                // Physics.Raycast(projectileSpawners[0].transform.position, projectileSpawners[0].transform.forward, 100f);
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
        _isFiring = context.ReadValueAsButton();
    }
}
