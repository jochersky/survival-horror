using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private TextMeshProUGUI ammoCounterText;

    private DraggableItem _primaryDragItem;
    private DraggableItem _secondaryDragItem;
    private Item _primaryItem;
    private Item _secondaryItem;
    private WeaponType _primaryType;
    private WeaponType _secondaryType;
    private Weapon _weaponInHand;
    private Weapon _primaryWeapon;
    private Weapon _secondaryWeapon;
    private List<DraggableItem> _primaryWeaponAmmoItems;
    private List<DraggableItem> _secondaryWeaponAmmoItems;
    private string _primaryAmmoName;
    private string _secondaryAmmoName;
    private int _primaryAmmoCount;
    private int _secondaryAmmoCount;
    public string lastThrownWeaponName = "";
    
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
        
        ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
        cm.OnStackableItemCountsUpdated += UpdateAmmoCounts;
    }
    
    public void OnZoom(InputAction.CallbackContext context)
    {
        _isZooming = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isPressingFire = context.ReadValueAsButton();

        if (!_weaponInHand) return;

        if (_isZooming)
        {
            if (_weaponInHand is Gun gun)
            {
                if (gun.BulletsRemaining > 0) _weaponInHand.AimAttack();
                int ammoCount = _weaponInHand == _primaryWeapon ?  _primaryAmmoCount : _secondaryAmmoCount;
                ammoCounterText.text = gun.BulletsRemaining + " // " + ammoCount;
            }
            else
            {
                _weaponInHand.AimAttack();
            }
        }
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

        if (_primaryType == WeaponType.Gun)
        {
            OnGunWeaponEquipped?.Invoke();
            ammoCounterText.gameObject.SetActive(true);
        }
        else if (_primaryType == WeaponType.Melee)
        { 
            OnMeleeWeaponEquipped?.Invoke();
            ammoCounterText.gameObject.SetActive(false);
        }
        
        if (_primaryWeapon is Gun gun)
            ammoCounterText.text = gun.BulletsRemaining + " // " + _primaryAmmoCount;
    }

    private void SwitchToSecondary()
    {
        _weaponInHand = _secondaryWeapon;
            
        if (secondaryObj) SetTransform(secondaryObj.transform, handTransform, _secondaryItem);
        if (primaryObj) SetTransform(primaryObj.transform, backTransform, _primaryItem);

        if (_secondaryType == WeaponType.Gun)
        {
            OnGunWeaponEquipped?.Invoke();
            ammoCounterText.gameObject.SetActive(true);
        }
        else if (_secondaryType == WeaponType.Melee)
        {
            OnMeleeWeaponEquipped?.Invoke();
            ammoCounterText.gameObject.SetActive(false);
        }
        
        if (_secondaryWeapon is Gun gun)
            ammoCounterText.text = gun.BulletsRemaining + " // " + _secondaryAmmoCount;
    }

    private void SetTransform(Transform t, Transform weaponTransform, Item item)
    {
        t.SetParent(weaponTransform);
        t.localPosition = Vector3.zero;
        if (weaponTransform == handTransform) item.MoveToHand();
        else item.MoveToBack();
    }

    // connected to primarySlot event OnWeaponEquipped
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
            if (_primaryType == WeaponType.Gun)
            {
                OnGunWeaponEquipped?.Invoke();
                ammoCounterText.gameObject.SetActive(true);
            }
            else if (_primaryType == WeaponType.Melee)
            {
                OnMeleeWeaponEquipped?.Invoke();
                ammoCounterText.gameObject.SetActive(false);
            }
        }
        _primaryWeapon = w;
        _primaryItem = primaryObj.GetComponentInChildren<Item>();
        
        if (_primaryWeapon is Gun gun)
        {
            // get all the ammo for the gun in the inventory
            ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
            _primaryWeaponAmmoItems = cm.GetAllDraggableItems("SmallgunAmmo"); // TODO: change to an ammo tag check
            _primaryAmmoName = "SmallgunAmmo";
            _primaryAmmoCount = CountAmmo(_primaryWeaponAmmoItems);
            ammoCounterText.text = gun.BulletsRemaining +  " // " + _primaryAmmoCount;
            gun.OnReloadComplete += UpdateBulletsRemaining;
            gun.OnFireComplete += UpdateBulletsRemaining;
        }
        
        SetTransform(primaryObj.transform, primaryInHand ? handTransform : backTransform, _primaryItem);
        _primaryItem.Equip();
    }

    // connected to secondarySlot event OnWeaponEquipped
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
            if (_secondaryType == WeaponType.Gun)
            {
                OnGunWeaponEquipped?.Invoke();
                ammoCounterText.gameObject.SetActive(true);
            }
            else if (_secondaryType == WeaponType.Melee)
            {
                OnMeleeWeaponEquipped?.Invoke();
                ammoCounterText.gameObject.SetActive(false);
            }
        }
        _secondaryWeapon = w;
        _secondaryItem = secondaryObj.GetComponentInChildren<Item>();
        
        if (_secondaryWeapon is Gun gun)
        {
            // get all the ammo for the gun in the inventory
            ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
            _secondaryWeaponAmmoItems = cm.GetAllDraggableItems("SmallgunAmmo"); // TODO: change to an ammo tag check
            _secondaryAmmoName = "SmallgunAmmo";
            _secondaryAmmoCount = CountAmmo(_secondaryWeaponAmmoItems);
            ammoCounterText.text = gun.BulletsRemaining +  " // " + _secondaryAmmoCount;
            gun.OnReloadComplete += UpdateBulletsRemaining;
            gun.OnFireComplete += UpdateBulletsRemaining;
        }

        SetTransform(secondaryObj.transform, !primaryInHand ? handTransform : backTransform, _secondaryItem);
        _secondaryItem.Equip();
    }

    // connected to primarySlot event OnWeaponUnequipped
    private void UnequipPrimary()
    {
        if (_primaryType == WeaponType.Gun) ammoCounterText.gameObject.SetActive(false);
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
        if (_secondaryType == WeaponType.Gun) ammoCounterText.gameObject.SetActive(false);
        secondarySlot.item = null;
        Destroy(secondaryObj);
        secondaryObj = null;
        _secondaryItem = null;
        _secondaryDragItem = null;
        _secondaryWeapon = null;
    }

    public void UnequipThrownWeapon()
    {
        if (_weaponInHand == _primaryWeapon)
        {
            lastThrownWeaponName = _primaryDragItem.itemData.itemName;
            primarySlot.item = null;
            primaryObj = null;
            _primaryItem = null;
            Destroy(_primaryDragItem.gameObject);
            _primaryDragItem = null;
            _primaryWeapon = null;
        }
        else if (_weaponInHand == _secondaryWeapon)
        {
            lastThrownWeaponName = _secondaryDragItem.itemData.itemName;
            secondarySlot.item = null;
            secondaryObj = null;
            _secondaryItem = null;
            Destroy(_secondaryDragItem.gameObject);
            _secondaryDragItem = null;
            _secondaryWeapon = null;
        }

        _weaponInHand = null;
    }

    public bool EquipThrownWeaponOnPickup(GameObject dragItemPrefab)
    {
        if (!primaryObj)
        {
            DraggableItem dragItemInst = InstanceDragItemPrefabIntoSlot(dragItemPrefab, primarySlot);
            EquipPrimary(dragItemInst);
            lastThrownWeaponName = "";
            return true;
        } 
        if (!secondaryObj)
        {
            DraggableItem dragItemInst = InstanceDragItemPrefabIntoSlot(dragItemPrefab, secondarySlot);
            EquipSecondary(dragItemInst);
            lastThrownWeaponName = "";
            return true;
        }

        return false;
    }

    private DraggableItem InstanceDragItemPrefabIntoSlot(GameObject dragItemPrefab, WeaponSlot slot)
    {
        GameObject inst = Instantiate(dragItemPrefab, slot.transform);
        DraggableItem di = inst.GetComponent<DraggableItem>();
        RectTransform rt = inst.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        return di;
    }

    private int CountAmmo(List<DraggableItem> dragItems)
    {
        if (dragItems.Count == 0) return 0;
        
        int totalAmmo = 0;
        foreach (DraggableItem dragItem in dragItems)
            totalAmmo += dragItem.Count;
        return totalAmmo;
    }

    private void UpdateAmmoCounts(string itemName)
    {
        if (itemName == _primaryAmmoName && _primaryWeapon is Gun priGun)
        {
            // get all the ammo for the gun in the inventory
            ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
            _primaryWeaponAmmoItems = cm.GetAllDraggableItems(itemName); // TODO: change to an ammo tag check
            _primaryAmmoCount = CountAmmo(_primaryWeaponAmmoItems);
            ammoCounterText.text = priGun.BulletsRemaining +  " // " + _primaryAmmoCount;
        }
        if (itemName == _secondaryAmmoName && _secondaryWeapon is Gun secGun)
        {
            // get all the ammo for the gun in the inventory
            ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
            _secondaryWeaponAmmoItems = cm.GetAllDraggableItems(itemName); // TODO: change to an ammo tag check
            _secondaryAmmoCount = CountAmmo(_secondaryWeaponAmmoItems);
            ammoCounterText.text = secGun.BulletsRemaining +  " // " + _secondaryAmmoCount;
        }
    }

    public int GetAmmoAmount(Gun gun, int amt)
    {
        if (gun == _primaryWeapon)
        {
            if (_primaryAmmoCount == 0) return 0;

            int numAmmo = amt;
            if (_primaryAmmoCount < amt)
            {
                numAmmo = _primaryAmmoCount;
                _primaryAmmoCount = 0;
            }
            else if (_primaryAmmoCount >= amt)
            {
                _primaryAmmoCount -= amt;
            }
            
            int amtToDecrement = numAmmo;
            foreach (DraggableItem di in _primaryWeaponAmmoItems)
            {
                if (di.Count <= amtToDecrement)
                {
                    amtToDecrement -= di.Count;
                }
                else
                {
                    di.Count -= amtToDecrement;
                }
                if (amtToDecrement == 0) break;
            }
            return numAmmo;
        }
        if (gun == _secondaryWeapon)
        {
            if (_secondaryAmmoCount == 0) return 0;

            int numAmmo = amt;
            if (_secondaryAmmoCount < amt)
            {
                numAmmo = _secondaryAmmoCount;
                _secondaryAmmoCount = 0;
            }
            else if (_secondaryAmmoCount >= amt)
            {
                _secondaryAmmoCount -= amt;
            }
            
            int amtToDecrement = numAmmo;
            foreach (DraggableItem di in _secondaryWeaponAmmoItems)
            {
                if (di.Count <= amtToDecrement)
                {
                    amtToDecrement -= di.Count;
                    Destroy(di.gameObject);
                }
                else
                {
                    di.Count -= amtToDecrement;
                }
                if (amtToDecrement == 0) break;
            }
            return numAmmo;
        }
        
        return 0;
    }

    private void UpdateBulletsRemaining(Gun gun)
    {
        ammoCounterText.text = gun.BulletsRemaining + " // ";
        if (gun == _primaryWeapon) ammoCounterText.text += _primaryAmmoCount;
        else if (gun == _secondaryWeapon) ammoCounterText.text += _secondaryAmmoCount;
    }
}
