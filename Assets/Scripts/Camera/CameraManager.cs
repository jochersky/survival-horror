using System;
using System.Diagnostics;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [SerializeField] private RemoteCamera[] remoteCameras;
    
    [SerializeField] private AnimationCurve blend;
    [SerializeField] private float blendTime;

    private Camera _camera;
    
    private RemoteCamera _previousRemoteCamera;
    private RemoteCamera _activeRemoteCamera;

    private bool _blend = false;
    private Vector3 _positionBlendingFrom;
    private Vector3 _positionBlendingTo;
    private Quaternion _rotationBlendingFrom;
    private Quaternion _rotationBlendingTo;
    
    public RemoteCamera ActiveRemoteCamera => _activeRemoteCamera;
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        if (remoteCameras.Length > 0)
        {
            _activeRemoteCamera = remoteCameras[0];
            transform.position = _activeRemoteCamera.transform.position;
            transform.rotation = _activeRemoteCamera.transform.rotation;
            _camera.fieldOfView = _activeRemoteCamera.FOV;
            _activeRemoteCamera.camera = _camera;
        }
    }

    private void LateUpdate()
    {
        CameraTransform ct = _activeRemoteCamera.UpdateCamera();

        transform.position = ct.position;
        transform.rotation = ct.rotation;
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

        // _previousRemoteCamera.gameObject.SetActive(false);
        // _activeRemoteCamera.gameObject.SetActive(true);
    }

    private void BlendBetweenCameras()
    {
        // TODO: actually blend between positions instead of setting final transform immediately
        transform.position = _activeRemoteCamera.transform.position;
        transform.rotation = _activeRemoteCamera.transform.rotation;
    }
}
