using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public delegate void SwingFinished();
    public event SwingFinished OnSwingFinished;
    
    public void SwingEnded()
    {
        OnSwingFinished?.Invoke();
    }
}
