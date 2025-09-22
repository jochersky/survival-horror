using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [SerializeField] private LayerMask interactionLayer;
    
    private InputSystem_Actions _actions;
    private InputSystem_Actions.PlayerActions _playerActions;
    
    private bool _isInteracting;
    
    void Awake()
    {
        _actions = new InputSystem_Actions(); // Asset object
        _playerActions = _actions.Player;     // Extract action map object
        
        // Subscribe the player input callbacks
        _playerActions.AddCallbacks(this);
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
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.green);
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5f, interactionLayer);
        if (hit.transform && _isInteracting && hit.transform.TryGetComponent(out IInteractable interactable))
            interactable.Interact();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        _isInteracting = context.ReadValueAsButton();
    }
}
