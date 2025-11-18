using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public static class InputLoader
{
    public static void LoadSavedBindings(InputSystem_Actions actions)
    {
        if (PlayerPrefs.HasKey("rebinds"))
        {
            actions.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));
        }
    }
}
