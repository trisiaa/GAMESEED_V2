using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    public SceneTransitionController sceneTransitionController; // Reference to the SceneTransitionController

    public void Play()
    {
        // Start the scene transition
        sceneTransitionController.StartSceneTransition();
    }

    public void Quit()
    {
        Debug.Log("Keluar");
        Application.Quit();
    }
}
