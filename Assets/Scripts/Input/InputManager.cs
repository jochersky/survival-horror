using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Singleton responsible for capturing and distributing player input to scripts.
 */
public class InputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset playerInputActionAsset;
    private InputActionMap _playerActions;
    
    // input actions
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _aimAction;
    private InputAction _attackAction;
    private InputAction _reloadAction;
    private InputAction _interactAction;
    private InputAction _switchWeaponAction;
    private InputAction _flashlightAction;
    private InputAction _mousewheelAction;

    private Vector2 _moveInput;
    private Vector2 _mouseInput;
    private bool _movePressed;
    private bool _aimPressed;
    private bool _attackPressed;
    private bool _reloadPressed;
    private bool _interactPressed;
    private bool _switchWeaponPerformed;
    private bool _flashlightPressed;
    private bool _mouseWheelPerformed;
    
    public static InputManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _playerActions = playerInputActionAsset.FindActionMap("Player");
        
        // assign input action callbacks
        _moveAction = _playerActions.FindAction("Move");
        _moveAction.started += OnMove;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _lookAction = _playerActions.FindAction("Look");
        _lookAction.started += OnLook;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        _aimAction = _playerActions.FindAction("Aim");
        _aimAction.started += OnAim;
        _aimAction.performed += OnAim;
        _aimAction.canceled += OnAim;
        _attackAction = _playerActions.FindAction("Attack");
        _attackAction.started += OnAttack;
        _attackAction.performed += OnAttack;
        _attackAction.canceled += OnAttack;
        _reloadAction = _playerActions.FindAction("Reload");
        _reloadAction.started += OnReload;
        _reloadAction.performed += OnReload;
        _reloadAction.canceled += OnReload;
        _interactAction = _playerActions.FindAction("Interact");
        _interactAction.started += OnInteract;
        _interactAction.performed += OnInteract;
        _interactAction.canceled += OnInteract;
        _switchWeaponAction = _playerActions.FindAction("SwitchWeapon");
        _switchWeaponAction.started += OnSwitchWeapon;
        _switchWeaponAction.performed += OnSwitchWeapon;
        _switchWeaponAction.canceled += OnSwitchWeapon;
        _flashlightAction = _playerActions.FindAction("Flashlight");
        _flashlightAction.started += OnFlashlight;
        _flashlightAction.performed += OnFlashlight;
        _flashlightAction.canceled += OnFlashlight;
        _mousewheelAction = _playerActions.FindAction("MouseWheel");
        _mousewheelAction.started += OnMouseWheel;
        _mousewheelAction.performed += OnMouseWheel;
        _mousewheelAction.canceled += OnMouseWheel;
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        _movePressed = _moveInput != Vector2.zero;
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        _aimPressed = context.ReadValueAsButton();
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        _attackPressed = context.ReadValueAsButton();
    }
    
    public void OnReload(InputAction.CallbackContext context)
    {
        _reloadPressed = context.ReadValueAsButton();
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        _interactPressed = context.ReadValueAsButton();
    }
    
    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        _switchWeaponPerformed = context.ReadValueAsButton();
    }
    
    private void OnFlashlight(InputAction.CallbackContext context)
    {
        _flashlightPressed = context.ReadValueAsButton();
    }

    private void OnMouseWheel(InputAction.CallbackContext context)
    {
        _mouseWheelPerformed = context.ReadValueAsButton();
    }
}
