using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public ExteriorManager exteriorManager;
    public InteriorManager interiorManager;

    private Scene interiorScene;
    private Scene exteriorScene;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        StartCoroutine(LoadGameScenes());
    }

    private IEnumerator LoadGameScenes() {
        var interiorParams = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        var exteriorParams = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        interiorScene = SceneManager.CreateScene("InteriorScene_Instance", interiorParams);
        exteriorScene = SceneManager.CreateScene("ExteriorScene_Instance", exteriorParams);
        var interiorLoad = SceneManager.LoadSceneAsync("InteriorScene", LoadSceneMode.Additive);
        var exteriorLoad = SceneManager.LoadSceneAsync("ExteriorScene", LoadSceneMode.Additive);

        while (!interiorLoad.isDone || !exteriorLoad.isDone)
            yield return null;
        Debug.Log("Both scenes loaded with independent physics.");

        exteriorManager = FindObjectOfType<ExteriorManager>();
        interiorManager = FindObjectOfType<InteriorManager>();

        if (exteriorManager && interiorManager) {
            interiorManager.RegisterExteriorManager(exteriorManager);
            exteriorManager.RegisterInteriorManager(interiorManager);
            Debug.Log("Linked Interior and Exterior managers.");
        } else {
            Debug.LogError("Could not find one of the scene managers!");
        }
    }
}
