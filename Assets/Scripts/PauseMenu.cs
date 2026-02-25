using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button pauseButton;

    [Header("Scene Transition")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        // Sanity checks
        if (pauseMenuPanel == null)
        {
            Debug.LogError("Pause Menu Panel not assigned in inspector!");
            enabled = false;
            return;
        }

        if (pauseButton == null)
        {
            Debug.LogError("Pause Button not assigned in inspector!");
            enabled = false;
            return;
        }

        // Initial state
        pauseMenuPanel.SetActive(false);
        pauseButton.interactable = true;
        
        // Button setup
        pauseButton.onClick.AddListener(TogglePauseMenu);
    }

    private void TogglePauseMenu()
    {
        bool shouldPause = !pauseMenuPanel.activeSelf;
        
        pauseMenuPanel.SetActive(shouldPause);
        Time.timeScale = shouldPause ? 0f : 1f;
        
        // Disable pause button while paused to prevent multiple clicks
        pauseButton.interactable = !shouldPause;
    }

    public void ResumeGame()
    {
        TogglePauseMenu();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Directly restart the level
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName); // Directly load the main menu scene
    }
}
