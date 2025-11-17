using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // [Header("References")]
    // [SerializeField] private InputActionAsset actions;
    // public GameObject primary;
    // public GameObject secondary;
    // private InputActionMap _playerActions;
    //
    // private WeaponType _primaryType;
    // private WeaponType _secondaryType;
    //
    // // input actions
    // private InputAction m_SwitchWeapon;
    //
    // public delegate void MeleeWeaponEquipped();
    // public event MeleeWeaponEquipped OnMeleeWeaponEquipped;
    // public delegate void GunWeaponEquipped();
    // public event GunWeaponEquipped OnGunWeaponEquipped;
    //
    // void Awake()
    // {
    //     // initialize references
    //     _playerActions = actions.FindActionMap("Player");
    //     
    //     // assign input action callbacks
    //     m_SwitchWeapon = _playerActions.FindAction("SwitchWeapon");
    //     m_SwitchWeapon.started += SwitchWeapon;
    // }
    //
    // void Start()
    // {
    //     secondary.SetActive(false);
    // }
    //
    // private void SwitchWeapon(InputAction.CallbackContext context)
    // {
    //     bool primaryActive = primary.activeSelf; 
    //     
    //     // enable weapon that was switched to
    //     primary.SetActive(!primaryActive);
    //     secondary.SetActive(primaryActive);
    //     
    //     // send event to other scripts with what kind of weapon was switched to
    //     if (primaryActive)
    //     {
    //         if (_primaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
    //         else if (_primaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    //     }
    //     else
    //     {
    //         if (_secondaryType == WeaponType.Gun) OnGunWeaponEquipped?.Invoke();
    //         else if (_secondaryType == WeaponType.Melee) OnMeleeWeaponEquipped?.Invoke();
    //     }
    //     
    //     
    // }
    //
    // public void EquipWeapon(DraggableItem item)
    // {
    //     WeaponData wd = (WeaponData)item.itemData;
    //     
    // }
}
