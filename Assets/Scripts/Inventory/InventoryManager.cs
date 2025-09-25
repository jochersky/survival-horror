using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Singleton for managing the player's inventory
 */
public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;
    private InputActionMap _playerActions;
    
    // parent of inventory and container UIs in Canvas
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Container playerInventoryContainer;

    // container that is being looked at
    public Container container;
    
    // input action
    private InputAction m_InventoryAction;
    
    // singleton instance
    public static InventoryManager instance { get; private set; }
    
    private bool _inventoryVisible;
    
    // getters and setters
    public Container PlayerInventoryContainer => playerInventoryContainer;

    private void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (instance && instance != this) Destroy(this);
        else instance = this;
        
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

    public void AddContainer(Container newContainer)
    {
        container = newContainer;
        container.ContainerUI.transform.SetParent(inventoryUI.transform);
        ToggleInventory();
        container.transform.localScale = Vector3.one;
    }

    private void RemoveContainer()
    {
        if (container)
        {
            container.ContainerUI.transform.SetParent(container.transform);
        }
    }
    
    private void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        ToggleInventory();
    }

    private void ToggleInventory()
    {
        _inventoryVisible = !_inventoryVisible;
        inventoryUI.SetActive(_inventoryVisible);
        if (!_inventoryVisible) RemoveContainer();
    }
}
