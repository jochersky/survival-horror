using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    // references
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerMoveOrientation;
    [SerializeField] private Health health;

    [SerializeField] private GameObject regularCam;
    [SerializeField] private GameObject zoomCam;

    [SerializeField] private GameObject crosshair;
    
    private InputActionMap _playerActions;
    private Camera _cam;
    
    // input actions
    private InputAction m_ZoomAction;
    private InputAction m_LookAction;
    
    private Vector2 _mouseInput;
    private bool _playerDead;

    // camera cine machine states
    public CameraState currentState;
    public enum CameraState
    {
        Regular,
        Zoom
    }
    
    private void Awake()
    {
        _cam = GetComponent<Camera>();

        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_ZoomAction = actions.FindAction("Zoom");
        m_ZoomAction.started += OnZoom;
        m_ZoomAction.performed += OnZoom;
        m_ZoomAction.canceled += OnZoom;
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

    public void OnZoom(InputAction.CallbackContext context)
    {
        bool isZooming = context.ReadValueAsButton();
        SwitchCameraState(isZooming ? CameraState.Zoom : CameraState.Regular);
    }

    private void SwitchCameraState(CameraState newState)
    {
        regularCam.SetActive(false);
        zoomCam.SetActive(false);

        if (newState == CameraState.Regular) regularCam.SetActive(true);
        if (newState == CameraState.Zoom) zoomCam.SetActive(true);
        
        crosshair.SetActive(newState == CameraState.Zoom);
        
        currentState = newState;
    }

    private void KillPlayer()
    {
        _playerDead = true;
    }
}
