using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [Header("Instance Values")]
    [SerializeField] private string keyName;
    public bool locked;
    public bool inaccessible;
    
    enum DoorStates
    {
        Static,
        Closing,
        Opening
    }
    
    private DoorStates _currState = DoorStates.Static;
    
    private int _isUnlockedHash;
    private int _isInteractingHash;

    // used to go from initial uninteracted state (Static) to interacted state (Closing & Opening)
    private bool _unlocked = false;
    
    void Awake()
    {
        _isUnlockedHash = Animator.StringToHash("isUnlocked");
        _isInteractingHash = Animator.StringToHash("interacted");
    }

    void Update()
    {
        if (_currState == DoorStates.Opening || _currState == DoorStates.Closing)
        {
            animator.SetTrigger(_isInteractingHash);
            _currState = DoorStates.Static;
        }
        else if (_currState == DoorStates.Static)
        {
            // hold animation keyframe and do nothing
        }
    }

    public void Interact()
    {
        // don't let the door open if its locked or inaccessible
        if (locked || inaccessible)
        {
            if (InventoryManager.instance.FindKeyInPlayerInventory(keyName)) locked = false;
            return;
        }
        
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
