using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.gb.statemachine_toolkit
{
    /// <summary>
    /// A class used to display a dialogue with buttons.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Dialogue : MonoBehaviour
    {
        StateBehaviour.DialogueType Type;
        [Header("SEQUENCE")]
        [Tooltip("The Transform containing the current text of the sequence")]
        public Transform sequenceContainer;
        [Tooltip("The text component to show the text sequence")]
        public TextMeshProUGUI sequenceText;
        [Tooltip("The next button that will be used for a sequence of texts, to move from one text to the next, or close the dialogue when the last text has been shown.")]
        public Button nextButton;
        [Header("QUESTION")]
        [Tooltip("The Transform containing the question and the answers")]
        public Transform questionContainer;
        [Tooltip("The text component to show the question")]
        public TextMeshProUGUI questionText;
        [Tooltip("The prefab to use for each answer. Consider using a LayoutGroup container to better dispose the answers in the view.")]
        public GameObject answersPrefab;
        [Tooltip("The transform where the answers will be parented to")]
        public Transform answersContainer;
        // The animator that will manage opening and closing of the Dialogue
        private Animator animator;

        private List<string> currentCopy;
        private Action _onLastText;
        private Action _onAnswer;

        private void Start()
        {
            animator = this.GetComponent<Animator>();
        }

        public void Show(List<string> copy)
        {
            if (!nextButton)
            {
                Debug.LogWarning("Next button not found in Dialogue. Please assign a Button to the Dialogue component.");
                //return;
            }
            if (!animator)
            {
                Debug.LogWarning("No animator found in Dialogue. Please add an animator. ");
            }

            Debug.Log($"Showing Dialogue with copy {copy}");
            nextButton?.onClick.RemoveAllListeners();
            var newCopy = copy;

            animator?.SetTrigger("Open");
            ShowText(newCopy);
        }

        public void Show(List<string> copy, Action onLastText)
        {
            _onLastText = onLastText;
            Show(copy);
        }

        private void ShowText(List<string> copy)
        {
            if (copy == null || copy.Count <= 0) return;

            Debug.Log("Showing text: " + copy[0]);
            if (sequenceContainer) sequenceContainer.gameObject.SetActive(true);
            else Debug.LogWarning("Cannot show dialogue sequence, sequence container not assigned in Dialogue inspector");
            if (questionContainer) questionContainer.gameObject.SetActive(false);

            if (!sequenceText)
            {
                Debug.LogError("Sequence Text not found, please assign a gameObject with the TextMeshProUGUI component to the sequenceText field. ");
                return;
            }

            sequenceText.SetText(copy[0]);

            if (copy.Count == 1)
            {
                // this is the last text to show for this id, 
                // set next button to close the panel
                nextButton?.onClick.AddListener(() =>
                {
                    nextButton.onClick.RemoveAllListeners();
                    animator.SetTrigger("Close");
                    _onLastText?.Invoke();
                    _onLastText = null;
                });

                return;
            }

            Debug.Log("copy texts still to display: " + copy.Count);
            copy.RemoveAt(0);

            nextButton?.onClick.AddListener(() =>
            {
                nextButton.onClick.RemoveAllListeners();
                ShowText(copy);
            });
        }

        public void ShowQuestion(List<string> copy, float timeToWait, Action<int> onAnswer)
        {
            // to show a question we need at least 2 copy texts, one for the question and one for the answer
            if (copy == null || copy.Count <= 1)
            {
                Debug.LogError("Cannot show question, copy is empty, or it doesn't have an answer. " +
                    "Please add at least two texts, one for the question and one for the answer.");
                return;
            }

            // make a copy of the copy string since the list will be modified
            var strings = new List<string>(copy);

            if (string.IsNullOrWhiteSpace(strings[0]))
            {
                Debug.LogError("Question is empty");
                return;
            }

            if (sequenceContainer) sequenceContainer.gameObject.SetActive(false);
            if (questionContainer) questionContainer.gameObject.SetActive(true);
            else Debug.LogWarning("Cannot show dialogue question, sequence container not assigned in Dialogue inspector");

            questionText.SetText(strings[0]);

            // remove first text after setting the question
            strings.RemoveAt(0);
            var answerID = 0;
            // the remaining texts are answers
            foreach (var answer in strings)
            {
                var ans = Instantiate(answersPrefab, answersContainer);
                var aComp = ans.GetComponent<Answer>();
                if (!aComp)
                {
                    Debug.LogError("No Answer component found on Answer prefab.");
                }
                var aID = answerID;
                aComp.SetAnswer(answer, () =>
                {
                    Utilities.WaitThenAct(this, timeToWait, () => onAnswer(aID));
                    animator.SetTrigger("Close");
                });
                answerID++;
            }

            animator.SetTrigger("Open");
        }
    }
}
