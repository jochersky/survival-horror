using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : Toggleable
{   
    private Light _lightObject;

    [Header("Flicker Properties")]
    [SerializeField] private float timeUntilFlicker = 0.5f;
    [SerializeField] private float flickerDuration = 0.1f;
    [SerializeField] private float flickerOffset = 0.1f;
    private float _timeSinceFlicker;
    
    private void Start()
    {
        _lightObject = GetComponent<Light>();
        
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
            _lightObject.enabled = true;
        }
        
        if (_timeSinceFlicker >= timeUntilFlicker)
        {
            _timeSinceFlicker = 0;
            _lightObject.enabled = false;
        }
        
        _timeSinceFlicker += Time.deltaTime;
    }
}
