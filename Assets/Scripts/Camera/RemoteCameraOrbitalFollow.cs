using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(RemoteCamera))]
public class RemoteCameraOrbitalFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    
    [Header("Settings")]
    [SerializeField] private OrbitStyle orbitStyle;
    [SerializeField] private float minXRotation;
    [SerializeField] private float maxXRotation;
    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;
    [Header("Point")]
    [SerializeField] private float minOrbitDistance;
    [SerializeField] private float maxOrbitDistance;
    [Header("Three Ring")]
    [SerializeField] private AnimationCurve botCurve;
    [SerializeField] private AnimationCurve topCurve;
    [SerializeField] private SplineContainer botSpline;
    [SerializeField] private SplineContainer midSpline;
    [SerializeField] private SplineContainer topSpline;
    [SerializeField, Range(0, 1)] private float interpolationSpeed; 
    
    private RemoteCamera _remoteCamera;

    private Vector2 _mouseInput;
    private float xRotation;
    private float yRotation;
    private float _xRotationRange;
    private bool _ringsAndCurvesAssigned;
    
    private CameraTransform _cameraTransform;
    
    public float MaxOrbitDistance => maxOrbitDistance;
    
    private enum OrbitStyle
    {
        Point,
        ThreeRing
    }
    
    private void Start()
    {
        _remoteCamera = GetComponent<RemoteCamera>();
        _ringsAndCurvesAssigned = botCurve != null && topCurve != null&& botSpline && midSpline && topSpline;
    }
    
    void Update()
    {
        GetMouseInput();
        
        // if (orbitStyle == OrbitStyle.Point) PointOrbit();
        // else if (orbitStyle == OrbitStyle.ThreeRing) ThreeRingOrbit(_remoteCamera.CameraTransform);
    }
    
    private void GetMouseInput()
    {
        _mouseInput.x = Input.GetAxis("Mouse X");
        _mouseInput.y = Input.GetAxis("Mouse Y");
    }

    public CameraTransform GetOrbitalCameraTransform(CameraTransform remoteCameraTransform)
    {
        return orbitStyle == OrbitStyle.Point ? PointOrbit() : ThreeRingOrbit(remoteCameraTransform);
    }
    
    private CameraTransform PointOrbit()
    {
        // update rotation with mouse input
        xRotation -= _mouseInput.y * xSensitivity;
        yRotation += _mouseInput.x * ySensitivity;
        float xRotationClamped = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        xRotation = xRotationClamped;
        
        _cameraTransform.rotation = Quaternion.Euler(xRotationClamped, yRotation, 0f);
        Vector3 offset = new Vector3(0f, 0f, maxOrbitDistance);
        _cameraTransform.position = cameraTarget.position - _cameraTransform.rotation * offset;

        return _cameraTransform;
    }

    private CameraTransform ThreeRingOrbit(CameraTransform remoteCameraTransform)
    {
        // Must have splines assigned before computing new transform
        if (!_ringsAndCurvesAssigned) return _cameraTransform;
        
        // update rotation with mouse input
        xRotation -= _mouseInput.y * xSensitivity;
        yRotation += _mouseInput.x * ySensitivity;
        
        // can include negative bound(s)
        float xRotationClamped = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        xRotation = xRotationClamped;
        
        // Determine position along splines based on y-axis rotation
        float pointRatio = (yRotation % 360) / 360;
        if (pointRatio < 0) pointRatio += 1;
        
        // Calculate rotation only using vector looking at camera target
        Quaternion targetRotation = Quaternion.LookRotation(cameraTarget.position - remoteCameraTransform.position);

        Vector3 botSplinePosition = botSpline.EvaluatePosition(pointRatio);
        Vector3 midSplinePosition = midSpline.EvaluatePosition(pointRatio);
        Vector3 topSplinePosition = topSpline.EvaluatePosition(pointRatio);
        
        // Distances between splines along y-axis
        float botDist = Math.Abs(midSplinePosition.y - botSplinePosition.y);
        float topDist = Math.Abs(topSplinePosition.y - midSplinePosition.y);
        // Ratios that represent the bottom's and top's share of the total degree of rotation along x-axis
        float botRotationRatio = botDist / (botDist + topDist);
        float topRotationRatio = 1 - botRotationRatio;
        
        float rotationRatio = (xRotationClamped + Math.Abs(minXRotation)) / _xRotationRange;
        float botMaxRotation = _xRotationRange * botRotationRatio;
        float topMaxRotation = _xRotationRange * topRotationRatio;
        
        Vector3 targetPosition;
        
        // Use the top-half curve
        if (xRotationClamped >= botMaxRotation)
        {
            // x value on curve and position scalar. add min rotation to account for negative value. range: [0, 1]
            float t = (xRotationClamped - botMaxRotation) / (maxXRotation - botMaxRotation);
            // y value on curve
            float yFactor = topCurve.Evaluate(t);
            // use difference vector to determine x/z component of new position
            Vector3 positionBetweenSplines = midSplinePosition + (topSplinePosition - midSplinePosition) * t;
            positionBetweenSplines.y = midSplinePosition.y + Math.Abs(topSplinePosition.y - midSplinePosition.y) * yFactor;
            targetPosition = positionBetweenSplines;
        }
        // Use the bottom-half curve
        else
        {
            // x value on curve and position scalar. add min rotation to account for negative value. range: [0, 1]
            float t = (xRotationClamped + Math.Abs(minXRotation)) / (botMaxRotation + Math.Abs(minXRotation));
            // y value on curve
            float yFactor = botCurve.Evaluate(t);
            // use difference vector to determine x/z component of new position
            Vector3 positionBetweenSplines = botSplinePosition + (midSplinePosition - botSplinePosition) * t;
            positionBetweenSplines.y = botSplinePosition.y + Math.Abs(midSplinePosition.y - botSplinePosition.y) * yFactor;
            targetPosition = positionBetweenSplines;
        }
        
        _cameraTransform.position = Vector3.Lerp(remoteCameraTransform.position, targetPosition, 0.2f);
        _cameraTransform.rotation = Quaternion.Lerp(remoteCameraTransform.rotation, targetRotation, 0.6f);
        
        return _cameraTransform;
    }
}
