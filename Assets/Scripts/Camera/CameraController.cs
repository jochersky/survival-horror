using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Referneces")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject playerMoveOrientation;
    [SerializeField] private TextMeshProUGUI cameraLocalPosition;

    [Header("Settings")] 
    [SerializeField] private float maxXRotation = 70f;
    [SerializeField] private float minXRotation = -45f;
    [SerializeField] private float maxOrbitDistance = 3f;
    [SerializeField] private float minOrbitDistance = 0.1f;
    [SerializeField] private float xSensitivity = 1f;
    [SerializeField] private float ySensitivity = 1f;
    [Range(0.01f, 2.0f), SerializeField] private float cameraRadius = 0.25f;
    [Range(0.01f, 0.5f), SerializeField] private float adjustAmount = 0.25f;
    
    private Camera _cam;
    
    private InputActionMap _playerActions;
    
    // input actions
    private InputAction m_AimAction;

    private Vector2 _oldMousePos;
    private Vector2 _mouseInput;
    private float xRotation;
    private float yRotation;
    private bool _isAiming;
    private bool _playerDead;
    
    private LayerMask _mask;
    
    private void Start()
    {
        _cam = GetComponent<Camera>();
        
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_AimAction = actions.FindAction("Aim");
        m_AimAction.started += OnAim;
        m_AimAction.canceled += OnAim;
        
        transform.position = cameraTarget.transform.position - cameraTarget.transform.forward * maxOrbitDistance;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Assign layers that the gun collides with
        _mask = LayerMask.GetMask("Environment");
    }

    private void LateUpdate()
    {
        // don't update when player is dead
        if (_playerDead) return;

        GetMouseInput();
        RotateCameraAroundPoint();
        RotatePlayerMoveOrientation();

        cameraLocalPosition.text = "Local Pos: (" + Math.Round(transform.localPosition.x, 2) + ", " + Math.Round(transform.localPosition.y, 2) + ", " + Math.Round(transform.localPosition.z, 2) + ")";
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
    }

    private void GetMouseInput()
    {
        _mouseInput.x = Input.GetAxis("Mouse X");
        _mouseInput.y = Input.GetAxis("Mouse Y");
    }

    private void RotateCameraAroundPoint()
    {
        // update rotation with mouse input
        xRotation -= _mouseInput.y * xSensitivity;
        yRotation += _mouseInput.x * ySensitivity;

        float xRotationClamped = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        Quaternion targetRotation = Quaternion.Euler(xRotationClamped, yRotation, 0f);
        Vector3 offset = new Vector3(0f, 0f, maxOrbitDistance);
        Vector3 targetPosition = cameraTarget.position - targetRotation * offset;
        
        // mouse input caused collision, adjust transform/rotation
        if (Physics.SphereCast(
                cameraTarget.position, cameraRadius, targetPosition.normalized, 
                out RaycastHit hit, maxOrbitDistance, _mask))
        {
            float distance = Vector3.Distance(cameraTarget.position, hit.point);
            distance -= 0.2f; // subtract how much the camera is offset from hit point

            //if (Math.Abs(distance) < 0.2f)
            //{
            //    distance -= 0.2f;
            //}

            offset.z = distance;
            targetPosition = cameraTarget.position - targetRotation * offset;
        }
        
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    private void RotatePlayerMoveOrientation()
    {
        Vector3 camPos = new Vector3(transform.position.x, cameraTarget.transform.position.y, transform.position.z);
        Vector3 viewDir = cameraTarget.transform.position - camPos;
        playerMoveOrientation.transform.forward = viewDir.normalized;
    }
}
