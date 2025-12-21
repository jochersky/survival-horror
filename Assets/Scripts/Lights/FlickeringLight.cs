using UnityEngine;

public class FlickeringLight : Toggleable
{   
    [SerializeField] private Light lightObject;

    [Header("Flicker Properties")]
    [SerializeField] private float timeUntilFlicker = 0.5f;
    [SerializeField] private float flickerDuration = 0.1f;
    [SerializeField] private float flickerOffset = 0.1f;
    private float _timeSinceFlicker;

    private void Start()
    {
        _timeSinceFlicker = flickerOffset;
    }
    
    private void Update()
    {
        if (!active) FlickerLight();
    }

    private void FlickerLight()
    {
        if (_timeSinceFlicker >= flickerDuration)
        {
            lightObject.enabled = true;
        }
        
        if (_timeSinceFlicker >= timeUntilFlicker)
        {
            _timeSinceFlicker = 0;
            lightObject.enabled = false;
        }
        
        _timeSinceFlicker += Time.deltaTime;
    }
}
