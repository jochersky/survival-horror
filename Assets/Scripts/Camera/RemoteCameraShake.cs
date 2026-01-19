using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RemoteCameraShake : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0.001f, 1f)] private float shakeAmount = 0.1f;
    [SerializeField] private float shakeDuration = 0.1f;

    private float _shakeTime = 0;
    
    public CameraTransform GetShakenCameraTransform(CameraTransform cameraTransform)
    {
        if (_shakeTime > 0)
        {
            // Random position within a sphere with a radius scaled by shakeAmount used as offset for shaking
            cameraTransform.position += Random.insideUnitSphere * shakeAmount;;
            _shakeTime -= Time.deltaTime;
        }
        
        return cameraTransform;
    }

    public void StartShake()
    {
        _shakeTime = shakeDuration;
    }
}
