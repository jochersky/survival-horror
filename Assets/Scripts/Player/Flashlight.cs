using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    private InputActionMap _playerActions;
    
    // input actions
    private InputAction m_FlashlightAction;
    
    private void Awake()
    {
        _playerActions = actions.FindActionMap("Player");
        // assign input action callbacks
        m_FlashlightAction = actions.FindAction("Flashlight");
        m_FlashlightAction.started += OnFlashlight;
        
        gameObject.SetActive(false);
    }

    private void OnFlashlight(InputAction.CallbackContext context)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        SfxType type = (gameObject.activeInHierarchy ? SfxType.FlashlightOff : SfxType.FlashlightOn);
        AudioManager.Instance.PlaySFX(type);
    }
}
