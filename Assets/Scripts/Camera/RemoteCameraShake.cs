using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RemoteCameraShake : MonoBehaviour
{
    // How intense the shake is (radius of sphere to find point in)
    private float _shakeIntensity;
    // Remaining time of camera shake
    private float _shakeTime = 0;
    
    public CameraTransform GetShakenCameraTransform(CameraTransform cameraTransform)
    {
        if (_shakeTime > 0)
        {
            // Shake is done by finding random position in sphere of radius _shakeIntensity
            cameraTransform.position += Random.insideUnitSphere * _shakeIntensity;
            _shakeTime -= Time.deltaTime;
        }
        
        return cameraTransform;
    }

    // Called to begin the camera shaking by setting intensity and duration
    public void StartShake(float intensity, float duration)
    {
        _shakeIntensity = intensity;
        _shakeTime = duration;
    }
}
