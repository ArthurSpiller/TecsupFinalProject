using UnityEngine;
using UnityEngine.InputSystem;

public class Station3D : MonoBehaviour
{
    public StationType stationType;
    [HideInInspector] public Camera stationCamera;
    [HideInInspector] public bool isOccupied;
    [HideInInspector] public PlayerStationHandler currentUser;

    private bool playerInRange;
    private PlayerStationHandler nearbyHandler;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        InputLoader.LoadSavedBindings(inputActions);
    }

    private void OnEnable() {
        inputActions.Enable();
        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable() {
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Disable();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
            nearbyHandler = other.GetComponentInParent<PlayerStationHandler>();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
            nearbyHandler = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx) {
        if (playerInRange && !isOccupied && nearbyHandler) {
            isOccupied = true;
            currentUser = nearbyHandler;
            nearbyHandler.EnterStation(this);
        }
    }

    public void Release()
    {
        isOccupied = false;
        currentUser = null;
    }
}
