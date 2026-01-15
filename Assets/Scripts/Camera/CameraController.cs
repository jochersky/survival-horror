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
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject playerMoveOrientation;
    [SerializeField] private TextMeshProUGUI cameraLocalPosition;
    
    private InputActionMap _playerActions;
    // input actions
    private InputAction m_AimAction;

    private bool _isAiming;
    private bool _playerDead;
    private LayerMask _mask;

    private void Start()
    {
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_AimAction = actions.FindAction("Aim");
        m_AimAction.started += OnAim;
        m_AimAction.canceled += OnAim;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        _mask = LayerMask.GetMask("Environment");
    }

    private void LateUpdate()
    {
        // don't update when player is dead
        if (_playerDead) return;

        RotatePlayerMoveOrientation();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
    }

    private void RotatePlayerMoveOrientation()
    {
        Vector3 camPos = new Vector3(transform.position.x, cameraTarget.transform.position.y, transform.position.z);
        Vector3 viewDir = cameraTarget.transform.position - camPos;
        playerMoveOrientation.transform.forward = viewDir.normalized;
    }
}
