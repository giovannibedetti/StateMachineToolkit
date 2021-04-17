using UnityEngine;
using UnityEngine.UI;
using System;

namespace com.gb.statemachine_toolkit
{
    public class Answer : MonoBehaviour
    {
        [Tooltip("The text component that will be used to display the answer")]
        public TMPro.TextMeshProUGUI text;
        [Tooltip("The button that will be used to select this answer")]
        public Button button;

        public void SetAnswer(string answer, Action action)
        {
            this.text.SetText(answer);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => action?.Invoke());
        }
    }
}
