using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

[RequireComponent(typeof(RemoteCamera))]
public class OrbitalFollow : MonoBehaviour
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
    
    private RemoteCamera _remoteCamera;

    private Vector2 _mouseInput;
    private float xRotation;
    private float yRotation;
    
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
    }
    
    void Update()
    {
        GetMouseInput();
        
        if (orbitStyle == OrbitStyle.Point) PointOrbit();
        else if (orbitStyle == OrbitStyle.ThreeRing) ThreeRingOrbit();
    }
    
    private void GetMouseInput()
    {
        _mouseInput.x = Input.GetAxis("Mouse X");
        _mouseInput.y = Input.GetAxis("Mouse Y");
    }

    public CameraTransform GetOrbitalCameraTransform()
    {
        return orbitStyle == OrbitStyle.Point ? PointOrbit() : ThreeRingOrbit();
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

    private CameraTransform ThreeRingOrbit()
    {
        // TODO:
        CameraTransform ct = new CameraTransform();
        ct.position = Vector3.zero;
        ct.rotation = Quaternion.identity;
        return ct;
    }
}
