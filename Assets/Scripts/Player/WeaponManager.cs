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
    }

    void Start()
    {
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
        if (_switchToPrimary)
        {
            if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
            
            SetTransform(primary.transform, handTransform);
            primaryItem.MoveToHand();
            SetTransform(secondary.transform, backTransform);
            secondaryItem.MoveToBack();
        }
        else
        {
            if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
            
            SetTransform(secondary.transform, handTransform);
            secondaryItem.MoveToHand();
            SetTransform(primary.transform, backTransform);
            primaryItem.MoveToBack();
        }
        
        _switchToPrimary = !_switchToPrimary;
    }

    private void SetTransform(Transform t, Transform weaponTransform)
    {
        t.SetParent(weaponTransform);
        t.localPosition = Vector3.zero;
    }
    
    public GameObject EquipWeapon(GameObject item, ItemData itemData)
    {
        // need this line because we can access the WeaponType through the WeaponData attached
        WeaponData wd = (WeaponData)itemData;
        
        // determine which weapon to equip over
        if (!primary || !_equippedPrimaryLast)
        {
            if (primary) Destroy(primary);
            
            _equippedPrimaryLast = true;
            Transform t = _switchToPrimary ? backTransform : handTransform;
            primary = Instantiate(item, t);
            primaryItem = primary.GetComponentInChildren<Item>();
            primaryItem.Equip();
            _primaryType = wd.weaponType;
            
            if (!_switchToPrimary) primaryItem.MoveToHand();
            else primaryItem.MoveToBack();
            
            if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
            
            return primary;
        }
        if (!secondary || _equippedPrimaryLast)
        {
            if (secondary) Destroy(secondary);
            
            _equippedPrimaryLast = false;
            Transform t = _switchToPrimary ? backTransform : handTransform;
            secondary = Instantiate(item, t);
            secondaryItem = secondary.GetComponentInChildren<Item>();
            secondaryItem.Equip();
            _secondaryType = wd.weaponType;
            
            if (!_switchToPrimary) secondaryItem.MoveToBack();
            else secondaryItem.MoveToHand();
            
            if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();

            return secondary;
        }

        return null;
    }
}
