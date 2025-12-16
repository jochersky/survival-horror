using UnityEngine;

public class ToggleableButton : Toggleable, IInteractable
{
    [SerializeField] private GameObject malfunctionParticles;
    
    public delegate void ButtonPressed();
    public event ButtonPressed OnButtonPressed;
    
    private bool _buttonPressed = false;
    
    private void Start()
    {
        OnActiveTrue += () => { malfunctionParticles.SetActive(false); };
    }
    
    public void Interact()
    {
        if (!active) return;
        if (!_buttonPressed)
        {
            OnButtonPressed?.Invoke();
            _buttonPressed = true;
        }
    }
}
