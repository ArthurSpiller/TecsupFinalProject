using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class IsoPlayer3DController : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;
    private PlayerStationHandler stationHandler;

    void Awake() {
        stationHandler = GetComponent<PlayerStationHandler>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (stationHandler != null && stationHandler.IsInputLocked)
            return; // 🚫 skip movement completely
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // Rotate input to match isometric axes
        Vector3 isoDir = Quaternion.Euler(0, 45, 0) * input;
        controller.SimpleMove(isoDir.normalized * speed);        
    }
}
