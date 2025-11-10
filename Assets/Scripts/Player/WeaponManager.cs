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

    private DraggableItem _primaryDragItem;
    private DraggableItem _secondaryDragItem;
    private Item _primaryItem;
    private Item _secondaryItem;
    private WeaponType _primaryType;
    private WeaponType _secondaryType;
    private Weapon _weaponInHand;
    private Weapon _primaryWeapon;
    private Weapon _secondaryWeapon;
    
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
    }
    
    public void OnZoom(InputAction.CallbackContext context)
    {
        _isZooming = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isPressingFire = context.ReadValueAsButton();

        if (!_weaponInHand) return;

        if (_isZooming) _weaponInHand.AimAttack();
        else _weaponInHand.SwingAttack();
    }

    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (_weaponInHand == _primaryWeapon && _secondaryWeapon) SwitchToSecondary();
        else if (_weaponInHand == _secondaryWeapon && _primaryWeapon) SwitchToPrimary();
        else if (!_weaponInHand)
        {
            if (_primaryWeapon) SwitchToPrimary();
            else SwitchToSecondary();
        }
    }

    private void SwitchToPrimary()
    {
        _weaponInHand = _primaryWeapon;
            
        if (primaryObj) SetTransform(primaryObj.transform, handTransform, _primaryItem);
        if (secondaryObj) SetTransform(secondaryObj.transform, backTransform, _secondaryItem);
            
        if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
        else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    }

    private void SwitchToSecondary()
    {
        _weaponInHand = _secondaryWeapon;
            
        if (secondaryObj) SetTransform(secondaryObj.transform, handTransform, _secondaryItem);
        if (primaryObj) SetTransform(primaryObj.transform, backTransform, _primaryItem);
            
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
        
        _primaryDragItem = dragItem;
        WeaponData wd = (WeaponData)dragItem.itemData;
        _primaryType = wd.weaponType;
        GameObject prefab = dragItem.itemPrefab;
        primaryObj = Instantiate(prefab);
        Weapon w = primaryObj.GetComponent<Weapon>();
        bool primaryInHand = _weaponInHand == _primaryWeapon;
        if (primaryInHand)
        {
            _weaponInHand = w;
            if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
        }
        _primaryWeapon = w;
        _primaryItem = primaryObj.GetComponentInChildren<Item>();
        
        SetTransform(primaryObj.transform, primaryInHand ? handTransform : backTransform, _primaryItem);
        _primaryItem.Equip();
    }

    private void EquipSecondary(DraggableItem dragItem)
    {
        if (secondaryObj) Destroy(secondaryObj);
        
        _secondaryDragItem = dragItem;
        WeaponData wd = (WeaponData)dragItem.itemData;
        _secondaryType = wd.weaponType;
        GameObject prefab = dragItem.itemPrefab;
        secondaryObj = Instantiate(prefab);
        Weapon w = secondaryObj.GetComponent<Weapon>();
        bool primaryInHand = _weaponInHand == _primaryWeapon;
        if (!primaryInHand)
        {
            _weaponInHand = w;
            if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
            else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
        }
        _secondaryWeapon = w;
        _secondaryItem = secondaryObj.GetComponentInChildren<Item>();

        SetTransform(secondaryObj.transform, !primaryInHand ? handTransform : backTransform, _secondaryItem);
        _secondaryItem.Equip();
    }

    // connected to primarySlot event OnWeaponUnequipped
    private void UnequipPrimary()
    {
        primarySlot.item = null;
        Destroy(primaryObj);
        primaryObj = null;
        _primaryItem = null;
        _primaryDragItem = null;
        _primaryWeapon = null;
    }

    // connected to secondarySlot event OnWeaponUnequipped
    private void UnequipSecondary()
    {
        secondarySlot.item = null;
        Destroy(secondaryObj);
        secondaryObj = null;
        _secondaryItem = null;
        _secondaryDragItem = null;
        _secondaryWeapon = null;
    }

    public void UnequipCurrent()
    {
        if (_weaponInHand == _primaryWeapon) UnequipPrimary();
        else if (_weaponInHand == _secondaryWeapon) UnequipSecondary();
    }

    public void DestroyCurrentDragItem()
    {
        if (_weaponInHand == _primaryWeapon) Destroy(_primaryDragItem.gameObject);
        else if (_weaponInHand == _secondaryWeapon) Destroy(_secondaryDragItem.gameObject);
    }

    public void UnequipThrownWeapon()
    {
        // TODO: replace Destroy(dragItem.gameObject); with temporary equipped item
        // for when the item is picked up and can be picked up again
        if (_weaponInHand == _primaryWeapon)
        {
            primarySlot.item = null;
            primaryObj = null;
            _primaryItem = null;
            Destroy(_primaryDragItem.gameObject);
            _primaryDragItem = null;
            _primaryWeapon = null;
        }
        else if (_weaponInHand == _secondaryWeapon)
        {
            secondarySlot.item = null;
            secondaryObj = null;
            _secondaryItem = null;
            Destroy(_secondaryDragItem.gameObject);
            _secondaryDragItem = null;
            _secondaryWeapon = null;
        }

        _weaponInHand = null;
    }
}
