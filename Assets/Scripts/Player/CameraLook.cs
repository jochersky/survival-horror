using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    // references
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private GameObject playerMoveOrientation;
    [SerializeField] private Health health;

    [SerializeField] private GameObject regularCam;
    [SerializeField] private GameObject aimCam;

    [SerializeField] private GameObject crosshair;
    
    private InputActionMap _playerActions;
    private Camera _cam;
    
    // input actions
    private InputAction m_AimAction;
    private InputAction m_LookAction;
    
    private Vector2 _mouseInput;
    private bool _isAiming;
    private bool _playerDead;

    // camera cine machine states
    public CameraStates currentState;
    public enum CameraStates
    {
        Regular,
        Aim
    }
    
    private void Awake()
    {
        _cam = GetComponent<Camera>();

        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_AimAction = actions.FindAction("Aim");
        m_AimAction.started += OnAim;
        m_AimAction.performed += OnAim;
        m_AimAction.canceled += OnAim;
        m_LookAction = actions.FindAction("Look");
        m_LookAction.started += OnLook;
        m_LookAction.performed += OnLook;
        m_LookAction.canceled += OnLook;

        // connect health events
        health.OnDeath += () => _playerDead = true;
        
        // connect inventory events
        InventoryManager.instance.OnInventoryVisibilityChanged += (bool vis) =>
        {
            if (vis) _playerActions.Disable();
            else _playerActions.Enable();
            regularCam.SetActive(!vis);
        };
        
        // connect weapon manager events
        WeaponManager.Instance.OnWeaponInHandThrown += () => { SwitchCameraState(CameraStates.Regular); };
        
        crosshair.SetActive(false);
    }

    private void Update()
    {
        // don't update when player is dead
        if (_playerDead) return;
        
        // rotate camera pivot
        Vector3 viewDir = cameraTarget.transform.position - new Vector3(transform.position.x, cameraTarget.transform.position.y, transform.position.z);
        playerMoveOrientation.transform.forward = viewDir.normalized;
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

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
        bool weaponEquipped = WeaponManager.Instance.weaponInHand;
        
        SwitchCameraState(weaponEquipped && _isAiming ? CameraStates.Aim : CameraStates.Regular);
    }

    private void SwitchCameraState(CameraStates newState)
    {
        regularCam.SetActive(false);
        aimCam.SetActive(false);

        if (newState == CameraStates.Regular) regularCam.SetActive(true);
        if (newState == CameraStates.Aim) aimCam.SetActive(true);
        
        crosshair.SetActive(newState == CameraStates.Aim);
        
        currentState = newState;
    }
}
