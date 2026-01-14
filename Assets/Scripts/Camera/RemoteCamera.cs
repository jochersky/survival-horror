using System;
using UnityEngine;

public class RemoteCamera : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private OrbitalFollow orbitalFollow;
    
    public CameraTransform UpdateCamera()
    {
        CameraTransform ct = orbitalFollow.GetOrbitalCameraTransform();
 
        // TODO: add calls to components based on their order of operations priority
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