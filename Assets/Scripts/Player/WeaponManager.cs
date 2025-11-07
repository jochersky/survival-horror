using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    private InputActionMap _playerActions;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform backTransform;
    public WeaponSlot primarySlot;
    public WeaponSlot secondarySlot;
    public GameObject primary;
    public GameObject secondary;

    private Item primaryItem;
    private Item secondaryItem;
    public WeaponType _primaryType;
    public WeaponType _secondaryType;
    private bool _switchToPrimary = false;
    private bool _equippedPrimaryLast = false;
    
    // singleton instance
    public static WeaponManager instance { get; private set; }
    
    // input actions
    private InputAction m_SwitchWeapon;
    
    public delegate void MeleeWeaponEquipped();
    public event MeleeWeaponEquipped OnMeleeWeaponEquipped;
    public delegate void GunWeaponEquipped();
    public event GunWeaponEquipped OnGunWeaponEquipped;

    void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (instance && instance != this) Destroy(this);
        else instance = this;
        
        // initialize references
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_SwitchWeapon = _playerActions.FindAction("SwitchWeapon");
        m_SwitchWeapon.started += SwitchWeapon;
        
        // subscribe to events
        primarySlot.OnWeaponEquipped += EquipPrimary;
        primarySlot.OnWeaponUnequipped += UnequipPrimary;
        secondarySlot.OnWeaponEquipped += EquipSecondary;
        secondarySlot.OnWeaponUnequipped += UnequipSecondary;
    }

    void Start()
    {
        // TODO: get references to weapons already in the slots before calling respective Equip funcs
        
        if (primary)
        {
            primaryItem = primary.GetComponentInChildren<Item>();
            primaryItem.Equip();
            primaryItem.MoveToHand();
        }
        if (secondary)
        {
            secondaryItem = secondary.GetComponentInChildren<Item>();
            secondaryItem.Equip();
            secondaryItem.MoveToBack();
        }
    }

    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        // send event to other scripts with what kind of weapon was switched to
        if (primary && _switchToPrimary)
        {
            if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();

            if (primary)
            {
                SetTransform(primary.transform, handTransform);
                primaryItem.MoveToHand();
            }
            if (secondary)
            {
                SetTransform(secondary.transform, backTransform);
                secondaryItem.MoveToBack();
            }
            
            _switchToPrimary = !_switchToPrimary;
        }
        else if (secondary && !_switchToPrimary)
        {
            if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();

            if (secondary)
            {
                SetTransform(secondary.transform, handTransform);
                secondaryItem.MoveToHand();
            }
            if (primary)
            {
                SetTransform(primary.transform, backTransform);
                primaryItem.MoveToBack();
            }
            _switchToPrimary = !_switchToPrimary;
        }
    }

    private void SetTransform(Transform t, Transform weaponTransform)
    {
        t.SetParent(weaponTransform);
        t.localPosition = Vector3.zero;
    }

    private void EquipPrimary(DraggableItem dragItem)
    {
        if (primary) Destroy(primary);
        
        WeaponData wd = (WeaponData)dragItem.itemData;
        GameObject prefab = dragItem.itemPrefab;
        
        Transform t = _switchToPrimary ? backTransform : handTransform;
        primary = Instantiate(prefab, t);
        primaryItem = primary.GetComponentInChildren<Item>();
        primaryItem.Equip();
        _primaryType = wd.weaponType;
        
        if (!_switchToPrimary) primaryItem.MoveToHand();
        else primaryItem.MoveToBack();
        
        if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
        else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    }

    private void EquipSecondary(DraggableItem dragItem)
    {
        if (secondary) Destroy(secondary);
        
        WeaponData wd = (WeaponData)dragItem.itemData;
        GameObject prefab = dragItem.itemPrefab;
        
        Transform t = _switchToPrimary ? handTransform : backTransform;
        secondary = Instantiate(prefab, t);
        secondaryItem = secondary.GetComponentInChildren<Item>();
        secondaryItem.Equip();
        _secondaryType = wd.weaponType;
        
        if (_switchToPrimary) secondaryItem.MoveToHand();
        else secondaryItem.MoveToBack();
        
        if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
        else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    }

    private void UnequipPrimary()
    {
        primarySlot.item = null;
        Destroy(primary);
        primary = null;
        primaryItem = null;
    }

    private void UnequipSecondary()
    {
        Destroy(secondary);
        secondarySlot.item = null;
        secondary = null;
        secondaryItem = null;
    }
}
