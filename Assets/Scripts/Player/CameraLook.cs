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

    [SerializeField] private GameObject regularCam;
    [SerializeField] private GameObject zoomCam;
    
    private InputActionMap _playerActions;
    private Camera _cam;
    
    // input actions
    private InputAction m_ZoomAction;
    private InputAction m_LookAction;
    
    private Vector2 _mouseInput;

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
    }

    private void Update()
    {
        // rotate camera pivot
        Vector3 viewDir = cameraTarget.transform.position - new Vector3(transform.position.x, cameraTarget.transform.position.y, transform.position.z);
        playerMoveOrientation.transform.forward = viewDir.normalized;
        
        // rotate player object
        if (currentState == CameraState.Zoom)
        {
            player.transform.forward = playerMoveOrientation.transform.forward;
        } 
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
        
        currentState = newState;
    }
}
