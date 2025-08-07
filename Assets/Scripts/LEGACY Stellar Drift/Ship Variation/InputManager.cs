using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerInputs PlayerControls { get; private set; }
    public Vector2 LeftThrustInput { get; private set; }
    public Vector2 RightThrustInput { get; private set; }
    public Vector2 SingleJoystickInput { get; set; }

    public InputAction LeftThrust { get; private set; }
    public InputAction RightThrust { get; private set; }
    
    // private void Awake()
    // {
    //     if (Instance == null)
    //         Instance = this;


    //     if (PlayerControls == null)
    //         PlayerControls = new PlayerInputs();

    // }

    // private void Start()
    // {
    //     PlayerControls.Enable();
    //     PlayerActions();
    // }

    // private void OnDestroy()
    // {
    //     if (PlayerControls != null)
    //     {
    //         PlayerControls.Disable();
    //         PlayerControls.Dispose();
    //     }
    // }

    // private void Update()
    // {
    //     LeftThrustInput = PlayerControls.Player.MoveLeftThrust.ReadValue<Vector2>();
    //     RightThrustInput = PlayerControls.Player.MoveRightThrust.ReadValue<Vector2>();
    // }

    // private void PlayerActions()
    // {
    //     LeftThrust = PlayerControls.Player.MoveLeftThrust;
    //     RightThrust = PlayerControls.Player.MoveRightThrust;
    // }    
}
