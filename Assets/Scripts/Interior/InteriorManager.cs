using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    private ExteriorManager exteriorManager;

    void Start() {
        var isoCam = FindObjectOfType<Camera>();
        if (isoCam != null) {
            isoCam.enabled = true;
            Camera.SetupCurrent(isoCam);
            Debug.Log($"[InteriorManager] Set active camera to {isoCam.name}");
        }
    }
     
    public void RegisterExteriorManager(ExteriorManager extMgr) {
        exteriorManager = extMgr;

        var stations = Object.FindObjectsByType<Station3D>(FindObjectsSortMode.None);
        foreach (var station in stations) {
            var extCam = exteriorManager.GetCameraForStation(station.stationType);
            station.stationCamera = extCam;

            Debug.Log($"[InteriorManager] Linked {station.stationType} → {extCam?.name}");
        }
    }
}
