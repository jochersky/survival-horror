using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

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
    [Range(0.01f, 2.0f), SerializeField] private float collisionOffset = 0.25f;
    
    [SerializeField] private AnimationCurve botCurve;
    [SerializeField] private AnimationCurve topCurve;
    [SerializeField] private SplineContainer botSpline;
    [SerializeField] private SplineContainer midSpline;
    [SerializeField] private SplineContainer topSpline;
    
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

    private float topSplineRatio;
    private float botSplineRatio;
    
    private LayerMask _mask;

    private float _xRotationRange;
    
    private void Start()
    {
        _xRotationRange = Math.Abs(maxXRotation) + Math.Abs(minXRotation);
        
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
        //RotateCameraAroundPoint();
        RotatePlayerMoveOrientation();
        RotateCameraAroundSpline();

        cameraLocalPosition.text = "topSplineRatio:  " + topSplineRatio + "\nbotSplineRatio: " + botSplineRatio;
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
            distance -= collisionOffset; // subtract how much the camera is offset from hit point
            offset.z = distance;
            // recalculate the target position with the new offset
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

    private void RotateCameraAroundSpline()
    {
        // update rotation with mouse input
        xRotation -= _mouseInput.y * xSensitivity;
        yRotation += _mouseInput.x * ySensitivity;
        float xRotationClamped = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        
        // Determine position along splines based on y-axis rotation
        float pointRatio = (yRotation % 360) / 360;
        if (pointRatio < 0) pointRatio += 1;
        
        Quaternion targetRotation = Quaternion.LookRotation(cameraTarget.position - transform.position);

        Vector3 botSplinePosition = botSpline.EvaluatePosition(pointRatio);
        Vector3 midSplinePosition = midSpline.EvaluatePosition(pointRatio);
        Vector3 topSplinePosition = topSpline.EvaluatePosition(pointRatio);
        
        float topDist = Math.Abs(topSplinePosition.y - midSplinePosition.y);
        float botDist = Math.Abs(midSplinePosition.y - botSplinePosition.y);
        float botRotationRatio = botDist / (botDist + topDist);
        float rotationRatio = xRotationClamped / _xRotationRange;
        
        Vector3 targetPosition;
        
        // TODO: this is the spline ratio of the whole camera system, need between individual splines
        float splineRatio = (xRotation + Math.Abs(minXRotation)) / _xRotationRange;
        
        // Use the top-half curve
        if (rotationRatio >= botRotationRatio)
        {
            // Determine height along curve based on x-axis rotation
            float topRotationRatio = 1 - botRotationRatio;
            topSplineRatio = (topRotationRatio* (xRotationClamped + Math.Abs(minXRotation))) / (topRotationRatio * _xRotationRange);
            Vector3 positionBetweenSplines = Vector3.Lerp(midSplinePosition, topSplinePosition, topSplineRatio);
            float yFactor = topCurve.Evaluate(topSplineRatio);
            positionBetweenSplines.y = midSplinePosition.y + Math.Abs(topSplinePosition.y - midSplinePosition.y) * yFactor;
            targetPosition = positionBetweenSplines;
        }
        // Use the bottom-half curve
        else
        {
            // Determine height along curve based on x-axis rotation
            botSplineRatio = (botRotationRatio * (xRotationClamped + Math.Abs(minXRotation))) / (botRotationRatio * _xRotationRange);
            Vector3 positionBetweenSplines = Vector3.Lerp(botSplinePosition, midSplinePosition, botSplineRatio);
            float yFactor = botCurve.Evaluate(botSplineRatio);
            positionBetweenSplines.y = botSplinePosition.y + Math.Abs(midSplinePosition.y - botSplinePosition.y) * yFactor;
            targetPosition = positionBetweenSplines;
        }
        
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
