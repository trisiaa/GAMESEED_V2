using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private SceneTransitionController transitionController;
    
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnCutsceneFinished;
        
        if (transitionController == null)
            Debug.LogWarning("SceneTransitionController reference missing!");
    }

    private void OnCutsceneFinished(VideoPlayer vp)
    {
        if (transitionController != null)
            transitionController.StartSceneTransition();
    }
}
