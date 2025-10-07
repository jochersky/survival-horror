using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerSensor : MonoBehaviour
{
    public delegate void PlayerEntered(Transform playerTransform);
    public delegate void PlayerExited(Vector3 lastSeenPlayerPosition);
    public event PlayerEntered OnPlayerEnter;
    public event PlayerExited OnPlayerExit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            OnPlayerEnter?.Invoke(player.transform);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            OnPlayerExit?.Invoke(player.transform.position);
    }
}
