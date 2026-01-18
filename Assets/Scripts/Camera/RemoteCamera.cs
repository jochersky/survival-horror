using System;
using UnityEngine;

public class RemoteCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(20, 120)] private int fov;
    
    // camera being transformed
    [HideInInspector] public Camera camera;
    
    private RemoteCameraOrbitalFollow _orbitalFollow;
    private RemoteCameraDeoccluder _deoccluder;

    // Struct that keeps track of position and rotation between components
    private CameraTransform _cameraTransform;

    public Camera Camera
    {
        get => camera;
        set
        {
            camera = value;
            _cameraTransform.position = camera.transform.position;
            _cameraTransform.rotation = camera.transform.rotation;
        }
    }

    public int FOV => fov;

    private void Start()
    {
        _orbitalFollow = GetComponent<RemoteCameraOrbitalFollow>();
        _deoccluder = GetComponent<RemoteCameraDeoccluder>();

        _cameraTransform.position = transform.position;
        _cameraTransform.rotation = transform.rotation;
    }
    
    public CameraTransform UpdateCamera()
    {
        // Get initial position and rotation of camera using the orbital follow component
        if (_orbitalFollow) 
            _cameraTransform = _orbitalFollow.GetOrbitalCameraTransform(_cameraTransform);
        // Get adjusted position when colliding with something in the environment using the deoccluder component
        if (_deoccluder) 
            _cameraTransform = _deoccluder.GetDeoccludedTransform(_cameraTransform, _orbitalFollow.MaxOrbitDistance);
        
        // add calls to components based on their order of operations priority
        // ...
        
        return _cameraTransform;
    }
}

// Struct used to combine position vector and quaternion rotation
// for use in RemoteCamera and RemoteCamera components
public struct CameraTransform
{
    public Vector3 position;
    public Quaternion rotation;
   
    // constructor
    public CameraTransform(Vector3 position = default, Quaternion rotation = default)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public override string ToString()
    {
        return "Position: " + position.ToString() + ", Rotation: " + rotation.ToString();;
    }
}