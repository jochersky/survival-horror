using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    // references
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private GameObject cursor;
    private RawImage _crosshairImage;
    [SerializeField] private Texture crosshairTexture;
    [SerializeField] private Texture doorLockedTexture;
    [SerializeField] private Texture doorUnlockedTexture;
    [SerializeField] private Texture openHandTexture;
    private InputActionMap _playerActions;    
    
    // input actions
    private InputAction m_InteractAction;
    private InputAction m_ZoomAction;
    
    private bool _isInteracting;
    private bool _isZooming;
    
    void Awake()
    {
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_InteractAction = actions.FindAction("Interact");
        m_InteractAction.started += OnInteract;
        m_InteractAction.canceled += OnInteract;
        m_ZoomAction = actions.FindAction("Zoom");
        m_ZoomAction.started += OnZoom;
        m_ZoomAction.canceled += OnZoom;
    }

    void Start()
    {
        _crosshairImage = cursor.GetComponent<RawImage>();
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
    
    void Update()
    {
        if (!CanInteract()) return;
        
        InteractWithObject();
    }

    private bool CanInteract()
    {
        if (_isZooming) _crosshairImage.texture = crosshairTexture;
        return !_isZooming;
    }

    private void InteractWithObject()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5f, interactionLayer, QueryTriggerInteraction.UseGlobal);
        
        cursor.SetActive(hit.transform);
        if (!hit.transform) return;
        
        // change cursor to interactable icon
        if (hit.collider.TryGetComponent<Door>(out Door door))
        {
            _crosshairImage.texture = door.locked ? doorLockedTexture: doorUnlockedTexture;
        } 
        else if (hit.collider.TryGetComponent<Container>(out Container container))
        {
            _crosshairImage.texture = openHandTexture;
        }
        else if (hit.collider.TryGetComponent<Item>(out Item item))
        {
            _crosshairImage.texture = openHandTexture;
        }

        if (_isInteracting && hit.collider && hit.collider.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
            _isInteracting = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        _isInteracting = context.ReadValueAsButton();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        _isZooming = context.ReadValueAsButton();
    }
}
