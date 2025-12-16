using UnityEngine;

public class FuseContainer : FunctionContainer
{
    [SerializeField] private GameObject fuseGraphics;
    [SerializeField] private GameObject malfunctionParticles;

    public override void Start()
    {
        base.Start();

        OnToggleFunctionality += () =>
        {
            fuseGraphics.SetActive(true);
            malfunctionParticles.SetActive(false);
        };
    }
}
