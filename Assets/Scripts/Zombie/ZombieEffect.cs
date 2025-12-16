using UnityEngine;

public class ZombieEffect : MonoBehaviour
{
    [SerializeField] private ZombieStateMachine zombieStateMachine;
    
    void Start()
    {
        zombieStateMachine.OnAggroChanged += aggroed => { gameObject.SetActive(aggroed); };
        gameObject.SetActive(false);
    }
}
