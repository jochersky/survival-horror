using UnityEngine;

[RequireComponent(typeof(RemoteCamera))]
public class RemoteCameraDeoccluder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject playerHead;
    [SerializeField] private GameObject playerBody;
    
    [Header("Settings")]
    [Range(0.01f, 2.0f), SerializeField] private float collisionOffset = 0.25f;
    [SerializeField] private float hidePlayerDistance = 0.5f;
    [SerializeField] private bool debug;

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
    
    public CameraTransform GetDeoccludedTransform(CameraTransform remoteCameraTransform, float maxOrbitDistance)
    {
        Vector3 targetPosition = remoteCameraTransform.position;
        Quaternion targetRotation = remoteCameraTransform.rotation;
        
        float distance = Vector3.Distance(cameraTarget.position, targetPosition);
        Vector3 dir = Vector3.Normalize(targetPosition - cameraTarget.position);
        float maxDistance = Vector3.Distance(cameraTarget.position, targetPosition);
        // adjustment to prevent jitter loop to occur (colliding -> fixed -> colliding..)
        if (remoteCameraTransform.colliding) maxDistance += collisionOffset;
        
        if (debug) Debug.DrawRay(cameraTarget.position, dir * maxDistance, Color.red);
        if (Physics.Raycast(cameraTarget.position, dir, out RaycastHit hit, maxDistance, _mask))
        {
            distance = Vector3.Distance(cameraTarget.position, hit.point) - collisionOffset;
            Vector3 offset = new Vector3(0, 0, distance);
            targetPosition = cameraTarget.position - targetRotation * offset;
            remoteCameraTransform.colliding = true;
        }
        else
        {
            remoteCameraTransform.colliding = false;
        }
        
        remoteCameraTransform.position = targetPosition;
        remoteCameraTransform.rotation = targetRotation;

        // hide player mesh when camera is too close so it doesn't obstruct their view
        _playerHeadSkinnedMeshRenderer.enabled = (distance >= hidePlayerDistance);
        _playerBodySkinnedMeshRenderer.enabled = (distance >= hidePlayerDistance);
        
        return remoteCameraTransform;
    }
}
