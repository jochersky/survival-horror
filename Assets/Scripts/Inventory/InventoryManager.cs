using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Singleton for managing the player's inventory
 */
public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    private InputActionMap _playerActions;
    public Canvas canvas;
    public GameObject inventoryUI;
    [SerializeField] private GameObject inventoryGrids;
    public GameObject playerInventoryContainerUI;
    public ContainerManager playerInventoryContainerManager;
    [SerializeField] private Transform itemSpawnTransform;
    [SerializeField] private Transform itemParentTransform;
    [SerializeField] private Health playerHealth;
    [SerializeField] private TextMeshProUGUI itemTooltipText; 

    // container that is being looked at
    public Container container;
    
    // input action
    private InputAction m_InventoryAction;
    
    // singleton instance
    public static InventoryManager instance { get; private set; }
    public Health PlayerHealth => playerHealth;
    
    private bool _inventoryVisible;
    
    public delegate void InventoryVisibilityChanged(bool visible);
    public event InventoryVisibilityChanged OnInventoryVisibilityChanged;

    private void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (instance && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        
        _playerActions = actions.FindActionMap("PlayerUI");
        
        // assign input action callbacks
        m_InventoryAction = actions.FindAction("Inventory");
        m_InventoryAction.started += OnInventory;
    }

    private void Start()
    {
        playerInventoryContainerManager.OnStartFinished += AddInventoryContainer;
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

    private void AddInventoryContainer()
    {
        playerInventoryContainerUI.transform.SetParent(inventoryGrids.transform);
        playerInventoryContainerUI.transform.localScale = Vector3.one;
    }

    public void AddContainer(Container newContainer)
    {
        container = newContainer;
        container.ContainerUI.transform.SetParent(inventoryGrids.transform);
        ToggleInventory();
        container.ContainerUI.transform.localScale = Vector3.one;
        container.ContainerUI.transform.eulerAngles = Vector3.zero;
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
        OnInventoryVisibilityChanged?.Invoke(_inventoryVisible);
        if (!_inventoryVisible) RemoveContainer();
    }

    public void SpawnItem(GameObject itemPrefab, int count, int ammoCount)
    {
        GameObject newItem = Instantiate(itemPrefab, itemSpawnTransform);
        newItem.transform.SetParent(itemParentTransform);
        Item item = newItem.GetComponentInChildren<Item>();
        if (item)
        {
            item.Count = count;
            item.AmmoCount = ammoCount;
        }
    }

    public void UpdateTooltip(string tooltip)
    {
        itemTooltipText.text = tooltip;
    }

    public DraggableItem PlayerInventoryHasItem(string itemName)
    {
        return playerInventoryContainerManager.FindDraggableItem(itemName);
    }

    public bool FindKeyInPlayerInventory(string keyName)
    {
        DraggableItem key = playerInventoryContainerManager.FindDraggableItem(keyName);
        if (key)
        {
            key.RemoveItemFromGrid();
            Destroy(key.gameObject);
            return true;
        }
        return false;
    }
}
