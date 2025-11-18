using UnityEngine;

public class ExteriorManager : MonoBehaviour
{
    [Header("Exterior Cameras")]
    public Camera movementCam;
    public Camera firingCam;
    public Camera lookoutCam;

    [Header("Machine Controller")]
    public MachineMovementController machineController;

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

    public void SetMachineControl(bool active)
    {
        if (machineController != null)
            machineController.LockInput(!active);
    }
}
