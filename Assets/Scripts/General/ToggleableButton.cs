using UnityEngine;

public class ToggleableButton : Toggleable, IInteractable
{
    [SerializeField] private GameObject malfunctionParticles;
    [SerializeField] private AudioSource source;

    private ParticleSystem _particles;
    
    public delegate void ButtonPressed();
    public event ButtonPressed OnButtonPressed;
    
    private bool _buttonPressed = false;
    private float _time = 0;
    
    private void Start()
    {
        OnActiveTrue += () => { malfunctionParticles.SetActive(false); };
        
        _particles = malfunctionParticles.GetComponent<ParticleSystem>();
    }
    
    private void Update()
    {
        if (_time >= _particles.main.duration && _particles.isEmitting)
        {
            AudioManager.Instance.PlaySFX(SfxType.MalfunctioningSpark, source, 1, Random.Range(0.95f, 1.05f));
            _time = 0;
        }
        _time += Time.deltaTime;
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
