using UnityEngine;

public class FuseContainer : FunctionContainer
{
    [SerializeField] private GameObject fuseGraphics;
    [SerializeField] private GameObject malfunctionParticles;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource poweredSource;

    private ParticleSystem _particles;

    private float _time = 0;

    public override void Start()
    {
        base.Start();

        OnToggleFunctionality += () =>
        {
            fuseGraphics.SetActive(true);
            malfunctionParticles.SetActive(false);
            AudioManager.Instance.PlaySFX(SfxType.PowerRestored, poweredSource);
        };
        
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
    
    public override void OnContainerAdded()
    {
        AudioManager.Instance.PlaySFX(SfxType.MetalContainerOpening);
    }

    public override void OnContainerRemoved()
    {
        AudioManager.Instance.PlaySFX(SfxType.MetalContainerClosing);
    }
}
