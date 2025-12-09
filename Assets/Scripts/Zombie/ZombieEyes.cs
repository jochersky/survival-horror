using UnityEngine;

public class ZombieEyes : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    [SerializeField] private TrailRenderer eyeTrail;

    private void Update()
    {
        transform.position = followTransform.position;
    }
}
