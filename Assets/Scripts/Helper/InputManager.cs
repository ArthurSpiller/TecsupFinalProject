using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    private static InputSystem_Actions _actions;

    public static InputSystem_Actions Actions {
        get {
            if (_actions == null) {

                _actions = new InputSystem_Actions();

                if (PlayerPrefs.HasKey("rebinds")) {
                    string json = PlayerPrefs.GetString("rebinds");

                    _actions.asset.LoadBindingOverridesFromJson(json);
                }
            }

            return _actions;
        }
    }
}
