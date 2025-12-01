using UnityEngine;
using UnityEngine.InputSystem;

public class MachineMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f;

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
        Vector3 direction = new Vector3(move.x, 0, move.y);

        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    public void LockInput(bool locked) {
        IsInputLocked = locked;
    }
}
