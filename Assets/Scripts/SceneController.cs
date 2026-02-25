using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    public string sceneToLoad; // Variable to specify a scene to load

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        // Load the next scene based on the current scene's build index
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        // Load the specified scene
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void LoadSceneFromVariable()
{
    // Load the scene specified in the sceneToLoad variable
    if (!string.IsNullOrEmpty(sceneToLoad))
    {
        Debug.Log($"Loading scene: {sceneToLoad}");
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
    else
    {
        Debug.LogWarning("sceneToLoad is not set.");
    }
}
}
