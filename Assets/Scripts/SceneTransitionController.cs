using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad;
    public float transitionSpeed = 2f;

    [Header("References")]
    [SerializeField] private Image transitionImage;
    
    private Material transitionMaterial;
    private bool shouldFadeOut = true; // Start with fade out (scene reveal)

    void Start()
    {
        // Create material instance to avoid modifying original
        transitionMaterial = new Material(transitionImage.material);
        transitionImage.material = transitionMaterial;
        
        // Start with screen fully covered (black)
        transitionMaterial.SetFloat("_Cutoff", -0.1f);
    }

    void Update()
    {
        if (shouldFadeOut)
        {
            // Fade out (reveal scene)
            float newCutoff = Mathf.MoveTowards(
                transitionMaterial.GetFloat("_Cutoff"),
                1.1f,
                transitionSpeed * Time.deltaTime
            );
            transitionMaterial.SetFloat("_Cutoff", newCutoff);
        }
        else
        {
            // Fade in (cover scene)
            float newCutoff = Mathf.MoveTowards(
                transitionMaterial.GetFloat("_Cutoff"),
                -0.1f,
                transitionSpeed * Time.deltaTime
            );
            transitionMaterial.SetFloat("_Cutoff", newCutoff);

            // Check if fade in is complete
            if (newCutoff <= -0.1f)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    public void StartSceneTransition()
    {
        shouldFadeOut = false; // Start fade in
    }
}
