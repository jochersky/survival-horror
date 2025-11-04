using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    private InputActionMap _playerActions;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform backTransform;
    public GameObject primary;
    public GameObject secondary;

    private Item primaryItem;
    private Item secondaryItem;
    private WeaponType _primaryType;
    private WeaponType _secondaryType;
    private bool _switchToPrimary = false;
    
    // input actions
    private InputAction m_SwitchWeapon;
    
    public delegate void MeleeWeaponEquipped();
    public event MeleeWeaponEquipped OnMeleeWeaponEquipped;
    public delegate void GunWeaponEquipped();
    public event GunWeaponEquipped OnGunWeaponEquipped;

    void Awake()
    {
        // initialize references
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_SwitchWeapon = _playerActions.FindAction("SwitchWeapon");
        m_SwitchWeapon.started += SwitchWeapon;
    }

    void Start()
    {
        if (primary)
        {
            primaryItem = primary.GetComponentInChildren<Item>();
            primaryItem.Equip();
            primaryItem.MoveToHand();
        }
        // if (secondary)
        // {
        //     secondaryItem = secondary.GetComponentInChildren<Item>();
        //     secondaryItem.Equip();
        // }
    }

    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        // send event to other scripts with what kind of weapon was switched to
        if (_switchToPrimary)
        {
            // if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            // else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
            
            SetTransform(primary.transform, handTransform);
            primaryItem.MoveToHand();
            SetTransform(secondary.transform, backTransform);
            // secondaryItem.MoveToBack();
        }
        else
        {
            // if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            // else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
            
            SetTransform(secondary.transform, handTransform);
            // secondaryItem.MoveToHand();
            SetTransform(primary.transform, backTransform);
            primaryItem.MoveToBack();
        }
        
        _switchToPrimary = !_switchToPrimary;
    }

    private void SetTransform(Transform t, Transform weaponTransform)
    {
        t.SetParent(weaponTransform);
        t.localPosition = Vector3.zero;
        // t.rotation = Quaternion.identity;
    }
    
    public void EquipWeapon(DraggableItem item)
    {
        // TODO: finish implementing this method
        WeaponData wd = (WeaponData)item.itemData;
        
    }
}
