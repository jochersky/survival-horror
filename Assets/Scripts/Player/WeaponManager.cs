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
    [SerializeField] private PlayerAnimationEvents playerAnimationEvents;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform backTransform;
    [SerializeField] private WeaponSlot primarySlot;
    [SerializeField] private WeaponSlot secondarySlot;
    private GameObject _primaryObj;
    private GameObject _secondaryObj;
    [SerializeField] private TextMeshProUGUI ammoCounterText;

    private DraggableItem _primaryDragItem;
    private DraggableItem _secondaryDragItem;
    private Item _primaryItem;
    private Item _secondaryItem;
    private WeaponType _primaryType;
    private WeaponType _secondaryType;
    [HideInInspector] public Weapon weaponInHand;
    private Weapon _primaryWeapon;
    private Weapon _secondaryWeapon;
    private List<DraggableItem> _primaryWeaponAmmoItems;
    private List<DraggableItem> _secondaryWeaponAmmoItems;
    private string _primaryAmmoName;
    private string _secondaryAmmoName;
    private int _primaryAmmoCount;
    private int _secondaryAmmoCount;
    [HideInInspector] public string lastThrownWeaponName = "";
    private bool _isAiming;
    private bool _isPressingFire;
    
    // singleton instance
    public static WeaponManager Instance { get; private set; }
    
    // input actions
    private InputAction _mSwitchWeapon;
    private InputAction _mAimAction;
    private InputAction _mAttackAction;
    
    public delegate void MeleeWeaponEquipped(Melee melee);
    public event MeleeWeaponEquipped OnMeleeWeaponEquipped;
    public delegate void GunWeaponEquipped(Gun gun);
    public event GunWeaponEquipped OnGunWeaponEquipped;
    public delegate void WeaponInHandThrown();
    public event WeaponInHandThrown OnWeaponInHandThrown;
    
    // TODO: create event to fire when a new weapon is in use and check whether it is a melee or gun type
    
    void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (Instance && Instance != this) Destroy(this);
        else Instance = this;
        
        // initialize references
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        _mSwitchWeapon = _playerActions.FindAction("SwitchWeapon");
        _mSwitchWeapon.started += SwitchWeapon;
        _mAimAction = _playerActions.FindAction("Aim");
        _mAimAction.started += OnAim;
        _mAimAction.canceled += OnAim;
        _mAttackAction = _playerActions.FindAction("Attack");
        _mAttackAction.started += OnAttack;
        
        // subscribe to events
        primarySlot.OnWeaponEquipped += EquipWeapon;
        primarySlot.OnWeaponUnequipped += UnequipWeapon;
        secondarySlot.OnWeaponEquipped += EquipWeapon;
        secondarySlot.OnWeaponUnequipped += UnequipWeapon;
    }

    void Start()
    {
        // TODO: get references to weapons already in the slots before calling respective Equip funcs
        
        ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
        cm.OnStackableItemCountsUpdated += UpdateAmmoCounts;

        playerAnimationEvents.OnSwingFinished += () =>
        {
            if (!weaponInHand) return;
            weaponInHand.SwingDamage.Deactivate();
        };
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        _isPressingFire = context.ReadValueAsButton();
        if (!weaponInHand) return;

        if (!_isAiming) weaponInHand.SwingAttack();
        else if (weaponInHand is Gun gun) gun.AimAttack();
        
        // melee weapons have AimAttack() called when animation event triggers
    }

    private void ThrowWeaponAttack()
    {
        if (_isAiming && weaponInHand is Melee melee)
            melee.AimAttack();
    }

    private void SwitchWeapon(InputAction.CallbackContext context)
    {   
        if (weaponInHand == _primaryWeapon && _secondaryWeapon) 
            SwitchWeapon(_secondaryWeapon);
        else if (weaponInHand == _secondaryWeapon && _primaryWeapon) 
            SwitchWeapon(_primaryWeapon);
        else if (!weaponInHand)
        {
            if (_primaryWeapon) 
                SwitchWeapon(_primaryWeapon);
            else if (_secondaryWeapon) 
                SwitchWeapon(_secondaryWeapon);
        }
    }

    private void SwitchWeapon(Weapon w)
    {
        weaponInHand = w;
        
        if (w == _primaryWeapon)
        {
            if (_primaryObj) SetTransform(_primaryObj.transform, handTransform, _primaryItem);
            if (_secondaryObj) SetTransform(_secondaryObj.transform, backTransform, _secondaryItem);
            if (_secondaryWeapon is Gun g)
            {
                playerAnimationEvents.OnFireBegin -= g.FireGun;
                playerAnimationEvents.OnReloadFinished -= g.ReloadGun;
            }
            if (_secondaryItem)
            {
                playerAnimationEvents.OnSwingBegin -= _secondaryItem.MoveToAttack;
                playerAnimationEvents.OnSwingFinished -= _secondaryItem.MoveToHand;
            }
            playerAnimationEvents.OnSwingBegin += _primaryItem.MoveToAttack;
            playerAnimationEvents.OnSwingFinished += _primaryItem.MoveToHand;
        }
        else if (w == _secondaryWeapon)
        {
            if (_secondaryObj) SetTransform(_secondaryObj.transform, handTransform, _secondaryItem);
            if (_primaryObj) SetTransform(_primaryObj.transform, backTransform, _primaryItem);
            if (_primaryWeapon is Gun g)
            {
                playerAnimationEvents.OnFireBegin -= g.FireGun;
                playerAnimationEvents.OnReloadFinished -= g.ReloadGun;
            }
            if (_primaryItem)
            {
                playerAnimationEvents.OnSwingBegin -= _primaryItem.MoveToAttack;
                playerAnimationEvents.OnSwingFinished -= _primaryItem.MoveToHand;
            }
            playerAnimationEvents.OnSwingBegin += _secondaryItem.MoveToAttack;
            playerAnimationEvents.OnSwingFinished += _secondaryItem.MoveToHand;
        }
        
        if (w is Gun gun)
        {
            playerAnimationEvents.OnFireBegin += gun.FireGun;
            playerAnimationEvents.OnReloadFinished += gun.ReloadGun;
            int ammoCount = w == _primaryWeapon ? _primaryAmmoCount : _secondaryAmmoCount;
            ammoCounterText.text = gun.BulletsRemaining + " // " + ammoCount;
            OnGunWeaponEquipped?.Invoke(gun);
            ammoCounterText.gameObject.SetActive(true);
        }
        else if (w is Melee melee)
        {
            OnMeleeWeaponEquipped?.Invoke(melee);
            ammoCounterText.gameObject.SetActive(false);
        }
        
        AudioManager.Instance.PlaySFX(SfxType.SwitchWeapons);
    }

    private void SetTransform(Transform t, Transform weaponTransform, Item item)
    {
        t.SetParent(weaponTransform);
        t.localPosition = Vector3.zero;
        if (weaponTransform == handTransform) item.MoveToHand();
        else item.MoveToBack();
    }

    // connected to WeaponSlot event OnWeaponEquipped
    private void EquipWeapon(DraggableItem dragItem, WeaponSlot weaponSlot)
    {
        DraggableItem di;
        GameObject go;
        Weapon w;
        Item i;
        
        bool primary = weaponSlot == primarySlot;
        if (primary)
        {
            _primaryObj = go = Instantiate(dragItem.itemPrefab);
            _primaryDragItem = di = dragItem;
            _primaryItem = i = _primaryObj.GetComponentInChildren<Item>();
            _primaryWeapon = w = _primaryObj.GetComponent<Weapon>();
            if (weaponInHand == _primaryWeapon || !weaponInHand) weaponInHand = w;
        }
        else
        {
            _secondaryObj = go = Instantiate(dragItem.itemPrefab);
            _secondaryDragItem = di = dragItem;
            _secondaryItem = i = _secondaryObj.GetComponentInChildren<Item>();
            _secondaryWeapon = w = _secondaryObj.GetComponent<Weapon>();
            if (weaponInHand == _secondaryWeapon || !weaponInHand) weaponInHand = w;
        }

        if (w is Gun gun)
        {
            gun.BulletsRemaining = di.AmmoCount;
            
            // get all the ammo for the gun in the inventory
            ContainerManager cm = InventoryManager.instance.playerInventoryContainerManager;
            if (w == _primaryWeapon)
            {
                _primaryWeaponAmmoItems = cm.GetAllDraggableItems("SmallgunAmmo"); // TODO: change to an ammo tag check
                _primaryAmmoName = "SmallgunAmmo"; // TODO: change to an ammo tag check
                _primaryAmmoCount = CountAmmo(_primaryWeaponAmmoItems);
                
            }
            else if (w == _secondaryWeapon)
            {
                _secondaryWeaponAmmoItems = cm.GetAllDraggableItems("SmallgunAmmo"); // TODO: change to an ammo tag check
                _secondaryAmmoName = "SmallgunAmmo"; // TODO: change to an ammo tag check
                _secondaryAmmoCount = CountAmmo(_secondaryWeaponAmmoItems);
            }
            
            if (weaponInHand == w)
            {
                OnGunWeaponEquipped?.Invoke(gun);
                playerAnimationEvents.OnFireBegin += gun.FireGun;
                playerAnimationEvents.OnReloadFinished += gun.ReloadGun;
                ammoCounterText.text = gun.BulletsRemaining +  " // " + (w == _primaryWeapon ? _primaryAmmoCount : _secondaryAmmoCount);
                ammoCounterText.gameObject.SetActive(true);
            }
            
            gun.OnReloadComplete += UpdateBulletsRemaining;
            gun.OnFireComplete += UpdateBulletsRemaining;
            di.OnAmmoCountChanged += UpdateMagazineBulletCount;
        }
        else if (w is Melee melee)
        {
            if (weaponInHand == w)
            {
                OnMeleeWeaponEquipped?.Invoke(melee);
                ammoCounterText.gameObject.SetActive(false);
            }
            playerAnimationEvents.OnWeaponThrown += ThrowWeaponAttack;
        }

        if (weaponInHand == w)
        {
            // connect to events which adjust transform of weapon when using swing attack
            playerAnimationEvents.OnSwingBegin += i.MoveToAttack;
            playerAnimationEvents.OnSwingFinished += i.MoveToHand;
        }
        
        // adjust transform depending on what weapon is currently in hand
        SetTransform(go.transform, weaponInHand == w ? handTransform : backTransform, i);
        i.Equip();
    }

    // connected to WeaponSlot event OnWeaponUnequipped
    private void UnequipWeapon(WeaponSlot weaponSlot)
    {
        bool primary = weaponSlot == primarySlot;
        DraggableItem di = primary ? _primaryDragItem : _secondaryDragItem;
        Weapon w = primary ? _primaryWeapon : _secondaryWeapon;
        Item i = primary ? _primaryItem : _secondaryItem;

        if (w is Gun gun)
        {
            di.AmmoCount = gun.BulletsRemaining;
            if (weaponInHand == w) ammoCounterText.gameObject.SetActive(false);
            di.OnAmmoCountChanged -= UpdateMagazineBulletCount;
            gun.OnFireComplete -= UpdateBulletsRemaining;
        }
        else if (w is Melee melee)
        {
            playerAnimationEvents.OnWeaponThrown -= ThrowWeaponAttack;
        }
        playerAnimationEvents.OnSwingBegin -= i.MoveToAttack;
        playerAnimationEvents.OnSwingFinished -= i.MoveToHand;

        weaponSlot.item = null;
        if (primary)
        {
            Destroy(_primaryObj);
            _primaryObj = null;
            _primaryItem = null;
            _primaryDragItem = null;
            _primaryWeapon = null;
        }
        else
        {
            Destroy(_secondaryObj);
            _secondaryObj = null;
            _secondaryItem = null;
            _secondaryDragItem = null;
            _secondaryWeapon = null;
        }
    }
    
    public void UnequipThrownWeapon()
    {
        bool primary = weaponInHand == _primaryWeapon;
        Weapon w = primary ? _primaryWeapon : _secondaryWeapon;
        Item i = primary ? _primaryItem : _secondaryItem;
        
        if (w is Melee melee)
        {
            playerAnimationEvents.OnWeaponThrown -= ThrowWeaponAttack;
        }
        playerAnimationEvents.OnSwingBegin -= i.MoveToAttack;
        playerAnimationEvents.OnSwingFinished -= i.MoveToHand;
        
        if (weaponInHand == _primaryWeapon)
        {
            lastThrownWeaponName = _primaryDragItem.itemData.itemName;
            primarySlot.item = null;
            _primaryObj = null;
            _primaryItem = null;
            Destroy(_primaryDragItem.gameObject);
            _primaryDragItem = null;
            _primaryWeapon = null;
        }
        else if (weaponInHand == _secondaryWeapon)
        {
            lastThrownWeaponName = _secondaryDragItem.itemData.itemName;
            secondarySlot.item = null;
            _secondaryObj = null;
            _secondaryItem = null;
            Destroy(_secondaryDragItem.gameObject);
            _secondaryDragItem = null;
            _secondaryWeapon = null;
        }

        weaponInHand = null;
        OnWeaponInHandThrown?.Invoke();
    }

    public bool EquipThrownWeaponOnPickup(GameObject dragItemPrefab)
    {
        if (!_primaryObj || !_secondaryObj)
        {
            WeaponSlot ws = !_primaryObj ? primarySlot : secondarySlot;
            DraggableItem instDragItem = InstanceDragItemPrefabIntoSlot(dragItemPrefab, ws);
            EquipWeapon(instDragItem, ws);
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
        bool primary = gun == _primaryWeapon;
        if (primary && _primaryAmmoCount == 0) return 0;
        if (!primary && _secondaryAmmoCount == 0) return 0;
        
        int numAmmo = amt;
        int ammoCount = (primary ? _primaryAmmoCount : _secondaryAmmoCount);
        if (ammoCount < amt)
        {
            numAmmo = ammoCount;
            if (primary) _primaryAmmoCount = 0;
            else _secondaryAmmoCount = 0;
        }
        else if (ammoCount >= amt)
        {
            if (primary) _primaryAmmoCount -= amt;
            else _secondaryAmmoCount -= amt;
        }

        int amtToDecrement = numAmmo;
        foreach (DraggableItem di in (primary ? _primaryWeaponAmmoItems : _secondaryWeaponAmmoItems))
        {
            if (di.Count <= amtToDecrement)
            {
                amtToDecrement -= di.Count;
                di.Count = 0;
            }
            else
            {
                di.Count -= amtToDecrement;
                amtToDecrement = 0;
            }
            if (amtToDecrement == 0) break;
        }
        
        // cleanup anu null ammo items after taking out ammo 
        (primary ? _primaryWeaponAmmoItems : _secondaryWeaponAmmoItems).RemoveAll((DraggableItem di) => di.Count == 0);

        if (numAmmo > 0)
        {
            if (primary) _primaryDragItem.AmmoCount = numAmmo;
            else _secondaryDragItem.AmmoCount = numAmmo;
        }

        return numAmmo;
    }

    public int AmmoCount()
    {
        if (weaponInHand) 
            return weaponInHand == _primaryWeapon ? _primaryAmmoCount : _secondaryAmmoCount;
        return -1;
    }

    private void UpdateBulletsRemaining(Gun gun)
    {
        if (weaponInHand == _primaryWeapon) _primaryDragItem.AmmoCount = gun.BulletsRemaining;
        else _secondaryDragItem.AmmoCount = gun.BulletsRemaining;
        ammoCounterText.text = gun.BulletsRemaining + " // " + (_primaryWeapon ? _primaryAmmoCount : _secondaryAmmoCount);
    }

    private void UpdateMagazineBulletCount(int ammoCount, DraggableItem dragItem)
    {
        DraggableItem di = dragItem == _primaryDragItem ?  _primaryDragItem : _secondaryDragItem;
        Weapon w = dragItem == _primaryDragItem ? _primaryWeapon : _secondaryWeapon;
        if (w is Gun gun)
        {
            gun.BulletsRemaining = ammoCount;
            UpdateBulletsRemaining(gun);
        }
    }
}
