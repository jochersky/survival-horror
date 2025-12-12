using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Cameras")] 
    [SerializeField] private CameraLook camLook;
    [SerializeField] private CinemachineCamera regularCamera;
    [SerializeField] private CinemachineCamera aimCamera;
    
    public static CameraShake Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin aimCamChannelPerlin;
    private CinemachineBasicMultiChannelPerlin regularCamChannelPerlin;
    
    private float regularTotalShakeTime;
    private float regularShakeTime;
    private float regularInitialIntensity;
    private float aimTotalShakeTime;
    private float aimShakeTime;
    private float aimInitialIntensity;
    
    private void Awake()
    {
        // ensure only one instance of CameraShake exists globally
        if (Instance && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        aimCamChannelPerlin = aimCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (regularShakeTime > 0)
        {
            regularShakeTime -= Time.deltaTime;
            regularCamChannelPerlin.AmplitudeGain = Mathf.Lerp(regularInitialIntensity, 0f, 1 - (regularShakeTime / regularTotalShakeTime));    
        } 
        else if (aimShakeTime > 0)
        {
            aimShakeTime -= Time.deltaTime;
            aimCamChannelPerlin.AmplitudeGain = Mathf.Lerp(aimInitialIntensity, 0f, 1 - (aimShakeTime / aimTotalShakeTime));
        }
    }
    
    public void ShakeAimCamera(float intensity, float duration)
    {
        aimCamChannelPerlin.AmplitudeGain = intensity;
        aimTotalShakeTime = aimShakeTime = duration;
        aimInitialIntensity = intensity;
    }

    public void ShakeRegularCamera(float intensity, float duration)
    {
        regularCamChannelPerlin.AmplitudeGain = intensity;
        regularTotalShakeTime = regularShakeTime = duration;
        regularInitialIntensity = intensity;
    }

    public void ShakeCurrentCamera(float intensity, float duration)
    {
        if (camLook.CurrentState == CameraLook.CameraStates.Regular)
            ShakeRegularCamera(intensity, duration);
        else if (camLook.CurrentState == CameraLook.CameraStates.Aim)
            ShakeAimCamera(intensity, duration);
    }
}
