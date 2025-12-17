using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

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
    private DoorStates _prevState = DoorStates.Static;
    
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
            SfxType type = (_currState == DoorStates.Opening ? SfxType.DoorOpen : SfxType.DoorClose);
            AudioManager.Instance.PlaySFX(type, audioSource);
            _prevState = (_currState == DoorStates.Opening) ? DoorStates.Opening : DoorStates.Closing;
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
            if (InventoryManager.instance.FindKeyInPlayerInventory(keyName))
            {
                AudioManager.Instance.PlaySFX(SfxType.DoorUnlock, audioSource);
                locked = false;
            }
            else
            {
                // TODO: play thud sound
                // AudioManager.Instance.PlaySFX(unlockSFX, audioSource);
            }
            return;
        }
        
        if (!_unlocked)
        {
            _unlocked = true;
            animator.SetBool(_isUnlockedHash, true);
            AudioManager.Instance.PlaySFX(SfxType.DoorOpen, audioSource);
        }
        else
        {
            if (_prevState == DoorStates.Static) _currState = DoorStates.Closing;
            else _currState = (_prevState == DoorStates.Opening) ? DoorStates.Closing : DoorStates.Opening;
        }
    }
}
