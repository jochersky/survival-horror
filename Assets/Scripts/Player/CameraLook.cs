using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerMoveOrientation;

    [SerializeField] private GameObject regularCam;
    [SerializeField] private GameObject zoomCam;
    
    private InputSystem_Actions _actions;
    private InputSystem_Actions.PlayerActions _playerActions;
    private Camera _cam;
    

    // [SerializeField] private float rotationSpeed = 5f;
    
    private Vector2 _mouseInput;

    public CameraState currentState;
    public enum CameraState
    {
        Regular,
        Zoom
    }
    
    private void Awake()
    {
        _cam = GetComponent<Camera>();
        
        _actions = new InputSystem_Actions(); // Asset object
        _playerActions = _actions.Player;     // Extract action map object
        
        // Subscribe the player input callbacks
        _playerActions.AddCallbacks(this);
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

    public void OnMove(InputAction.CallbackContext context) { }

    public void OnSprint(InputAction.CallbackContext context) { }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        bool isZooming = context.ReadValueAsButton();
        SwitchCameraState(isZooming ? CameraState.Zoom : CameraState.Regular);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // throw new NotImplementedException();
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
