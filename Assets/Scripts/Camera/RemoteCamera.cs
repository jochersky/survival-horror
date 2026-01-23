using System;
using UnityEngine;

public class RemoteCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(20, 120)] private int fov;
    
    private RemoteCameraOrbitalFollow _orbitalFollow;
    private RemoteCameraShake _shake;
    private RemoteCameraDeoccluder _deoccluder;

    // Struct that keeps track of position and rotation between components
    private CameraTransform _cameraTransform;

    // camera being transformed
    private Camera _camera;
    public Camera Camera
    {
        get => _camera;
        set
        {
            _camera = value;
            _cameraTransform.position = _camera.transform.position;
            _cameraTransform.rotation = _camera.transform.rotation;
        }
    }

    public int FOV => fov;

    private void Start()
    {
        _orbitalFollow = GetComponent<RemoteCameraOrbitalFollow>();
        _shake = GetComponent<RemoteCameraShake>();
        _deoccluder = GetComponent<RemoteCameraDeoccluder>();

        _cameraTransform.position = transform.position;
        _cameraTransform.rotation = transform.rotation;
        _cameraTransform.colliding = false;
        _cameraTransform.mouseMoved = false;
    }
    
    public CameraTransform UpdateCamera()
    {
        // Get initial position and rotation of camera using the orbital follow component
        if (_orbitalFollow)
            _cameraTransform = _orbitalFollow.GetOrbitalCameraTransform(_cameraTransform);
        // Shake camera by offsetting its position
        if (_shake)
            _cameraTransform = _shake.GetShakenCameraTransform(_cameraTransform);
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
    // Position of camera passed between RemoteCamera components
    public Vector3 position;
    // Rotation of camera passed between RemoteCamera components
    public Quaternion rotation;
    // Indicator of the camera colliding and deoccluding itself
    public bool colliding;
    // Indicator of whether the player influenced the position by moving the mouse
    public bool mouseMoved;
   
    // constructor
    public CameraTransform(Vector3 position = default, Quaternion rotation = default,  bool colliding = false, bool mouseMoved = false)
    {
        this.position = position;
        this.rotation = rotation;
        this.colliding = false;
        this.mouseMoved = false;
    }

    public override string ToString()
    {
        return "Position: " + position.ToString() + 
               ", Rotation: " + rotation.ToString() + 
               ", Colliding: " + colliding.ToString() + 
               ", Mouse Moved: " + mouseMoved.ToString();
    }
}