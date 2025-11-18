using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerControlContext {
    InteriorCharacter,
    ExteriorMachine
}

public class PlayerStationHandler : MonoBehaviour
{
    [Header("Interior References")]
    public Camera isoCamera;
    public GameObject playerBody;

    private Station3D currentStation;
    private InputSystem_Actions inputActions;
    public bool IsInputLocked { get; private set; }
    public PlayerControlContext currentContext { get; private set; } = PlayerControlContext.InteriorCharacter;

    private void Awake() {
        inputActions = new InputSystem_Actions();
        InputLoader.LoadSavedBindings(inputActions);
    }

    private void OnEnable() {
        inputActions.Enable();
        inputActions.Player.ExitStation.performed += OnExitStation;
    }

    private void OnDisable() {
        inputActions.Player.ExitStation.performed -= OnExitStation;
        inputActions.Disable();
    }

    private void OnExitStation(InputAction.CallbackContext ctx) {
        ExitStation();
    }

    public void EnterStation(Station3D station) {
        currentStation = station;
        LockInput(true);

        playerBody.SetActive(false);
        isoCamera.enabled = false;

        if (station.stationCamera)
            station.stationCamera.enabled = true;

        if (station.stationType == StationType.Movement) {
            GameManager.Instance.exteriorManager.SetMachineControl(true);
        }
    }

    public void ExitStation() {
        if (currentStation != null) {
            if (currentStation.stationCamera)
                currentStation.stationCamera.enabled = false;

            if (currentStation.stationType == StationType.Movement)
                GameManager.Instance.exteriorManager.SetMachineControl(false);

            currentStation.Release();
            currentStation = null;
        }

        LockInput(false);

        playerBody.SetActive(true);
        isoCamera.enabled = true;
    }

    public void LockInput(bool locked){
        IsInputLocked = locked;
    }
}
