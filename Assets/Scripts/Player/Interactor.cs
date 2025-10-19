using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    // references
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private LayerMask interactionLayer;
    private InputActionMap _playerActions;
    
    // input actions
    private InputAction m_InteractAction;
    
    private bool _isInteracting;
    
    void Awake()
    {
        _playerActions = actions.FindActionMap("Player");
        
        // assign input action callbacks
        m_InteractAction = actions.FindAction("Interact");
        m_InteractAction.started += OnInteract;
        m_InteractAction.canceled += OnInteract;
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
        if (_isInteracting)
        {
            Debug.DrawRay(transform.position, transform.forward * 5f, Color.green);
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5f, interactionLayer);
            if (hit.transform && _isInteracting && hit.transform.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
                _isInteracting = false;
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        _isInteracting = context.ReadValueAsButton();
    }
}
