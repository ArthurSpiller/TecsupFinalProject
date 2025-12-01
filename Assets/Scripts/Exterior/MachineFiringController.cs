using UnityEngine;
using UnityEngine.InputSystem;

public class MachineFiringController : MonoBehaviour
{
    private InputSystem_Actions input;

    [Header("Firing Setup")]
    public bool active = false;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 800f;

    [Header("Firing Ammo")]
    public int ammo = 50;

    private void Awake()
    {
        input = InputManager.Actions;
    }

    private void OnEnable()
    {
        input.Player.Fire.performed += OnFire;
    }

    private void OnDisable()
    {
        input.Player.Fire.performed -= OnFire;
    }

    public void SetActive(bool _active) {
        active = _active;
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (!active) return;

        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing bulletPrefab or firePoint!");
            return;
        }

        if (ammo <= 0) {
            Debug.Log("No more Ammo");
            return;
        }
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
        }
        ammo--;
        Debug.Log("Fired projectile!");
    }
}
