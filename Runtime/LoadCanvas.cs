using UnityEngine;
using UnityEngine.UI;

namespace com.gb.statemachine_toolkit
{
    public class LoadCanvas : MonoBehaviour
    {
        public Button startButton;
        public GameObject introText;
        public GameObject progressImage;
        public Image progressFillImage;
        public void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
