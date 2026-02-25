using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] bool goNextLevel; // Flag to determine if the next level should be loaded
    [SerializeField] string levelName; // Name of the level to load

    // This method can be called when the button is clicked
    public void OnButtonClick()
    {
        if (goNextLevel)
        {
            SceneController.instance.NextLevel(); // Use SceneController to load the next level
        }
        else if (!string.IsNullOrEmpty(levelName))
        {
            SceneController.instance.LoadScene(levelName); // Load the specified scene
        }
        else
        {
            Debug.LogWarning("No level specified to load."); // Log a warning if no level is specified
        }
    }
}
