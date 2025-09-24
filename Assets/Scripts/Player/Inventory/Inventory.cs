using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : Container
{
    // references
    [SerializeField] private InputActionAsset actions;
    private GameObject _interactedWithContainer;
    private InputActionMap _playerActions;
    
    // input action
    private InputAction m_InventoryAction;
    
    private bool _inventoryVisible;
    
    void Awake()
    {
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_InventoryAction = actions.FindAction("Inventory");
        m_InventoryAction.started += OnInventory;
    }
    private void OnEnable()
    {
        // enable the character controls action map
        _playerActions.Enable();
    }
    
    private void OnDisable()
    {
        // disable the character controls action map
        _playerActions.Disable();
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        _inventoryVisible = !_inventoryVisible;
        ContainerUI.SetActive(_inventoryVisible);
    }
}
