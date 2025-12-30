using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Referneces")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject playerMoveOrientation;

    [Header("Settings")] 
    [SerializeField] private float maxXRotation = 70f;
    [SerializeField] private float minXRotation = -45f;
    [SerializeField] private float maxOrbitDistance = 3f;
    [SerializeField] private float minOrbitDistance = 0.1f;
    [SerializeField] private float xSensitivity = 5f;
    [SerializeField] private float ySensitivity = 5f;
    
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
    }

    private void LateUpdate()
    {
        // don't update when player is dead
        if (_playerDead) return;

        GetMouseInput();
        RotateCameraAroundPoint();
        RotatePlayerMoveOrientation();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
    }

    private void GetMouseInput()
    {
        _mouseInput.x = Input.GetAxis("Mouse X");
        _mouseInput.y = Input.GetAxis("Mouse Y");
        _mouseInput.Normalize();
    }

    private void RotateCameraAroundPoint()
    {
        xRotation -= Input.GetAxis("Mouse Y");
        yRotation += Input.GetAxis("Mouse X");
        
        float xRotationClamped = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        Quaternion targetRotation = Quaternion.Euler(xRotationClamped, yRotation, 0f);

        Vector3 offset = new Vector3(0f, 0f, maxOrbitDistance);
        transform.position = cameraTarget.position - targetRotation * offset;
        transform.rotation = targetRotation;
    }

    private void RotatePlayerMoveOrientation()
    {
        Vector3 camPos = new Vector3(transform.position.x, cameraTarget.transform.position.y, transform.position.z);
        Vector3 viewDir = cameraTarget.transform.position - camPos;
        playerMoveOrientation.transform.forward = viewDir.normalized;
    }
}
