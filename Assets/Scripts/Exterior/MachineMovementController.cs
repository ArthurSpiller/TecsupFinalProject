using UnityEngine;
using UnityEngine.InputSystem;

public class MachineMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float rotationSpeed = 120f;

    private InputSystem_Actions inputActions;

    public bool IsInputLocked { get; private set; } = true;

    private void Awake() {
        inputActions = InputManager.Actions;
    }

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }

    private void Update() {
        if (IsInputLocked)
            return;

        Vector2 move = inputActions.Player.Move.ReadValue<Vector2>();

        float forward = move.y;     // W / S or up/down stick
        float turn = move.x;        // A / D or left/right stick

        // --- ROTATION ---
        if (Mathf.Abs(turn) > 0.01f) {
            transform.Rotate(Vector3.up * turn * rotationSpeed * Time.deltaTime);
        }

        // --- FORWARD MOVEMENT ---
        if (Mathf.Abs(forward) > 0.01f) {
            Vector3 forwardDir = transform.forward * forward;
            transform.Translate(forwardDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void LockInput(bool locked) {
        IsInputLocked = locked;
    }
}
