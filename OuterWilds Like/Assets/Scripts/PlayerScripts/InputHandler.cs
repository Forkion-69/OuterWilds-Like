using UnityEngine;
using UnityEngine.InputSystem;


public class InputHandler : MonoBehaviour
{
    public static PlayerInput playerInput;

    public static Vector2 MoveVector;
    public static Vector2 LookVector;
    public static Vector2 LookMouseVector;
    public static float ThrustAxis;

    public static bool jumpWasPressed;
    public static bool jumpWasHeld;
    public static bool jumpWasReleased;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _thrustAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions["Move"];
        _lookAction = playerInput.actions["Look"];
        
        _jumpAction = playerInput.actions["Jump"];
        _thrustAction = playerInput.actions["Thrust"];
        
    }

    private void Update()
    {
        MoveVector = _moveAction.ReadValue<Vector2>();
        LookVector = _lookAction.ReadValue<Vector2>();
        LookMouseVector = Mouse.current.delta.ReadValue();
        
        ThrustAxis = _thrustAction.ReadValue<float>();

        jumpWasPressed = _jumpAction.WasPressedThisFrame();
        jumpWasHeld = _jumpAction.IsPressed();
        jumpWasReleased = _jumpAction.WasReleasedThisFrame();

    }

}
