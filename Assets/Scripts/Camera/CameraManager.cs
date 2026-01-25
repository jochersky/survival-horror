using System;
using System.Diagnostics;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [SerializeField] private RemoteCamera[] remoteCameras;

    [Header("Blend Settings")]
    [SerializeField] private bool blendTransforms = false;
    [SerializeField] private bool blendFOV = false;
    [SerializeField] private AnimationCurve blend;
    [SerializeField] private float blendSpeed = 1;
    

    private Camera _camera;
    
    private RemoteCamera _previousRemoteCamera;
    private RemoteCamera _activeRemoteCamera;

    private bool _blend = false;
    private float _blendPoint = 0;
    
    public RemoteCamera ActiveRemoteCamera => _activeRemoteCamera;
    
    private bool _stopUpdating = false;
    public bool StopUpdating { get => _stopUpdating; set => _stopUpdating = value; }
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        if (remoteCameras.Length > 0)
        {
            _activeRemoteCamera = remoteCameras[0];
            transform.position = _activeRemoteCamera.transform.position;
            transform.rotation = _activeRemoteCamera.transform.rotation;
            _camera.fieldOfView = _activeRemoteCamera.FOV;
            _activeRemoteCamera.Camera = _camera;
        }
    }

    private void LateUpdate()
    {
        if (_stopUpdating) return;

        if (!_blend)
        {
            CameraTransform ct = _activeRemoteCamera.UpdateCamera();

            transform.position = ct.position;
            transform.rotation = ct.rotation;
        }
        else
        {
            BlendBetweenCameras();
        }
    }

    public void SwitchRemoteCamera(RemoteCamera newCamera)
    {
        // 1. update camera manager fields
        // 2. begin blending process between cameras
        // 3?. make the new camera active and the old camera inactive
        
        _previousRemoteCamera = _activeRemoteCamera;
        _activeRemoteCamera = newCamera;
        
        BlendBetweenCameras();

        _camera.fieldOfView = newCamera.FOV;

        _blend = true;
        _blendPoint = 0;
    }

    private void BlendBetweenCameras()
    {
        _blendPoint = Mathf.Clamp(_blendPoint + Time.deltaTime * blendSpeed, 0, 1);
        float t = blend.Evaluate(_blendPoint);
        
        if (!blendTransforms)
        {
            transform.position = _activeRemoteCamera.transform.position;
            transform.rotation = _activeRemoteCamera.transform.rotation;
        }
        else
        {
            CameraTransform prevCT = _previousRemoteCamera.CameraTransform;
            CameraTransform activeCT = _activeRemoteCamera.CameraTransform;
            transform.position = Vector3.Lerp(prevCT.position, activeCT.position, t);
            transform.rotation = Quaternion.Lerp(prevCT.rotation, activeCT.rotation, t);
        }

        if (!blendFOV)
        {
            _camera.fieldOfView = _activeRemoteCamera.FOV;
        }
        else
        {
            _camera.fieldOfView = Mathf.Lerp(_previousRemoteCamera.FOV, _activeRemoteCamera.FOV, t);
        }

        if (!blendTransforms || _blendPoint >= 1)
        {
            _blend = false;
            _blendPoint = 0;
        }
    }
}
