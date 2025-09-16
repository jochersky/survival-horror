using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("References")]
    [SerializeField] private GameObject orientation;
    private CharacterController _characterController;
    private InputSystem_Actions _actions;
    private InputSystem_Actions.PlayerActions _playerActions;

    [Header("Movement Properties")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float moveAccel = 0.5f;
    [SerializeField] private float stopDrag = 0.6f;
    [SerializeField] private float gravity = -9.8f;

    // Player input and movement values
    private Vector2 _moveInput;
    private bool _movePressed;
    private Vector3 _moveVelocity;
    private Vector3 _verticalVelocity;
    private float _currentHorizontalSpeed;

    // State Variables
    private PlayerBaseState _currentState;
    private PlayerStateDictionary _states;

    // Getters and Setters
    public CharacterController CharacterController { get { return _characterController; } } 
    
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }

    public float MoveAccel => moveAccel;
    public float MaxMoveSpeed => maxMoveSpeed;
    public float StopDrag => stopDrag;
    public float Gravity => gravity;

    public bool MovePressed { get { return _movePressed; } set { _movePressed = value; } }

    public Vector2 MoveInput { get { return _moveInput; } }

    public float MoveVelocityX { get => _moveVelocity.x; set => _moveVelocity.x = value; }
    public float MoveVelocityY { get => _moveVelocity.y; set => _moveVelocity.y = value; }
    public float MoveVelocityZ { get => _moveVelocity.z; set => _moveVelocity.z = value; }
    public Vector3 MoveVelocity { get { return _moveVelocity; } set { _moveVelocity = value; } }
    public float VerticalVelocityY { get => _verticalVelocity.y; set => _verticalVelocity.y = value; }
    public Vector3 VerticalVelocity { get { return _verticalVelocity; } set { _verticalVelocity = value; } }

    public float CurrentHorizontalSpeed { get { return _currentHorizontalSpeed; } set { _currentHorizontalSpeed = value; } }
    public Vector3 ForwardDir => orientation.transform.forward;
    public Vector3 RightDir => orientation.transform.right;

    private void Awake()
    {
        // Initialize references
        _characterController = GetComponent<CharacterController>();

        _actions = new InputSystem_Actions(); // Asset object
        _playerActions = _actions.Player;     // Extract action map object
        
        // Subscribe the player input callbacks
        _playerActions.AddCallbacks(this);
        
        // State machine + initial state setup
        _states = new PlayerStateDictionary(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
        _characterController.Move((MoveVelocity + _verticalVelocity) * Time.deltaTime);
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

    // callback handler function to set the player input values
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        MovePressed = _moveInput != Vector2.zero;
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
}
