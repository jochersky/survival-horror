using UnityEngine;

[RequireComponent(typeof(RemoteCamera))]
public class RemoteCameraDeoccluder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    
    [Header("Settings")]
    [Range(0.01f, 2.0f), SerializeField] private float cameraRadius = 0.25f;
    [Range(0.01f, 2.0f), SerializeField] private float collisionOffset = 0.25f;

    private LayerMask _mask;
    private CameraTransform _cameraTransform;

    private void Start()
    {
        _mask = LayerMask.GetMask("Environment");
    }
    
    public CameraTransform GetDeoccludedTransform(CameraTransform oldCameraTransform, float maxOrbitDistance)
    {
        Vector3 targetPosition = oldCameraTransform.position;
        Quaternion targetRotation = oldCameraTransform.rotation;

        // Cast sphere from the camera target to the desired position to check for collisions
        if (Physics.SphereCast(
            cameraTarget.position, cameraRadius, targetPosition.normalized,
            out RaycastHit hit, maxOrbitDistance, _mask))
        {
            // collision occured, bring camera forward with a shorter distance to the camera target
            float distance = Vector3.Distance(cameraTarget.position, hit.point);
            distance -= collisionOffset; 
            // subtract how much the camera is offset from hit point
            Vector3 offset = new Vector3(0, 0, distance);
            // recalculate the old camera position with the new offset
            targetPosition = cameraTarget.position - targetRotation * offset;
        }
        
        _cameraTransform.position = targetPosition;
        _cameraTransform.rotation = targetRotation;
        
        return _cameraTransform;
    }
}
