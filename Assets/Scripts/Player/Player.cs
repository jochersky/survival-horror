using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject melee;
    private InputActionMap _playerActions;

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
        melee.SetActive(false);
    }

    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (gun.activeSelf)
        {
            gun.SetActive(false);
            melee.SetActive(true);
            OnMeleeWeaponEquipped?.Invoke();
        }
        else
        {
            gun.SetActive(true);
            melee.SetActive(false);
            OnGunWeaponEquipped?.Invoke();
        }
    }
}
