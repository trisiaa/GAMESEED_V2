    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;

    public class UIController : MonoBehaviour
    {
        public DialogManager dialogManager;
        public TextMeshProUGUI dialogText;
        public Image dialogImage;
        public Image backgroundImage;
        public Button nextButton;
        public float typingSpeed = 0.05f;
        public SceneTransitionController transitionController;

        void Start()
        {
            ShowDialog(dialogManager.GetCurrentDialog());
            nextButton.onClick.AddListener(OnNextButtonClick);
            UpdateButtonState();
        }

        private void ShowDialog(Dialog dialog)
        {
            if (dialog != null)
            {
                StopAllCoroutines();
                StartCoroutine(TypeText(dialog.text));
                dialogImage.sprite = dialog.image;
                backgroundImage.sprite = dialog.backgroundImage;
            }
            else
            {
                // No more dialogs - trigger transition immediately
                if (transitionController != null)
                {
                    transitionController.StartSceneTransition();
                }
                else
                {
                    Debug.LogWarning("No transition controller assigned!");
                }
            }
        }

        private IEnumerator TypeText(string text)
        {
            dialogText.text = "";
            foreach (char letter in text.ToCharArray())
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        private void UpdateButtonState()
        {
            // Button should be active if there are more dialogs OR we need to transition
            nextButton.interactable = dialogManager.CanMoveNext() || !dialogManager.CanMoveNext();
        }

        private void OnNextButtonClick()
        {
            if (dialogManager.MoveNext())
            {
                ShowDialog(dialogManager.GetCurrentDialog());
            }
            else
            {
                // No more dialogs - trigger transition
                if (transitionController != null)
                {
                    transitionController.StartSceneTransition();
                }
            }
            UpdateButtonState();
        }
    }