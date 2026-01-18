using UnityEngine;

[RequireComponent(typeof(RemoteCamera))]
public class RemoteCameraDeoccluder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject playerHead;
    [SerializeField] private GameObject playerBody;
    
    [Header("Settings")]
    [Range(0.01f, 2.0f), SerializeField] private float cameraRadius = 0.25f;
    [Range(0.01f, 2.0f), SerializeField] private float collisionOffset = 0.25f;
    [SerializeField] private float hidePlayerDistance = 0.5f;

    private LayerMask _mask;
    private SkinnedMeshRenderer _playerHeadSkinnedMeshRenderer;
    private SkinnedMeshRenderer _playerBodySkinnedMeshRenderer;
    private CameraTransform _cameraTransform;

    private void Start()
    {
        _mask = LayerMask.GetMask("Environment");
        _playerHeadSkinnedMeshRenderer = playerHead.GetComponent<SkinnedMeshRenderer>();
        _playerBodySkinnedMeshRenderer = playerBody.GetComponent<SkinnedMeshRenderer>();
    }
    
    public CameraTransform GetDeoccludedTransform(CameraTransform oldCameraTransform, float maxOrbitDistance)
    {
        Vector3 targetPosition = oldCameraTransform.position;
        Quaternion targetRotation = oldCameraTransform.rotation;
        float distance = Vector3.Distance(cameraTarget.position, targetPosition);
        
        // Cast sphere from the camera target to the desired position to check for collisions
        if (Physics.SphereCast(
            cameraTarget.position, cameraRadius, targetPosition.normalized,
            out RaycastHit hit, maxOrbitDistance, _mask))
        {
            // collision occured, bring camera forward with a shorter distance to the camera target
            distance = Vector3.Distance(cameraTarget.position, hit.point);
            distance -= collisionOffset; 
            // subtract how much the camera is offset from hit point
            Vector3 offset = new Vector3(0, 0, distance);
            // recalculate the old camera position with the new offset
            targetPosition = cameraTarget.position - targetRotation * offset;
        }
        
        _cameraTransform.position = targetPosition;
        _cameraTransform.rotation = targetRotation;

        // hide player mesh when camera is too close
        _playerHeadSkinnedMeshRenderer.enabled = (distance >= hidePlayerDistance);
        _playerBodySkinnedMeshRenderer.enabled = (distance >= hidePlayerDistance);
        
        return _cameraTransform;
    }
}
