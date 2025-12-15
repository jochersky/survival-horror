using UnityEngine;

public class Toggleable : MonoBehaviour
{
    public bool active = false;
    
    protected delegate void ActiveTrue();
    protected ActiveTrue OnActiveTrue;

    public void Toggle()
    {
        active = !active;
        if (active) OnActiveTrue?.Invoke();
    }
}
