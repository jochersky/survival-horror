using System;
using UnityEngine;

public class RemoteCamera : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private OrbitalFollow orbitalFollow;
    [SerializeField] private RemoteCameraDeoccluder remoteCameraDeoccluder;
    
    public CameraTransform UpdateCamera()
    {
        // Get initial position and rotation of camera using the OrbitalFollow component
        CameraTransform ct = orbitalFollow.GetOrbitalCameraTransform();
        // Get adjusted position when colliding with something in the environment using the RemoteCameraDeoccluder component
        ct = remoteCameraDeoccluder.GetDeoccludedTransform(ct, orbitalFollow.MaxOrbitDistance);
        
        // add calls to components based on their order of operations priority
        // ...
        
        return ct;
    }
}

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