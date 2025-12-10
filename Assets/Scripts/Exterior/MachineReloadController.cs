using UnityEngine;

public class MachineReloadController : MonoBehaviour
{
    [Header("Reload Settings")]
    public bool active = false;
    public int maxAmmo = 50;
    public float reloadRate = 5f;
    public float reloadDelay = 0.5f;

    [Header("External Dependencies")]
    public MachineFiringController firingController;

    private bool inReloadZone = false;
    private float reloadTimer = 0f;

    private void Update() {
        if (!active) return;
        if (!inReloadZone) return;
        if (firingController == null) return;

        if (firingController.ammo >= maxAmmo) return;

        reloadTimer += Time.deltaTime;

        if (reloadTimer >= reloadDelay) {
            firingController.ammo++;

            Debug.Log($"Reloading... Ammo = {firingController.ammo}");

            reloadTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ReloadZone")) {
            inReloadZone = true;
            Debug.Log("Entered Reload Zone");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ReloadZone")) {
            inReloadZone = false;
            Debug.Log("Exited Reload Zone");
        }
    }

    public void SetActive(bool _active) {
        active = _active;
        reloadTimer = 0f;
    }
}
