using UnityEngine;
using UnityEngine.UI;

public class ToggleableLight : Toggleable
{
    [SerializeField] private Light lightToActivate;
    
    private void Start()
    {
        lightToActivate.enabled = false;
        OnActiveTrue += () => { lightToActivate.enabled = true; };
    }
}
