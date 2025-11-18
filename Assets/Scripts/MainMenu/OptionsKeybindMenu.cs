using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class OptionsKeybindMenu : MonoBehaviour
{
    public Transform listParent;
    public RebindItemUI rebindItemPrefab;
    public Button backButton;

    [Header("Main Menu")]
    public GameObject mainMenuPanel;

    private InputSystem_Actions actions;

    private void Awake()
    {
        actions = new InputSystem_Actions();

        if (PlayerPrefs.HasKey("rebinds"))
            actions.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));

        GenerateList();
        backButton.onClick.AddListener(BackToMainMenu);
    }

    private void BackToMainMenu() {
        gameObject.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void GenerateList()
    {
        InputAction[] playerActions = new InputAction[]
        {
            actions.Player.Move,
            actions.Player.Interact,
            actions.Player.ExitStation
        };

        foreach (var action in playerActions)
        {
            if (action == null) continue;

            for (int i = 0; i < action.bindings.Count; i++) {
                if (action.bindings[i].isComposite) continue;
//                if (action.bindings[i].isPartOfComposite) continue;

                var item = Instantiate(rebindItemPrefab, listParent);
                item.Setup(action, i);
            }
        }
    }

}
