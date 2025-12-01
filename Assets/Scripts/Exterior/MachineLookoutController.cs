using UnityEngine;
using UnityEngine.InputSystem;

public class MachineLookoutController : MonoBehaviour
{
    private InputSystem_Actions input;
    public Transform pivotX;
    public Transform pivotY;
    public float sensitivity = 60f;

    private bool active = false;

    private void Awake()
    {
        input = InputManager.Actions;
    }

    public void SetActive(bool _active) {
        active = _active;
    }

    private void Update()
    {
        if (!active) return;

        Vector2 look = input.Player.Look.ReadValue<Vector2>();

        pivotX.Rotate(Vector3.up,   look.x * sensitivity * Time.deltaTime);
        pivotY.Rotate(Vector3.right, -look.y * sensitivity * Time.deltaTime);
    }
}
