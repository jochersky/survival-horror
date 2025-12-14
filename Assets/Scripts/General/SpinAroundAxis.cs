using UnityEngine;

public class Spin : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField] private Vector3 axis;
    [SerializeField] private float spinSpeed;

    void Update()
    {
        if (axis == Vector3.zero) return;
        transform.eulerAngles += spinSpeed * Time.deltaTime * axis;
    }
}
