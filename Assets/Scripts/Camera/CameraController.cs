using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

/*
 * Script which tells the CameraManager script when to switch between cameras.
 */
public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform exploreCameraTarget;
    [SerializeField] private Transform aimCameraTarget;
    [SerializeField] private GameObject playerMoveOrientation;
    [SerializeField] private Health health;
    [SerializeField] private RemoteCamera explorationCamera;
    [SerializeField] private RemoteCamera aimCamera;
    [SerializeField] private GameObject crosshair;
    private CameraManager _cameraManager;
    
    private InputActionMap _playerActions;
    // input actions
    private InputAction m_AimAction;

    private bool _isAiming;
    private bool _playerDead;
    private LayerMask _mask;
    
    private bool usingAimCamera = false;

    private void Start()
    {
        _cameraManager = GetComponent<CameraManager>();
        _playerActions = actions.FindActionMap("Player");
        _mask = LayerMask.GetMask("Environment");
        
        // assign input action callbacks
        m_AimAction = actions.FindAction("Aim");
        m_AimAction.started += OnAim;
        m_AimAction.canceled += OnAim;
        
        // connect health events
        health.OnDeath += () => _playerDead = true;
        
        // connect inventory events
        InventoryManager.instance.OnInventoryVisibilityChanged += (bool vis) =>
        {
            if (vis) _playerActions.Disable();
            else _playerActions.Enable();
            _cameraManager.ActiveRemoteCamera.gameObject.SetActive(!vis);
        };
        
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // don't update when player is dead
        if (_playerDead) return;

        RotatePlayerMoveOrientation();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
        bool weaponEquipped = WeaponManager.Instance.weaponInHand;
        if (!weaponEquipped) return;

        bool switchToAimCamera = context.started;
        _cameraManager.SwitchRemoteCamera(switchToAimCamera ? aimCamera : explorationCamera);
        usingAimCamera = switchToAimCamera;
        crosshair.SetActive(switchToAimCamera);
    }

    private void RotatePlayerMoveOrientation()
    {
        // we don't want to update the orientation when the mouse hasn't moved so that movement
        // is predictable when the camera changes its transform suddenly (e.g. collision with wall)
        bool mouseMoved = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) != Vector2.zero;
        if (!usingAimCamera && !mouseMoved) return;
        
        Vector3 cameraTargetPosition = usingAimCamera ? aimCameraTarget.position : exploreCameraTarget.position;
        Vector3 camPos = new Vector3(transform.position.x, cameraTargetPosition.y, transform.position.z);
        Vector3 viewDir = cameraTargetPosition - camPos;
        playerMoveOrientation.transform.forward = viewDir.normalized;
    }
}
