using UnityEngine;

public class ExteriorManager : MonoBehaviour
{
    private InteriorManager interiorManager;

    [Header("Exterior Cameras")]
    public Camera movementCam;
    public Camera firingCam;
    public Camera lookoutCam;

    [Header("Controllers")]
    public MachineMovementController machineMovementController;
    public MachineFiringController machineFiringController;
    public MachineLookoutController machineLookoutController;

    public void RegisterInteriorManager(InteriorManager intMgr) {
        interiorManager = intMgr;
    }

    public Camera GetCameraForStation(StationType type)
    {
        switch (type)
        {
            case StationType.Movement: return movementCam;
            case StationType.Firing:   return firingCam;
            case StationType.Lookout:  return lookoutCam;
            default: return null;
        }
    }

    public void SetMachineControl(bool active) {
        if (machineMovementController != null)
            machineMovementController.LockInput(!active);
    }

    public void SetMachineFiring(bool active) {
        machineFiringController.SetActive(active);
    }

    public void SetMachineLookout(bool active) {
        machineLookoutController.SetActive(active);
    }
}
