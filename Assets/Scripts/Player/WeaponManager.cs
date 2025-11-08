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
    public GameObject primaryObj;
    public GameObject secondaryObj;

    private Item primaryItem;
    private Item secondaryItem;
    public WeaponType _primaryType;
    public WeaponType _secondaryType;
    private bool _switchToPrimary = false;
    private bool _equippedPrimaryLast = false;
    private Weapon weaponInHand;
    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    
    public bool _isZooming;
    public bool _isPressingFire;
    
    // singleton instance
    public static WeaponManager instance { get; private set; }
    
    // input actions
    private InputAction m_SwitchWeapon;
    private InputAction m_ZoomAction;
    private InputAction m_AttackAction;
    
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
        m_ZoomAction = _playerActions.FindAction("Zoom");
        m_ZoomAction.started += OnZoom;
        m_ZoomAction.canceled += OnZoom;
        m_AttackAction = _playerActions.FindAction("Attack");
        m_AttackAction.started += OnAttack;
        m_AttackAction.canceled += OnAttack;
        
        // subscribe to events
        primarySlot.OnWeaponEquipped += EquipPrimary;
        primarySlot.OnWeaponUnequipped += UnequipPrimary;
        secondarySlot.OnWeaponEquipped += EquipSecondary;
        secondarySlot.OnWeaponUnequipped += UnequipSecondary;
    }

    void Start()
    {
        // TODO: get references to weapons already in the slots before calling respective Equip funcs
        
        if (primaryObj)
        {
            primaryItem = primaryObj.GetComponentInChildren<Item>();
            primaryItem.Equip();
            primaryItem.MoveToHand();
        }
        if (secondaryObj)
        {
            secondaryItem = secondaryObj.GetComponentInChildren<Item>();
            secondaryItem.Equip();
            secondaryItem.MoveToBack();
        }
    }
    
    public void OnZoom(InputAction.CallbackContext context)
    {
        _isZooming = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isPressingFire = context.ReadValueAsButton();

        if (!weaponInHand) return;
        
        if (_isZooming) weaponInHand.AimAttack();
        else weaponInHand.SwingAttack();
    }

    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (weaponInHand == primaryWeapon) SwitchToSecondary();
        else if (weaponInHand == secondaryWeapon) SwitchToPrimary();
    }

    private void SwitchToPrimary()
    {
        weaponInHand = primaryWeapon;
            
        if (primaryObj) SetTransform(primaryObj.transform, handTransform, primaryItem);
        if (secondaryObj) SetTransform(secondaryObj.transform, backTransform, secondaryItem);
            
        if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
        else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    }

    private void SwitchToSecondary()
    {
        weaponInHand = secondaryWeapon;
            
        if (secondaryObj) SetTransform(secondaryObj.transform, handTransform, secondaryItem);
        if (primaryObj) SetTransform(primaryObj.transform, backTransform, primaryItem);
            
        if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
        else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    }

    private void SetTransform(Transform t, Transform weaponTransform, Item item)
    {
        t.SetParent(weaponTransform);
        t.localPosition = Vector3.zero;
        if (weaponTransform == handTransform) item.MoveToHand();
        else item.MoveToBack();
    }

    private void EquipPrimary(DraggableItem dragItem)
    {
        if (primaryObj) Destroy(primaryObj);
        
        WeaponData wd = (WeaponData)dragItem.itemData;
        GameObject prefab = dragItem.itemPrefab;
        primaryObj = Instantiate(prefab);
        Weapon w = primaryObj.GetComponent<Weapon>();
        
        bool primaryInHand = weaponInHand == primaryWeapon;
        if (primaryInHand) weaponInHand = w;
        primaryWeapon = w;
        primaryItem = primaryObj.GetComponentInChildren<Item>();
        _primaryType = wd.weaponType;

        SetTransform(primaryObj.transform, primaryInHand ? handTransform : backTransform, primaryItem);
        primaryItem.Equip();
    }

    private void EquipSecondary(DraggableItem dragItem)
    {
        if (secondaryObj) Destroy(secondaryObj);
        
        WeaponData wd = (WeaponData)dragItem.itemData;
        GameObject prefab = dragItem.itemPrefab;
        secondaryObj = Instantiate(prefab);
        Weapon w = secondaryObj.GetComponent<Weapon>();
        
        bool primaryInHand = weaponInHand == primaryWeapon;
        if (!primaryInHand) weaponInHand = w;
        secondaryWeapon = w;
        secondaryItem = secondaryObj.GetComponentInChildren<Item>();
        _secondaryType = wd.weaponType;

        SetTransform(secondaryObj.transform, !primaryInHand ? handTransform : backTransform, secondaryItem);
        secondaryItem.Equip();
    }

    public void UnequipPrimary()
    {
        primarySlot.item = null;
        Destroy(primaryObj);
        primaryObj = null;
        primaryItem = null;
    }

    public void UnequipSecondary()
    {
        Destroy(secondaryObj);
        secondarySlot.item = null;
        secondaryObj = null;
        secondaryItem = null;
    }

    public void UnequipCurrent()
    {
        if (weaponInHand == primaryWeapon) UnequipPrimary();
        else if (weaponInHand == secondaryWeapon) UnequipSecondary();
    }
}
