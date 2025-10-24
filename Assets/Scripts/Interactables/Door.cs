using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    public bool locked = false;
    
    enum DoorStates
    {
        Static,
        Closing,
        Opening
    }
    
    private DoorStates _currState = DoorStates.Static;
    
    private int _isUnlockedHash;
    private int _isInteractingHash;

    private bool _unlocked = false;
    
    void Awake()
    {
        _isUnlockedHash = Animator.StringToHash("isUnlocked");
        _isInteractingHash = Animator.StringToHash("interacted");
    }

    void Update()
    {
        if (_currState == DoorStates.Opening)
        {
            animator.SetTrigger(_isInteractingHash);
            _currState = DoorStates.Static;
        }
        else if (_currState == DoorStates.Closing)
        {
            animator.SetTrigger(_isInteractingHash);
            _currState = DoorStates.Static;
        } else if (_currState == DoorStates.Static)
        {
            // hold animation keyframe and do nothing
        }
    }

    public void Interact()
    {
        // don't let the door open if its locked
        // TODO: check player inventory for key to unlock door
        if (locked) return;
        
        if (!_unlocked)
        {
            _unlocked = true;
            animator.SetBool(_isUnlockedHash, true);
        }
        else
        {
            _currState = (_currState == DoorStates.Opening ? DoorStates.Closing : DoorStates.Opening);
        }
    }
}
