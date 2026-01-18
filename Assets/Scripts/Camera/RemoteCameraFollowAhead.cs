using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class RemoteCameraFollowAhead : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform moveOrientation;

    [Header("Settings")] 
    [SerializeField] private Vector2[] moveInputsToIgnore;
    [SerializeField] private MoveInputFollowAhead[] customTranslations;
    [SerializeField, Range(0.01f, 50)] private float maxLookAheadDistance = 1;
    [SerializeField, Range(0.001f, 5)] private float distanceSpeed = 0.5f;
    [SerializeField, Range(0.001f, 5)] private float lookAheadChangeSpeed = 0.25f;
    
    private InputActionMap _playerActions;
    
    // input actions
    private InputAction m_MoveAction;

    private Vector2 _moveInput;
    private float _currentDistance = 0;
    
    private void Start()
    {
        _playerActions = actions.FindActionMap("Player");
        m_MoveAction = _playerActions.FindAction("Move");
        m_MoveAction.started += OnMove;
        m_MoveAction.performed += OnMove;
        m_MoveAction.canceled += OnMove;
    }
    
    private void LateUpdate()
    {
        MakeCameraTargetFollowAhead();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    
    private void MakeCameraTargetFollowAhead()
    {
        // Ignore moving cameraTarget towards inputs included in moveInputsToIgnore
        foreach (Vector2 dir in moveInputsToIgnore) if (_moveInput == dir) return;
        
        // Shift the cameraTarget in direction of player's movement over time up to the max distance
        _currentDistance += Time.deltaTime * distanceSpeed;
        _currentDistance = Mathf.Clamp(_currentDistance, 0, maxLookAheadDistance);
        
        // Translate any specified inputs into their custom follow ahead dir
        Vector2 followAheadDir = _moveInput;
        foreach (MoveInputFollowAhead c in customTranslations)
            if (_moveInput == c.moveInput)
                followAheadDir = c.followAheadDir;
        
        followAheadDir *= _currentDistance;
        
        // Align the new local position vector to the move orientation
        Vector3 v = moveOrientation.forward * followAheadDir.y + moveOrientation.right * followAheadDir.x;
        cameraTarget.localPosition = Vector3.MoveTowards(cameraTarget.localPosition, v, lookAheadChangeSpeed * Time.deltaTime);
    }
}

// Used for translating move input -> follow ahead direction for specific camera behavior
[Serializable]
struct MoveInputFollowAhead
{
    public Vector2 moveInput;
    public Vector2 followAheadDir;

    public MoveInputFollowAhead(Vector2 moveInput, Vector2 followAheadDir)
    {
        this.moveInput = moveInput;
        this.followAheadDir = followAheadDir;
    }
}
