using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraShake : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private RemoteCameraShake explorationCameraShake;
    [SerializeField] private RemoteCameraShake aimCameraShake;

    private CameraController _cameraController;
    
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        // ensure only one instance of CameraShake exists globally
        if (Instance && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        _cameraController = GetComponent<CameraController>();
    }

    public void ShakeAimCamera(float intensity, float duration)
    {
        aimCameraShake.StartShake(intensity, duration);
    }

    public void ShakeExplorationCamera(float intensity, float duration)
    {
        explorationCameraShake.StartShake(intensity, duration);
    }

    public void ShakeCurrentCamera(float intensity, float duration)
    {
        if (_cameraController.CurrentState == CameraController.CameraStates.Exploration)
            ShakeExplorationCamera(intensity, duration);
        else
        if (_cameraController.CurrentState == CameraController.CameraStates.Aim)
            ShakeAimCamera(intensity, duration);
    }
}
