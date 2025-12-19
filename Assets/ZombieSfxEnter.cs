using UnityEngine;

public class ZombieSfxEnter : StateMachineBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private SfxType type;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
