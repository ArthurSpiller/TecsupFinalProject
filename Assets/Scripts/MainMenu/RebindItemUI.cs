using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class RebindItemUI : MonoBehaviour
{
    public TMP_Text actionNameText;
    public TMP_Text bindingText;
    public Button rebindButton;

    private InputAction action;
    private int bindingIndex;

    public void Setup(InputAction action, int bindingIndex)
    {
        this.action = action;
        this.bindingIndex = bindingIndex;

        var binding = action.bindings[bindingIndex];

        // ✅ Show direction names for composite parts
        if (binding.isPartOfComposite && !string.IsNullOrEmpty(binding.name))
            actionNameText.text = $"{action.name} ({binding.name})";
        else
            actionNameText.text = action.name;

        bindingText.text = binding.ToDisplayString();

        rebindButton.onClick.AddListener(StartRebind);
    }

    public void StartRebind()
    {
        bindingText.text = "...";

        action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                operation.Dispose();
                bindingText.text = action.bindings[bindingIndex].ToDisplayString();

                // Save all overrides
                string json = action.actionMap.asset.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString("rebinds", json);
            })
            .Start();
    }
}
